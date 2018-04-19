//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Tambourine Sound
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

[CustomEditor(typeof(Button_TambourineSound))]
public class CustomEditor_ButtonTambourineSound : CustomEditor_ButtonBase<Button_TambourineSound>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is handles the input for when the player clicks on a Tambourine\n" +
					"Drum Area. This includes playing associated sound and showing the drum\n" +
					"highlight effect.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawButtonTypeOption();

		Target.m_rSoundManager = DrawObjectOption("Tambourine Sounds Manager: ", Target.m_rSoundManager);
		Target.m_rChallengeGameManager = DrawObjectOption("Challenge Game Manager Ref: ", Target.m_rChallengeGameManager);
		Target.m_eSoundType = (TambourineSoundsManager.SoundTypes)EditorGUILayout.EnumPopup("Sound Type: ", Target.m_eSoundType);

		Target.m_rTambInstrumentManager = DrawObjectOption("Tambourine Instrument Manager Ref: ", Target.m_rTambInstrumentManager);
	}
}
