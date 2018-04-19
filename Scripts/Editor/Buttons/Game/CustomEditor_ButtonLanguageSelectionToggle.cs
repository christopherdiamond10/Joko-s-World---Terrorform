//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Language Selection Toggle
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

[CustomEditor(typeof(Button_LanguageSelectionToggle))]
public class CustomEditor_ButtonLanguageSelectionToggle : CustomEditor_ButtonBase<Button_LanguageSelectionToggle>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This button, when clicked, will cause the currently selected language\n" +
					"to change. Refreshing currently visible Multi-Language Text-Components to\n" +
					"reflect the change.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawButtonNameOption();
		DrawButtonTypeOption();
		AddSpaces(3);
		AddSpaces(1);
		Target.m_rLanguageManager = DrawObjectOption("Language Manager:", Target.m_rLanguageManager, "Reference to the Language Manager, which will be informed of changes when this button is clicked");
		EditorGUI.indentLevel += 1;
		{
			Target.m_rButtonBackground = DrawObjectOption("Button Background Renderer:", Target.m_rButtonBackground, "Reference to the SpriteRenderer for the Button Background (the Brown Indented Colour), which uses the SolidColour Material");
			EditorGUI.indentLevel += 1;
			{
				Target.m_rCheckBoxRenderer = DrawObjectOption("CheckBox Renderer:", Target.m_rCheckBoxRenderer, "Reference to the checkbox renderer, which should be a child of this object.");
			}
			EditorGUI.indentLevel -= 1;
		}
		EditorGUI.indentLevel -= 1;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimationOptions()
	{
		Target.m_eLanguageSelection = (GameManager.SystemLanguages)EditorGUILayout.EnumPopup(new GUIContent("Associated Language:", "Which language to switch to when this button is clicked on."), Target.m_eLanguageSelection);
		AddSpaces(2);
		Target.m_acClipToPlay = DrawAudioClipOption("SFX When Language Available:", Target.m_acClipToPlay, "Sound Effect to be played when Button is clicked and the selected language can be displayed (ie. Available)");
		Target.m_acLanguageUnavailableSoundEffect = DrawAudioClipOption("SFX When Language Unavailable:", Target.m_acLanguageUnavailableSoundEffect, "Sound Effect to be played when Button is clicked and the selected language can NOT be displayed (ie. Unavailable)");
	}
}
