//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Sound Rhythm Memory Game
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

[CustomEditor(typeof(SoundsRhythmMemoryGame))]
public class CustomEditor_SoundsRhythmMemoryGame : CustomEditor_Base<SoundsRhythmMemoryGame>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private SoundsRhythmMemoryGame.Playlist m_eCurrentPlaylist = SoundsRhythmMemoryGame.Playlist.CHALLENGE_01;

	private bool m_bFirstRun = true;
	private bool m_bShowAllNotes = false;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected SoundsRhythmMemoryGame.PlaybackInfo[] CurrentPlaylist { get { return Target.GetPlaylist(m_eCurrentPlaylist); } }
	protected override string ScriptDescription
	{
		get
		{
			return "This script plays a rhythm on the instrument. When doing so it will deactivate\n" +
					"all input from the player until it is completed.\n\n" +
					"If it is called from somewhere, it has the capacity to bring the player\n" +
					"back to where it was called from.";
        }
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On First Run
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnFirstRun()
	{
		m_bFirstRun = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rTambourineSoundsManager = DrawObjectOption("Tambourine Sounds Manager Reference: ", Target.m_rTambourineSoundsManager);

		SoundsRhythmMemoryGame.Playlist eChoice = m_eCurrentPlaylist;
		eChoice = (SoundsRhythmMemoryGame.Playlist)Mathf.Clamp((int)((SoundsRhythmMemoryGame.Playlist)EditorGUILayout.EnumPopup(new GUIContent("Selected Playlist: ", "Which playlist will we be editing?"), eChoice)), 0, (int)SoundsRhythmMemoryGame.Playlist.TOTAL_PLAYLISTS - 1);

		if (Target.m_arTrackSoundInfo.Length != (int)SoundsRhythmMemoryGame.Playlist.TOTAL_PLAYLISTS)
		{
			ResizeArray(ref Target.m_arTrackSoundInfo, (int)SoundsRhythmMemoryGame.Playlist.TOTAL_PLAYLISTS);
		}

		if (eChoice != m_eCurrentPlaylist)
		{
			m_eCurrentPlaylist = eChoice;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		// On First Iteration
		if (m_bFirstRun)
		{
			OnFirstRun();
		}

		if (GUILayout.Button("Read From Midi File"))
		{
			Target.AccessMidiFile(m_eCurrentPlaylist);
		}
		Target.m_arTrackSoundInfo[(int)m_eCurrentPlaylist].loopCount = DrawChangeableIntegerOption("Loop Count: ", Target.m_arTrackSoundInfo[(int)m_eCurrentPlaylist].loopCount, tooltip: "How many times will the track loop itself?");
		DrawPlaylistSizeOptions();
		DrawPlaylistOptions();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Playlist Size Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawPlaylistSizeOptions()
	{
		int iNewSize = DrawChangeableIntegerOption("Total Playlist Notes: ", CurrentPlaylist.Length);
		if (iNewSize != CurrentPlaylist.Length)
		{
			Target.ResizePlaylistArray(m_eCurrentPlaylist, iNewSize);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Playlist Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawPlaylistOptions()
	{
		AddSpaces(3);
		m_bShowAllNotes = EditorGUILayout.Foldout(m_bShowAllNotes, (m_bShowAllNotes ? "Hide All Notes" : "Show All Notes"));
		if(m_bShowAllNotes)
		{
			SoundsRhythmMemoryGame.PlaybackInfo[] aCurrentPlaylist = CurrentPlaylist;
			for(int i = 0; i < aCurrentPlaylist.Length; ++i)
			{
				string labelFieldText = (m_eCurrentPlaylist.ToString() + "- Note " + ((((i + 1) < 10) ? "0" : "") + (i + 1).ToString()) + ": ") + "Selected Sound: ";
				float enumWidth = 170.0f;
				float fltWidth = 80.0f;
				float txtWidth = 90.0f;

				EditorGUILayout.LabelField(labelFieldText);
				Rect drawPos = GetScaledRect();
				drawPos.x += (drawPos.width - fltWidth);
				drawPos.width = fltWidth;
				aCurrentPlaylist[i].endTime = EditorGUI.FloatField(drawPos, aCurrentPlaylist[i].endTime);
				drawPos.x -= 60.0f;
				drawPos.width = txtWidth;
				EditorGUI.LabelField(drawPos, "EndTime:");
				drawPos.x -= 70.0f;
				drawPos.width = fltWidth;
				aCurrentPlaylist[i].startTime = EditorGUI.FloatField(drawPos, aCurrentPlaylist[i].startTime);
				drawPos.x -= 65.0f;
				drawPos.width = txtWidth;
				EditorGUI.LabelField(drawPos, "StartTime:");
				drawPos.x -= enumWidth;
				drawPos.width = (enumWidth + 5.0f);
				aCurrentPlaylist[i].type = (TambourineSoundsManager.SoundTypes)EditorGUI.EnumPopup(drawPos, new GUIContent("", "The sound which will be played"), aCurrentPlaylist[i].type);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Debug Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawDebugOptions()
	{
		Target.m_bPlayOnAwake = EditorGUILayout.Toggle("Play Playlist When Game Starts: ", Target.m_bPlayOnAwake);
		Target.m_ePlaylistToPlay = (SoundsRhythmMemoryGame.Playlist)EditorGUILayout.EnumPopup("Which Playlist: ", Target.m_ePlaylistToPlay);
	}
}