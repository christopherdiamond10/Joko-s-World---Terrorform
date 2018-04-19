//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Tambourine Shake Detector
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

[CustomEditor(typeof(TambourineShakeDetector))]
public class CustomEditor_TambourineShakeDetector : CustomEditor_Base<TambourineShakeDetector>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script watches out for motion in the devices gyro sensor (or accelerator).\n" +
					"When this is moved beyond the user-defined sensitivity point, it will be\n" +
					"marked as a shake of the device. This script from there plays a sound effect\n" +
					"and changes the visuals of the tambourine temporarily to reflect this.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rInstrumentManager	 = DrawObjectOption("Instrument Manager Ref: ", Target.m_rInstrumentManager);
		Target.m_rTambSoundManager	 = DrawObjectOption("Tambourine Sounds Manager Ref: ", Target.m_rTambSoundManager);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_fShakeShowTime = EditorGUILayout.FloatField(new GUIContent("Shake Visual Cue Time: ", "How long will the shake visual cue appear?"), Target.m_fShakeShowTime);
		AddSpaces(2);
		Target.m_iRattleShakesNeeded = EditorGUILayout.IntField("Consecutive Shakes Required for Rattle Sound: ", Target.m_iRattleShakesNeeded);
		Target.m_fTimeBetweenRattleShakes = EditorGUILayout.FloatField("Allowed Wait Time Between Rattle Shakes: ", Target.m_fTimeBetweenRattleShakes);
		AddSpaces(2);
		Target.MinMovementSpeed = EditorGUILayout.FloatField("Required Movement Speed Before Shake Detection: ", Target.MinMovementSpeed);
		Target.SoftShakeSpeed = EditorGUILayout.FloatField("Speed To Reach For A 'Soft Shake': ", Target.SoftShakeSpeed);
		Target.HardShakeSpeed = EditorGUILayout.FloatField("Speed To Reach For A 'Hard Shake': ", Target.HardShakeSpeed);
		Target.MinShakeValue = EditorGUILayout.FloatField("Min Shake Value: ", Target.MinShakeValue);
		Target.MaxShakeValue = EditorGUILayout.FloatField("Max Shake Value: ", Target.MaxShakeValue);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimationOptions()
	{
		Target.m_fShakeShowTime = EditorGUILayout.FloatField("How Long To Show Shake: ", Target.m_fShakeShowTime);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Debug Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawDebugOptions()
	{
		float percentage = Target.m_fCurrentSliderValue;
		Target.m_fCurrentSliderValue = EditorGUILayout.Slider("Shake Sensitivity: ", Target.m_fCurrentSliderValue, 0.0f, 1.0f);
		if (percentage != Target.m_fCurrentSliderValue)
		{
			Target.SetSensitivity(Target.m_fCurrentSliderValue);
		}

		if (Target.m_rInstrumentManager != null && GUILayout.Button("Toggle Sprite"))
		{
			Target.m_rInstrumentManager.ToggleInstrumentState();
        }
	}
}