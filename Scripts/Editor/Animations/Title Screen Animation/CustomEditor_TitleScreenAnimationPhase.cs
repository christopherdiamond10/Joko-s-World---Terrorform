//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Title Screen Phase
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

[CustomEditor(typeof(TitleScreenAnimationPhase))]
public class CustomEditor_TitleScreenAnimationPhase : CustomEditor_Base<TitleScreenAnimationPhase>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is manages a single individual Phase during the title screen\n" +
					"sequence. This script is mainly used as a way for Unity's in-built animation\n" +
					"system to communicate when an animation has reached a certain point.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rTitleScreenAnimationPhase = DrawObjectOption("Title Screen Animation Reference: ", Target.m_rTitleScreenAnimationPhase, "Reference to the Title Screen Animation Manager");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Animator Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimatorOptions()
	{
		Target.m_bMoveOnToNextPhase = DrawToggleField("Move Onwards To The Next Phase? (Current Frame)", Target.m_bMoveOnToNextPhase, "Move on to the next Title Screen Animation Component?\n\n* This is an option intended to be used in the Unity Animator");
		Target.m_bDeactivateAnimator = DrawToggleField("Deactivate This Object's Animator? (Current Frame)", Target.m_bDeactivateAnimator, "Deactivate this Component's Animator. Preventing it from animating ever again?\n\n* This is an option intended to be used in the Unity Animator");

		AddSpaces(2);

		Target.m_bKeepAnimatorActive = DrawToggleField("Keep The Animator Activate? (On Next Phase)", Target.m_bKeepAnimatorActive, "When animation phase moves on to the next component; shall we keep animating?");
		Target.m_bDestroyThisScriptOnNextPhase = DrawToggleField("Destroy This Script Instance? (On Next Phase)", Target.m_bDestroyThisScriptOnNextPhase, "When animation phase moves on to the next component; should this script instance be destroyed. Stopping it from doing anything in the scene again?");
	}
}
