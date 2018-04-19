//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Tambourine Sound
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: December 11, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is handles the input for when the player clicks on a Tambourine
//      Drum Area. This includes playing associated sound and showing the drum
//		highlight effect.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Button_TambourineSound : Button_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TambourineSoundsManager m_rSoundManager;
	public ChallengeGameManager m_rChallengeGameManager;
	public TambourineSoundsManager.SoundTypes m_eSoundType = TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA;
	public InstrumentManager m_rTambInstrumentManager;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		if (m_rTambInstrumentManager != null)
		{
			m_rTambInstrumentManager.InstrumentColoursManager.BeginHighlightEffect(m_eSoundType);
		}

		// Challenge Mode Active? Make Sure Sound can be played before it will can be heard
		if (m_rChallengeGameManager != null && m_rChallengeGameManager.Active)
		{
			if (m_rChallengeGameManager.IsChallenge)
			{
				m_rChallengeGameManager.SuccessfulHit(m_eSoundType);
				if (m_rSoundManager != null)
				{
					AudioSourceManager.PlayAudioClip( m_rSoundManager.GetTambourineSound(m_eSoundType) );
					if(m_rTambInstrumentManager != null)
						m_rTambInstrumentManager.CurrentInstrumentHitCount += 1;
                }
			}
		}

		// Otherwise Play Normally.
		else if (m_rSoundManager != null)
		{
			AudioSourceManager.PlayAudioClip( m_rSoundManager.GetTambourineSound(m_eSoundType) );
			if(m_rTambInstrumentManager != null)
				m_rTambInstrumentManager.CurrentInstrumentHitCount += 1;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Additional Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnAdditionalTrigger()
	{
		base.OnAdditionalTrigger();
		OnTrigger(); // Just pretend like it was the same as the first time. Still play the Sound and show the Visual Cue :D
	}
}
