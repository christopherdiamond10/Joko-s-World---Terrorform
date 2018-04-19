//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Tutorial Manager - Main Instrument Area
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

[CustomEditor(typeof(TutorialManager_MainInstrumentArea))]
public class CustomEditor_TutorialManagerMainInstrumentArea : CustomEditor_TutorialManager_Base<TutorialManager_MainInstrumentArea>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "Deriving from the TutorialManager_Base class, this specific tutorial is\n" +
					"made and intended for use by the Main Instrument Area. This includes\n" +
					"handling situations that users will find themselves in such as the\n" +
					"SettingsMenu and the InstrumentSpecial; such as blowing into the\n" +
					"microphone (Joko's Flute) or shaking the device (Joko's Tambourine).";
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private ShowState m_eShowstate = ShowState.SHOW_FULL_VERSION_TEXT;

	private enum ShowState
	{
		SHOW_FULL_VERSION_TEXT,
		SHOW_LITE_VERSION_TEXT,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		base.DrawInspectorOptions();
		AddSpaces(2);
		Target.m_rRiqCenterRimButton		= DrawObjectOption("Riq - Center Rim Button: ", Target.m_rRiqCenterRimButton, "Reference to the actual Center Rim Button. Upon moving past this tutorial point. The button will be reactivated, allowing the user to continue playing even whilst on other Tutorial Points");
		Target.m_rRiqMiddleRimButton		= DrawObjectOption("Riq - Middle Rim Button: ", Target.m_rRiqMiddleRimButton, "Reference to the actual Middle Rim Button. Upon moving past this tutorial point. The button will be reactivated, allowing the user to continue playing even whilst on other Tutorial Points");
		Target.m_rRiqOuterRimButton			= DrawObjectOption("Riq - Outer Rim Button: ",  Target.m_rRiqOuterRimButton,  "Reference to the actual Outer Rim Button. Upon moving past this tutorial point. The button will be reactivated, allowing the user to continue playing even whilst on other Tutorial Points");
		AddSpaces(1);
		Target.m_goArrowsToCymbals			= DrawObjectOption("Arrows to Riq-Tambourine Cymbals:", Target.m_goArrowsToCymbals, "Reference to the Parent Object of the Arrows that point to the Riq Cymbals. These arrows will be swicthed off post-tutorial step so that they don't continue to harass the user");
		DrawArrayOptions("Riq- Tutorial Cymbals:", "m_arCymbals", "Reference to the cymbals that are to be tapped during the tutorial. This needs to keep active to allow for smooth animations");
        AddSpaces(1);
		Target.m_rTambourineZoomHandler		= DrawObjectOption("Tambourine Zoom Handler: ", Target.m_rTambourineZoomHandler, "Reference to the Tambourine Zoom Handler; we will be resizing the Tambourine. This reference will be doing the handling for us.");
		Target.m_rShakeDetector				= DrawObjectOption("Tambourine Shake Detector Ref: ", Target.m_rShakeDetector, "Reference to the Tambourine Shake Detector. This reference will inform the tutorial whenever the Tambourine has been Shaken."); 
		Target.m_rInstrumentManager			= DrawObjectOption("Instrument Manager Ref: ", Target.m_rInstrumentManager, "Reference to the Instrument Manager. This will simply be used to store the current Instrument Mode, so that it can be resorted back to the chosen state when the tutorial is finished");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived/Overwritten Method: Draw Multi-Language Text Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawMultiLanguageTextOptions()
	{
		if(Target.m_iSelectedTutorialPointID == (int)TutorialManager_MainInstrumentArea.TutorialPhases.OPTIONS_MENU__KANJIRA_SELECT)
		{
			m_eShowstate = (ShowState)EditorGUILayout.EnumPopup(new GUIContent("Show Lite/Full Version Text:", "Text displayed will be different depending on whether or not the app is Full/Lite version... Which text do you want to edit?"), m_eShowstate);
			AddSpaces(3);
			if(m_eShowstate == ShowState.SHOW_LITE_VERSION_TEXT)
			{
				Target.m_oLiteVersionKanjiraSelectText.dm_rTextRenderer = Target.m_rTutorialTextComponent;
				DrawMultiLanguageText(Target.m_oLiteVersionKanjiraSelectText, GameManager.SystemLanguages.TOTAL_LANGUAGES);
			}
			else
			{
				base.DrawMultiLanguageTextOptions();
			}
		}

		else if(Target.m_iSelectedTutorialPointID == (int)TutorialManager_MainInstrumentArea.TutorialPhases.OPTIONS_MENU__PANDEIRO_SELECT)
		{
			m_eShowstate = (ShowState)EditorGUILayout.EnumPopup(new GUIContent("Show Lite/Full Version Text:", "Text displayed will be different depending on whether or not the app is Full/Lite version... Which text do you want to edit?"), m_eShowstate);
			AddSpaces(3);
			if(m_eShowstate == ShowState.SHOW_LITE_VERSION_TEXT)
			{
				Target.m_oLiteVersionPandeiroSelectText.dm_rTextRenderer = Target.m_rTutorialTextComponent;
				DrawMultiLanguageText(Target.m_oLiteVersionPandeiroSelectText, GameManager.SystemLanguages.TOTAL_LANGUAGES);
			}
			else
			{
				base.DrawMultiLanguageTextOptions();
			}
		}

		else
		{
			base.DrawMultiLanguageTextOptions();
		}
	}
}
