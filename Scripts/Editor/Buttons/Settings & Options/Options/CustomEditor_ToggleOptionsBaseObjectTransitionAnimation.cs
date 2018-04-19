//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Toggle Options ~ Object Transition Animation ~ Base
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

public class CustomEditor_ToggleOptionsBaseObjectTransitionAnimation<T> : CustomEditor_Base<T>		where T : ToggleOptions_Base_ObjectTransitionAnimation
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override InspectorRegion[] AdditionalRegions
	{
		get
		{
			return new InspectorRegion[] { new InspectorRegion() { label = "~Selectable Toggle Options~ (Options To Transition To)", representingDrawMethod = DrawSelectableToggleOptions } };
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Selectable Toggle Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawSelectableToggleOptions()
	{
		DrawArrayOptions("Selectable Options:", "m_arSelectableOptions", "Objects that can be transitioned to when selected");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_bSavePreference = DrawToggleField("Save Selected Option?:", Target.m_bSavePreference, "Save this option (when modified) to the registry?. This means that the option will remain at the same value when next using the app");
		if(Target.m_bSavePreference)
		{
			EditorGUI.indentLevel += 1;
			{
				Target.m_sSavedKeyPreference = EditorGUILayout.TextField(new GUIContent("Saved Key ID:", "What ID should be given to the Saved Key. Must be unique to all other saved key IDs in the system"), Target.m_sSavedKeyPreference);
			}
			EditorGUI.indentLevel -= 1;
		}
	}
}
