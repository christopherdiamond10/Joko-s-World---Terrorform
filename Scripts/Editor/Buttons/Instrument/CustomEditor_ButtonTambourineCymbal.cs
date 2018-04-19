//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button -Tambourine Cymbal
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

[CustomEditor(typeof(Button_TambourineCymbal))]
public class CustomEditor_ButtonTambourineCymbal : CustomEditor_ButtonBase<Button_TambourineCymbal>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is handles the input for when the player clicks on a Tambourine\n" +
					"Cymbal. This includes shaking the cymbal after being pressed and playing\n" +
					"the associated sound.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rSoundManager = DrawObjectOption("Tambourine Sounds Manager: ", Target.m_rSoundManager);
		Target.m_rChallengeGameManager = DrawObjectOption("Challenge Game Manager Ref: ", Target.m_rChallengeGameManager);
		Target.m_rTambInstrumentManager = DrawObjectOption("Tambourine Instrument Manager Ref:", Target.m_rTambInstrumentManager);
		Target.m_eSoundType = (TambourineSoundsManager.SoundTypes)EditorGUILayout.EnumPopup("Sound Type: ", Target.m_eSoundType);

		DrawCymbalSpriteOptions();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		DrawShakeOptions();
		DrawPositionOptions();
		DrawRotationOptions();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Debug Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawDebugOptions()
	{
		DrawButtonOptions();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Cymbal Sprite Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawCymbalSpriteOptions()
	{
		// Draw Cymbal Sprite Options
		Sprite newSprite = Target.GetComponent<SpriteRenderer>().sprite;
		if(newSprite != null && newSprite != Target.m_sprUnPressedSprite && newSprite != Target.m_sprPressedSprite)
		{
			if(newSprite.name.ToUpper().Contains("ANIMATED") || newSprite.name.ToUpper().Contains("SHAKE"))
				Target.m_sprPressedSprite = newSprite;
			else
				Target.m_sprUnPressedSprite = newSprite;
		}
		DrawUnpressedSpriteOption("Normal Sprite: ", false, "Sprite to show when cymbal has not been tapped");
		DrawPressedSpriteOption("Shaken Sprite: ", true, "Sprite to show when cymbal has been tapped");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Shake Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawShakeOptions()
	{
		Target.m_fShakeIntervals = EditorGUILayout.FloatField(new GUIContent("Shake Intervals: ", "How much time will it take to switch from one sprite (\"Normal Cymbal\" to \"Touched Cymbal\") whilst animating a cymbal that has recently been pressed by the user"), Target.m_fShakeIntervals);
		Target.m_fTotalShakeTime = EditorGUILayout.FloatField(new GUIContent("Total Shake Time: ", "How long will the touch animation itself last for"), Target.m_fTotalShakeTime);
		AddSpaces(3);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Position Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawPositionOptions()
	{
		Vector3 previous_normal = Target.m_vNormalPos;
		{
			Target.m_vNormalPos = EditorGUILayout.Vector3Field(new GUIContent("Normal Cymbal Pos: ", "What position will the \"Normal Cymbal\" be located at by default"), Target.m_vNormalPos);
			if (previous_normal != Target.m_vNormalPos)
			{
				Target.transform.localPosition = Target.m_vNormalPos;
			}
		}

		Vector3 previous_shake = Target.m_vShakePos;
		{
			Target.m_vShakePos = EditorGUILayout.Vector3Field(new GUIContent("Touched Cymbal Pos: ", "What position will the \"Touched Cymbal\" be located at by default"), Target.m_vShakePos);
			if (previous_shake != Target.m_vShakePos)
			{
				Target.transform.localPosition = Target.m_vShakePos;
			}
		}

		AddSpaces(3);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Rotation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawRotationOptions()
	{
		Vector3 previous_normal_rotation = Target.m_vNormalRotation;
		{
			Target.m_vNormalRotation = EditorGUILayout.Vector3Field(new GUIContent("Normal Cymbal Rotation: ", "What rotation will the \"Normal Cymbal\" have by default"), Target.m_vNormalRotation);
			if (previous_normal_rotation != Target.m_vNormalRotation)
			{
				Target.transform.localRotation = Quaternion.Euler(Target.m_vNormalRotation);
			}
		}

		Vector3 previous_shaken_rotation = Target.m_vShakeRotation;
		{
			Target.m_vShakeRotation = EditorGUILayout.Vector3Field(new GUIContent("Touched Cymbal Rotation: ", "What rotation will the \"Touched Cymbal\" have by default"), Target.m_vShakeRotation);
			if (previous_shaken_rotation != Target.m_vShakeRotation)
			{
				Target.transform.localRotation = Quaternion.Euler(Target.m_vShakeRotation);
			}
		}

		AddSpaces(3);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Button Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawButtonOptions()
	{
		if(Target.NormalMode) // NORMAL CYMBAL
		{
			EditorGUILayout.LabelField("~Normal Cymbal~");
			if (GUILayout.Button(new GUIContent("Copy Sprite to \"Normal Cymbal\" Sprite", "whatever sprite is attached to the sprite renderer currently will be applied as the new \"Normal Sprite\" for this cymbal.")))
				Target.m_sprUnPressedSprite = Target.GetComponent<SpriteRenderer>().sprite;
			if (GUILayout.Button(new GUIContent("Lock \"Normal Cymbal\" Pos Coordinates", "Whatever the current position coordinates are will be applied to the \"Normal Cymbal\" as it's position to switch to whilst animating between \"touched\" and \"normal\" modes.")))
				Target.m_vNormalPos = Target.transform.localPosition;
			if (GUILayout.Button(new GUIContent("Lock \"Normal Cymbal\" Rotation Coordinates", "Whatever the current rotation coordinates are will be applied to the \"Normal Cymbal\" as it's rotation to switch to whilst animating between \"touched\" and \"normal\" modes.")))
				Target.m_vNormalRotation = Target.transform.localRotation.eulerAngles;
		}
		else				// TOUCHED CYMBAL
		{
			EditorGUILayout.LabelField("~Touched Cymbal~");
			if(GUILayout.Button(new GUIContent("Copy Sprite to \"Touched Cymbal\" Sprite", "whatever sprite is attached to the sprite renderer currently will be applied as the new \"Touched Sprite\" for this cymbal.")))
				Target.m_sprPressedSprite = Target.GetComponent<SpriteRenderer>().sprite;
			if (GUILayout.Button(new GUIContent("Lock \"Touched Cymbal\" Pos Coordinates", "Whatever the current position coordinates are will be applied to the \"Touched Cymbal\" as it's position to switch to whilst animating between \"touched\" and \"normal\" modes.")))
				Target.m_vShakePos = Target.transform.localPosition;
			if (GUILayout.Button(new GUIContent("Lock \"Touched Cymbal\" Rotation Coordinates", "Whatever the current rotation coordinates are will be applied to the \"Touched Cymbal\" as it's rotation to switch to whilst animating between \"touched\" and \"normal\" modes.")))
				Target.m_vShakeRotation = Target.transform.localRotation.eulerAngles;
		}

		// Switch between the two modes!
		if (GUILayout.Button(new GUIContent("Toggle Sprite", "Toggle between the \"Normal Cymbal\" and \"Touched Cymbal\" modes. Revealing different options")))
			Target.ToggleSprite();

		// If wanting to hear what this cymbal sounds like; here's the code to interact with the TambourineSoundManager (AKA: The SoundBank)
		AddSpaces(3);
		if(Target.m_sprUnPressedSprite != null)
		{
			InstrumentManager.InstrumentMode eTambMode = (Target.m_sprUnPressedSprite.name.ToUpper().Contains("RIQ") ? InstrumentManager.InstrumentMode.RIQ_TAMBOURINE : (Target.m_sprUnPressedSprite.name.ToUpper().Contains("KANJIRA") ? InstrumentManager.InstrumentMode.KANJIRA_TAMBOURINE : InstrumentManager.InstrumentMode.PANDEIRO_TAMBOURINE));
			DrawAudioClipOption("Selected Tambourine Sound (Readonly):", Target.m_rSoundManager.GetTambourineSound(eTambMode, Target.m_eSoundType), "The AudioCLip Associated with this Tambourine Cymbal");
		}
	}
}
