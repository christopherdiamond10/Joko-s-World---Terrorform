//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Challenge Note Movement
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
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ChallengeNoteMovement))]
public class CustomEditor_ChallengeNoteMovement : CustomEditor_Base<ChallengeNoteMovement>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static bool sm_bAutoPlaceOtherNotations = false;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rChallengeModeInfo = DrawObjectOption("Challenge Mode Info Ref: ", Target.m_rChallengeModeInfo, "Reference to Challenge Mode Info!");

		AddSpaces(2);

		Target.m_acHitSound			= DrawObjectOption("Hit Sound Clip: ", Target.m_acHitSound, "Sound to play when Note Successfully Hit!");
		Target.m_acMissedSound		= DrawObjectOption("Missed Sound Clip: ", Target.m_acMissedSound, "Sound to play when Note Missed!");
		
		AddSpaces(2);

		Sprite newNormSpr			= DrawObjectOption("Normal Sprite: ", Target.m_sprNormalSprite, "Sprite To Show when Note is traveling to the goal!");
		Target.m_sprSuccessSprite	= DrawObjectOption("Success Sprite: ", Target.m_sprSuccessSprite, "Sprite To Show when Note is successfully hit");
		Target.m_sprMissedSprite	= DrawObjectOption("Missed Sprite: ", Target.m_sprMissedSprite, "Sprite To Show when Note is missed");


		// Renaming GameObject based on Selected Sprite and place the sprite where it should be
		if (newNormSpr != Target.m_sprNormalSprite)
		{
			ChangeSpriteInfo(newNormSpr);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		if(Target.m_rChallengeModeInfo != null)
		{
			TambourineSoundsManager.SoundTypes eNewSoundType = (TambourineSoundsManager.SoundTypes)EditorGUILayout.EnumPopup(new GUIContent("Sound Type: ", "What Sound Type is this Representing?"), Target.m_eSoundType);
			if (eNewSoundType != Target.m_eSoundType)
			{
				ChangeSpriteInfo(eNewSoundType);
			}


			float beatPos = DrawFloatField("Beat Position: ", Target.m_fBeatPos, "Which Beat/Metronome Position will this Notation start at?");
			float linePos = DrawFloatField("Line Position: ", Target.m_fLinePos, "Which Line will this Notation sit on?");

			if (beatPos != Target.m_fBeatPos)
			{
				// Move This notation to the specified Beat Position
				Target.m_fBeatPos = beatPos;
				float newX = Target.m_rChallengeModeInfo.victoryLocation.x + Target.m_rChallengeModeInfo.separationDifference.x * beatPos;
				Target.transform.localPosition = new Vector3(newX, Target.transform.localPosition.y, Target.transform.localPosition.z);

				// Placing OTHER Notations after this one on the next few lines over. Makes it look nicer in my opinion!
				if(sm_bAutoPlaceOtherNotations)
				{
					LinkedList<Transform> lOtherNotations = Target.m_rChallengeModeInfo.GetNotations(Target.transform.name);
					if (lOtherNotations.Count > 0)
					{
						newX += Target.m_rChallengeModeInfo.separationDifference.x * 4;
						foreach (Transform otherNotation in lOtherNotations)
						{
							otherNotation.GetComponent<ChallengeNoteMovement>().m_fBeatPos = Target.m_fBeatPos + 4;
							otherNotation.localPosition = new Vector3(newX, otherNotation.localPosition.y, otherNotation.localPosition.z);
						}

						// Place the Next Notation directly after this one. Keeping the remaining notations further away
						lOtherNotations.First.Value.GetComponent<ChallengeNoteMovement>().m_fBeatPos = Target.m_fBeatPos + 1;
						lOtherNotations.First.Value.localPosition = new Vector3(Target.transform.localPosition.x + Target.m_rChallengeModeInfo.separationDifference.x, lOtherNotations.First.Value.localPosition.y, lOtherNotations.First.Value.localPosition.z);
					}
				}
			}

			if (linePos != Target.m_fLinePos)
			{
				// Move This notation to the specified Line Position
				Target.m_fLinePos = linePos;
				float newY = Target.m_rChallengeModeInfo.victoryLocation.y + Target.m_rChallengeModeInfo.separationDifference.y * linePos;
				Target.transform.localPosition = new Vector3(Target.transform.localPosition.x, newY, Target.transform.localPosition.z);
			}
		}
		else
		{
			DrawLabel("No Challenge Mode Info has been assigned!", Color.red);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimationOptions()
	{
		EditorGUILayout.LabelField("To Score Animation!", EditorStyles.boldLabel);
		DrawAnimationEffectOptions(ref Target.m_arToScoreAnimation, Target.transform);

		EditorGUILayout.LabelField("Into Bag Animation!", EditorStyles.boldLabel);
		DrawAnimationEffectOptions(ref Target.m_arIntoBagAnimation, Target.transform);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Debug Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawDebugOptions()
	{
		sm_bAutoPlaceOtherNotations = DrawToggleField("Auto Place Other Notations?: ", sm_bAutoPlaceOtherNotations, "Should the Notations after this one be auto placed?");

		AddSpaces(2);

		Target.m_sprRedAreaNormal		= DrawObjectOption("Red Area Normal Sprite: ", Target.m_sprRedAreaNormal);
		Target.m_sprBlueAreaNormal		= DrawObjectOption("Blue Area Normal Sprite: ", Target.m_sprBlueAreaNormal);
		Target.m_sprYellowAreaNormal	= DrawObjectOption("Yellow Area Normal Sprite: ", Target.m_sprYellowAreaNormal);
		Target.m_sprCymbalNormal		= DrawObjectOption("Cymbal Normal Sprite: ", Target.m_sprCymbalNormal);
		Target.m_sprShakeNormal			= DrawObjectOption("Shake Normal Sprite: ", Target.m_sprShakeNormal);

		AddSpaces(2);

		Target.m_sprRedAreaHighlight	= DrawObjectOption("Red Area Highlight Sprite: ", Target.m_sprRedAreaHighlight);
		Target.m_sprBlueAreaHighlight	= DrawObjectOption("Blue Area Highlight Sprite: ", Target.m_sprBlueAreaHighlight);
		Target.m_sprYellowAreaHighlight = DrawObjectOption("Yellow Area Highlight Sprite: ", Target.m_sprYellowAreaHighlight);
		Target.m_sprCymbalHighlight		= DrawObjectOption("Cymbal Highlight Sprite: ", Target.m_sprCymbalHighlight);
		Target.m_sprShakeHighlight		= DrawObjectOption("Shake Highlight Sprite: ", Target.m_sprShakeHighlight);
		
		AddSpaces(2);

		Target.m_sprAreaSymbolMissed	= DrawObjectOption("Area Symbol Missed Sprite: ", Target.m_sprAreaSymbolMissed);
		Target.m_sprCymbalMissed		= DrawObjectOption("Cymbal Missed Sprite: ", Target.m_sprCymbalMissed);
		Target.m_sprShakeMissed			= DrawObjectOption("Shake Missed Sprite: ", Target.m_sprShakeMissed);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Change Position and Sprite Information
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private KeyValuePair<string, float> ChangePositionAndSpriteInformation(string name)
	{
		string newName = "";
		if (name == "(Challenge Game) Blue Area Icon (Not Triggered)")
		{
			if(Target.m_rChallengeModeInfo != null)
				Target.m_fLinePos = Target.m_rChallengeModeInfo.blueAreaLinePos;
			newName = "~ (Blue)";
			Target.m_sprSuccessSprite = Target.m_sprBlueAreaHighlight;
			Target.m_sprMissedSprite = Target.m_sprAreaSymbolMissed;
		}
		else if (name == "(Challenge Game) Red Area Icon (Not Triggered)")
		{
			if(Target.m_rChallengeModeInfo != null)
				Target.m_fLinePos = Target.m_rChallengeModeInfo.redAreaLinePos;
			newName = "~ (Red)";
			Target.m_sprSuccessSprite = Target.m_sprRedAreaHighlight;
			Target.m_sprMissedSprite = Target.m_sprAreaSymbolMissed;
		}
		else if (name == "(Challenge Game) Yellow Area Icon (Not Triggered)")
		{
			if (Target.m_rChallengeModeInfo != null)
				Target.m_fLinePos = Target.m_rChallengeModeInfo.yellowAreaLinePos;
			newName = "~ (Yellow)";
			Target.m_sprSuccessSprite = Target.m_sprYellowAreaHighlight;
			Target.m_sprMissedSprite = Target.m_sprAreaSymbolMissed;
		}
		else if (name == "(Challenge Game) Shake Icon (Not Triggered)")
		{
			if (Target.m_rChallengeModeInfo != null)
				Target.m_fLinePos = Target.m_rChallengeModeInfo.cymbalLinePos;
			newName = "~ (Shake)";
			Target.m_sprSuccessSprite = Target.m_sprShakeHighlight;
			Target.m_sprMissedSprite = Target.m_sprShakeMissed;
		}
		else
		{
			if (Target.m_rChallengeModeInfo != null)
				Target.m_fLinePos = Target.m_rChallengeModeInfo.shakeLinePos;
			newName = "~ (Cymbal)";
			Target.m_sprSuccessSprite = Target.m_sprCymbalHighlight;
			Target.m_sprMissedSprite = Target.m_sprCymbalMissed;
		}


		// /////
		if (Target.m_rChallengeModeInfo != null)
		{
			float newY = Target.m_rChallengeModeInfo.victoryLocation.y + Target.m_rChallengeModeInfo.separationDifference.y * Target.m_fLinePos;
			return new KeyValuePair<string, float>(newName, newY);
		}
		else
		{
			return new KeyValuePair<string, float>(newName, 0.0f);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Change Sprite Information
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ChangeSpriteInfo(TambourineSoundsManager.SoundTypes eNewSoundType)
	{
		Target.m_eSoundType = eNewSoundType;

		switch (eNewSoundType)
		{
			case TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA:  
				ChangeSpriteInfo(Target.m_sprRedAreaNormal); 
				break; 
			case TambourineSoundsManager.SoundTypes.MIDDLE_TAMBOURINE_AREA: 
				ChangeSpriteInfo(Target.m_sprBlueAreaNormal); 
				break; 
			case TambourineSoundsManager.SoundTypes.OUTER_TAMBOURINE_AREA: 
				ChangeSpriteInfo(Target.m_sprYellowAreaNormal); 
				break; 
			case TambourineSoundsManager.SoundTypes.SOFT_TAMBOURINE_SHAKE: case TambourineSoundsManager.SoundTypes.HARD_TAMBOURINE_SHAKE: case TambourineSoundsManager.SoundTypes.MOVE_TAMBOURINE_SOUND: case TambourineSoundsManager.SoundTypes.RATTLE_TAMBOURINE_SHAKE:
				ChangeSpriteInfo(Target.m_sprShakeNormal); 
				break; 
			default: 
				ChangeSpriteInfo(Target.m_sprCymbalNormal);
				break;
		}
	}

	private void ChangeSpriteInfo(Sprite newNormSpr)
	{
		// Changing to a Nothing (Null)? Remove the name
		if (newNormSpr == null)
		{
			Target.transform.name = Target.transform.name.Replace(ChangePositionAndSpriteInformation(Target.m_sprNormalSprite.name).Key, "");
			Target.m_sprNormalSprite = null;
		}

		else
		{
			// Were we a null Sprite before this? Just extend the name and place where needed.
			if (Target.m_sprNormalSprite == null)
			{
				KeyValuePair<string, float> newInfo = ChangePositionAndSpriteInformation(newNormSpr.name);
				Target.m_sprNormalSprite = newNormSpr;
				Target.transform.localPosition = new Vector3(Target.transform.localPosition.x, newInfo.Value, Target.transform.localPosition.z);
				Target.transform.name += newInfo.Key;
			}

			// Then in that case Remove the previous extension name we gave it and replace it with the new one. Also Change its position to the appropriate line
			else
			{
				string prevName = ChangePositionAndSpriteInformation(Target.m_sprNormalSprite.name).Key;
				KeyValuePair<string, float> newInfo = ChangePositionAndSpriteInformation(newNormSpr.name);
				Target.m_sprNormalSprite = newNormSpr;
				Target.transform.localPosition = new Vector3(Target.transform.localPosition.x, newInfo.Value, Target.transform.localPosition.z);

				if (Target.transform.name.Contains(prevName))
				{
					Target.transform.name = Target.transform.name.Replace(prevName, newInfo.Key);
				}
				else
				{
					Target.transform.name += newInfo.Key;
				}

				// Place Next Notation Right after this one.
				if (Target.m_rChallengeModeInfo != null && sm_bAutoPlaceOtherNotations)
				{
					LinkedList<Transform> lOtherNotations = Target.m_rChallengeModeInfo.GetNotations(Target.transform.name);
					if (lOtherNotations.Count > 0)
					{
						lOtherNotations.First.Value.GetComponent<ChallengeNoteMovement>().m_fBeatPos = Target.m_fBeatPos + 1;
						lOtherNotations.First.Value.localPosition = new Vector3(Target.transform.localPosition.x + Target.m_rChallengeModeInfo.separationDifference.x, lOtherNotations.First.Value.localPosition.y, lOtherNotations.First.Value.localPosition.z);
					}
				}
			}
		}

		// Change the Sprite in the Renderer to match!
		SpriteRenderer sprRend = Target.GetComponent<SpriteRenderer>();
		if (sprRend != null)
			sprRend.sprite = Target.m_sprNormalSprite;
	}
}
