//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Display Text Over Time
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

[CustomEditor(typeof(MovementViaCircle))]
public class CustomEditor_MovementViaCircle : CustomEditor_Base<MovementViaCircle>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_bStartWithOppositeDirection = EditorGUILayout.Toggle(new GUIContent("Start With Opposite Direction: ", "If checked, will begin moving in the opposite direction than it normally would"), Target.m_bStartWithOppositeDirection);
		Target.m_bMoveHorizontally = EditorGUILayout.Toggle(new GUIContent("Move Horizontally: ", "Can Move Horizontally?"), Target.m_bMoveHorizontally);
		Target.m_bMoveVertically = EditorGUILayout.Toggle(new GUIContent("Move Vertically: ", "Can Move Vertically?"), Target.m_bMoveVertically);
		
		AddSpaces(3);

		if (Target.m_bMoveHorizontally)
		{
			EditorGUILayout.LabelField("Horizontal Movement", EditorStyles.boldLabel);
			EditorGUI.indentLevel += 1;
			{
				Target.m_fHorizontalMovementBoundary = EditorGUILayout.FloatField("Movement Range: ", Target.m_fHorizontalMovementBoundary);
				Target.m_fHorizontalSpeed = EditorGUILayout.FloatField("Movement Speed: ", Target.m_fHorizontalSpeed);
			}
			EditorGUI.indentLevel -= 1;

			if (Target.m_bMoveVertically)
			{
				AddSpaces(3);
			}
		}

		if (Target.m_bMoveVertically)
		{
			EditorGUILayout.LabelField("Vertical Movement", EditorStyles.boldLabel);
			EditorGUI.indentLevel += 1;
			{
				Target.m_fVerticalMovementBoundary = EditorGUILayout.FloatField("Movement Range: ", Target.m_fVerticalMovementBoundary);
				Target.m_fVerticalSpeed = EditorGUILayout.FloatField("Movement Speed: ", Target.m_fVerticalSpeed);
			}
			EditorGUI.indentLevel -= 1;
		}
	}
}
