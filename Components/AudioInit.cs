using UnityEngine;
using System.Collections;


public class AudioInit : MonoBehaviour 
{
	int sceneToLoad = 1;
	
	void Awake()
	{
		if( AudioSettings.outputSampleRate != 44100 )
		{
			AudioSettings.outputSampleRate = 44100;
		}
		
		Application.LoadLevel( sceneToLoad );
	}
}

