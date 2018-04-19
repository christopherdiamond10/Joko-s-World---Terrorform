//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Play Sound Effect
//             Author: Christopher Diamond
//             Date: October 23, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This button simply plays a Sound Effect when pressed. It can also prevent
//      itself from playing multiple times at once if desired.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_PlaySoundEffect : Button_Base 
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public PlayType m_ePlayType = PlayType.ALWAYS;
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*- Provate Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private int m_iProvidedAudioID = -1;

	private static int sm_iUsedAudioID = -1;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public enum PlayType
    {
        ALWAYS,							// Sound Will always play when button is pressed. Regardless of other factors
        CONSECUTIVELY,					// Sound will play when button is pressed, but only if the sound it started playing earlier has looped through to the end.
		CONSECUTIVELY_STATIC,			// Sound will play when button is pressed, but only if no other sound is currently playing
		CONSECUTIVELY_STATIC_PRIORITY,	// Sound will play when button is pressed, if any other sounds in this group are playin, they will be stopped so that this one can play without sound overlap issues.
        ONLY_ONCE,						// Sound will only play the first time this button is pressed. And will be ignored after that.
    }
	
	

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Play Button Sound
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public override void PlayButtonSound()
    {
        if (m_acClipToPlay != null)
        {
            switch (m_ePlayType)
            {
                case PlayType.CONSECUTIVELY:					{ OnConsecutivelyPlayTriggered();				break; }
				case PlayType.CONSECUTIVELY_STATIC:				{ OnConsecutivelyStaticPlayTriggered();			break; }
				case PlayType.CONSECUTIVELY_STATIC_PRIORITY:	{ OnConsecutivelyStaticPriorityPlayTriggered(); break; }
				case PlayType.ONLY_ONCE:						{ OnOnlyOncePlayTriggered();					break; }
                default: /*PlayType.ALWAYS*/					{ OnAlwaysPlayTriggered();						break; }
            }
        }
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: On "CONSECUTIVELY" Play Triggered
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnConsecutivelyPlayTriggered()
	{
		if(m_iProvidedAudioID == -1 || !AudioSourceManager.IsPlayingAudio(m_iProvidedAudioID, m_acClipToPlay))
		{
			m_iProvidedAudioID = AudioSourceManager.PlayAudioClip(m_acClipToPlay);
			LowerSceneBGM();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: On "CONSECUTIVELY_STATIC" Play Triggered
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnConsecutivelyStaticPlayTriggered()
	{
		if(sm_iUsedAudioID == -1 || !AudioSourceManager.IsPlayingAudio(sm_iUsedAudioID))
		{
			sm_iUsedAudioID = AudioSourceManager.PlayAudioClip(m_acClipToPlay);
			LowerSceneBGM();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: On "CONSECUTIVELY_STATIC_PRIORITY" Play Triggered
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnConsecutivelyStaticPriorityPlayTriggered()
	{
		if(sm_iUsedAudioID == -1 || !AudioSourceManager.IsPlayingAudio(sm_iUsedAudioID))
		{
			sm_iUsedAudioID = AudioSourceManager.PlayAudioClip(m_acClipToPlay);
			LowerSceneBGM();
        }
		else if(AudioSourceManager.GetSelectedAudioClip(sm_iUsedAudioID) != m_acClipToPlay)
		{
			AudioSourceManager.StopAudio(sm_iUsedAudioID);
			sm_iUsedAudioID = AudioSourceManager.PlayAudioClip(m_acClipToPlay);
			LowerSceneBGM();
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: On "ONLY ONCE" Play Triggered
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnOnlyOncePlayTriggered()
	{
		if(m_iProvidedAudioID == -1)
		{
			m_iProvidedAudioID = AudioSourceManager.PlayAudioClip(m_acClipToPlay);
			LowerSceneBGM();
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: On "Always" Play Triggered
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnAlwaysPlayTriggered()
	{
		m_iProvidedAudioID = AudioSourceManager.PlayAudioClip(m_acClipToPlay);
		LowerSceneBGM();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Lower Scene BGM
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void LowerSceneBGM()
	{
		if(GameManager.CurrentSubscene != null)
		{
			float fadeoutWaitTime = m_acClipToPlay.length;
			GameManager.CurrentSubscene.LowerBGMVolumeForSetTime(fadeoutWaitTime);
		}
	}
}
