//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Tutorial Manager - Music Challenges Area
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

[CustomEditor(typeof(TutorialManager_MusicChallengesArea))]
public class CustomEditor_TutorialManagerMusicChallengesArea : CustomEditor_TutorialManager_Base<TutorialManager_MusicChallengesArea>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "Deriving from the TutorialManager_Base class, this specific tutorial is\n" +
					"made and intended for use by the Music Challenges Area. This includes\n" +
					"handling situations that users will find themselves in such as\n" +
					"Moving and hitting a note at the correct time. ";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		base.DrawInspectorOptions();
		AddSpaces(2);
		Target.m_rInstrumentZoomHandler = DrawObjectOption("Instrument Zoom Handler: ", Target.m_rInstrumentZoomHandler, "Reference to the Zoom Handler for the Instrument, the instrument will be resized for the duration of the Tutorial");
		Target.m_rSettingsMenuManager	= DrawObjectOption("Settings Menu Manager:", Target.m_rSettingsMenuManager, "Reference to the Settings Menu so that it can be turned off/on as necessary");
		AddSpaces(2);
		Target.m_rChallengeGameManager = DrawObjectOption("Challenge Game Manager:", Target.m_rChallengeGameManager);
		Target.m_rArrowToExampleButton = DrawObjectOption("Arrow to Challenge Example Button:", Target.m_rArrowToExampleButton, "This arrow will become active/inactive throughout the tutorial, particularly when displaying the actual tutorial");
		AddSpaces(2);
		DrawLabel("Red Tambourine Area:");
		EditorGUI.indentLevel += 2;
		{
			Target.m_oRedTambourineAreaInfo.challengeNote = DrawObjectOption("Red Challenge Note ~ Tutorial:", Target.m_oRedTambourineAreaInfo.challengeNote, "The challenge note associated with the Red area of the Tambourine");
			Target.m_oRedTambourineAreaInfo.instrumentButton = DrawObjectOption("Red Tambourine Sound Button:", Target.m_oRedTambourineAreaInfo.instrumentButton, "The sound button associated with the Red Area on the Tambourine");
			Target.m_oRedTambourineAreaInfo.instrumentHighlight = DrawObjectOption("Red Area Highlight:", Target.m_oRedTambourineAreaInfo.instrumentHighlight, "The highlight component for the Red Area on the Tambourine");
		}
		EditorGUI.indentLevel -= 2;
		DrawLabel("Blue Tambourine Area:");
		EditorGUI.indentLevel += 2;
		{
			Target.m_oBlueTambourineAreaInfo.challengeNote = DrawObjectOption("Blue Challenge Note ~ Tutorial:", Target.m_oBlueTambourineAreaInfo.challengeNote, "The challenge note associated with the Blue area of the Tambourine");
			Target.m_oBlueTambourineAreaInfo.instrumentButton = DrawObjectOption("Blue Tambourine Sound Button:", Target.m_oBlueTambourineAreaInfo.instrumentButton, "The sound button associated with the Blue Area on the Tambourine");
			Target.m_oBlueTambourineAreaInfo.instrumentHighlight = DrawObjectOption("Blue Area Highlight:", Target.m_oBlueTambourineAreaInfo.instrumentHighlight, "The highlight component for the Blue Area on the Tambourine");
		}
		EditorGUI.indentLevel -= 2;
		DrawLabel("Green Tambourine Area:");
		EditorGUI.indentLevel += 2;
		{
			Target.m_oGreenTambourineAreaInfo.challengeNote = DrawObjectOption("Green Challenge Note ~ Tutorial:", Target.m_oGreenTambourineAreaInfo.challengeNote, "The challenge note associated with the Green area of the Tambourine");
			Target.m_oGreenTambourineAreaInfo.instrumentButton = DrawObjectOption("Green Tambourine Sound Button:", Target.m_oGreenTambourineAreaInfo.instrumentButton, "The sound button associated with the Green Area on the Tambourine");
			Target.m_oGreenTambourineAreaInfo.instrumentHighlight = DrawObjectOption("Green Area Highlight:", Target.m_oGreenTambourineAreaInfo.instrumentHighlight, "The highlight component for the Green Area on the Tambourine");
		}
		EditorGUI.indentLevel -= 2;
		DrawLabel("Tambourine Jingle:");
		EditorGUI.indentLevel += 2;
		{
			Target.m_oTambourineJingleInfo.challengeNote = DrawObjectOption("Tambourine Jingle Note ~ Tutorial:", Target.m_oTambourineJingleInfo.challengeNote, "The challenge note associated with the Tambourine Jingle of the Tambourine");
			Target.m_oTambourineJingleInfo.instrumentButton = DrawObjectOption("Tambourine Jingle Sound Button:", Target.m_oTambourineJingleInfo.instrumentButton, "The sound button associated with the Tambourine Jingle on the Tambourine");
			Target.m_oTambourineJingleInfo.instrumentHighlight = DrawObjectOption("Tambourine Jingle Highlight:", Target.m_oTambourineJingleInfo.instrumentHighlight, "The highlight component for the Tambourine Jingle on the Tambourine");
		}
		EditorGUI.indentLevel -= 2;
		AddSpaces(2);
		DrawArrayOptions("Challenge Selection Buttons", "m_arChallengeSelectionButtons", "Make sure the Selection Buttons can update (but not necessarily be pressable) so that text and highlighting is animating still whilst Tutorial is active.");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		base.DrawEditableValuesOptions();
		AddSpaces(2);
		Target.m_vChallengeModeTextBoxPosition	= EditorGUILayout.Vector3Field(new GUIContent("Challenge Mode TextBox Position:", "The new position of the textbox when entering ChallengeMode"), Target.m_vChallengeModeTextBoxPosition);
		Target.m_fPositionLerpTime				= DrawChangeableFloatOption("Lerp Time:", Target.m_fPositionLerpTime, 0.01f, "How long will it take to lerp between the Normal TextBoxPosition and the New TextBox Position when entering ChallengeMode");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Callback Method: On Selected Tutorial Changed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnSelectedTutorialChanged(int previousTutorialID, int currentTutorialID)
	{
		base.OnSelectedTutorialChanged(previousTutorialID, currentTutorialID);

		// Move to Special Position?
		if(Target.m_rTextBoxTransitionEffect != null && currentTutorialID >= (int)TutorialManager_MusicChallengesArea.TutorialPhases.CHALLENGE_MODE_EXPLANATION)
		{
			Target.m_rTextBoxTransitionEffect.transform.localPosition = Target.m_vChallengeModeTextBoxPosition;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Draw Multi-Language Text Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawMultiLanguageTextOptions()
	{
		if(Target.m_iSelectedTutorialPointID == (int)TutorialManager_MusicChallengesArea.TutorialPhases.HIT_ALL_NOTES)
		{
			Target.m_arTutorialPhases[Target.m_iSelectedTutorialPointID].rTutorialTextDisplay.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text = "";
			AddSpaces(2);
			EditorGUILayout.LabelField("~~~Red Tambourine Area Text~~~", EditorStyles.boldLabel);
			{
				Target.m_oRedTambourineAreaInfo.firstAttemptText.dm_rTextRenderer = Target.m_rTutorialTextComponent;
				Target.m_oRedTambourineAreaInfo.retryAttemptText.dm_rTextRenderer = Target.m_rTutorialTextComponent;
				EditorGUILayout.LabelField("~~~First Attempt Text~~~", EditorStyles.boldLabel);
				DrawMultiLanguageText(Target.m_oRedTambourineAreaInfo.firstAttemptText, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
				EditorGUILayout.LabelField("~~~Retry Attempt Text~~~", EditorStyles.boldLabel);
				DrawMultiLanguageText(Target.m_oRedTambourineAreaInfo.retryAttemptText, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
			}
			AddSpaces(2);
			EditorGUILayout.LabelField("~~~Blue Tambourine Area Text~~~", EditorStyles.boldLabel);
			{
				Target.m_oBlueTambourineAreaInfo.firstAttemptText.dm_rTextRenderer = Target.m_rTutorialTextComponent;
				Target.m_oBlueTambourineAreaInfo.retryAttemptText.dm_rTextRenderer = Target.m_rTutorialTextComponent;
				EditorGUILayout.LabelField("~~~First Attempt Text~~~", EditorStyles.boldLabel);
				DrawMultiLanguageText(Target.m_oBlueTambourineAreaInfo.firstAttemptText, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
				EditorGUILayout.LabelField("~~~Retry Attempt Text~~~", EditorStyles.boldLabel);
				DrawMultiLanguageText(Target.m_oBlueTambourineAreaInfo.retryAttemptText, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
			}
			AddSpaces(2);
			EditorGUILayout.LabelField("~~~Green Tambourine Area Text~~~", EditorStyles.boldLabel);
			{
				Target.m_oGreenTambourineAreaInfo.firstAttemptText.dm_rTextRenderer = Target.m_rTutorialTextComponent;
				Target.m_oGreenTambourineAreaInfo.retryAttemptText.dm_rTextRenderer = Target.m_rTutorialTextComponent;
				EditorGUILayout.LabelField("~~~First Attempt Text~~~", EditorStyles.boldLabel);
				DrawMultiLanguageText(Target.m_oGreenTambourineAreaInfo.firstAttemptText, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
				EditorGUILayout.LabelField("~~~Retry Attempt Text~~~", EditorStyles.boldLabel);
				DrawMultiLanguageText(Target.m_oGreenTambourineAreaInfo.retryAttemptText, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
			}
			AddSpaces(2);
			EditorGUILayout.LabelField("~~~Tambourine Jingle Text~~~", EditorStyles.boldLabel);
			{
				Target.m_oTambourineJingleInfo.firstAttemptText.dm_rTextRenderer = Target.m_rTutorialTextComponent;
				Target.m_oTambourineJingleInfo.retryAttemptText.dm_rTextRenderer = Target.m_rTutorialTextComponent;
				EditorGUILayout.LabelField("~~~First Attempt Text~~~", EditorStyles.boldLabel);
				DrawMultiLanguageText(Target.m_oTambourineJingleInfo.firstAttemptText, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
				EditorGUILayout.LabelField("~~~Retry Attempt Text~~~", EditorStyles.boldLabel);
				DrawMultiLanguageText(Target.m_oTambourineJingleInfo.retryAttemptText, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
			}
		}
		else
		{
			base.DrawMultiLanguageTextOptions();
		}
	}
}