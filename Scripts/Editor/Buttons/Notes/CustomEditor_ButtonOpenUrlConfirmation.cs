//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Open URL Confirmation Window
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

[CustomEditor(typeof(Button_OpenUrlConfirmation))]
public class CustomEditor_ButtonOpenUrlConfirmation : CustomEditor_ButtonBase<Button_OpenUrlConfirmation> 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This button, when clicked, will transfer it's URL data over to a confirmation\n" +
					"window which asks the user if they are sure they wish to leave the app.\n\n" +
					"This is intended to comply with the app/play store guidelines for the \n" +
					"childrens section.";
		}
	}




	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawNonSpriteOptions(false);

		AddSpaces(2);

		Target.m_rOwningScene				= DrawObjectOption("Owning Scene:", Target.m_rOwningScene, "When a choice is selected; components of this scene (Vignette, BGM) will be reactivated. Or the scene itself if it is not active");
		Target.m_rOwningPage				= DrawObjectOption("Owning Page:", Target.m_rOwningPage, "When a choice is selected; the owning note will be shown again if one has been assigned");

		AddSpaces(2);

		Target.m_rUrlButton					= DrawObjectOption("Open URL Button Ref: ", Target.m_rUrlButton, "Reference to the Open URL Button");
		Target.m_rConfirmationWindowDisplay	= DrawObjectOption("Confirmation Window Display Ref: ", Target.m_rConfirmationWindowDisplay, "Reference to the Transition Handler for the 'Open URL Confirmation Window'");
		Target.m_rTextDisplay				= DrawObjectOption("Confirmation Text Display Ref: ", Target.m_rTextDisplay, "Reference to the TextRenderer in the 'Open URL Confirmation Window' which will be replaced with our own text");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_eURLType = (Button_OpenURL.URLType)EditorGUILayout.EnumPopup(new GUIContent("URL Type: ", "Will this URL Link work on Both Android & iOS devices; or should the link be different for both (ie Link to the Google Marketplace/Apple App Store)"), Target.m_eURLType);
		if (Target.m_eURLType == Button_OpenURL.URLType.APPLE_AND_ANDROID)
		{
			Target.m_sAppleURL = EditorGUILayout.TextField(new GUIContent("HTTP URL Link: ", "Link to open the Web Browser to.\n* Which will work on both Android & iOS Devices"), Target.m_sAppleURL);
		}
		else
		{
			Target.m_sAndroidURL = EditorGUILayout.TextField(new GUIContent("Android ~ HTTP URL Link: ", "Link to open the Web Browser to.\n* Which will work only on Android Devices"), Target.m_sAndroidURL);
			Target.m_sAppleURL = EditorGUILayout.TextField(new GUIContent("Apple ~ HTTP URL Link: ", "Link to open the Web Browser to.\n* Which will only work on iOS Devices"), Target.m_sAppleURL);
		}


		AddSpaces(3);
		DrawLabel("URL Confirmation Description", EditorStyles.boldLabel);
		Target.m_oURLConfrimationDescription.dm_rTextRenderer = Target.m_rTextDisplay;
		DrawMultiLanguageText(Target.m_oURLConfrimationDescription, GameManager.SystemLanguages.TOTAL_LANGUAGES, true);
	}
}
