//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Challenge Results Manager
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

[CustomEditor(typeof(ChallengeResultsManager))]
public class CustomEditor_ChallengeResultsManager : CustomEditor_Base<ChallengeResultsManager>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script fades in/out the Results Screen for the Challenge Game;\n" +
					"including removing the Challenge Game Options and settings menu.\n\n" +
					"This script also handles input for the buttons located\n" +
					"inside of the results screen.";
        }
	}

	protected override InspectorRegion[] AdditionalRegions
	{
		get
		{
			return new InspectorRegion[] { new InspectorRegion() { label = "~Audio Items~ (AudioClips to Play with the ChallengeResultsScreen", representingDrawMethod = DrawAudioItemsOptions } };
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		if (GUILayout.Button("Show Results"))
		{
			ToggleResults(true);
		}
		if (GUILayout.Button("Hide Results"))
		{
			ToggleResults(false);
		}

		Target.m_rChallengeGameManager				= DrawObjectOption("Challenge Game Manager Ref: ", Target.m_rChallengeGameManager);
		Target.m_rChallengeGameManagerTransition	= DrawObjectOption("Challenge Game Manager Transition Ref: ", Target.m_rChallengeGameManagerTransition);
		Target.m_rChallengeGameSelectionScene		= DrawObjectOption("Challenge Game Selection Ref: ", Target.m_rChallengeGameSelectionScene);
		Target.m_rSettingsMenu						= DrawObjectOption("Settings Menu Ref: ", Target.m_rSettingsMenu);
		Target.m_rEncouragementTextRenderer			= DrawObjectOption("Encouragement Text Ref: ", Target.m_rEncouragementTextRenderer);
		Target.m_rEncouragementTextAnimation		= DrawObjectOption("Encouragement Text Animation:", Target.m_rEncouragementTextAnimation);
        AddSpaces(1);
		Target.m_sprPageBackground					= DrawObjectOption("Page Background Ref: ", Target.m_sprPageBackground);
		Target.m_sprJokosReaction					= DrawObjectOption("Joko's Reaction Ref: ", Target.m_sprJokosReaction);
		AddSpaces(1);
		Target.m_sprChallengeFeatherNotifier		= DrawObjectOption("Challenge Feather Ref: ", Target.m_sprChallengeFeatherNotifier);
		Target.m_rChallengeFeatherNotificationText	= DrawObjectOption("Challenge Feather Notification Text Ref: ", Target.m_rChallengeFeatherNotificationText);
		AddSpaces(1);
		Target.m_imgExperienceBarBackground			= DrawObjectOption("Experience Bar Background Ref: ", Target.m_imgExperienceBarBackground);
		Target.m_imgExperienceBar					= DrawObjectOption("Experience Bar (Filled) Ref: ", Target.m_imgExperienceBar);
		AddSpaces(1);
		Target.m_rFeathersPopupManager				= DrawObjectOption("Feathers Pop-up Manager Ref:", Target.m_rFeathersPopupManager);
        AddSpaces(1);
		Target.m_aSprFeathers[0]					= DrawObjectOption("Joko's Feather ~ 01   Ref: ", Target.m_aSprFeathers[0]);
		Target.m_aSprFeathers[1]					= DrawObjectOption("Joko's Feather ~ 02   Ref:", Target.m_aSprFeathers[1]);
		Target.m_aSprFeathers[2]					= DrawObjectOption("Joko's Feather ~ 03   Ref: ", Target.m_aSprFeathers[2]);
		AddSpaces(1);
		Target.m_rUnlockedItemsNotifierBounceAE		= DrawObjectOption("Unlocked Items Notifier Animation:", Target.m_rUnlockedItemsNotifierBounceAE);
		Target.m_rUnlockedItemsText					= DrawObjectOption("Unlocked Items Notifier Text:", Target.m_rUnlockedItemsText);
        AddSpaces(1);
		Target.m_aResultsButtons[0]					= DrawObjectOption("Exit Button Ref: ", Target.m_aResultsButtons[0]);
		Target.m_aResultsButtons[1]					= DrawObjectOption("Retry Button Ref: ", Target.m_aResultsButtons[1]);
		Target.m_aResultsButtons[2]					= DrawObjectOption("Next Challenge Button Ref: ", Target.m_aResultsButtons[2]);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_fRevealAnimationSpeed		= EditorGUILayout.FloatField("Reveal Animation Speed: ", Target.m_fRevealAnimationSpeed);
		Target.m_fDisappearAnimationSpeed	= EditorGUILayout.FloatField("Disappear Animation Speed: ", Target.m_fDisappearAnimationSpeed);

		AddSpaces(2);

		Target.m_sprEnabledFeatherSprite = DrawObjectOption("Enabled Feather Sprite: ", Target.m_sprEnabledFeatherSprite);
		Target.m_sprDisabledFeatherSprite = DrawObjectOption("Disabled Feather Sprite: ", Target.m_sprDisabledFeatherSprite);

		AddSpaces(2);

		string[] spriteName = new string[4] { "Defeated Reaction: ", "Okay Reaction: ", "Good Reaction: ", "Great Reaction: " };
		string[] labelName = new string[4] { "Defeated Encouragement Text: ", "Okay Encouragement Text: ", "Good Encouragement Text: ", "Great Encouragement Text: " };
		for (int i = 0; i < 4; ++i)
		{
			Sprite newSprite = DrawObjectOption(spriteName[i], Target.m_arReactions[i].reactionImage);
			EditorGUILayout.LabelField(labelName[i], EditorStyles.boldLabel);
			Target.m_arReactions[i].encouragementText.dm_rTextRenderer = Target.m_rEncouragementTextRenderer;
			DrawMultiLanguageText(Target.m_arReactions[i].encouragementText, GameManager.SystemLanguages.TOTAL_LANGUAGES);
			if (newSprite != Target.m_arReactions[i].reactionImage)
			{
				Target.m_arReactions[i].reactionImage = newSprite;
				Target.m_sprJokosReaction.sprite = newSprite;
			}
			AddSpaces(2);
		}

		if(Target.m_rUnlockedItemsText != null)
		{
			AddSpaces(5);
			for(int i = 0; i < Target.m_aoUnlockedItemsText.Length; ++i)
			{
				if(Target.m_aoUnlockedItemsText[i] == null)
					Target.m_aoUnlockedItemsText[i] = new MultiLanguageText();

				Target.m_aoUnlockedItemsText[i].dm_rTextRenderer = Target.m_rUnlockedItemsText;
				EditorGUILayout.LabelField( ((ChallengeResultsManager.UnlockedItems)i).ToString(), EditorStyles.boldLabel);
				DrawMultiLanguageText(Target.m_aoUnlockedItemsText[i], GameManager.SystemLanguages.TOTAL_LANGUAGES);
            }
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Vignette Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawVignetteInfoOptions()
	{
		DrawVignetteOptions(Target.m_oVignetteOptions);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawAudioItemsOptions()
	{
		Target.m_acFeatherGETSoundEffect		= DrawAudioClipOption("Feather GET SFX: ", Target.m_acFeatherGETSoundEffect);
		Target.m_acChallengeNotifierWhooshSFX	= DrawAudioClipOption("Challenge Notifier Whoosh SFX: ", Target.m_acChallengeNotifierWhooshSFX);
		Target.m_acDissapointmentSFX			= DrawAudioClipOption("Disappointment SFX: ", Target.m_acDissapointmentSFX);
		Target.m_acExcitedAudioClip				= DrawAudioClipOption("Excited SFX: ", Target.m_acExcitedAudioClip);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimationOptions()
	{
		if (Target.m_rSettingsMenu != null)
		{
			EditorGUILayout.LabelField("Settings Menu: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rSettingsMenuAE, Target.m_rSettingsMenu.transform);
		}
		if (Target.m_sprPageBackground != null)
		{
			EditorGUILayout.LabelField("Page Background: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rPageBackgroundAE, Target.m_sprPageBackground.transform);
		}
		if (Target.m_sprJokosReaction != null)
		{
			EditorGUILayout.LabelField("Joko's Reaction: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rJokosReactionAE, Target.m_sprJokosReaction.transform);
		}
		if (Target.m_sprChallengeFeatherNotifier != null)
		{
			EditorGUILayout.LabelField("Challenge Feather: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rChallengeFeatherNotifierAE[0], Target.m_sprChallengeFeatherNotifier.transform);
		}
		if (Target.m_rChallengeFeatherNotificationText != null)
		{
			EditorGUILayout.LabelField("Challenge Feather Text Notification: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rChallengeFeatherNotifierAE[1], Target.m_rChallengeFeatherNotificationText.transform);
		}
		if (Target.m_imgExperienceBarBackground != null)
		{
			EditorGUILayout.LabelField("Experience Bar Background: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rExperienceBarAE[0], Target.m_imgExperienceBarBackground.transform);
		}
		if (Target.m_imgExperienceBar != null)
		{
			EditorGUILayout.LabelField("Experience Bar (Filled): ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rExperienceBarAE[1], Target.m_imgExperienceBar.transform);
		}
		if (Target.m_aSprFeathers[0] != null)
		{
			EditorGUILayout.LabelField("Feather ~ 01: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rFeatherSpritesAE[0], Target.m_aSprFeathers[0].transform);
		}
		if (Target.m_aSprFeathers[1] != null)
		{
			EditorGUILayout.LabelField("Feather ~ 02: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rFeatherSpritesAE[1], Target.m_aSprFeathers[1].transform);
		}
		if (Target.m_aSprFeathers[2] != null)
		{
			EditorGUILayout.LabelField("Feather ~ 03: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rFeatherSpritesAE[2], Target.m_aSprFeathers[2].transform);
		}
		if(Target.m_rUnlockedItemsNotifierBounceAE != null)
		{
			EditorGUILayout.LabelField("Unlocked Items Notifier Intro Animation: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rUnlockedItemsNotifierAE, Target.m_rUnlockedItemsNotifierBounceAE.transform);
        }
        if (Target.m_aResultsButtons[0] != null)
		{
			EditorGUILayout.LabelField("Exit Button: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rButtonSpritesAE[0], Target.m_aResultsButtons[0].transform);
		}
		if (Target.m_aResultsButtons[1] != null)
		{
			EditorGUILayout.LabelField("Retry Button: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rButtonSpritesAE[1], Target.m_aResultsButtons[1].transform);
		}
		if (Target.m_aResultsButtons[2] != null)
		{
			EditorGUILayout.LabelField("Next Challenge Button: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rButtonSpritesAE[2], Target.m_aResultsButtons[2].transform);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Toggle Results
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ToggleResults(bool show = true)
	{
		if (Target.m_rEncouragementTextRenderer != null)
		{
			if(show)
			{
				Target.m_rEncouragementTextRenderer.gameObject.SetActive(true);
				Target.m_arReactions[0].encouragementText.ApplyEffects(Target.m_rEncouragementTextRenderer);
			}
			else
			{
				Target.m_rEncouragementTextRenderer.gameObject.SetActive(false);
				Target.m_rEncouragementTextRenderer.text = "";
			}
		}
		if(Target.m_oVignetteOptions != null)
		{
			if(show)
			{
				VignetteManager.CurrentColour = Target.m_oVignetteOptions.newColour;
				VignetteManager.CurrentSortOrder = Target.m_oVignetteOptions.orderInLayer;
			}
			else
			{
				VignetteManager.CurrentAlpha = 0.0f;
			}
		}
		if (Target.m_sprPageBackground != null)
		{
			Target.m_sprPageBackground.gameObject.SetActive(show);
			if (!show) { Target.m_rPageBackgroundAE.ShowBeginTransform(); } else { Target.m_rPageBackgroundAE.ShowEndTransform(); }
		}
		if (Target.m_sprJokosReaction != null)
		{
			Target.m_sprJokosReaction.gameObject.SetActive(show);
			if (!show) { Target.m_rJokosReactionAE.ShowBeginTransform(); } else { Target.m_rJokosReactionAE.ShowEndTransform(); }
		}
		if (Target.m_sprChallengeFeatherNotifier != null)
		{
			Target.m_sprChallengeFeatherNotifier.gameObject.SetActive(show);
			if (!show) { Target.m_rChallengeFeatherNotifierAE[0].ShowBeginTransform(); } else { Target.m_rChallengeFeatherNotifierAE[0].ShowEndTransform(); }
		}
		if (Target.m_rChallengeFeatherNotificationText != null)
		{
			Target.m_rChallengeFeatherNotificationText.gameObject.SetActive(show);
			if (!show) { Target.m_rChallengeFeatherNotifierAE[1].ShowBeginTransform(); } else { Target.m_rChallengeFeatherNotifierAE[1].ShowEndTransform(); }
		}
		if (Target.m_imgExperienceBarBackground != null)
		{
			Target.m_imgExperienceBarBackground.gameObject.SetActive(show);
			if (!show) { Target.m_rExperienceBarAE[0].ShowBeginTransform(); } else { Target.m_rExperienceBarAE[0].ShowEndTransform(); }
		}
		if (Target.m_imgExperienceBar != null)
		{
			Target.m_imgExperienceBar.gameObject.SetActive(show);
			if (!show) { Target.m_rExperienceBarAE[1].ShowBeginTransform(); } else { Target.m_rExperienceBarAE[1].ShowEndTransform(); }
		}
		if (Target.m_aSprFeathers[0] != null)
		{
			Target.m_aSprFeathers[0].gameObject.SetActive(show);
			if (!show) { Target.m_rFeatherSpritesAE[0].ShowBeginTransform(); } else { Target.m_rFeatherSpritesAE[0].ShowEndTransform(); }
		}
		if (Target.m_aSprFeathers[1] != null)
		{
			Target.m_aSprFeathers[1].gameObject.SetActive(show);
			if (!show) { Target.m_rFeatherSpritesAE[1].ShowBeginTransform(); } else { Target.m_rFeatherSpritesAE[1].ShowEndTransform(); }
		}
		if (Target.m_aSprFeathers[2] != null)
		{
			Target.m_aSprFeathers[2].gameObject.SetActive(show);
			if (!show) { Target.m_rFeatherSpritesAE[2].ShowBeginTransform(); } else { Target.m_rFeatherSpritesAE[2].ShowEndTransform(); }
		}
		if (Target.m_aResultsButtons[0] != null)
		{
			Target.m_aResultsButtons[0].gameObject.SetActive(show);
			if (!show) { Target.m_rButtonSpritesAE[0].ShowBeginTransform(); } else { Target.m_rButtonSpritesAE[0].ShowEndTransform(); }
		}
		if (Target.m_aResultsButtons[1] != null)
		{
			Target.m_aResultsButtons[1].gameObject.SetActive(show);
			if (!show) { Target.m_rButtonSpritesAE[1].ShowBeginTransform(); } else { Target.m_rButtonSpritesAE[1].ShowEndTransform(); }
		}
		if (Target.m_aResultsButtons[2] != null)
		{
			Target.m_aResultsButtons[2].gameObject.SetActive(show);
			if (!show) { Target.m_rButtonSpritesAE[2].ShowBeginTransform(); } else { Target.m_rButtonSpritesAE[2].ShowEndTransform(); }
		}
	}
}
