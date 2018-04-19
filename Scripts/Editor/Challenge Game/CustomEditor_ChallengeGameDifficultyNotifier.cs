//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Challenge Game Difficulty Notifier
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

[CustomEditor(typeof(ChallengeGameDifficultyNotifier))]
public class CustomEditor_ChallengeGameDifficultyNotifier : CustomEditor_Base<ChallengeGameDifficultyNotifier>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_asprRendNotifiers[0] = DrawObjectOption("Left Sprite Renderer: ", Target.m_asprRendNotifiers[0]);
		Target.m_asprRendNotifiers[1] = DrawObjectOption("Middle Sprite Renderer: ", Target.m_asprRendNotifiers[1]);
		Target.m_asprRendNotifiers[2] = DrawObjectOption("Right Sprite Renderer: ", Target.m_asprRendNotifiers[2]);

		AddSpaces(3);

		Target.m_sprNonFilledDifficulty = DrawObjectOption("Non-Filled Difficulty Sprite: ", Target.m_sprNonFilledDifficulty);
		Target.m_sprFilledDifficulty = DrawObjectOption("Filled Difficulty Sprite: ", Target.m_sprFilledDifficulty);
	}
}
