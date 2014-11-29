using UnityEngine;
using System.Collections;
using GAudio.iOS;

namespace GAudio.Examples
{
	public class MonoPitchShifterGUI : StereoPitchShifterGUI
	{
		DiracMonoPitchShifter _monoDirac;

		void Awake()
		{
			if( pitchShifter.GetType() != typeof( DiracMonoPitchShifter ) )
			{
				Debug.LogError( "MonoPitchShifterGUI works with DiracMonoPitchShifter components only! " );
				this.enabled = false;
				return;
			}

			_monoDirac = ( DiracMonoPitchShifter )pitchShifter;
		}
		protected override void ExtraGUI()
		{
			GUILayout.Label( "Pan: " + _monoDirac.stereoPan.ToString( "0.00" ) );
			_monoDirac.stereoPan = GUILayout.HorizontalSlider( _monoDirac.stereoPan, 0f, 1f );
			GUILayout.Space( 20f );
			
			GUILayout.Label( "Gain: " + _monoDirac.gain.ToString( "0.00" ) );
			_monoDirac.gain = GUILayout.HorizontalSlider( _monoDirac.gain, 0f, 2f );
			GUILayout.Space( 20f );
		}	
	}
}

