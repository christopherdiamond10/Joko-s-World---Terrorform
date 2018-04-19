//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Joko Dance Animation
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

[CustomEditor(typeof(JokoDanceAnimation))]
public class CustomEditor_JokoDanceAnimation : CustomEditor_Base<JokoDanceAnimation> 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_tLeftHandMusicalNotes = DrawObjectOption("Left Side Musical Notes: ", Target.m_tLeftHandMusicalNotes);
		Target.m_tRightHandMusicalNotes = DrawObjectOption("Right Side Musical Notes: ", Target.m_tRightHandMusicalNotes);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_iMinSwayChance = EditorGUILayout.IntField("Lowest Sway Chance: ", Target.m_iMinSwayChance);
		EditorGUI.indentLevel += 1;
		{
			Target.m_iSwayChanceIncrement = EditorGUILayout.IntField("Increment By: ", Target.m_iSwayChanceIncrement);
			Target.m_iMaxFails = EditorGUILayout.IntField("Allowable Fails: ", Target.m_iMaxFails);
		}
		EditorGUI.indentLevel -= 1;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Animaotr Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimatorOptions()
	{
		EditorGUILayout.LabelField("~Animator Options~ (To Be Used With Unity's 2D Animation System)", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			Target.m_bFullSwayCheck			= EditorGUILayout.ToggleLeft(" Check For Full Sway? (Current Frame)", Target.m_bFullSwayCheck);
			Target.m_bDeactivateFullSway	= EditorGUILayout.ToggleLeft(" Stop Full Sway Animation? (Current Frame)", Target.m_bDeactivateFullSway);
			Target.m_bShowLeftMusicalNote	= EditorGUILayout.ToggleLeft(" Show Left Musical Note? (Current Frame)", Target.m_bShowLeftMusicalNote);
			Target.m_bShowRightMusicalNote	= EditorGUILayout.ToggleLeft(" Show Right Musical Note? (Current Frame)", Target.m_bShowRightMusicalNote);
		}
		EditorGUI.indentLevel -= 1;
	}
}
