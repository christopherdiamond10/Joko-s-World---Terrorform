//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Instructions Manager
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: February 12, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script plays a rhythm on the instrument. When doing so it will deactivate 
//		all input from the player until it is completed.
//
//	  If it is called from somewhere, it has the capacity to bring the player 
//		back to where it was called from.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class SoundsRhythmMemoryGame : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TambourineSoundsManager m_rTambourineSoundsManager;

	// Keeping these tracks as separate arrays rather than a 2D Array; so that if we add new ones we don't replace the old ones in the unity editor (*-*)
	public TrackInfo[] m_arTrackSoundInfo = new TrackInfo[ (int)Playlist.TOTAL_PLAYLISTS ];

#if UNITY_EDITOR
	public bool m_bPlayOnAwake = true;
	public Playlist m_ePlaylistToPlay = Playlist.CHALLENGE_01;
#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	// Variables which hold the information required to play through the list of sounds.
	private float m_fStartDelayTime = 0.0f;
	private float m_fPlaytime = 0.0f;
	private float m_fEndSongTime = 0.0f;
	private LinkedList<PlaybackInfo> m_lPlaylist = new LinkedList<PlaybackInfo>();
	
	private bool m_bIsInvoked = false;							// Begin the process of playing sheet music?
	private bool m_bIsPlaying = false;							// Is actually playing the sheet music?
	private Playlist m_eSelectedPlaylist = Playlist.CHALLENGE_01;	// Currently selected playlist.
	private MultipleNotesManager m_rOwningSheetMusicNote;		// When playing a playlist from a sheet music, pass in the Sheet Music Reference.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsPlaying {  get { return m_bIsPlaying; } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum Playlist
	{
		// Beginner
		CHALLENGE_01 = 0,
		CHALLENGE_02 = 1,
		CHALLENGE_03 = 2,
		CHALLENGE_04 = 3,
		CHALLENGE_05 = 4,

		// Student
		CHALLENGE_06 = 5,
		CHALLENGE_07 = 6,
		CHALLENGE_08 = 7,
		CHALLENGE_09 = 8,
		CHALLENGE_10 = 9,

		// Virtuoso
		CHALLENGE_11 = 10,
		CHALLENGE_12 = 11,
		CHALLENGE_13 = 12,
		CHALLENGE_14 = 13,
		CHALLENGE_15 = 14,

		// Maestro
		CHALLENGE_16 = 15,
		CHALLENGE_17 = 16,
		CHALLENGE_18 = 17,
		CHALLENGE_19 = 18,
		CHALLENGE_20 = 19,

		// Ustad
		CHALLENGE_21 = 20,
		CHALLENGE_22 = 21,
		CHALLENGE_23 = 22,
		CHALLENGE_24 = 23,
		CHALLENGE_25 = 24,

		// Tutorials
		TUTORIAL = 25,

		TOTAL_PLAYLISTS,	// <= Leave Last!
	}

	[System.Serializable]
	public class TrackInfo
	{
		public PlaybackInfo[] notes = new PlaybackInfo[1] { new PlaybackInfo() };
		public int loopCount = 1;
	}

	[System.Serializable]
	public class PlaybackInfo
	{
		public TambourineSoundsManager.SoundTypes type = TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA;

		public float startTime = 0.0f;
		public float endTime = 0.0f;
	}



