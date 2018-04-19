//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Instructions Progression
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

[CustomEditor(typeof(Button_ShowTutorial))]
public class CustomEditor_ButtonShowTutorial : CustomEditor_ButtonBase<Button_ShowTutorial>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawNonSpriteOptions(false);
		AddSpaces(3);
		Target.m_rTutorialManager = DrawObjectOption("Tutorial Manager: ", Target.m_rTutorialManager, "The tutorial to be loading");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		AddSpaces(1);
		Target.m_iTutorialStartPointID = CustomEditor_TutorialManager_Base<TutorialManager_Base>.DrawTutorialPointSelectionEnum(new GUIContent("TutorialPoint Selection:", "The Tutorial Point to Start From"), Target.m_iTutorialStartPointID, Target.m_rTutorialManager);
	}
}
