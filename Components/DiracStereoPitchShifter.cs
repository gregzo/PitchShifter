using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace GAudio.iOS
{
	public class DiracStereoPitchShifter : DiracPitchShifterBase
	{
		// No public fields / properties / methods.
		// Play/Stop/Pitch already exposed in base class.

		#region Private and Protected
		DiracWrapper _dirac2;

		protected override void AllocateNativeResources( int bufferSize )
		{
			base.AllocateNativeResources( bufferSize );
			_dirac2 = new DiracWrapper( bufferSize );
		}

		protected override void ReleaseNativeResources()
		{
			base.ReleaseNativeResources();	
			_dirac2.Dispose();
		}

		protected override void ProcessDirac( float[] data, int channels )
		{
			ExtractChannel( data, _dirac.buffer,  channels, 0 );
			ExtractChannel( data, _dirac2.buffer, channels, 1 );

			_DiracDemoProcess( _dirac.diracPointer, _dirac.bufferPointer, _pitch, _bufferSize );
			_DiracDemoProcess( _dirac2.diracPointer, _dirac2.bufferPointer, _pitch, _bufferSize );

			Interleave( _dirac.buffer, _dirac2.buffer, data, _bufferSize );
		}

		public override void ResetBuffers()
		{
			base.ResetBuffers();
			_dirac2.ResetBuffers();
		}

		void Interleave( float[] left, float[] right, float[] target, int numFrames )
		{
			int i;
			int targetI;

			for( i = 0; i < numFrames; i++ )
			{
				targetI = i * 2;

				target[ targetI ] = left[ i ];
				target[ targetI + 1 ] = right[ i ];
			}
		}

		#endregion
	}
}

