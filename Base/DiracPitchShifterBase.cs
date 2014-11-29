using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace GAudio.iOS
{
	public abstract class DiracPitchShifterBase : MonoBehaviour 
	{
		#region Public Fields and Properties

		/// <summary>
		/// Set to true if you are pitch shifting the
		/// AudioListener, or if you are generating your own audio
		/// in OnAudioFilterRead().
		/// </summary>
		public bool alwaysProcess;

		/// <summary>
		/// Gets or sets the pitch shift factor.
		/// Clamped between .5d and 2d ( one octave lower to
		/// one octave higher ).
		/// </summary>
		public double Pitch
		{ 
			get{ return _pitch;  }
			set
			{	
				if( value > 2.0d ){ value = 2.0d; }else
				if( value < 0.5d ){ value = 0.5d; } 
				_pitch = value; 
			}
		}

		/// <summary>
		/// IsPlaying is not the same as AudioSource.IsPlaying:
		/// as Dirac has it's own latency, audio will keep on playing
		/// 4096 frames ( roughly .1s ) after the end of the clip.
		/// Calling stop will stop immediately, though.
		/// </summary>
		public bool IsPlaying
		{ 
			get
			{
				return _playingState != PlayingState.Stopped;
			}
		}
		
		public readonly float DIRAC_LATENCY 		= 0.093f;
		public readonly int   DIRAC_LATENCY_FRAMES 	= 4096;

		#endregion

		#region Public Methods
		/// <summary>
		/// Use this instead of AudioSource.Play();
		/// </summary>
		public void Play()
		{
			if( _source == null )
			{
				if( Debug.isDebugBuild )
					Debug.Log( "No AudioSource to play!" );
				return;
			}
			
			if( _playingState != PlayingState.Stopped )
				return;
			
			if( _buffersNeedReset )
			{
				ResetBuffers();
				_buffersNeedReset = false;
			}
			_doProcess = true;
			_source.Play();
			_playingState = PlayingState.Playing;
		}

		/// <summary>
		/// Use this instead of AudioSource.Stop();
		/// </summary>
		public void Stop()
		{
			if( _source == null )
				return;
			
			_doProcess = false;
			_source.Stop();
			_playingState = PlayingState.Stopped;
			_buffersNeedReset = true;
		}

		/// <summary>
		/// Resets dirac's state. Use only if you know 
		/// what you're doing!
		/// </summary>
		public virtual void ResetBuffers()
		{
			_dirac.ResetBuffers();
		}
		
		#endregion

		#region Private and Protected Fields
		
		enum PlayingState{ Stopped, Playing, ProcessingTail }
		PlayingState _playingState = PlayingState.Stopped;

		protected DiracWrapper _dirac;

		protected double   _pitch = 1.0d;
		protected int 	   _bufferSize;

		bool	 	  	   _initialized;
		volatile bool 	   _doProcess;
		
		AudioSource _source;
		float       _endProcessingTime;
		bool		_buffersNeedReset;

		#endregion

		#region Private Methods
		void Awake()
		{
			#if UNITY_EDITOR || !UNITY_IPHONE
			this.enabled = false;
			Debug.LogWarning( "DiracPitchShifter only works on iOS devices." );
			#endif

			if( enabled )
				Initialize();
		}
		
		void Initialize()
		{
			Debug.Log( "Dirac Initialize" );
			if( AudioSettings.outputSampleRate != 44100 )
			{
				Debug.LogError( "DiracLE pitch shifting only works at 44100 kHz. Use the InitScene to force 44.1 kHz output on iOS." );
				this.enabled = false;
				return;
			}
			
			int numBuffers;
			
			AudioSettings.GetDSPBufferSize( out _bufferSize, out numBuffers );
			
			AllocateNativeResources( _bufferSize );

			_initialized    = true;
			
			_source = gameObject.GetComponent< AudioSource >();
			
			if( _source != null )
			{
				if( _source.playOnAwake == true && _source.clip != null )
				{
					_playingState = PlayingState.Playing;
					_doProcess = true;
					_buffersNeedReset = true;
				}
			}
			
			if( alwaysProcess )
				_doProcess = true;
		}

		void Update()
		{
			if( _source == null )
				return;
			
			if( _playingState == PlayingState.Playing )
			{
				if( !_source.isPlaying )
				{
					_playingState      = PlayingState.ProcessingTail;
					_endProcessingTime = Time.time + DIRAC_LATENCY; 
				}
			}
			else if( _playingState == PlayingState.ProcessingTail )
			{
				if( Time.time > _endProcessingTime )
				{
					_doProcess = false;
					_playingState = PlayingState.Stopped;
				}
			}
		}

		void OnDestroy()
		{
			if( _initialized )
			{
				_doProcess   = false;
				_initialized = false;
				ReleaseNativeResources();
			}
		}
		
		void OnAudioFilterRead( float[] data, int channels )
		{
			if( !_initialized || !_doProcess )
				return;
			
			ProcessDirac( data, channels );
		}

		protected void ExtractChannel( float[] interleavedData, float[] target, int numChannels, int channel )
		{
			int i;
			int targetI = 0;
			
			for( i = channel; i < interleavedData.Length; i += numChannels )
			{
				target[ targetI ] = interleavedData[ i ];
				targetI++;
			}
		}

		#endregion

		#region Protected Overridable Methods
		protected abstract void ProcessDirac( float[] DataMisalignedException, int channels );

		protected virtual void AllocateNativeResources( int bufferSize )
		{
			_dirac = new DiracWrapper( bufferSize );
		}

		protected virtual void ReleaseNativeResources()
		{
			_dirac.Dispose();
		}
		#endregion

		#region DiracWrapper 
		protected class DiracWrapper : IDisposable
		{
			public readonly float[] buffer;
			public readonly IntPtr  bufferPointer;
			public  IntPtr  diracPointer;

			GCHandle _handle;
			bool 	 _disposed;

			public DiracWrapper( int bufferSize )
			{
				buffer 			= new float[ bufferSize ];
				_handle 		= GCHandle.Alloc( buffer, GCHandleType.Pinned );
				bufferPointer 	= _handle.AddrOfPinnedObject();
				diracPointer    = _DiracDemoCreate();
			}

			public void ResetBuffers()
			{
				// This should be done with DiracFXReset, but it seems buggy
				// and pops every time. Re-creating a Dirac instance is cheap 
				// and works fine.
				_DiracDemoRelease( diracPointer );
				diracPointer = _DiracDemoCreate();
			}

			public void Dispose()
			{
				Dispose( true );
				GC.SuppressFinalize( this );
			}

			void Dispose( bool explicitely )
			{
				if( _disposed )
					return;

				_handle.Free();
				_DiracDemoRelease( diracPointer );
				_disposed = true;
			}

			~DiracWrapper()
			{
				Dispose( false );
			}
		}
		#endregion

		#region External Bindings

		[ DllImport( "__Internal" ) ]
		protected static extern IntPtr _DiracDemoCreate();
		
		[ DllImport( "__Internal" ) ]
		protected static extern void _DiracDemoProcess( IntPtr diracRef, IntPtr buffer, double pitch, int numFrames );
		
		[ DllImport( "__Internal" ) ]
		protected static extern void _DiracDemoResetBuffers( IntPtr diracRef );
		
		[ DllImport( "__Internal" ) ]
		protected static extern void _DiracDemoRelease( IntPtr diracRef );

		#endregion
	}
}



