//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Tambourine Targets Sprite Toggle
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

[CustomEditor(typeof(Button_TambourineTargetsToggle))]
public class CustomEditor_ButtonTambourineTargetsToggle : CustomEditor_ButtonBase<Button_TambourineTargetsToggle>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawButtonTypeOption(includeSpaces: false);
		Target.m_acClipToPlay = DrawAudioClipOption("Audio Clip To Play When Clicked: ", Target.m_acClipToPlay);

		Target.m_rTambTargetsManager = DrawObjectOption("Tambourine Targets Manager Ref: ", Target.m_rTambTargetsManager);
		Target.m_sprNormalTambourineIcon = DrawObjectOption("SpriteRenderer of Normal Tambourine: ", Target.m_sprNormalTambourineIcon);
		Target.m_sprTargetTambourineIcon = DrawObjectOption("SpriteRenderer of Targetted Tambourine: ", Target.m_sprTargetTambourineIcon);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void  DrawEditableValuesOptions()
	{
		Target.m_cDisabledColour = EditorGUILayout.ColorField("Disabled Colour: ", Target.m_cDisabledColour);
	}
}