//
//  DiracDemoPitchShift.m
//  Unity-iPhone
//
//  Created by Gregorio Zanon on 29.06.14.
//
//


#import "Dirac.h"

extern "C"
{
    void * _DiracDemoCreate()
    {
        void * dirac = nil;
        
        dirac = DiracFxCreate( kDiracQualityBest, 44100, 1 );
        
        return dirac;
    }
    
    void _DiracDemoProcess( void * diracRef, float * buffer, double pitch, int numFrames )
    {
        DiracFxProcessFloat( 1.0, pitch, &buffer, &buffer, numFrames, diracRef );
    }
    
    void _DiracDemoResetBuffers( void * diracRef )
    {
        DiracFxReset( true, diracRef );
    }
    
    void _DiracDemoRelease( void * diracRef )
    {
        DiracFxDestroy( diracRef );
    }
}