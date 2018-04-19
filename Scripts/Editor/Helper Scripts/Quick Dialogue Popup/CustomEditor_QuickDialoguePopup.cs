//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Quick Dialogue Popup Box
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
using UnityEditor;

[CustomEditor(typeof(QuickDialoguePopup))]
public class CustomEditor_MoreFeathersRequiredPopupDialogue : CustomEditor_Base<QuickDialoguePopup>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script animates a Dialogue Pop-up box that is meant to be displayed\n" +
					"only briefly. It will show a message and then fade away.\n\n" +
					"It is in use primarily for whenever the user taps on a challenge that has\n" +
					"yet to be unlocked via feather count.";
        }
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rPopupDialogueManager	= DrawObjectOption("Pop-up Dialogue Manager: ", Target.m_rPopupDialogueManager, "The Manager of The Pop Dialogue Messages. It will be used to identify parenting habits");
		Target.m_rBackgroundPanel		= DrawObjectOption("Background Panel: ", Target.m_rBackgroundPanel, "The background Panel of the DialogueBox. This will be what animates");
		Target.m_rNotificationText		= DrawObjectOption("Notification Text Renderer: ", Target.m_rNotificationText, "The Notification Text of the DialogueBox, the opacity of the text will change as it animates");
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimationOptions()
	{
		DrawAnimationEffectOptions(ref Target.m_oFadeAwayAnimation, Target.transform);
	}
}
