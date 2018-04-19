//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Object Zoom Effect
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

[CustomEditor(typeof(ObjectZoomEffect))]
public class CustomEditor_ObjectZoomEffect : CustomEditor_Base<ObjectZoomEffect>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is used to adjust the scale of objects based on a min and max\n" +
					"value. It is to be used in conjunction with other scripts which will give\n" +
					"it the required instructions to do so.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_tObjectToScale = DrawObjectOption("Object To Scale: ", Target.m_tObjectToScale);
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
			Target.m_sSavedOptionKey = EditorGUILayout.TextField(new GUIContent("Key: ", "The key which will be used to store this data (a string). You may use this key in other areas of the app to access the stored data"), Target.m_sSavedOptionKey);
			Target.m_fDefaultValue = EditorGUILayout.FloatField(new GUIContent("Default Value:", "Default Value of the Key when nothing else has been set"), Target.m_fDefaultValue);
			EditorGUI.indentLevel -= 1;
		}

		AddSpaces(2);

		EditorGUILayout.LabelField("Lowest Scale");
		EditorGUI.indentLevel += 1;
		{
			Target.m_vMinPos = EditorGUILayout.Vector3Field("Position: ", Target.m_vMinPos);
			Target.m_vMinScale = EditorGUILayout.Vector3Field("Scale: ", Target.m_vMinScale);
		}
		EditorGUI.indentLevel -= 1;

		AddSpaces(2);

		EditorGUILayout.LabelField("Highest Scale");
		EditorGUI.indentLevel += 1;
		{
			Target.m_vMaxPos = EditorGUILayout.Vector3Field("Position: ", Target.m_vMaxPos);
			Target.m_vMaxScale = EditorGUILayout.Vector3Field("Scale: ", Target.m_vMaxScale);
		}
		EditorGUI.indentLevel -= 1;
	}
}
