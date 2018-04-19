//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Facebook Handler
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

[CustomEditor(typeof(FacebookHandler))]
public class CustomEditor_FacebookHandler : CustomEditor_ButtonBase<FacebookHandler>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script integrates a Facebook Sharing Button. Allowing you to set the\n" +
					"Captions & Images in the process.";
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
		Target.m_sShareLinkImageURL = EditorGUILayout.TextField(new GUIContent("Image HTTP URL: ", "URL link to the image that is to be displayed"), Target.m_sShareLinkImageURL);
		AddSpaces(1);
		Target.m_bUseCustomUrl = DrawToggleField("Use a Custom URL: ", Target.m_bUseCustomUrl);
		if (Target.m_bUseCustomUrl)
		{
			EditorGUI.indentLevel += 1;
			{
				Target.m_sCustomUrlLink = EditorGUILayout.TextField(new GUIContent("Custom URL Link: ", "A custom URL link. This link will be used, instead, when a friend of the user clicks to view their shared content, rather than the default generated link"), Target.m_sCustomUrlLink);
			}
			EditorGUI.indentLevel -= 1;
		}

		AddSpaces(3);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Multi-Language Text Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawMultiLanguageTextOptions()
	{
		EditorGUILayout.LabelField("FACEBOOK SHARE TITLE:", EditorStyles.boldLabel);
		DrawMultiLanguageText(Target.m_oShareLinkName, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
		AddSpaces(3);
		EditorGUILayout.LabelField("FACEBOOK SHARE CAPTION:", EditorStyles.boldLabel);
		DrawMultiLanguageText(Target.m_oShareLinkCaption, GameManager.SystemLanguages.TOTAL_LANGUAGES, false);
	}
}
