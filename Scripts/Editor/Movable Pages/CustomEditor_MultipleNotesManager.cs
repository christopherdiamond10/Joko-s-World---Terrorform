//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Multiple Notes Manager
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

[CustomEditor(typeof(MultipleNotesManager))]
public class CustomEditor_MultipleNotesManager : CustomEditor_Base<MultipleNotesManager>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is used in conjunction with multiple notes, so as to add extra\n" +
					"functionality to the Notes scripts including the ability to support managing\n" +
					"more than one note.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rPreviousNote = DrawObjectOption("Previous Note: ", Target.m_rPreviousNote, "If we choose to go to the previous page... Which Page would that be?\n\n** Can be NULL");
		Target.m_rNextNote = DrawObjectOption("Next Note: ", Target.m_rNextNote, "If we choose to go to the next page... Which Page would that be?\n\n** Can be NULL");

		AddSpaces(2);

		DrawArrayOptions("Exit Buttons: ", "m_arExitButtons", "Assigned Exit Buttons");
		Target.m_rNotesMovement = DrawObjectOption("Notes Movement Ref: ", Target.m_rNotesMovement, "Reference to the Owned Notes Movement Script. Once reveal animations are finished. Movement script is enabled so that the notes can be moved");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimationOptions()
	{
		DrawAnimationEffectOptions(ref Target.m_aeLeftRevealAnimation, Target.transform);
		DrawAnimationEffectOptions(ref Target.m_aeRightRevealAnimation, Target.transform);
		DrawAnimationEffectOptions(ref Target.m_aeLeftDisappearAnimation, Target.transform);
		DrawAnimationEffectOptions(ref Target.m_aeRightDisappearAnimation, Target.transform);
	}
}
