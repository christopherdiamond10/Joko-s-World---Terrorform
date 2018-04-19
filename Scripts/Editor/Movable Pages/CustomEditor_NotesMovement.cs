//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Notes Movement
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

[CustomEditor(typeof(NotesMovement))]
public class CustomEditor_NotesMovement : CustomEditor_Base<NotesMovement>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is used to both Move & Zoom any page (texture) that displays\n" +
					"notes. Such as the Cultural Notes and the Credits Page.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rSubsceneToHide = DrawObjectOption("Subscene To Hide (if any):", Target.m_rSubsceneToHide, "Is there a subscene attached to this Page? If so, attach here. Subscene will be hidden if the user closes the page by tapping quickly on it.");
		DrawArrayOptions("Objects To Disable When Touched: ", "m_agoDisableOnTouch", "Objects that will be disabled when Note is touched and re-enabled once Note is released");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_fMovementSensitivity	= EditorGUILayout.FloatField(new GUIContent("Movement Sensitivity: ", "Multiplies the movement speed by this amount. Thus adjusting sensitivity by modifying the rate at which the note moves with the user's movement"), Target.m_fMovementSensitivity);
		Target.m_fTapWindow				= EditorGUILayout.FloatField(new GUIContent("Single Tap Window Time: ", "(How Long Is the User Allowed to Touch the Screen Before it's no longer a single tap) {In Seconds}"), Target.m_fTapWindow);
		Target.m_fDoubleTapWindow		= EditorGUILayout.FloatField(new GUIContent("Double Tap Window Time: ", "(How quick must the user touch the screen twice for it to count as a 'double click') {In Seconds}"), Target.m_fDoubleTapWindow);
		Target.m_cIllegalActionColour	= EditorGUILayout.ColorField(new GUIContent("Illegal Action Colour: ", "The colour the Note will transition to once an illegal action is being perform on it (like moving note out of bounds)"), Target.m_cIllegalActionColour);

		AddSpaces(2);

		EditorGUILayout.LabelField("Minimum Scale", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			Target.m_vMinZoomScale			  = EditorGUILayout.Vector3Field(new GUIContent("Scale: ", "The minimum size of the Page that we will support."), Target.m_vMinZoomScale);
			Target.m_vMinMovementBoundaries.x = EditorGUILayout.FloatField(new GUIContent("Left Boundary: ", "Cannot drag any further to the right"), Target.m_vMinMovementBoundaries.x);
			Target.m_vMinMovementBoundaries.z = EditorGUILayout.FloatField(new GUIContent("Right Boundary: ", "Cannot drag any further to the left"), Target.m_vMinMovementBoundaries.z);
			Target.m_vMinMovementBoundaries.y = EditorGUILayout.FloatField(new GUIContent("Top Boundary: ", "Cannot drag any further downwards"), Target.m_vMinMovementBoundaries.y);
			Target.m_vMinMovementBoundaries.w = EditorGUILayout.FloatField(new GUIContent("Bottom Boundary: ", "Cannot drag any further upwards"), Target.m_vMinMovementBoundaries.w);
		}
		EditorGUI.indentLevel -= 1;

		AddSpaces(2);

		EditorGUILayout.LabelField("Maximum Scale", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			Target.m_vMaxZoomScale			  = EditorGUILayout.Vector3Field(new GUIContent("Scale: ", "The maximum size of the Page that we will support."), Target.m_vMaxZoomScale);
			Target.m_vMaxMovementBoundaries.x = EditorGUILayout.FloatField(new GUIContent("Left Boundary: ", "Cannot drag any further to the right"), Target.m_vMaxMovementBoundaries.x);
			Target.m_vMaxMovementBoundaries.z = EditorGUILayout.FloatField(new GUIContent("Right Boundary: ", "Cannot drag any further to the left"), Target.m_vMaxMovementBoundaries.z);
			Target.m_vMaxMovementBoundaries.y = EditorGUILayout.FloatField(new GUIContent("Top Boundary: ", "Cannot drag any further downwards"), Target.m_vMaxMovementBoundaries.y);
			Target.m_vMaxMovementBoundaries.w = EditorGUILayout.FloatField(new GUIContent("Bottom Boundary: ", "Cannot drag any further upwards"), Target.m_vMaxMovementBoundaries.w);
		}
		EditorGUI.indentLevel -= 1;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Debug Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawDebugOptions()
	{
		if (GUILayout.Button("Apply Current Scale to Minimum Zoom Scale"))
		{
			Target.m_vMinZoomScale = Target.transform.localScale;
		}

		if (GUILayout.Button("Apply Current Scale to Maximum Zoom Scale"))
		{
			Target.m_vMaxZoomScale = Target.transform.localScale;
		}

		AddSpaces(2);

		if (GUILayout.Button(new GUIContent("Assign Values To Minimum Scale Boundaries", "Will apply current values to the minimum scale boundary variables. This will only work as intended if the current position is set to the top left of the notes")))
		{
			Target.m_vMinMovementBoundaries.x = Target.transform.localPosition.x;
			Target.m_vMinMovementBoundaries.z = -Target.transform.localPosition.x;
			Target.m_vMinMovementBoundaries.y = (Target.transform.localPosition.y < 0.0f ? Target.transform.localPosition.y : -Target.transform.localPosition.y);
			Target.m_vMinMovementBoundaries.w = (Target.transform.localPosition.y > 0.0f ? Target.transform.localPosition.y : -Target.transform.localPosition.y);
		}

		if (GUILayout.Button(new GUIContent("Assign Values To Maximum Scale Boundaries", "Will apply current values to the maximum scale boundary variables. This will only work as intended if the current position is set to the top left of the notes")))
		{
			Target.m_vMaxMovementBoundaries.x = Target.transform.localPosition.x;
			Target.m_vMaxMovementBoundaries.z = -Target.transform.localPosition.x;
			Target.m_vMaxMovementBoundaries.y = (Target.transform.localPosition.y < 0.0f ? Target.transform.localPosition.y : -Target.transform.localPosition.y);
			Target.m_vMaxMovementBoundaries.w = (Target.transform.localPosition.y > 0.0f ? Target.transform.localPosition.y : -Target.transform.localPosition.y);
		}
	}
}
