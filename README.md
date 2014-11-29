----------------------------------------------
           G-Audio iOS Pitch Shift
       Copyright © 2014 Gregorio Zanon
               Version 1.0
          www.G-Audio-Unity.com
        support@g-audio-unity.com
----------------------------------------------

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
							*** Setting Up ***
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

1) In the project window, move G-Audio_FreePitchShifter/Plugins/GATDiracFree.mm to Plugins/iOS

4) Download DiracLE here: http://dirac.dspdimension.com/Dirac3_Technology_Home_Page/Dirac3_Technology.html

5) You only need 2 files in the --DIRAC3LE-- folder you've just downloaded : libDIRAC_iOS-fat.a and Dirac.h
They are located at --DIRAC3LE--/DIRAC3-Mobile/Common Files/src/
Import them in the Plugins/iOS folder of your project.

6) Build your project, just build, not build and run. Open the Xcode project that was just built.

7) Add the Accelerate framework to your Xcode project( select Unity-iPhone ( root of the project ), select Targets/Unity-iPhone, Build Phases tab, Link Binary With Libraries, + button, select Accelerate Framework ).
Remember that when you build from Unity, choosing Replace instead of Append will create a new XCode project and you'll need to add the Accelerate framework again.

If you use Dirac in your projects, please respect the license and credit the owner:
"DIRAC Time Stretch/Pitch Shift technology (c) 2005-2010 Stephan M. Bernsee, www.dspdimension.com" 

Finally, if you use DiracLE ( the free version of the library ), it only supports 44.1 kHz playback. Add the provided InitScene as the first scene of your project,
it will set the sample rate to 44.1 kHz for you.


'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
							*** Release Notes ***
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

*** 1.0 ***

Components: 

- DiracStereoPitchShifter: place after any AudioSource or AudioListener component.
- DiracMonoPitchShifter: place after any AudioSource component. Less resource intensive 
than the stereo pitch shifter. Pan and gain can be adjusted.

When placing these components after AudioSource components, use their Play and Stop methods,
not the AudioSource's.

Important:

Dirac adds 4096 frames of latency: it requires enough audio data to analyze in order to perform it's
high quality pitch shifting.


