//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Language Selection Toggle
//             Author: Christopher Diamond
//             Date: February 15, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This button, when clicked, will cause the currently selected language
//		to change. Refreshing currently visible Multi-Language Text-Components to
//		reflect the change.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_LanguageSelectionToggle : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public LanguageSelectionManager m_rLanguageManager;
	public SpriteRenderer m_rButtonBackground;
	public SpriteRenderer m_rCheckBoxRenderer;

	public GameManager.SystemLanguages m_eLanguageSelection = GameManager.SystemLanguages.ENGLISH;
	public AudioClip m_acLanguageUnavailableSoundEffect;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		if(m_rLanguageManager != null && m_rLanguageManager.IsLanguageAvailable(m_eLanguageSelection))
		{
			m_rLanguageManager.SelectLanguage(this);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Play Button Sound
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void PlayButtonSound()
	{
		if(m_rLanguageManager != null && m_rLanguageManager.IsLanguageAvailable(m_eLanguageSelection))
			AudioSourceManager.PlayAudioClip(m_acClipToPlay);
		else
			AudioSourceManager.PlayAudioClip(m_acLanguageUnavailableSoundEffect);
	}
}
