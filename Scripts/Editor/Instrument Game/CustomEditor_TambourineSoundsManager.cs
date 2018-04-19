//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Tambourine Sounds Manager
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

[CustomEditor(typeof(TambourineSoundsManager))]
public class CustomEditor_TambourineSoundsManager : CustomEditor_Base<TambourineSoundsManager>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script manages the Tambourine sounds and their playback. This is heavily\n" +
					"used in conjunction with the SoundBank and SoundRhythmGame.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rInstrumentManager = DrawObjectOption("Instrument Manager Ref: ", Target.m_rInstrumentManager);

		EditorGUILayout.LabelField("Normal Tambourine: ", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			DrawSoundArray(ref Target.m_aacNormalTambourineSounds);
		}
		EditorGUI.indentLevel -= 1;

		EditorGUILayout.LabelField("Pandeiro Tambourine: ", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			DrawSoundArray(ref Target.m_aacPandeiroTambourineSounds);
		}
		EditorGUI.indentLevel -= 1;

		EditorGUILayout.LabelField("Kanjira Tambourine: ", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			DrawSoundArray(ref Target.m_aacKanjiraTambourineSounds);
		}
		EditorGUI.indentLevel -= 1;


		EditorGUILayout.LabelField("Riq Tambourine: ", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			DrawSoundArray(ref Target.m_aacRiqTambourineSounds);
		}
		EditorGUI.indentLevel -= 1;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Sound Array
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawSoundArray(ref AudioClip[] audArray)
	{
		for (int i = 0; i < (int)TambourineSoundsManager.SoundTypes.RATTLE_TAMBOURINE_SHAKE + 1; ++i)
		{
			audArray[i] = DrawObjectOption(((TambourineSoundsManager.SoundTypes)i).ToString() + ": ", audArray[i]);
		}
	}
}