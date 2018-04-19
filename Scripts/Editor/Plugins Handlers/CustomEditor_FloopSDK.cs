//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Floop SDK
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

[CustomEditor(typeof(FloopSdk))]
public class CustomEditor_FloopSDK : CustomEditor_Base<FloopSdk>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return  "This script initialises the FloopSDK, which is in charge of\nhandling the Required Parental Gate for iOS platforms.\n\n" +
					"Insert data as required using the app IDs you can\nobtain by going to the Floop Website.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		GameManager gmInstance = Target.gameObject.GetComponent<GameManager>();
		if(gmInstance != null)
		{
			Target.isFullVersion = DrawToggleField("Is Full Version (Read-Only):", gmInstance.m_eBuildVersion != GameManager.BuildVersion.LITE_VERSION, "Is App currently in Full Version Mode? (Tied to GameManager Component)");
		}
		else
		{
			Target.isFullVersion = DrawToggleField("Is Full Version:", Target.isFullVersion, "Is App currently in Full Version Mode?", true);
		}

		EditorGUI.indentLevel += 1;
		{
			if(Target.isFullVersion)
			{
				Target.floopFullAndroidAppKey = EditorGUILayout.TextField(new GUIContent("Floop - Android App Key:", "Get the App Key for this App from the Floop Website by registering the App Details"), Target.floopFullAndroidAppKey);
				Target.floopFullIOSAppKey = EditorGUILayout.TextField(new GUIContent("Floop - iOS App Key:", "Get the App Key for this App from the Floop Website by registering the App Details"), Target.floopFullIOSAppKey);
			}
			else
			{
				Target.floopLiteAndroidAppKey = EditorGUILayout.TextField(new GUIContent("Floop - Android App Key:", "Get the App Key for this App from the Floop Website by registering the App Details"), Target.floopLiteAndroidAppKey);
				Target.floopLiteIOSAppKey = EditorGUILayout.TextField(new GUIContent("Floop - iOS App Key:", "Get the App Key for this App from the Floop Website by registering the App Details"), Target.floopLiteIOSAppKey);
			}
		}
		EditorGUI.indentLevel -= 1;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Debug Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawDebugOptions()
	{
		Target.isDebug = DrawToggleField("Is Debug:", Target.isDebug, "Show debug messages?");
		if(Target.isDebug)
		{
			EditorGUI.indentLevel += 1;
			{
				Target.LogLevel = (Floop.FloopLogLevel)EditorGUILayout.EnumPopup(new GUIContent("Log Level:", "How should the debug logs be presented?"), Target.LogLevel);
			}
			EditorGUI.indentLevel -= 1;
		}
	}
}