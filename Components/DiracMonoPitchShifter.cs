using UnityEngine;
using System.Collections;

namespace GAudio.iOS
{
	/// <summary>
	/// Dirac pitch shifting is quite resource intensive.
	/// Use this class to spare cpu, or if your audio is mono.
    /// Note: Will override the AudioSource's pan parameter. 
	/// </summary>
	public class DiracMonoPitchShifter : DiracPitchShifterBase 
	{
		/// <summary>
		/// If AudioSpeakerMode is stereo, even mono files will
		/// output stereo data. In that case, we chose a channel to process.
		/// </summary>
		public int channelToProcess = 0;

		/// <summary>
		/// When used in stereo output context, allows panning of the processed channel.
		/// </summary>
		[ Range( 0f, 1f ) ]
		public float stereoPan = 0.5f;

		/// <summary>
		/// If AudioSpeakerMode is stereo, one mono channel will be de-interleaved, processed and
		/// panned to the audio buffer. This can result in a loss of volume - adjust gain accordingly.
		/// </summary>
		[ Range( 0f, 2f ) ]
		public float gain 	   = 1.0f;

		#region Private and Protected
		protected override void ProcessDirac( float[] data, int channels )
		{
			if( channels == 1 )
			{
				System.Array.Copy( data, _dirac.buffer, _bufferSize );
				_DiracDemoProcess( _dirac.diracPointer, _dirac.bufferPointer, _pitch, _bufferSize );
				System.Array.Copy( _dirac.buffer, data, data.Length );
			}
			else
			{
				ExtractChannel( data, _dirac.buffer, channels, channelToProcess );
				_DiracDemoProcess( _dirac.diracPointer, _dirac.bufferPointer, _pitch, _bufferSize );
				CopyPanned( _dirac.buffer, data, stereoPan, gain );
			}
		}
		
		void CopyPanned( float[] monoSource, float[] stereoTarget, float pan, float gain )
		{
			int i;
			int targetI;
			for( i = 0; i < monoSource.Length; i++ )
			{
				targetI = i * 2;
				stereoTarget[ targetI ]		= monoSource[ i ] * ( 1.0f - pan ) * gain;
				stereoTarget[ targetI + 1 ] = monoSource[ i ] * pan * gain;
			}
		}

		#endregion
	}
}

