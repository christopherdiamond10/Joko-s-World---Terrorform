//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Challenge Mode Info
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

[CustomEditor(typeof(ChallengeModeInfo))]
public class CustomEditor_ChallengeModeInfo : CustomEditor_Base<ChallengeModeInfo>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Private Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static bool sm_bLinkedAndroidIOSParameters = true;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script does nothing special besides keeping information regarding the\n" +
					"individual challenge modes in the Challenge Game. Such as 'Target Location',\n" +
					"'BPM', and 'Separation Values' used to automate the placement process\n" + 
					"of the Challenge Notes.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		if(GUILayout.Button(new GUIContent("Create From MIDI File", "Create challenge using a information provided via a Midi File")))
			Target.UseMidiForAutoPlacement();
		if(GUILayout.Button(new GUIContent("Import Challenge Info Text File", "Read Information regarding the challenge from a text file")))
			ImportChallengeInfoTextFile();

		Target.m_rGameManager					= DrawObjectOption("Challenge Game Manager Ref: ", Target.m_rGameManager, "Reference to the Challenge Game Manager, which will be used to change the current Instrument and start/stop challenges");
		Target.m_rChallengeTitleText			= DrawObjectOption("Challenge Name Text Ref: ", Target.m_rChallengeTitleText, "Reference to the TextRenderer which will display the name of this challenge");
		Target.m_rInstrumentSoundTypeRenderer	= DrawObjectOption("Instrument Sound Type SprRenderer: ", Target.m_rInstrumentSoundTypeRenderer, "Reference to the SpriteRenderer which will show the Instrument that is to be used during this Challenge");
		//Target.m_rPracticeButton				= DrawObjectOption("\"Example\" Button Ref: ", Target.m_rPracticeButton, "Reference to the Challenge \"Example\" Button which will have it's \"Current Challenge\" reference changed to the reference of this Challenge");
		Target.m_rSoundsRhythmMemoryGame		= DrawObjectOption("Rhythm Game Ref: ", Target.m_rSoundsRhythmMemoryGame, "Reference to the Rhythm Game, so that when this challenge is called so show an example. It can tell the RhythmGame How to perform it");
		AddSpaces(1);
		Target.m_goChallengeNotePrefab			= DrawObjectOption("Challenge Note Prefab: ", Target.m_goChallengeNotePrefab, "Reference to the Challenge Note Prefab, so that this script can Generate/Build challenges for us.");
		AddSpaces(3);
		Target.m_ePlaylistTrack					= (SoundsRhythmMemoryGame.Playlist)Mathf.Clamp((int)((SoundsRhythmMemoryGame.Playlist)EditorGUILayout.EnumPopup(new GUIContent("Representing Track: ", "Which challenge is this object supposed to represent"), Target.m_ePlaylistTrack)), 0, (int)SoundsRhythmMemoryGame.Playlist.TOTAL_PLAYLISTS - 1);


		if (Target.m_rInstrumentSoundTypeRenderer != null)
		{
			InstrumentManager.InstrumentMode eTambMode = (InstrumentManager.InstrumentMode)EditorGUILayout.EnumPopup(new GUIContent("Instrument Type: ", "Which Instrument should be active when this challenge is in effect?"), Target.m_eInstrumentSoundType);
			if (eTambMode != Target.m_eInstrumentSoundType)
			{
				Target.m_eInstrumentSoundType = eTambMode;
				Target.m_sprInstrumentSoundType =	(eTambMode == InstrumentManager.InstrumentMode.NORMAL_TAMBOURINE)	?	Target.m_sprNormalTambourine :
													(eTambMode == InstrumentManager.InstrumentMode.KANJIRA_TAMBOURINE)	?	Target.m_sprKanjiraTambourine :
													(eTambMode == InstrumentManager.InstrumentMode.PANDEIRO_TAMBOURINE)	?	Target.m_sprPandeiroTambourine :
																															Target.m_sprRiqTambourine;
				Target.m_rInstrumentSoundTypeRenderer.sprite = Target.m_sprInstrumentSoundType;
			}			
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_bOnlyAvailableToFullVersion = DrawToggleField("Challenge Is Only Available in Full Version:", Target.m_bOnlyAvailableToFullVersion, "Is this challenge only Available in the Full Version of the App...\n\nie. User has to pay for it?", true);
		EditorGUI.indentLevel += 1;
		{
			if(!Target.m_bOnlyAvailableToFullVersion)
				Target.m_iRequiredLiteVersionFeathers = DrawChangeableIntegerOption("Required Lite Version Feathers:", Target.m_iRequiredLiteVersionFeathers, 1, "Required Feathers to Unlock this Challenge in the Lite Version");

			Target.m_iRequiredFullVersionFeathers = DrawChangeableIntegerOption("Required Full Version Feathers:", Target.m_iRequiredFullVersionFeathers, 1, "Required Feathers to Unlock this Challenge in the Full Version");
		}
		EditorGUI.indentLevel -= 1;
		AddSpaces(4);
		DrawLabel("Challenge Background Music:", new Color32(209, 29, 229, 255));
		DrawAudioOptions(Target.m_oChallengeBackingTrackInfo);
		AddSpaces(4);
		DrawLabel("Android:");
		sm_bLinkedAndroidIOSParameters = EditorGUI.Toggle(RectPos, sm_bLinkedAndroidIOSParameters);
		EditorGUI.indentLevel += 1;
		{
			float bpm	= DrawChangeableFloatOption("BPM: ", Target.BPM, 1.0f, "The Beats Per Minute to be used with this track (Challenge Mode) when using Android");
			float delay	= DrawChangeableFloatOption("Backing Track Delay:", Target.backingTrackDelayTime, 0.01f, "How long will the Backing track be delayed before starting?");
			if (bpm != Target.BPM)
			{
				Target.BPM = bpm;
				if (sm_bLinkedAndroidIOSParameters)
					Target.iosBPM = bpm;
			}
			if (delay != Target.backingTrackDelayTime)
			{
				Target.backingTrackDelayTime = delay;
				if (sm_bLinkedAndroidIOSParameters)
					Target.iosBackingTrackDelay = delay;
			}
		}
		AddSpaces(2);
		DrawLabel("IOS:");
		EditorGUI.indentLevel += 1;
		{
			float bpm	= DrawChangeableFloatOption("BPM: ", Target.iosBPM, 1.0f, "The Beats Per Minute to be used with this track (Challenge Mode) when using IOS");
			float delay	= DrawChangeableFloatOption("Backing Track Delay:", Target.iosBackingTrackDelay, 0.01f, "How long will the Backing track be delayed before starting when using IOS?");
			if (bpm != Target.iosBPM)
			{
				Target.iosBPM = bpm;
				if (sm_bLinkedAndroidIOSParameters)
					Target.BPM = bpm;
			}
			if (delay != Target.iosBackingTrackDelay)
			{
				Target.iosBackingTrackDelay = delay;
				if (sm_bLinkedAndroidIOSParameters)
					Target.backingTrackDelayTime = delay;
			}
		}
		AddSpaces(2);
		Target.targetLocation		= EditorGUILayout.Vector3Field(new GUIContent("Miss Location", "The Position that the notations are aiming to head towards. If this point is reached the note has been missed"), Target.targetLocation);
		Target.victoryLocation		= EditorGUILayout.Vector3Field(new GUIContent("Victory/Goal Location", "The Position area where the notations can be hit. - Also used with automatic placement"), Target.victoryLocation);
		AddSpaces(1);
		Target.separationDifference = EditorGUILayout.Vector2Field(new GUIContent("Separation Difference", "The difference between notation positions, both ways"), Target.separationDifference);
		AddSpaces(3);
		Target.cymbalLinePos		= EditorGUILayout.FloatField(new GUIContent("Cymbal Line Position: ", "Where should the Cymbal be placed automatically on this challenge?"), Target.cymbalLinePos);
		Target.redAreaLinePos		= EditorGUILayout.FloatField(new GUIContent("Red Symbol Line Position: ", "Where should the Red Symbol be placed automatically on this challenge?"), Target.redAreaLinePos);
		Target.blueAreaLinePos		= EditorGUILayout.FloatField(new GUIContent("Blue Symbol Line Position: ", "Where should the Blue Symbol be placed automatically on this challenge?"), Target.blueAreaLinePos);
		Target.yellowAreaLinePos	= EditorGUILayout.FloatField(new GUIContent("Yellow Symbol Line Position: ", "Where should the Yellow Symbol be placed automatically on this challenge?"), Target.yellowAreaLinePos);
		Target.shakeLinePos			= EditorGUILayout.FloatField(new GUIContent("Yellow Symbol Line Position: ", "Where should the Shake Symbol be placed automatically on this challenge?"), Target.shakeLinePos);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Multi-Language Text Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawMultiLanguageTextOptions()
	{
		if(Target.m_rChallengeTitleText != null)
		{
			Target.m_oChallengeName.dm_rTextRenderer = Target.m_rChallengeTitleText;
			DrawMultiLanguageText(Target.m_oChallengeName, GameManager.SystemLanguages.TOTAL_LANGUAGES);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Debug Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawDebugOptions()
	{
		Target.gizmoCircleRadius = EditorGUILayout.FloatField(new GUIContent("Gizmo Circle Radius: ", "Radius of the Circle drawn by the Editor/Gizmos"), Target.gizmoCircleRadius);

		Target.m_sprNormalTambourine	= DrawObjectOption("Normal Tambourine Sprite", Target.m_sprNormalTambourine);
		Target.m_sprKanjiraTambourine	= DrawObjectOption("Kanjira Tambourine Sprite", Target.m_sprKanjiraTambourine);
		Target.m_sprPandeiroTambourine	= DrawObjectOption("Pandeiro Tambourine Sprite", Target.m_sprPandeiroTambourine);
		Target.m_sprRiqTambourine		= DrawObjectOption("Riq Tambourine Sprite", Target.m_sprRiqTambourine);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Import Challenge Info Text File
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ImportChallengeInfoTextFile()
	{
		string openFilename = ShowOpenFileDialogueWindow("txt");
		if(openFilename != "")
		{
			using(System.IO.StreamReader reader = new System.IO.StreamReader(openFilename))
			{
				string[] lines = reader.ReadToEnd().Replace("\r", "").Split('\n');

				// Assign name; BPM; Backing Delay; Backing Volume
				Target.m_oChallengeName.EnglishTranslation = lines[0].Remove(0, "Challenge Name = ".Length);
                Target.BPM = (float)System.Convert.ToDouble(lines[1].Remove(0, "Challenge BPM = ".Length));
				Target.backingTrackDelayTime = (float)System.Convert.ToDouble(lines[3].Remove(0, "Backing Delay = ".Length));
				Target.m_oChallengeBackingTrackInfo.m_fMaxVolume = (float)System.Convert.ToDouble(lines[4].Remove(0, "Backing Volume = ".Length));


				// Show Challenge Name
				if(Target.m_rChallengeTitleText != null)
					Target.m_rChallengeTitleText.text = Target.m_oChallengeName.EnglishTranslation;

				// Apply Backing Track
                string audioClipName = lines[2].Remove(0, "Backing Filename = ".Length);
				if(audioClipName == "(None Imported)" || audioClipName.Length < 3)
				{
					Target.m_oChallengeBackingTrackInfo.m_acAudioToPlay = null;
					Target.dm_sSuggestedBackingTrackFilename = "";
				}
				else
				{
					const string PATH_DIRECTORY = "Assets/Audio/Challenge Backing Tracks/";
                    AudioClip foundAudioClip = AssetDatabase.LoadAssetAtPath(PATH_DIRECTORY + audioClipName + ".ogg", typeof(AudioClip)) as AudioClip;
					if(foundAudioClip == null) foundAudioClip = AssetDatabase.LoadAssetAtPath(PATH_DIRECTORY + audioClipName + ".mp3", typeof(AudioClip)) as AudioClip;
					if(foundAudioClip == null) foundAudioClip = AssetDatabase.LoadAssetAtPath(PATH_DIRECTORY + audioClipName + ".wav", typeof(AudioClip)) as AudioClip;
					if(foundAudioClip == null)
					{
						Target.dm_sSuggestedBackingTrackFilename = audioClipName;
					}
					else
					{
						Target.m_oChallengeBackingTrackInfo.m_acAudioToPlay = foundAudioClip;
						Target.dm_sSuggestedBackingTrackFilename = "";
					}
                }
			}
		}
	}
}
