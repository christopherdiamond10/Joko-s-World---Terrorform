//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Main Camera Transition
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

[CustomEditor(typeof(MainCameraTransition))]
public class CustomEditor_MainCameraTransition : CustomEditor_Base<MainCameraTransition>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return  "This script is used to transition between the two main scenes of the app\n(Title Screen & Game). As such, it handles the fade out/in and\n" +
					"movement/scaling of the scene during transitions.";
		}
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_goTitleSceneObject						= DrawObjectOption("'Title Screen' Scene Object: ", Target.m_goTitleSceneObject);
		Target.m_goGameSceneObject						= DrawObjectOption("'Game Screen' Scene Object: ", Target.m_goGameSceneObject);
		
		AddSpaces(3);

		Target.m_rBackgroundLandscapeTransitionEffect	= DrawObjectOption("'Background Landscape' Transition Effect: ", Target.m_rBackgroundLandscapeTransitionEffect);
		Target.m_rTitleSceneFadeEffect					= DrawObjectOption("'Title Screen' Fade Effect: ", Target.m_rTitleSceneFadeEffect);
		Target.m_rGameSceneFadeEffect					= DrawObjectOption("'Game Screen' Fade Effect: ", Target.m_rGameSceneFadeEffect);
	}
}
