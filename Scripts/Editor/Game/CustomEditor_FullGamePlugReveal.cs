//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Full Game Plug Reveal
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

[CustomEditor(typeof(FullGamePlugReveal))]
public class CustomEditor_FullGamePlugReveal : CustomEditor_Base<FullGamePlugReveal>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script fades in/out the Full-Game Plug Screen.\n" +
					"including making sure that other pages have been hidden before displaying.\n\n" +
					"This script also handles input for the buttons located inside of the Plug.";
        }
	}

	protected override CustomEditor_Base<FullGamePlugReveal>.InspectorRegion[] AdditionalRegions
	{
		get { return new InspectorRegion[] { new InspectorRegion() { label = "~URL Options~ (Linking to the full version of the app)", representingDrawMethod = DrawURLOptions } }; }
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		if (GUILayout.Button("Show Full-Game Plug Screen"))
		{
			ToggleResults(true);
		}
		if (GUILayout.Button("Hide Full-Game Plug Screen"))
		{
			ToggleResults(false);
		}

		Target.m_goSettingsMenu						= DrawObjectOption("Settings Menu Ref: ", Target.m_goSettingsMenu);
		Target.m_sprPageBackground					= DrawObjectOption("Page Background Ref: ", Target.m_sprPageBackground);
		Target.m_sprJokosInstrumentDisplay			= DrawObjectOption("Joko's Instrument Display Ref: ", Target.m_sprJokosInstrumentDisplay);
		Target.m_rFullGamePlugTextRenderer			= DrawObjectOption("Plug Description Text Renderer Ref: ", Target.m_rFullGamePlugTextRenderer);
		AddSpaces(1);
		Target.m_aSelectableButtons[0]				= DrawObjectOption("\"Grab Now\" Button Ref: ", Target.m_aSelectableButtons[0]);
		Target.m_aSelectableButtons[1]				= DrawObjectOption("\"Maybe Later\" Button Ref: ", Target.m_aSelectableButtons[1]);
		AddSpaces(1);
		Target.m_rOpenURLConfirmationWindow			= DrawObjectOption("Open URL Confirmation Window Ref:: ", Target.m_rOpenURLConfirmationWindow);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_fRevealAnimationSpeed		= EditorGUILayout.FloatField("Reveal Animation Speed: ", Target.m_fRevealAnimationSpeed);
		Target.m_fDisappearAnimationSpeed	= EditorGUILayout.FloatField("Disappear Animation Speed: ", Target.m_fDisappearAnimationSpeed);

		AddSpaces(2);

		DrawLabel("Full-Game Plug Description");
		Target.m_oFullGamePlugTextDescription.dm_rTextRenderer = Target.m_rFullGamePlugTextRenderer;
        DrawMultiLanguageText(Target.m_oFullGamePlugTextDescription, GameManager.SystemLanguages.TOTAL_LANGUAGES);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimationOptions()
	{
		if (Target.m_goSettingsMenu != null)
		{
			EditorGUILayout.LabelField("Settings Menu: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rSettingsMenuAE, Target.m_goSettingsMenu.transform);
		}
		if (Target.m_sprPageBackground != null)
		{
			EditorGUILayout.LabelField("Page Background: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rPageBackgroundAE, Target.m_sprPageBackground.transform);
		}
		if (Target.m_sprJokosInstrumentDisplay != null)
		{
			EditorGUILayout.LabelField("Joko's Instrument Display: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rJokosInstrumentDisplayAE, Target.m_sprJokosInstrumentDisplay.transform);
		}
		if (Target.m_aSelectableButtons[0] != null)
		{
			EditorGUILayout.LabelField("\"Grab Now\" Button: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rButtonSpritesAE[0], Target.m_aSelectableButtons[0].transform);
		}
		if (Target.m_aSelectableButtons[1] != null)
		{
			EditorGUILayout.LabelField("\"Maybe Later\" Button: ", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_rButtonSpritesAE[1], Target.m_aSelectableButtons[1].transform);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Vignette Info Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawVignetteInfoOptions()
	{
		DrawVignetteOptions(Target.m_oVignetteInfo);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw URL Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawURLOptions()
	{
		if (Target.m_asFullGameAndroidURL.Length != (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT)
			ResizeNonReferenceArray(ref Target.m_asFullGameAndroidURL, (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT);
		if (Target.m_asFullGameAppleURL.Length != (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT)
			ResizeNonReferenceArray(ref Target.m_asFullGameAppleURL, (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT);
		
		for (int i = 0; i < (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT; ++i)
		{
			AddSpaces(2);
			DrawLabel(((GameManager.DisplayableLanguagesTypes)i).ToString() + " Full Game Url Links:", EditorStyles.boldLabel);
			EditorGUI.indentLevel += 1;
			{
				Target.m_asFullGameAndroidURL[i] = EditorGUILayout.TextField("Android ~ HTTP URL Link: ", Target.m_asFullGameAndroidURL[i]);
				Target.m_asFullGameAppleURL[i] = EditorGUILayout.TextField("Apple ~ HTTP URL Link: ", Target.m_asFullGameAppleURL[i]);
			}
			EditorGUI.indentLevel -= 1;
		}

		AddSpaces(3);

		DrawLabel("URL Confirmation Description");
		Target.m_oURLConfirmationDescription.dm_rTextRenderer = Target.m_rOpenURLConfirmationWindow.m_rTextDisplay;
		DrawMultiLanguageText(Target.m_oURLConfirmationDescription, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Toggle Results
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ToggleResults(bool show = true)
	{
		if (Target.m_rFullGamePlugTextRenderer != null)
		{
			if(show)
			{
				Target.m_rFullGamePlugTextRenderer.gameObject.SetActive(true);
				Target.m_oFullGamePlugTextDescription.ApplyEffects(Target.m_rFullGamePlugTextRenderer);
			}
			else
			{
				Target.m_rFullGamePlugTextRenderer.gameObject.SetActive(false);
				Target.m_rFullGamePlugTextRenderer.text = "";
			}
		}
		if (Target.m_oVignetteInfo != null)
		{
			if(show)
			{
				VignetteManager.CurrentColour = Target.m_oVignetteInfo.newColour;
				VignetteManager.CurrentSortOrder = Target.m_oVignetteInfo.orderInLayer;
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
		if (Target.m_sprJokosInstrumentDisplay != null)
		{
			Target.m_sprJokosInstrumentDisplay.gameObject.SetActive(show);
			if (!show) { Target.m_rJokosInstrumentDisplayAE.ShowBeginTransform(); } else { Target.m_rJokosInstrumentDisplayAE.ShowEndTransform(); }
		}
		if (Target.m_aSelectableButtons[0] != null)
		{
			Target.m_aSelectableButtons[0].gameObject.SetActive(show);
			if (!show) { Target.m_rButtonSpritesAE[0].ShowBeginTransform(); } else { Target.m_rButtonSpritesAE[0].ShowEndTransform(); }
		}
		if (Target.m_aSelectableButtons[1] != null)
		{
			Target.m_aSelectableButtons[1].gameObject.SetActive(show);
			if (!show) { Target.m_rButtonSpritesAE[1].ShowBeginTransform(); } else { Target.m_rButtonSpritesAE[1].ShowEndTransform(); }
		}
	}
}
