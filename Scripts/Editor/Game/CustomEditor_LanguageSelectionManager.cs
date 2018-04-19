//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Language Selection Manager
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

[CustomEditor(typeof(LanguageSelectionManager))]
public class CustomEditor_LanguageSelectionManager : CustomEditor_Base<LanguageSelectionManager> 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script does manages the Language Selection Window. Including handling\n" +
					"Language Button Selection Input and disabling visibility options for\n" +
					"non-selected languages.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawArrayOptions("Visible Multi-Language Text Components:", "m_arVisibleMultiLanguageText", "Array of Multi-Language Text Components that are visible when changing the language. These Multi-Language Components will confirm the change with the user.");

		AddSpaces(2);

		if(Target.m_arLanguageSelectionButtons.Length < (int)GameManager.SystemLanguages.TOTAL_LANGUAGES)
			ResizeArray(ref Target.m_arLanguageSelectionButtons, (int)GameManager.SystemLanguages.TOTAL_LANGUAGES);

		for(int i = 0; i < (int)GameManager.SystemLanguages.TOTAL_LANGUAGES; ++i)
		{
			Target.m_arLanguageSelectionButtons[i] = DrawObjectOption(((GameManager.SystemLanguages)i).ToString() + ":", Target.m_arLanguageSelectionButtons[i], "Which Button is associated with this language?");
		}

		AddSpaces(3);

		Target.m_rTickSprite = DrawObjectOption("Tick Sprite:", Target.m_rTickSprite, "Reference to the Sprite which represents the tick when a language is Selected");
		Target.m_rCheckboxSprite = DrawObjectOption("Check-Box Sprite:", Target.m_rCheckboxSprite, "Reference to the CheckBox Sprite which is shown when a language is not selected");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		if(Target.m_abAvailableLanguages.Length != (int)GameManager.SystemLanguages.TOTAL_LANGUAGES)
			ResizeNonReferenceArray(ref Target.m_abAvailableLanguages, (int)GameManager.SystemLanguages.TOTAL_LANGUAGES);

		AddSpaces(2);

		for(int i = 0; i < (int)GameManager.SystemLanguages.TOTAL_LANGUAGES; ++i)
		{
			Target.m_abAvailableLanguages[i] = DrawToggleField(((GameManager.SystemLanguages)i).ToString() + " is Available for View:", Target.m_abAvailableLanguages[i], "Translations for " + ((GameManager.SystemLanguages)i).ToString() + " have been completed and placed in app? Can we view this language?");
		}

		AddSpaces(3);

		Target.m_cNormalColour		= EditorGUILayout.ColorField(new GUIContent("Non-Selected Button Colour:", "Colour of the button when the associated language is not selected"), Target.m_cNormalColour);
		Target.m_cHighlightedColour = EditorGUILayout.ColorField(new GUIContent("Selected Button Colour:", "Colour of the button when the associated language is selected"), Target.m_cHighlightedColour);
		Target.m_cUnavailableColour = EditorGUILayout.ColorField(new GUIContent("Unavailable Button Colour:", "Colour of the button when the selected language is unavailable.\n\nie. Cannot be selected"), Target.m_cUnavailableColour);
	}
}