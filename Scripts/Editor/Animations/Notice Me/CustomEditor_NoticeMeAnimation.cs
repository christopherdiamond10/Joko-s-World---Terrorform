//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Notice Me Animation
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

[CustomEditor(typeof(NoticeMeAnimation))]
public class CustomEditor_NoticeMeAnimation : CustomEditor_Base<NoticeMeAnimation>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script updates an animation intended to be used to direct the player's\n" +
				   "attention to it. \"Notice Me\"\n" +
				   "It includes a start/end and loop animation giving you flexibility\n" +
				   "to determine how the start animation blends into the loop blending into the\n" +
				   "end animation.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_iLoopedAnimationCycleLoopCount = DrawIntField("Animation Cycle Loop Count: ", Target.m_iLoopedAnimationCycleLoopCount, "How many times will the Looped Animation loop for before continuing on to the End Animation Sequence?");
		Target.m_fWaitTimeBetweenRepeats		= DrawFloatField("Wait Time Between Repeats: ", Target.m_fWaitTimeBetweenRepeats, "How long will the animation wait after finishing before playing again? (in seconds)");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimationOptions()
	{
		DrawLabel("Start Animation", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			DrawAnimationEffectOptions(ref Target.m_aeStartAnimation, Target.transform);
		}
		EditorGUI.indentLevel -= 1;
		AddSpaces(2);

		DrawLabel("Looped Animation", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			DrawAnimationEffectOptions(ref Target.m_aaeLoopedAnimationCycle, Target.transform);
		}
		EditorGUI.indentLevel -= 1;
		AddSpaces(2);

		DrawLabel("End Animation", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			DrawAnimationEffectOptions(ref Target.m_aeEndAnimation, Target.transform);
		}
		EditorGUI.indentLevel -= 1;
	}
}