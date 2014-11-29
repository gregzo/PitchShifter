using UnityEngine;
using System.Collections;
using GAudio.iOS;

namespace GAudio.Examples
{
	public class StereoPitchShifterGUI : MonoBehaviour 
	{
		public DiracPitchShifterBase pitchShifter;

		static Rect __area = new Rect( 20f, 20f, 300f, 300f );

		void OnGUI()
		{
			GUILayout.BeginArea( __area );

			if( pitchShifter.IsPlaying == false )
			{
				if( GUILayout.Button( "Play" ) )
				{
					pitchShifter.Play();
				}
			}
			else 
			{
				if( GUILayout.Button( "Stop" ) )
				{
					pitchShifter.Stop();
				}
			}

			GUILayout.Label( "Pitch: " + pitchShifter.Pitch.ToString( "0.00" ) );
			pitchShifter.Pitch = ( double )GUILayout.HorizontalSlider( ( float )pitchShifter.Pitch, .5f, 2f );
			GUILayout.Space( 20f );

			ExtraGUI();

			GUILayout.EndArea();
		}

		protected virtual void ExtraGUI(){}
	}

}
