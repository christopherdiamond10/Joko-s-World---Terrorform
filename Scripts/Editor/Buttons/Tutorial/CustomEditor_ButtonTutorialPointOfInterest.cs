//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Tutorial Point of Interest
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

[CustomEditor(typeof(Button_TutorialPointOfInterest))]
public class CustomEditor_ButtonTutorialPointOfInterest : CustomEditor_ButtonBase<Button_TutorialPointOfInterest>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This button overlays on top of other buttons during the tutorial in order\n" +
					"prevent the user from clicking on things we don't want them clicking on\n" +
					"during the tutorial.\n" +
					"When clicked, this button will both call upon the actual button that\n" +
					"needed to be pressed and allow it to do it's job, and inform the tutorial\n" +
					"manager that it's ok to move on to the next phase.";
        }
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rButtonToCall = DrawObjectOption("Point of Interest Button: ", Target.m_rButtonToCall, "Button which will be activated whenever this Tutorial Button is clicked on");
		Target.m_rTutorialManager = DrawObjectOption("Tutorial Manager Ref: ", Target.m_rTutorialManager, "Tutorial Manager Reference: This Tutorial Manager will be notified that it can proceed to the next Point of Interest");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_eContinueType = (Button_TutorialPointOfInterest.ContinueType)EditorGUILayout.EnumPopup(new GUIContent("Tutorial Continue Type: ", "If NEXT_TUTORIAL_POINT:\nThe tutorial will move on to the next phase as listed in the Tutorial System itself.\n\nIf SELECTIVE\nA new option will be available that will allow you to select which specific tutorial point to move on to when this \"Point of Interest\" is clicked"), Target.m_eContinueType);
		if(Target.m_eContinueType == Button_TutorialPointOfInterest.ContinueType.SELECTIVE && Target.m_rTutorialManager != null)
		{
			EditorGUI.indentLevel += 1;
			{
				AddSpaces(1);
				Target.m_iSelectedNextTutorialPointID = CustomEditor_TutorialManager_Base<TutorialManager_Base>.DrawTutorialPointSelectionEnum(new GUIContent("Next Tutorial Point:", "The selected tutorial point will be the next shown tutorial point if this \"TutorialPointOfInterest\" is clicked"), Target.m_iSelectedNextTutorialPointID, Target.m_rTutorialManager);
				AddSpaces(2);
			}
			EditorGUI.indentLevel -= 1;
		}
	}
}
