//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - URL Opener
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

[CustomEditor(typeof(Button_OpenURL))]
public class CustomEditor_ButtonOpenURL : CustomEditor_ButtonBase<Button_OpenURL>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is handles the input for when the player clicks an button\n" +
					"which will open a url in a web browser (PC/Mobile Devices)";
		}
	}




	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawNonSpriteOptions(false);

		AddSpaces(2);

		Target.m_rOwningPage				= DrawObjectOption("Owning Page:", Target.m_rOwningPage, "When Clicked on; The Owning Page will disappear from view");
		Target.m_rDoNotOpenURLButton		= DrawObjectOption("\"Do Not Open URL\" Button Ref:", Target.m_rDoNotOpenURLButton, "Reference to the \"No\"/\"Do Not Open URL\" button; So that information can be passed along regarding previous objects & scenes");
		Target.m_rConfirmationWindowDisplay = DrawObjectOption("Confirmation Window Display Ref: ", Target.m_rConfirmationWindowDisplay, "When a URL is being contemplated, this window will be opened to display the user some information");
		Target.m_rTextDisplay				= DrawObjectOption("Confirmation Text Display Ref: ", Target.m_rTextDisplay, "Text Component of the Confirmation Window. Used to deliver the changing outbound description.");
	}
	////~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	////	* Overwritten Method: Draw Editable Values Options
	////~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//protected override void DrawEditableValuesOptions()
	//{
	//	Target.m_eType = (Button_OpenURL.URLType)EditorGUILayout.EnumPopup(new GUIContent("URL Type: ", "Will this URL Link work on Both Android & iOS devices; or should the link be different for both (ie Link to the Google Marketplace/Apple App Store)"), Target.m_eType);
	//	if (Target.m_eType == Button_OpenURL.URLType.APPLE_AND_ANDROID)
	//	{
	//		Target.URL = EditorGUILayout.TextField(new GUIContent("HTTP URL Link: ", "Link to open the Web Browser to.\n* Which will work on both Android & iOS Devices"), Target.URL);
	//	}
	//	else
	//	{
	//		Target.androidURL = EditorGUILayout.TextField(new GUIContent("Android ~ HTTP URL Link: ", "Link to open the Web Browser to.\n* Which will work only on Android Devices"), Target.androidURL);
	//		Target.URL = EditorGUILayout.TextField(new GUIContent("Apple ~ HTTP URL Link: ", "Link to open the Web Browser to.\n* Which will only work on iOS Devices"), Target.URL);
	//	}
	//}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Vignette Info Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawVignetteInfoOptions()
	{
		DrawVignetteOptions(Target.m_oVignetteInfo);
	}
}