#if UNITY_EDITOR
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake()
	{
		// If playing an audio tune ASAP. Play it
		if (m_bPlayOnAwake)
		{
			PlayThroughRhythmList(m_ePlaylistToPlay);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		//if (GetComponent<AudioSource>() == null)
		//    gameObject.AddComponent<AudioSource>();
		//GetComponent<AudioSource>().clip = Combine();
		//GetComponent<AudioSource>().loop = true;
		//GetComponent<AudioSource>().Play();
	}
#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update() 
	{
		// Play Sheet Music?
		if (m_bIsInvoked)
		{
			if (m_rOwningSheetMusicNote == null || !m_rOwningSheetMusicNote.IsCurrentlyActive)
			{
				PlayThroughRhythmList(m_eSelectedPlaylist);
				m_bIsInvoked = false;
			}
		}

		// If Playing Sheet Music, Does the user wish to stop?
		else if (m_bIsPlaying)
		{
			m_fPlaytime += Time.deltaTime;
			while (m_lPlaylist.Count > 0 && m_lPlaylist.First.Value.startTime < m_fPlaytime)
			{
				StartCoroutine( PlayNote(m_lPlaylist.First.Value) );
				m_lPlaylist.RemoveFirst();
			}
			if (m_fPlaytime > m_fEndSongTime)
			{
				// Set current point in the RhythmList to the end. This will force the rhythm to stop playback ASAP
				StopPlayback();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Playlist
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public PlaybackInfo[] GetPlaylist(Playlist ePlaylist)
	{
		return m_arTrackSoundInfo[(int)ePlaylist].notes;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Play Through Rhythm List
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void PlayThroughRhythmList(Playlist ePlaylist)
	{
		// Stop Player Input
		//ButtonManager.ToggleAllButtons(false);
		TambourineShakeDetector.CheckForShake = false;

		// Setup Rhythm List Variables (These will be used to keep track of where we are in the playlist)
		PlaybackInfo[] aCurrentPlaylist = GetPlaylist(ePlaylist);
        int iLoopCount = m_arTrackSoundInfo[(int)ePlaylist].loopCount;

		// Begin Playback
		m_bIsPlaying = true;


		// Create playlist. Making sure to loop through more than once if the rhythm requires looping
		m_lPlaylist.Clear();
		for (int i = 0; i < iLoopCount; ++i)
		{
			for (int j = 0; j < aCurrentPlaylist.Length; ++j)
			{
				// Put all sound into list. If on second loop; multiply start and end times by the last note finish time. This will push notes BACK after the last note, creating a loop
				PlaybackInfo pbi = new PlaybackInfo();
				pbi.type = aCurrentPlaylist[j].type;
				pbi.startTime = m_fStartDelayTime + aCurrentPlaylist[j].startTime + aCurrentPlaylist[aCurrentPlaylist.Length - 1].endTime * i;
				pbi.endTime = m_fStartDelayTime + aCurrentPlaylist[j].endTime + aCurrentPlaylist[aCurrentPlaylist.Length - 1].endTime * i;
				m_lPlaylist.AddLast(pbi);
			}
		}
		
		// Reset Management Values. Add on an additional half second to the End Song Time. This gives a slight cooldown time after the song finishes before the Sheet music returns to screen.
		m_fPlaytime = 0.0f;
		m_fEndSongTime = m_lPlaylist.Last.Value.endTime + 0.5f;
	}

	public void PlayThroughRhythmList(Playlist ePlaylist, MultipleNotesManager rOwningSheetMusic, float fDelayTime = 0.0f)
	{
		// Assign and then close down the Sheet Music
		m_rOwningSheetMusicNote = rOwningSheetMusic;
		if(rOwningSheetMusic != null)
			rOwningSheetMusic.Disappear(false);

		m_bIsInvoked = true;
		m_eSelectedPlaylist = ePlaylist;
		m_fStartDelayTime = fDelayTime;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Playback
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StopPlayback()
	{
		// Not Playing anymore... are we?
		m_lPlaylist.Clear();
		m_bIsPlaying = false;		

		// If there was an owning note (we pressed play from the sheet music notes), then open that note back up.
		if (m_rOwningSheetMusicNote != null)
		{
			m_rOwningSheetMusicNote.Reveal();
			m_rOwningSheetMusicNote = null;
		}

		// Otherwise just reactivate input from the user.
		else
		{
			ButtonManager.ToggleAllButtons(true);
			TambourineShakeDetector.CheckForShake = true;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Play Current Note
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void PlayNote(TambourineSoundsManager.SoundTypes eDesiredSound, float visibilityTime)
	{
		PlaybackInfo pbi = new PlaybackInfo();
		pbi.type = eDesiredSound;
		pbi.startTime = 0.0f;
		pbi.endTime = visibilityTime;
		StartCoroutine(PlayNote(pbi));
	}

	private IEnumerator PlayNote(PlaybackInfo playbackInfo)
	{
		m_rTambourineSoundsManager.PlayTambourineSound(playbackInfo.type);
		m_rTambourineSoundsManager.HighlightSoundObject(playbackInfo.type);

		// Wait For 65% of the waiting period, then revert the visual cue back to the normal sprite. Then wait the for remaining 35%.
		// This is so that the visual cues do not stack upon each other and make the sounds/visual cues appear so suddenly. Additionally this also
		// helps prevent two consecutive visual cues from looking like one long visual cue.
		float waitTime = playbackInfo.endTime - playbackInfo.startTime;
		yield return new WaitForSeconds(waitTime * 0.65f);
		m_rTambourineSoundsManager.RevertSoundObject(playbackInfo.type);
	}

	private IEnumerator PlayNote(PlaybackInfo playbackInfo, float WaitTime)
	{
		yield return new WaitForSeconds(WaitTime);

		m_rTambourineSoundsManager.PlayTambourineSound(playbackInfo.type);
		m_rTambourineSoundsManager.HighlightSoundObject(playbackInfo.type);

		// See other Overloaded Method
		float waitTime = playbackInfo.endTime - playbackInfo.startTime;
		yield return new WaitForSeconds(waitTime * 0.65f);
		m_rTambourineSoundsManager.RevertSoundObject(playbackInfo.type);
	}

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Resize Array
	//	: Simply Resizes an array and repopulates its memory
	//	  with its previous data.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
	public void ResizePlaylistArray(Playlist ePlaylist, int NewSize)
	{
		PlaybackInfo[] NewPlaybackInfo = new PlaybackInfo[NewSize];
		PlaybackInfo[] OldInfo = GetPlaylist(ePlaylist);

		for (int i = 0; i < NewPlaybackInfo.Length; ++i)
		{
			NewPlaybackInfo[i] = new PlaybackInfo();
			if (i < OldInfo.Length)
			{
				NewPlaybackInfo[i].type = OldInfo[i].type;
			}
		}

		m_arTrackSoundInfo[(int)ePlaylist].notes = NewPlaybackInfo;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Access Midi File
	//	: Uses external C++ code to read a MIDI file's contents
	//	  and populate the playlist array with said contents
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	[DllImport("MidiInputGrabber", EntryPoint = "Init", CallingConvention = CallingConvention.StdCall)]
	static extern void Midi__Init();
	[DllImport("MidiInputGrabber", EntryPoint = "Dispose", CallingConvention = CallingConvention.StdCall)]
	static extern void Midi__Dispose();
	[DllImport("MidiInputGrabber", EntryPoint = "ReadMidiFile", CallingConvention = CallingConvention.StdCall)]
	static extern void Midi__ReadMidiFile();
	[DllImport("MidiInputGrabber", EntryPoint = "Empty", CallingConvention = CallingConvention.StdCall)]
	static extern bool Midi__Empty();
	[DllImport("MidiInputGrabber", EntryPoint = "Size", CallingConvention = CallingConvention.StdCall)]
	static extern int Midi__Size();
	[DllImport("MidiInputGrabber", EntryPoint = "BPM", CallingConvention = CallingConvention.StdCall)]
	static extern int Midi__BPM();
	[DllImport("MidiInputGrabber", EntryPoint = "NextNote", CallingConvention = CallingConvention.StdCall)]
	static extern void Midi__NextNote();
	[DllImport("MidiInputGrabber", EntryPoint = "GetChannel", CallingConvention = CallingConvention.StdCall)]
	static extern int Midi__GetChannel();
	[DllImport("MidiInputGrabber", EntryPoint = "GetKey", CallingConvention = CallingConvention.StdCall)]
	static extern int Midi__GetKey();
	[DllImport("MidiInputGrabber", EntryPoint = "GetStartTime", CallingConvention = CallingConvention.StdCall)]
	static extern float Midi__GetStartTime();
	[DllImport("MidiInputGrabber", EntryPoint = "GetEndTime", CallingConvention = CallingConvention.StdCall)]
	static extern float Midi__GetEndTime();

	public void AccessMidiFile(Playlist ePlaylist)
	{
		Midi__Init();				// Init DLL Stuff
		Midi__ReadMidiFile();		// Will open up an OpenFileDialog Window that the user shall use to select a midi
		if (!Midi__Empty())
		{
			ResizePlaylistArray(ePlaylist, Midi__Size());
			int iCurrentIndex = 0;
			while (!Midi__Empty())
			{
				SetupEvent(ePlaylist, iCurrentIndex, Midi__GetKey(), Midi__GetStartTime(), Midi__GetEndTime());

				iCurrentIndex += 1;
				Midi__NextNote();
			}
		}

		Midi__Dispose();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Event
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetupEvent(Playlist ePlaylist, int iCurrentIndex, int keyID, float startTime, float endTime)
	{
		GetPlaylist(ePlaylist)[iCurrentIndex].type = GetMIDISoundTypeFromKey(keyID);
		GetPlaylist(ePlaylist)[iCurrentIndex].startTime = startTime;
		GetPlaylist(ePlaylist)[iCurrentIndex].endTime = endTime;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Sound Type From MIDI Channel
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TambourineSoundsManager.SoundTypes GetMIDISoundTypeFromChannel(int iChannel)
	{
		switch (iChannel)
		{
			case 0: return TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA;
			case 1: return TambourineSoundsManager.SoundTypes.MIDDLE_TAMBOURINE_AREA;
			case 2: return TambourineSoundsManager.SoundTypes.OUTER_TAMBOURINE_AREA;
			case 3: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_1;
			case 4: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_2;
			case 5: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_3;
			case 6: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_4;
			case 7: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_5;
			case 8: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_6;
			case 9: return TambourineSoundsManager.SoundTypes.SOFT_TAMBOURINE_SHAKE;
			case 10: return TambourineSoundsManager.SoundTypes.HARD_TAMBOURINE_SHAKE;
			case 11: return TambourineSoundsManager.SoundTypes.MOVE_TAMBOURINE_SOUND;
			default: return TambourineSoundsManager.SoundTypes.RATTLE_TAMBOURINE_SHAKE;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Sound Type From MIDI Key
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TambourineSoundsManager.SoundTypes GetMIDISoundTypeFromKey(int iKeyID)
	{
		switch (iKeyID)
		{
			case 60: return TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA; // 
			case 61: return TambourineSoundsManager.SoundTypes.MIDDLE_TAMBOURINE_AREA;
			case 62: return TambourineSoundsManager.SoundTypes.OUTER_TAMBOURINE_AREA;
			case 63: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_1;
			case 64: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_2;
			case 65: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_3;
			case 66: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_4;
			case 67: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_5;
			case 68: return TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_6;
			case 69: return TambourineSoundsManager.SoundTypes.SOFT_TAMBOURINE_SHAKE;
			case 70: return TambourineSoundsManager.SoundTypes.HARD_TAMBOURINE_SHAKE;
			case 71: return TambourineSoundsManager.SoundTypes.MOVE_TAMBOURINE_SOUND;
			default: return TambourineSoundsManager.SoundTypes.RATTLE_TAMBOURINE_SHAKE;
		}
	}
#endif



#if false // Debugging Audio Generation!
	struct PlaybackRecord 
	{ 
		public float start; 
		public int clipID; 
	}
	public AudioClip Combine()
	{
		const int sampleRate = 32000;
		const int maxSeconds = 5;
		// Make Clips
		AudioClip[] clips = new AudioClip[]
		{
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.MIDDLE_TAMBOURINE_AREA).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.OUTER_TAMBOURINE_AREA).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.SOFT_TAMBOURINE_SHAKE).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.HARD_TAMBOURINE_SHAKE).OwningSound,			
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_1).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_2).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_3).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_4).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_5).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_6).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.MOVE_TAMBOURINE_SOUND).OwningSound,
			m_rTambourineSoundsManager.GetSoundAssociates(TambourineSoundsManager.SoundTypes.RATTLE_TAMBOURINE_SHAKE).OwningSound,
		};
		// Get Sample Rates
		float[][] samples = new float[clips.Length][];
		for (int i = 0; i < samples.Length; ++i)
		{
			samples[i] = new float[clips[i].samples * clips[i].channels];
			clips[i].GetData(samples[i], 0);
		}

		// Find Max Channels & Samples
		int iMaxChannels = 0;
		int iMaxSamples = 0;
		for (int i = 0; i < clips.Length; ++i)
		{
			iMaxChannels = Mathf.Max(iMaxChannels, clips[i].channels);
			iMaxSamples  = Mathf.Max(iMaxSamples, clips[i].samples);
		}

		LinkedList<PlaybackRecord> lPlayback = new LinkedList<PlaybackRecord>();
		lPlayback.AddLast(new PlaybackRecord() { start = 1.75f, clipID = 0 });
		lPlayback.AddLast(new PlaybackRecord() { start = 2.25f, clipID = 1 });
		lPlayback.AddLast(new PlaybackRecord() { start = 2.35f, clipID = 8 });
		lPlayback.AddLast(new PlaybackRecord() { start = 3.05f, clipID = 0 });
		lPlayback.AddLast(new PlaybackRecord() { start = 3.75f, clipID = 2 });
		lPlayback.AddLast(new PlaybackRecord() { start = 3.95f, clipID = 5 });
		lPlayback.AddLast(new PlaybackRecord() { start = 4.45f, clipID = 7 });
		
		
		//iMaxChannels *= Mathf.Min(lPlayback.Count, clips.Length);
		int sampleLength = sampleRate * iMaxChannels * maxSeconds;
		float[] data = new float[sampleLength];

		foreach (PlaybackRecord pr in lPlayback)
		{
			int samplesIndex = (int)(pr.start * sampleRate * iMaxChannels);
			int channelOffsetSamples = 0;
			int failedAttempts = 0;
			bool bd = false;
			while (data[samplesIndex + channelOffsetSamples] != 0.0f && failedAttempts < iMaxChannels)
			{
				failedAttempts += 1;
				channelOffsetSamples += 2;
				if (channelOffsetSamples >= iMaxChannels)
					channelOffsetSamples = 0;
			}

			for (int j = 0; j < samples[pr.clipID].Length; j += clips[pr.clipID].channels)
			{

				for (int k = 0; k < clips[pr.clipID].channels; ++k)
				{
					if (samplesIndex < data.Length)
					{
						data[samplesIndex] = data[samplesIndex] + (samples[pr.clipID][j + k]) - (data[samplesIndex] * (samples[pr.clipID][j + k]));
						samplesIndex++; if (!bd)
						{
							bd = true;
							Debug.Log(samplesIndex);
						}
					}
					//samplesIndex += iMaxChannels;
				}
			}
		}
		//float startPoint = 1.75f;
		//int samplesIndex = (int)(startPoint * sampleRate * iMaxChannels);
		//int clipID = 0;
		//for (int j = 0; j < samples[clipID].Length; j += clips[clipID].channels)
		//{
		//    for (int k = 0; k < clips[clipID].channels; ++k)
		//    {
		//        if (samplesIndex < data.Length)
		//        {
		//            data[samplesIndex] = data[samplesIndex] + (samples[clipID][j + k]) - (data[samplesIndex] * (samples[clipID][j + k]));
		//            samplesIndex++;
		//        }
		//    }
		//}
		//startPoint = 2.25f;
		//samplesIndex = (int)(startPoint * sampleRate * iMaxChannels);
		//clipID = 1;
		//for (int j = 0; j < samples[clipID].Length; j += clips[clipID].channels)
		//{
		//    for (int k = 0; k < clips[clipID].channels; ++k)
		//    {
		//        if (samplesIndex < data.Length)
		//        {
		//            data[samplesIndex] = data[samplesIndex] + (samples[clipID][j + k]) - (data[samplesIndex] * (samples[clipID][j + k]));
		//            samplesIndex++;
		//        }
		//    }
		//}
		//startPoint = 2.35f;
		//samplesIndex = (int)(startPoint * sampleRate * iMaxChannels);
		//clipID = 8;
		//for (int j = 0; j < samples[clipID].Length; j += clips[clipID].channels)
		//{
		//    for (int k = 0; k < clips[clipID].channels; ++k)
		//    {
		//        if (samplesIndex < data.Length)
		//        {
		//            data[samplesIndex] = data[samplesIndex] + (samples[clipID][j + k]) - (data[samplesIndex] * (samples[clipID][j + k]));
		//            samplesIndex++;
		//        }
		//    }
		//}

		AudioClip result = AudioClip.Create("Combine", sampleLength / iMaxChannels, iMaxChannels, sampleRate, false);
		result.SetData(data, 0);

		SavWav.Save("myfile", result, data);
		return result;
	}
#endif
}
