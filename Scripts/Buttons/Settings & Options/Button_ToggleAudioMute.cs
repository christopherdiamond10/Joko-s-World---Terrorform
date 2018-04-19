//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Toggle Audio Mute
//             Author: Christopher Diamond
//             Date: February 11, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This button, when clicked, will toggle the muted property of the AudioManager.
//		Muting Audio will stop BackgroundMusic from playing to give the user peace
//		of mind if they don't like the music we have to offer.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_ToggleAudioMute : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public QuickDialoguePopupManager m_rAudioMutedDialoguePopup;
	public QuickDialoguePopupManager m_rAudioUnmutedDialoguePopup;


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();
		AudioSourceManager.MuteBGM = !AudioSourceManager.MuteBGM;

		if(AudioSourceManager.MuteBGM)
		{
			ShowMutedAudioSprite();
			m_rAudioMutedDialoguePopup.ShowQuickDialoguePopup(this.transform);
		}
		else
		{
			ShowUnmutedAudioSprite();
			m_rAudioUnmutedDialoguePopup.ShowQuickDialoguePopup(this.transform);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnEnable()
	{
		base.OnEnable();

		if(AudioSourceManager.MuteBGM)
			ShowMutedAudioSprite();
		else
			ShowUnmutedAudioSprite();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Show Pressed Sprite
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void ShowPressedSprite()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Show Unpressed Sprite
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void ShowUnpressedSprite()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Unmuted Audio Sprite
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ShowUnmutedAudioSprite()
	{
		if(m_sprUnPressedSprite != null && SprRenderer != null)
		{
			SprRenderer.sprite = m_sprUnPressedSprite;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Muted Audio Sprite
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ShowMutedAudioSprite()
	{
		if(m_sprPressedSprite != null && SprRenderer != null)
		{
			SprRenderer.sprite = m_sprPressedSprite;
		}
	}
}
