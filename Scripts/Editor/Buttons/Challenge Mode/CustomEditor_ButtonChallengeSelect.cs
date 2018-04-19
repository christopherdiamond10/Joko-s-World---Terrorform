//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Challenge Select
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

[CustomEditor(typeof(Button_ChallengeSelect))]
public class CustomEditor_ButtonChallengeSelect : CustomEditor_ButtonBase<Button_ChallengeSelect> 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawNonSpriteOptions(false);
		AddSpaces(3);
		Target.m_rChallengeManager					= DrawObjectOption("Challenge Manager Ref: ", Target.m_rChallengeManager, "Reference to the Challenge Mode");
		Target.m_rChallengeGameTransitioner			= DrawObjectOption("Challenge Game Transitioner:", Target.m_rChallengeGameTransitioner, "The component which transitions from here to the Challenge itself");
		AddSpaces(2);
		Target.m_rChallengeSelectorSubsceneManager	= DrawObjectOption("Challenge Selector Subscene Manager:", Target.m_rChallengeSelectorSubsceneManager, "The Subscene holding the challenge selector");
		Target.m_rChallengeSelectorAnimator			= DrawObjectOption("Challenge Selector Animator Ref: ", Target.m_rChallengeSelectorAnimator, "Reference to The animator which reveals the Challenge Selection Page");
		AddSpaces(2);
		Target.m_rRequiredFeathersIcon				= DrawObjectOption("Required Feathers Icon:", Target.m_rRequiredFeathersIcon, "The Feather Icon to be displayed when showing how many feathers must be earned to unlock this challenge");
		Target.m_rChallengeLockedSymbol				= DrawObjectOption("Challenge Locked Symbol Icon:", Target.m_rChallengeLockedSymbol, "The Lock Symbol to be associated with this Challenge if/when it is locked");
		EditorGUI.indentLevel += 1;
		{
			Target.m_arAwardedFeathersNotifier[0]	= DrawObjectOption("Awarded Challenge Feather Icon ~ 01:", Target.m_arAwardedFeathersNotifier[0], "When the user has earned at least one feather on this challenge, this icon will be highlighted to reflect this achievement");
			Target.m_arAwardedFeathersNotifier[1]	= DrawObjectOption("Awarded Challenge Feather Icon ~ 01:", Target.m_arAwardedFeathersNotifier[1], "When the user has earned at least two feathers on this challenge, this icon will be highlighted to reflect this achievement");
			Target.m_arAwardedFeathersNotifier[2]	= DrawObjectOption("Awarded Challenge Feather Icon ~ 01:", Target.m_arAwardedFeathersNotifier[2], "When the user has earned at least three feathers on this challenge, this icon will be highlighted to reflect this achievement");
		}
		EditorGUI.indentLevel -= 1;
		Target.m_rNeedMoreFeathersPopupManager		= DrawObjectOption("\"NeedMoreFeathers\" Dialogue Pop-up Manager:", Target.m_rNeedMoreFeathersPopupManager, "The Quick Dialogue Pop-up Manager that will show the user that they need more feathers to unlock this challenge");

		if (Target.m_rChallengeManager && Target.m_rChallengeManager.DoesChallengeRequireFullVersion(Target.m_eChallengeID))
		{
			AddSpaces(2);
			Target.m_rFullGamePlugScreen			= DrawObjectOption("Full-Game Plug Screen Ref: ", Target.m_rFullGamePlugScreen, "Reference to the \"Buy the Full-Game\" Dialogue Window");
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_eChallengeID = (SoundsRhythmMemoryGame.Playlist)EditorGUILayout.EnumPopup(new GUIContent("Challenge ID: ", "Which CHallenge will be opened up when this Button is Tapped?"), Target.m_eChallengeID);
		AddSpaces(2);
		Target.m_cButtonUnavailableColour	= EditorGUILayout.ColorField(new GUIContent("Button Available Colour:", "Colour of the Button when the the Challenge is unavailable"), Target.m_cButtonUnavailableColour);
		Target.m_cNormalTextColour			= EditorGUILayout.ColorField(new GUIContent("Normal Text Colour:", "Colour of the Associated Text when the the Challenge is available"), Target.m_cNormalTextColour);
		Target.m_cUnavailableTextColour		= EditorGUILayout.ColorField(new GUIContent("Unavailable Text Colour:", "Colour of the Associated Text when the the Challenge is unavailable"), Target.m_cUnavailableTextColour);
		AddSpaces(2);
		Target.m_acBuzzerSound				= DrawAudioClipOption("Buzzer Sound:", Target.m_acBuzzerSound, "Buzzer sound that will be played whenever the User attempts to open a challenge that is currently unavailable");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Multi Language Text Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawMultiLanguageTextOptions()
	{
		AddSpaces(3);
		DrawLabel("~~~~~ When Challenge Unavailable~~~~~");
		Target.m_oUnavailableText.dm_rTextRenderer = Target.TextRenderer;
		DrawMultiLanguageText(Target.m_oUnavailableText, GameManager.SystemLanguages.TOTAL_LANGUAGES);

		if(GUILayout.Button("Apply New CHallenge Name"))
		{
			Target.TextRenderer.GetComponent<MultiLanguageTextDisplay>().m_oMultiLanguageText.EnglishTranslation = Target.m_rChallengeManager.m_arChallenges[(int)Target.m_eChallengeID].m_oChallengeName.EnglishTranslation.ToUpper();
			Target.TextRenderer.text = Target.TextRenderer.GetComponent<MultiLanguageTextDisplay>().m_oMultiLanguageText.EnglishTranslation;
		}
	}
}
