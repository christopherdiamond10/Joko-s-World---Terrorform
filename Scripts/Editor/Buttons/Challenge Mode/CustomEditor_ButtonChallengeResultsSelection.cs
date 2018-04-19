//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Challenge Results Selection
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

[CustomEditor(typeof(Button_ChallengeResultsSelection))]
public class CustomEditor_ButtonChallengeResultsSelection : CustomEditor_ButtonBase<Button_ChallengeResultsSelection>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This button interacts with the ChallengeResultsManager so that the\n" +
					"manager exits and reveals the desired/selected option by the user.";
        }
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawNonSpriteOptions(false);
		AddSpaces(2);
		Target.m_rChallengeResultsManager = DrawObjectOption("Challenge Results Manager:", Target.m_rChallengeResultsManager, "Reference to the Challenge Results Manager. It will be inform of which button (this one) was clicked on and what should happen once the Results Scene fades away.");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_eExitSelection = (ChallengeResultsManager.ExitSelection)EditorGUILayout.EnumPopup(new GUIContent("Exit Selection Type:", "What should occur when this button is clicked on?\n\n* EXIT:\nWill Exit the Challenge Results Screen and return to the Challenge Select Screen.\n\n* Retry:\nWill reopen the challenge that was just attempted and allow the user to play it again.\n\n* Next Challenge:\nAttempts to open the next challenge directly after the attempted one. If it is unavailable currently; will instead return to the Challenge Selection Menu."), Target.m_eExitSelection);
	}
}
