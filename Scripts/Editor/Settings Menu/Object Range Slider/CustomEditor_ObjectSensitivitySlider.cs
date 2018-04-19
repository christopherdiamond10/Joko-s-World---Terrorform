//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Object Sensitivity Slider
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

[CustomEditor(typeof(ObjectSensitivitySlider))]
public class CustomEditor_ObjectSensitivitySlider : CustomEditor_Base<ObjectSensitivitySlider>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is used to adjust the sensitivty for the Tambourine Shake";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rTambourine = DrawObjectOption("Tambourine Shake Manager: ", Target.m_rTambourine);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_bUseSavedOption = EditorGUILayout.Toggle(new GUIContent("Save Option: ", "If true, the position of the slider will be saved in memory. This means that when the app is next opened, the slider will remain at the same position as last time"), Target.m_bUseSavedOption);
		if (Target.m_bUseSavedOption)
		{
			EditorGUI.indentLevel += 1;
			Target.m_sSavedOptionKey = EditorGUILayout.TextField(new GUIContent("Key : ", "The key which will be used to store this data (a string). You may use this key in other areas of the app to access the stored data"), Target.m_sSavedOptionKey);
			Target.m_fDefaultValue = EditorGUILayout.FloatField(new GUIContent("Default Value:", "Default Value of the Key when nothing else has been set"), Target.m_fDefaultValue);
			EditorGUI.indentLevel -= 1;
		}
	}
}
