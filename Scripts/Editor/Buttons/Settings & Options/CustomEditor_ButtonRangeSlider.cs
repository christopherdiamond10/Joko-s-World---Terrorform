//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Range Slider
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

[CustomEditor(typeof(Button_RangeSlider))]
public class CustomEditor_ButtonRangeSlider : CustomEditor_ButtonBase<Button_RangeSlider>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is handles the input for when the player clicks and holds on the\n" +
					"Slider Symbol. It includes the code to handle the movement of the slider\n" +
					"and the boundaries of the slider.";
        }
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawButtonTypeOption(includeSpaces: false);
		Target.m_rTextObj = DrawObjectOption("Text UI Reference: ", Target.m_rTextObj, "Reference to a TextRenderer which will be used to displayed the current percentage of the slider");
		Target.m_rObjRangeSliderEffect = DrawObjectOption("Object To Apply Effect Towards: ", Target.m_rObjRangeSliderEffect, "Which script will benefit from out slider value?");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_bSaveOption = EditorGUILayout.Toggle(new GUIContent("Save Option: ", "If true, the position of the slider will be saved in memory. This means that when the app is next opened, the slider will remain at the same position as last time"), Target.m_bSaveOption);
		if (Target.m_bSaveOption)
		{
			EditorGUI.indentLevel += 1;
				Target.m_sSavedOptionKey = EditorGUILayout.TextField(new GUIContent("Key : ", "The key which will be used to store this data (a string). You may use this key in other areas of the app to access the stored data"), Target.m_sSavedOptionKey);
			EditorGUI.indentLevel -= 1;
		}
		Target.m_sLabel = EditorGUILayout.TextField(new GUIContent("Text Label: ", "The text which will show in front of the percentage"), Target.m_sLabel);
		Target.m_fMinPos = EditorGUILayout.FloatField("Min/Left Position: ", Target.m_fMinPos);
		Target.m_fMaxPos = EditorGUILayout.FloatField("Max/Right Position: ", Target.m_fMaxPos);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Slider Option
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawDebugOptions()
	{
		float completion = Target.GetPercentage();
		completion = EditorGUILayout.Slider("Position Select: ", completion, 0.0f, 1.0f);
		if (Target.GetPercentage() != completion)
		{
			Target.SetPosition(completion);
			if (Target.m_rObjRangeSliderEffect != null)
			{
				Target.m_rObjRangeSliderEffect.SetValue(completion);
			}
			if (Target.m_rTextObj != null)
			{
				Target.m_rTextObj.text = Target.m_sLabel + ((int)(completion * 100.0f)).ToString() + "%";
			}
		}
	}
}
