//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - FullGame Window Selection
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

[CustomEditor(typeof(Button_FullGameWindowSelection))]
public class CustomEditor_ButtonFullGameWindowSelection : CustomEditor_ButtonBase<Button_FullGameWindowSelection>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This button interacts with the 'Full Game Plug Window' so that the\n" +
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
		Target.m_rFullGamePlugRevealManager = DrawObjectOption("'Full-Game' Plug Manager:", Target.m_rFullGamePlugRevealManager, "Reference to the 'Full-Game' Plug Manager. It will be informed of which button (this one) was clicked on and what should happen once the 'Full-Game' Plug Window fades away.");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_eExitSelection = (FullGamePlugReveal.ExitSelection)EditorGUILayout.EnumPopup(new GUIContent("Exit Selection Type:", "What should occur when this button is clicked on?\n\n* EXIT:\nWill Exit the window and return to the previous subscene.\n\n* Open URL Confirmation:\nWill open up the URL Confirmation window asking users' if they are sure they wish to be directed out of the app."), Target.m_eExitSelection);
	}
}
