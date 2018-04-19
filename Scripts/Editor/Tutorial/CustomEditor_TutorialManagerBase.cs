//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Tutorial Manager - Base
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

public class CustomEditor_TutorialManager_Base<K> : CustomEditor_Base<K>
										 where K  : TutorialManager_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override InspectorRegion[] AdditionalRegions
	{
		get { return new InspectorRegion[] { new InspectorRegion() { label = "~Tutorial Phase Options~ (Editing values related to the Tutorial)", representingDrawMethod = DrawTutorialPhaseOptions } }; }
	}

	protected int FirstTutorialPointID
	{
		get
		{
			return 0;
		}
	}

	protected int FinalTutorialPointID
	{
		get
		{
			return Target.GetType() == typeof(TutorialManager_MainInstrumentArea)	? (int)TutorialManager_MainInstrumentArea.TutorialPhases.PHASES_COUNT - 1	:
				   Target.GetType() == typeof(TutorialManager_GuideBookArea)		? (int)TutorialManager_GuideBookArea.TutorialPhases.PHASES_COUNT - 1		:
				   Target.GetType() == typeof(TutorialManager_MusicChallengesArea)	? (int)TutorialManager_MusicChallengesArea.TutorialPhases.PHASES_COUNT - 1	:
                    1;
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rOwningSubscene			= DrawObjectOption("Owning SubScene: ", Target.m_rOwningSubscene, "Reference to the Owning Subscene; In case of premature quitting by the user, scene BGM and Vignette will show/play if the Scene is actively in focus");
		Target.m_rTutorialTextComponent		= DrawObjectOption("Tutorial Text Component: ", Target.m_rTutorialTextComponent, "Reference to the TextRenderer which will be showing the contents/description of the Tutorial points of interest");
		Target.m_rTextAnimationEffect		= DrawObjectOption("Tutorial Text Animation Effect: ", Target.m_rTextAnimationEffect, "The AnimationEffectHandler for the Tutorial TextBox");
		Target.m_rTextBoxTransitionEffect	= DrawObjectOption("TextBox Transition Effect: ", Target.m_rTextBoxTransitionEffect, "Reference to the ObjectTransitionEffect for the TextBox which both Shows/Hides the textbox from view");
        Target.m_rTextBoxCollider			= DrawObjectOption("TextBox HiddenObjectsPrevention Collider: ", Target.m_rTextBoxCollider, "Reference to the box collider attached to the text box which is there to prevent touching things hidden under the TextBox. Will be disabled/enabled depending on certain points in tutorial");
        Target.m_rTutorialExitButton		= DrawObjectOption("Tutorial Exit Button: ", Target.m_rTutorialExitButton, "Reference to the Exit Button that will quit out of the tutorial when clicked");
		Target.m_rTutorialContinueButton	= DrawObjectOption("Continue Tutorial Button: ", Target.m_rTutorialContinueButton, "Reference to the Confirmation Button; when clicked will move the tutorial forward");

		if (Target.m_arTutorialPhases.Length != FinalTutorialPointID + 1)
		{
			ResizeArray(ref Target.m_arTutorialPhases, FinalTutorialPointID + 1);
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_fTextFadeinTime = EditorGUILayout.FloatField(new GUIContent("Text Fadein Time: ", "How long will it take the new line of text to fade into view?"), Target.m_fTextFadeinTime);
		Target.m_sPlayerPrefsID = EditorGUILayout.TextField(new GUIContent("Player Prefs ID: ", "What will the id of this particular tutorial be? This will be stored in the registry (externally).\n\nUpon completion of tutorial a boolean relating to this ID will be stored. This will prevent the tutorial from starting automatically next time."), Target.m_sPlayerPrefsID);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Tutorial Phase Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawTutorialPhaseOptions()
	{
		DrawTutorialSelectionOptions();
		DrawTutorialPointOfInterestOptions();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Tutorial Point Selection Enum
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static int DrawTutorialPointSelectionEnum(Rect drawPos, int currentSelectionID, TutorialManager_Base tutorialReference)
	{
		if (tutorialReference.GetType() == typeof(TutorialManager_MainInstrumentArea))
		{
			int selectedItem = (int)((TutorialManager_MainInstrumentArea.TutorialPhases)EditorGUI.EnumPopup(drawPos, new GUIContent("", "Changes which set of options you can edit"), (TutorialManager_MainInstrumentArea.TutorialPhases)currentSelectionID));
			return Mathf.Clamp(selectedItem, 0, (int)TutorialManager_MainInstrumentArea.TutorialPhases.PHASES_COUNT - 1);
		}
		else if (tutorialReference.GetType() == typeof(TutorialManager_GuideBookArea))
		{
			int selectedItem = (int)((TutorialManager_GuideBookArea.TutorialPhases)EditorGUI.EnumPopup(drawPos, new GUIContent("", "Changes which set of options you can edit"), (TutorialManager_GuideBookArea.TutorialPhases)currentSelectionID));
			return Mathf.Clamp(selectedItem, 0, (int)TutorialManager_GuideBookArea.TutorialPhases.PHASES_COUNT - 1);
		}
		else if (tutorialReference.GetType() == typeof(TutorialManager_MusicChallengesArea))
		{
			int selectedItem = (int)((TutorialManager_MusicChallengesArea.TutorialPhases)EditorGUI.EnumPopup(drawPos, new GUIContent("", "Changes which set of options you can edit"), (TutorialManager_MusicChallengesArea.TutorialPhases)currentSelectionID));
			return Mathf.Clamp(selectedItem, 0, (int)TutorialManager_MusicChallengesArea.TutorialPhases.PHASES_COUNT - 1);
		}
		else
		{
			return currentSelectionID;
		}
	}

	public static int DrawTutorialPointSelectionEnum(GUIContent label, int currentSelectionID, TutorialManager_Base tutorialReference)
	{
		Rect drawPos = GetScaledRect();
		EditorGUI.LabelField(drawPos, label);
		drawPos.width /= 2;
		drawPos.x += drawPos.width;
		return DrawTutorialPointSelectionEnum(drawPos, currentSelectionID, tutorialReference);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Tutorial Selection Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawTutorialSelectionOptions()
	{
		AddSpaces(1);
		Rect drawPos = GetScaledRect();
		EditorGUI.LabelField(drawPos, new GUIContent("Tutorial Phase to Show:", "Changes which set of options you can edit"));
        drawPos.width /= 2;
		drawPos.x += drawPos.width;
		int iNewSelection = DrawTutorialPointSelectionEnum(drawPos, Target.m_iSelectedTutorialPointID, Target);
		drawPos.width = 80.0f;
		drawPos.x -= 70.0f;
		iNewSelection = Mathf.Clamp(DrawChangeableIntegerOption(drawPos, "", iNewSelection), FirstTutorialPointID - 1, FinalTutorialPointID + 1);
		AddSpaces(2);

		// IF SELECTION CHANGED~
		if(iNewSelection != Target.m_iSelectedTutorialPointID)
		{
			OnSelectedTutorialChanged(Target.m_iSelectedTutorialPointID, iNewSelection);
            Target.m_iSelectedTutorialPointID = Mathf.Clamp(iNewSelection, 0, FinalTutorialPointID);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Tutorial Point Of Interest Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawTutorialPointOfInterestOptions()
	{
		TutorialManager_Base.TutorialPhaseInfo rTutorialInfo = Target.m_arTutorialPhases[Target.m_iSelectedTutorialPointID];
		if(rTutorialInfo == null)
			return;

		AddSpaces(2);

		// CHANGE AMOUNT OF TUTORIAL POINTS OF INTEREST AND ALLOW INSPECTOR TO ASSIGN
		int newSelectedSize = Mathf.Clamp(DrawChangeableIntegerOption("Total Tutorial Points of Interest: ", rTutorialInfo.rButtons.Length, 1, "Change total number of Interactable Buttons during this part of the tutorial", false), 0, 100);
		if(rTutorialInfo.rButtons.Length != newSelectedSize)
		{
			ResizeArray(ref rTutorialInfo.rButtons, newSelectedSize);
			if(newSelectedSize <= 0)
				Target.m_rTutorialContinueButton.gameObject.SetActive(true);
		}

		EditorGUI.indentLevel += 1;
		{
			for(int i = 0; i < rTutorialInfo.rButtons.Length; ++i)
			{
				string label = "Point of Interest Button ~ " + ((i + 1) < 100 ? "0" : "") + ((i + 1) < 10 ? "0" : "") + (i + 1).ToString() + ":";
				rTutorialInfo.rButtons[i] = DrawObjectOption(label, rTutorialInfo.rButtons[i], "Button which the player should be touching at this point in the tutorial");
			}
		}
		EditorGUI.indentLevel -= 1;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Vignette Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawVignetteInfoOptions()
	{
		DrawVignetteOptions(Target.m_arTutorialPhases[Target.m_iSelectedTutorialPointID].rVignetteInfo);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw AudioHandlerInfo Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAudioHandlerInfoOptions()
	{
		DrawAudioOptions(Target.m_oAudioHandlerInfo);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Multi-Language Text Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawMultiLanguageTextOptions()
	{
		Target.m_arTutorialPhases[Target.m_iSelectedTutorialPointID].rTutorialTextDisplay.dm_rTextRenderer = Target.m_rTutorialTextComponent;
		Target.m_arTutorialPhases[Target.m_iSelectedTutorialPointID].rFollowUpTextDisplay.dm_rTextRenderer = Target.m_rTutorialTextComponent;

		GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
		foldoutStyle.fontStyle = FontStyle.Bold;
		foldoutStyle.normal.textColor = new Color32(72, 164, 26, 255);
		foldoutStyle.onActive.textColor = new Color32(0, 0, 255, 255);
		AddSpaces(1);
		Target.dm_bShowTutorialTextOptionsInInspector = EditorGUI.Foldout(GetScaledRect(), Target.dm_bShowTutorialTextOptionsInInspector, "Tutorial Text:", true, foldoutStyle);
		AddSpaces(3);
		if(Target.dm_bShowTutorialTextOptionsInInspector)
		{
			DrawMultiLanguageText(Target.m_arTutorialPhases[Target.m_iSelectedTutorialPointID].rTutorialTextDisplay, GameManager.SystemLanguages.TOTAL_LANGUAGES);
		}
		Target.dm_bShowFollowUpTextOptionsInInspector = EditorGUI.Foldout(GetScaledRect(), Target.dm_bShowFollowUpTextOptionsInInspector, "Follow-Up Text:", true, foldoutStyle);
		AddSpaces(3);
		if(Target.dm_bShowFollowUpTextOptionsInInspector)
		{
			DrawMultiLanguageText(Target.m_arTutorialPhases[Target.m_iSelectedTutorialPointID].rFollowUpTextDisplay, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: On Selected Tutorial Changed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnSelectedTutorialChanged(int previousTutorialID, int currentTutorialID)
	{
		// TURN OFF EVERYTHING~!
		if(currentTutorialID == FirstTutorialPointID - 1 || currentTutorialID == FinalTutorialPointID + 1)
		{
			// TURN OFF ACTIVE COMPONENTS OF CURRENT SELECTION; NOW ACTIVATE COMPONENTS OF THE NEW SELECTION
			foreach(Button_TutorialPointOfInterest button in Target.m_arTutorialPhases[previousTutorialID].rButtons)
				if(button != null)
					button.gameObject.SetActive(false);

			// TURN OFF VIGNETTE
			VignetteManager.CurrentAlpha = 0.0f;

			// TURN OFF TUTORIAL TEXT BOX
			if(Target.m_rTextBoxTransitionEffect != null)
			{
				Target.m_rTextBoxTransitionEffect.m_aRevealAnimationEffect[0].ShowFirstFrame();
				Target.m_rTextBoxTransitionEffect.gameObject.SetActive(false);
			}
		}

		// SWITCH FROM CURRENT TUTORIAL AREA VISUALS TO NEXT TUTORIAL AREA VISUALS~!
		else
		{
			// TURN ON TUTORIAL TEXT BOX
			if(Target.m_rTextBoxTransitionEffect != null)
			{
				Target.m_rTextBoxTransitionEffect.m_aRevealAnimationEffect[0].ShowLastFrame();
				Target.m_rTextBoxTransitionEffect.gameObject.SetActive(true);
			}

			// ENSURE NEW SELECTION HAS ALL REQUIRED COMPONENTS SETUP
			if(Target.m_arTutorialPhases[currentTutorialID] == null)
			{
				Target.m_arTutorialPhases[currentTutorialID] = new TutorialManager_Base.TutorialPhaseInfo();
				CopyTextRendererValuesIntoMultiLanguageTextComponent(Target.m_arTutorialPhases[currentTutorialID].rTutorialTextDisplay.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH], Target.m_rTutorialTextComponent);
            }

			// TURN OFF ACTIVE COMPONENTS OF CURRENT SELECTION; NOW ACTIVATE COMPONENTS OF THE NEW SELECTION
			Target.m_rTutorialContinueButton.gameObject.SetActive(true);
			if(previousTutorialID >= 0 && previousTutorialID < FinalTutorialPointID + 1)
			{
				foreach(Button_TutorialPointOfInterest button in Target.m_arTutorialPhases[previousTutorialID].rButtons)
				{
					if(button != null)
						button.gameObject.SetActive(false);
				}
			}
			foreach(Button_TutorialPointOfInterest button in Target.m_arTutorialPhases[currentTutorialID].rButtons)
			{
				Target.m_rTutorialContinueButton.gameObject.SetActive(false);
				if(button != null)
				{
					button.gameObject.SetActive(true);
				}
			}
			

			// SHOW TEXT THAT'S ASSOCIATED WITH THE NEW SELECTION ALSO
			if(Target.m_rTutorialTextComponent != null && Target.m_arTutorialPhases[currentTutorialID].rTutorialTextDisplay.SelectedFontSize > 0)
				Target.m_arTutorialPhases[currentTutorialID].rTutorialTextDisplay.ApplyEffects(Target.m_rTutorialTextComponent);

			// APPLY SAVED VIGNETTE OPTIONS. If no saved Vignette Options. Copy current Vignette Options
			if(Target.m_arTutorialPhases[currentTutorialID].rVignetteInfo.orderInLayer < 0)
			{
				Target.m_arTutorialPhases[currentTutorialID].rVignetteInfo.newColour = VignetteManager.CurrentColour;
                Target.m_arTutorialPhases[currentTutorialID].rVignetteInfo.orderInLayer = VignetteManager.CurrentSortOrder;
			}
			else
			{
				VignetteManager.CurrentColour = Target.m_arTutorialPhases[currentTutorialID].rVignetteInfo.newColour;
				VignetteManager.CurrentSortOrder = Target.m_arTutorialPhases[currentTutorialID].rVignetteInfo.orderInLayer;
			}
		}
	}
}