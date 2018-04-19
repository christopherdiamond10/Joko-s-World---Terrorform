//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Challenge Game Manager
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

[CustomEditor(typeof(ChallengeGameManager))]
public class CustomEditor_ChallengeGameManager : CustomEditor_Base<ChallengeGameManager>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script simply keeps track of and integrates with the challenge game.\n" +
					"Allowing other scripts to interact with the current challenge.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rScoreManager				= DrawObjectOption("Score Manager Ref: ", Target.m_rScoreManager, "Reference to the Challenge Game Score Manager which will handle Incrementing/Decrementing the user's score.");
		Target.m_rResultsManager			= DrawObjectOption("Challenge Results Manager Ref: ", Target.m_rResultsManager, "Reference to the Challenge Results Manager");
        Target.m_rMetronomeTracker			= DrawObjectOption("Metronome Tracker Ref: ", Target.m_rMetronomeTracker, "Reference to the Metornome Tracker");
		Target.m_rInstrumentManager			= DrawObjectOption("Instrument Manager Ref: ", Target.m_rInstrumentManager, "Reference to the Instrument Manager");
		AddSpaces(3);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		if (Target.m_arChallenges.Length != (int)SoundsRhythmMemoryGame.Playlist.TOTAL_PLAYLISTS)
			ResizeArray(ref Target.m_arChallenges, (int)SoundsRhythmMemoryGame.Playlist.TOTAL_PLAYLISTS);

		for (int i = 0; i < (int)SoundsRhythmMemoryGame.Playlist.TOTAL_PLAYLISTS; ++i)
		{
			Target.m_arChallenges[i] = DrawObjectOption( ((SoundsRhythmMemoryGame.Playlist)i).ToString() + ": ", Target.m_arChallenges[i], "Which challenge will be representing the " + ((SoundsRhythmMemoryGame.Playlist)i).ToString() +" Slot?");
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Debug Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawDebugOptions()
	{
		if (GUILayout.Button(new GUIContent("Apply Challenge Data Info", "Heads into the Challenge Data Folder, reading challenge data and changing up, building new challenges based off of it")))
		{
			ApplyChallengeDataFileInfoToChallenges();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Apply Challenge Data File Info To Challenges
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ApplyChallengeDataFileInfoToChallenges()
	{
		for (int i = 0; i < Target.m_arChallenges.Length; ++i)
		{
			if (Target.m_arChallenges[i] != null)
			{
				Target.CurrentChallengeID = i;
				string path = (Application.dataPath + "/DEBUG STUFF/Challenge Data/" + Target.CurrentChallengeIDAsString + ".txt");
				if (System.IO.File.Exists(path))
				{
					using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
					{
						string[] challengeData = reader.ReadToEnd().Replace("\r", "").Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
						Target.CurrentChallenge.m_oChallengeName.EnglishTranslation = challengeData[0].Replace("ChallengeName=", "");
						Target.CurrentChallenge.BPM = (float)System.Convert.ToDouble(challengeData[1].Replace("ChallengeBPM=", ""));
						Target.CurrentChallenge.backingTrackDelayTime = (float)System.Convert.ToDouble(challengeData[4].Replace("BackingDelay=", ""));
#if UNITY_EDITOR
						Target.CurrentChallenge.iosBPM = Target.CurrentChallenge.BPM;
						Target.CurrentChallenge.iosBackingTrackDelay = Target.CurrentChallenge.backingTrackDelayTime;
#endif
						string vol = challengeData[5].Replace("BackingVolume=", "");
						if (vol != "N/A")
						{
							Target.CurrentChallenge.m_oChallengeBackingTrackInfo.m_fMaxVolume = (float)System.Convert.ToDouble(vol);
						}


						System.Collections.Generic.LinkedList<GameObject> children = new System.Collections.Generic.LinkedList<GameObject>();
						while (Target.CurrentChallenge.transform.childCount > 0)
						{
							Transform child = Target.CurrentChallenge.transform.GetChild(0);
							child.parent = null;
							children.AddLast(child.gameObject);
						}
						foreach (GameObject child in children)
						{
							DestroyImmediate(child);
						}
						for (int j = 7; j < challengeData.Length; ++j)
						{
							string[] noteData = challengeData[j].Replace("NoteData=", "").Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
							TambourineSoundsManager.SoundTypes eSoundType = (TambourineSoundsManager.SoundTypes)System.Convert.ToInt32(noteData[0]);
							float fBeatPos = (float)System.Convert.ToDouble(noteData[1]);
							Target.CurrentChallenge.AddNewChallengeNote(eSoundType, fBeatPos);
						}

						EditorUtility.SetDirty(Target.CurrentChallenge);
					}
				}
				Target.m_arChallenges[i].gameObject.SetActive(false);
			}
		}

		if(Target.m_arChallenges[0] != null)
			Target.m_arChallenges[0].gameObject.SetActive(true);
	}
}
