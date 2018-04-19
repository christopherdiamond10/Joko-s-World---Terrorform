//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Sub Scene Manager
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

[CustomEditor(typeof(SubSceneManager))]
public class CustomEditor_SubSceneManager : CustomEditor_Base<SubSceneManager>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is called upon to Enter, Exit and Update SubScene Areas'\n" +
					"These include for example: the Guide Book & Music Challenges Selection.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rAssignedSubsceneTutorial = DrawObjectOption("Tutorial To Autoplay: ", Target.m_rAssignedSubsceneTutorial, "The tutorial that will be started when this Subscene becomes active. Will not play if Tutorial has already been completed");
		Target.m_rStartSceneObjectTransitionAnimation = DrawObjectOption("Main Scene Object Reveal Animation: ", Target.m_rStartSceneObjectTransitionAnimation, "Reference to the ObjectTransitionAnimation Component attached to the main scene object which will be animated into view upon Subscene becoming active");
		DrawArrayOptions("Scene Exit Buttons:", "m_arExitButtons", "The Button(s) that will force the subscene to exit/hide when clicked. Effectively ending the subscene");
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_fFadeinAudioTime = EditorGUILayout.FloatField(new GUIContent("Audio Fadein Time: ", "How long will it take for the Audio to Fadein when Subscene is activated?"), Target.m_fFadeinAudioTime);
		if (Target.m_rAssignedSubsceneTutorial != null)
		{
			AddSpaces(1);
			Target.m_iTutorialStartPoint = CustomEditor_TutorialManager_Base<TutorialManager_Base>.DrawTutorialPointSelectionEnum(new GUIContent("Tutorial Start Point:", "The tutorial area to start at when \"autoplaying\" tutorial"), Target.m_iTutorialStartPoint, Target.m_rAssignedSubsceneTutorial);
		}
		AddSpaces(3);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Audio Handler Info Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAudioHandlerInfoOptions()
	{
		DrawAudioOptions(Target.m_oSubsceneAudioHandler);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Vignette Info Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawVignetteInfoOptions()
	{
		DrawVignetteOptions(Target.m_oVignetteInfo);
	}
}