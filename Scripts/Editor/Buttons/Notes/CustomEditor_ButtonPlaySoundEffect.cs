//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Play Sound Effect
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

[CustomEditor(typeof(Button_PlaySoundEffect))]
public class CustomEditor_ButtonPlaySoundEffect : CustomEditor_ButtonBase<Button_PlaySoundEffect>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This button simply plays a Sound Effect when pressed. It can also prevent\n" +
					"itself from playing multiple times at once if desired.";
		}
	}




	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawNonSpriteOptions(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		const string tooltip =	"ALWAYS:\nSound Will always play when button is pressed. Regardless of other factors.\n\n" +
								"CONSECUTIVELY:\nSound will play when button is pressed, but only if the sound it started playing earlier has looped through to the end.\n\n" +
								"CONSECUTIVELY_STATIC:\nSound will play when button is pressed, but only if no other sound is currently playing.\n\n" +
								"CONSECUTIVELY_STATIC_PRIORITY:\nSound will play when button is pressed, if any other sounds in this group are playing, they will be stopped so that this one can play without sound overlap issues.\n\n" +
								"ONLY_ONCE:\nSound will only play the first time this button is pressed. And will be ignored after that.";
        Target.m_ePlayType = (Button_PlaySoundEffect.PlayType)EditorGUILayout.EnumPopup(new GUIContent("Play Type: ", tooltip), Target.m_ePlayType);
	}
}
