//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Toggle Audio Mute Status
//             Author: Christopher Diamond
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    A custom editor is used to add additional functionality to the Unity 
//		inspector when dealing with the aforementioned class data.
//
//	  This includes the addition of adding in buttons or calling a method when a 
//		value is changed.
//	  Most importantly, a custom editor is used to make the inspector more 
//		readable and easier to edit.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Button_ToggleAudioMute))]
public class CustomEditor_ButtonToggleAudioMute : CustomEditor_ButtonBase<Button_ToggleAudioMute> 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This button, when clicked, will toggle the muted property of the AudioManager.\n" +
					"Muting Audio will stop BackgroundMusic from playing to give the user peace\n" +
					"of mind if they don't like the music we have to offer.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawNonSpriteOptions(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Sprite newSprite = DrawObjectOption("Unmuted Audio Button Icon:", Target.m_sprUnPressedSprite, "Button Icon to show when Audio is unmuted");
		if(newSprite != Target.m_sprUnPressedSprite)
		{
			Target.m_sprUnPressedSprite = newSprite;
			if(Target.GetComponent<SpriteRenderer>() != null)
				Target.GetComponent<SpriteRenderer>().sprite = newSprite;
		}

		newSprite = DrawObjectOption("Muted Audio Button Icon:", Target.m_sprPressedSprite, "Button Icon to show when Audio is muted");
		if(newSprite != Target.m_sprPressedSprite)
		{
			Target.m_sprPressedSprite = newSprite;
			if(Target.GetComponent<SpriteRenderer>() != null)
				Target.GetComponent<SpriteRenderer>().sprite = newSprite;
		}

		AddSpaces(3);

		Target.m_rAudioMutedDialoguePopup = DrawObjectOption("Muted Audio - Dialogue Popup Manager:", Target.m_rAudioMutedDialoguePopup, "The dialogue pop-up manager for the occasion when the audio becomes muted");
		Target.m_rAudioUnmutedDialoguePopup = DrawObjectOption("UnMuted Audio - Dialogue Popup Manager:", Target.m_rAudioUnmutedDialoguePopup, "The dialogue pop-up manager for the occasion when the audio becomes unmuted");
	}
}
