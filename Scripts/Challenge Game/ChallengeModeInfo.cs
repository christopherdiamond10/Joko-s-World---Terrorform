//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Challenge Game - Challenge Mode Info
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 13, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script does nothing special besides keeping information regarding the
//		individual challenge modes in the Challenge Game. Such as 'Target Location',
//		'BPM', and 'Separation Values' used to automate the placement process of 
//		the Challenge Notes.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

//#define UseMemoryGameManager

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class ChallengeModeInfo : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	// Variables Used to reference and provide information to the user
	public ChallengeGameManager m_rGameManager;
	public SoundsRhythmMemoryGame m_rSoundsRhythmMemoryGame;
	//public Button_PlaySheetMusic m_rPracticeButton;

	public UnityEngine.UI.Text m_rChallengeTitleText;
	public MultiLanguageText m_oChallengeName = new MultiLanguageText();

	public bool m_bOnlyAvailableToFullVersion = false;	// Challenge Can Only Be Played in Full Version?
	public int m_iRequiredLiteVersionFeathers = 0;      // How many Feathers are Required to Unlock this Challenge in the 'Lite/Free' Version of the App?
	public int m_iRequiredFullVersionFeathers = 0;      // How many Feathers are required to Unlock this Challenge in the 'Full/Pro' Version of the App?

	public Sprite m_sprInstrumentSoundType;
	public SpriteRenderer m_rInstrumentSoundTypeRenderer;
	public InstrumentManager.InstrumentMode m_eInstrumentSoundType = InstrumentManager.InstrumentMode.NORMAL_TAMBOURINE;
	public SoundsRhythmMemoryGame.Playlist m_ePlaylistTrack = SoundsRhythmMemoryGame.Playlist.CHALLENGE_01;
	public AudioSourceManager.AudioHandlerInfo m_oChallengeBackingTrackInfo = new AudioSourceManager.AudioHandlerInfo();


	// Debug Variables. Only Usable during Editor Mode!
	public int	  dm_iImportedAudioSize = 0;
	public string dm_sImportedAudioFilename = ""; // Used with the ChallengeBuildManager
	public GameObject m_goChallengeNotePrefab;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
	public string dm_sSuggestedBackingTrackFilename = "";
	public Sprite m_sprNormalTambourine;
	public Sprite m_sprKanjiraTambourine;
	public Sprite m_sprPandeiroTambourine;
	public Sprite m_sprRiqTambourine;
#endif


	// Variables Used By Challenge Notes
	public float BPM						= 102.0f;
	public float iosBPM						= 102.0f;
	public float backingTrackDelayTime		= 0.0f;
	public float iosBackingTrackDelay		= 0.0f;
	public Vector3 targetLocation			= new Vector3(0.2f, 0.2f, 0.2f);
	public Vector3 victoryLocation			= new Vector3(0.2f, 0.2f, 0.2f);
	public Vector2 separationDifference		= new Vector2(0.2f, 0.2f);
	public float notationSquareOffset		= 2.0f;		// How Many Squares Away from the victory location is the target location?

	public float cymbalLinePos		= -1.5f;
	public float redAreaLinePos		= -1.0f;
	public float blueAreaLinePos	= 0.0f;
	public float yellowAreaLinePos	= 1.0f;
	public float shakeLinePos		= -1.5f;


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private float m_fPreviousBeatTime = 0.0f;
    private LinkedList<ChallengeNoteMovement> m_lActiveChallengeNotes = new LinkedList<ChallengeNoteMovement>();
	private ChallengeActivity m_eChallengeActivity = ChallengeActivity.IDLE;
	private Coroutine m_rActiveChallengeCoroutine;

	private static float sm_fScore = 0.0f;
	private static float sm_fVisibleScore = 0.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsChallengeAvailable
	{
		get { return IsChallengeAvailableToPlay(); }
	}

	public bool RequiresFullVersion
	{
		get { return m_bOnlyAvailableToFullVersion; }
	}

	public int RequiredFeathers
	{
		get { return (GameManager.IsFullVersion ? m_iRequiredFullVersionFeathers : m_iRequiredLiteVersionFeathers); }
	}

	public int RemainingRequiredFeathers
	{
		get { return Mathf.Max( (RequiredFeathers - ChallengeFeathersInfo.AccumulatedFeathers), 0 ); }
	}

	public bool Active
	{
		get { return m_lActiveChallengeNotes.Count > 0; }
	}

	public float ChallengeScore
	{
		get { return sm_fScore; }
		set { ModifyScore(value); }
	}

	public float Score
	{
		get { return sm_fScore; }
		set { sm_fScore = value; }
	}

	public float VisibleScore
	{
		get { return sm_fVisibleScore; }
		set { sm_fVisibleScore = Mathf.Max(0.0f, Mathf.Min(1.0f, value)); } 
	}

	public float NoteScore
	{
		get { return (1.0f / transform.childCount); }
	}

	public float MetronomeTime
	{
		get { return ((1.0f / ChallengeBPM) * 60.0f); }
	}

	public float ChallengeBPM
	{
		get
		{
//#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_ANDROID
			return BPM;
//#else
			//return iosBPM;
//#endif
		}
	}

	public float BackingTrackDelay
	{
		get
		{
//#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_ANDROID
			return backingTrackDelayTime;
//#else
			//return iosBackingTrackDelay;
//#endif
		}
	}

	public float CurrentBeatPercentage
    {
        get { return (Time.realtimeSinceStartup - m_fPreviousBeatTime) / MetronomeTime; }
    }

	public float StartDelayTime
	{	// (((1 / BPM) * Seconds Per Minute) * Beats)
		get { return 0.0f; }// (MetronomeTime * DelayBeatMarker); }
	}

	public float DelayBeatMarker
	{
		get { return 8.0f; }
	}

	public bool IsChallengeMode
	{
		get { return m_eChallengeActivity == ChallengeActivity.CHALLENGE_MODE; }
	}

	public bool IsPracticeMode
	{
		get { return m_eChallengeActivity == ChallengeActivity.PRACTICE_MODE; }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum ChallengeActivity
	{
		IDLE,
		PRACTICE_MODE,
		CHALLENGE_MODE,
	}



    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Derived Method: Start
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Start()
    {
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
    {
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Move Challenge Track With Metronome
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private IEnumerator MoveChallengeTrackWithMetronome()
	{		
		float metronomeTime = MetronomeTime;// * 0.25f;
		float currentTime = Time.realtimeSinceStartup;
		float previousTime = currentTime;
		float delayTime = 0.0f;

		yield return new WaitForSeconds(metronomeTime);
		AdvanceChallengeNotes();

		while(m_lActiveChallengeNotes.Count > 0)
		{
			previousTime = currentTime;
			currentTime = Time.realtimeSinceStartup;
			delayTime = (metronomeTime - Mathf.Max(0.0f, ((currentTime - previousTime) - metronomeTime)));
			yield return new WaitForSeconds(delayTime);
			m_fPreviousBeatTime = Time.realtimeSinceStartup;
			AdvanceChallengeNotes();
		}
		m_rActiveChallengeCoroutine = null;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Advance Challenge Notes
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void AdvanceChallengeNotes()
	{
		const float beatMovement = 1.0f;
		foreach(ChallengeNoteMovement note in m_lActiveChallengeNotes)
		{
			note.BeatPos -= beatMovement;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Challenge Available?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool IsChallengeAvailableToPlay()
	{
		if(RequiresFullVersion)
			return (GameManager.IsFullVersion && (RemainingRequiredFeathers == 0));
		else
			return (RemainingRequiredFeathers == 0);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset Notations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ResetNotations()
	{
		foreach (Transform child in transform)
			child.GetComponent<ChallengeNoteMovement>().Reset();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Start Note Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StartNoteMovement()
	{
		StopNoteMovement();
		m_rActiveChallengeCoroutine = StartCoroutine(MoveChallengeTrackWithMetronome());
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Note Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StopNoteMovement()
	{
		if(m_rActiveChallengeCoroutine != null)
		{
			StopCoroutine(m_rActiveChallengeCoroutine);
			m_rActiveChallengeCoroutine = null;
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Play Backing Track
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void PlayBackingTrack(float delayTime = 0.0f)
	{
		StartCoroutine(PlayChallengeBackingTrack(delayTime));
    }

	private IEnumerator PlayChallengeBackingTrack(float delayTime = 0.0f)
	{
		if(delayTime > 0.0f)
			yield return new WaitForSeconds(delayTime);

		// Challenge Hasn't been stopped during the delay?
		if(m_eChallengeActivity != ChallengeActivity.IDLE)
			AudioSourceManager.PlayAudioClip(m_oChallengeBackingTrackInfo);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Backing Track
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StopBackingTrack()
	{
		AudioSourceManager.FadeoutAudio(m_oChallengeBackingTrackInfo);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Start Challenge
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StartChallenge()
	{
		ChallengeScore = 0.0f;
		VisibleScore = 0.0f;
		m_lActiveChallengeNotes.Clear();
		m_eChallengeActivity = ChallengeActivity.CHALLENGE_MODE;

		foreach(Transform child in transform)
		{
			ChallengeNoteMovement cni = child.GetComponent<ChallengeNoteMovement>();
			cni.Reset();
			cni.BeginMovement();
			m_lActiveChallengeNotes.AddLast(cni);
		}

		StartNoteMovement();
		PlayBackingTrack(BackingTrackDelay);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Challenge
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StopChallenge()
	{
		StopNoteMovement();
        m_lActiveChallengeNotes.Clear();
		m_eChallengeActivity = ChallengeActivity.IDLE;
		foreach (Transform child in transform)
		{
			ChallengeNoteMovement cni = child.GetComponent<ChallengeNoteMovement>();
			cni.Reset();
		}

		StopBackingTrack();
#if UseMemoryGameManager
		if (m_rSoundsRhythmMemoryGame != null)
			m_rSoundsRhythmMemoryGame.StopPlayback();
#endif
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Start AutoPlay
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StartAutoPlay()
	{
		if (IsPracticeMode)
		{
			ResetNotations();
			m_eChallengeActivity = ChallengeActivity.IDLE;
#if UseMemoryGameManager
			if (m_rSoundsRhythmMemoryGame != null)
				m_rSoundsRhythmMemoryGame.StopPlayback();
#endif
		}
		else
		{
			ChallengeScore = 0.0f;
			VisibleScore = 0.0f;
			m_eChallengeActivity = ChallengeActivity.PRACTICE_MODE;
			m_lActiveChallengeNotes.Clear();
			foreach (Transform child in transform)
			{
				ChallengeNoteMovement cni = child.GetComponent<ChallengeNoteMovement>();
				cni.Reset();
				cni.BeginAutoPlay();
				m_lActiveChallengeNotes.AddLast(cni);
			}

			StartNoteMovement();
			PlayBackingTrack(BackingTrackDelay);

			// Play Auto-Example
#if UseMemoryGameManager
			if(m_rSoundsRhythmMemoryGame != null)
			{
				m_rSoundsRhythmMemoryGame.PlayThroughRhythmList(m_ePlaylistTrack, null, StartDelayTime);
			}
#endif
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove Note From Sequence
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void RemoveNote(ChallengeNoteMovement cni)
	{
		if(m_lActiveChallengeNotes.Count > 0)
		{
			m_lActiveChallengeNotes.Remove(cni);

			// If removing that last note made us run out of Notes then the challenge has been finished. Tell the Game Manager to cease operations.
			if(m_lActiveChallengeNotes.Count == 0)
			{
				OnChallengeFinish();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Remove Front Note
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void RemoveFrontNote()
	{
		if (m_lActiveChallengeNotes.Count > 0)
		{
			m_lActiveChallengeNotes.RemoveFirst();

			// If removing that last note made us run out of Notes then the challenge has been finished. Tell the Game Manager to cease operations.
			if (m_lActiveChallengeNotes.Count == 0)
			{
				OnChallengeFinish();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get First Moving Note
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private ChallengeNoteMovement GetFirstMovingNote()
	{
		foreach (ChallengeNoteMovement challengeNote in m_lActiveChallengeNotes)
			if (challengeNote.CurrentState == ChallengeNoteMovement.MovementPhase.MOVEMENT)
				return challengeNote;

		return null;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get First Matching Note
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private ChallengeNoteMovement GetFirstMatchingNoteType(TambourineSoundsManager.SoundTypes eSoundType)
	{
		foreach(ChallengeNoteMovement challengeNote in m_lActiveChallengeNotes)
			if(challengeNote.CurrentState == ChallengeNoteMovement.MovementPhase.MOVEMENT && challengeNote.CheckMatchingSoundType(eSoundType))
				return challengeNote;

		return null;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check Successful Hit?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool CheckSuccessfulHit(TambourineSoundsManager.SoundTypes eSoundType)
	{
		if (m_lActiveChallengeNotes.Count > 0)
		{
			ChallengeNoteMovement matchingNote = GetFirstMovingNote();
			if (matchingNote != null && matchingNote.CheckMatchingSoundType(eSoundType) && matchingNote.CheckSuccessfulHit())
			//ChallengeNoteMovement matchingNote = GetFirstMatchingNoteType(eSoundType);
			//if(matchingNote != null && matchingNote.CheckSuccessfulHit())
			{
				m_lActiveChallengeNotes.Remove(matchingNote);
				return true;
            }
			else
			{
				ChallengeScore -= 0.01f;
				Score -= 0.01f;
				VisibleScore -= 0.01f;
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Modify Score
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ModifyScore(float newScore)
	{
		sm_fScore = Mathf.Min(1.0f, Mathf.Max(0.0f, newScore));
		m_rGameManager.CurrentScore = Mathf.CeilToInt(sm_fScore * 100.0f);
	}

	public void IncrementScore()
	{
		VisibleScore += NoteScore;
		m_rGameManager.CurrentScore = Mathf.CeilToInt(VisibleScore * 100.0f);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Challenge Finish
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnChallengeFinish()
	{
		// Show Results?
		bool bShowResults = m_eChallengeActivity == ChallengeActivity.CHALLENGE_MODE;

		// Was Challenge Mode? Then we can update the feathers as needed
		if (bShowResults && !TutorialManager_Base.TutorialOpened)
		{
			ChallengeGameScoreManager.ScoreResult eScoreResult = ChallengeGameScoreManager.GetScoreResult(Mathf.CeilToInt(sm_fScore * 100.0f));
			int iChallengeScore =	(eScoreResult == ChallengeGameScoreManager.ScoreResult.TERRIBLE)	? 0 :
									(eScoreResult == ChallengeGameScoreManager.ScoreResult.OKAY)		? 1 :
									(eScoreResult == ChallengeGameScoreManager.ScoreResult.GOOD)		? 2 : 3;
			int iCompletionCount = SavedPreferenceTool.GetInt(m_ePlaylistTrack.ToString() + "_FeatherCount", 0);
			if (iChallengeScore > iCompletionCount)
			{
				ChallengeFeathersInfo.NewlyObtainedFeathers = (iChallengeScore - iCompletionCount);
                SavedPreferenceTool.SaveInt(m_ePlaylistTrack.ToString() + "_FeatherCount", iChallengeScore);	// Save to the Challenge itself, now you'll see these feathers light up when in the challenge select!
				ChallengeFeathersInfo.AccumulatedFeathers += ChallengeFeathersInfo.NewlyObtainedFeathers;		// Add to the total accumulated Feathers
			}
			else
			{
				ChallengeFeathersInfo.NewlyObtainedFeathers = 0;
            }
		}

		// Stop Active Challenge!
		m_rGameManager.StopChallenge(bShowResults);
		StopBackingTrack();
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnEnable()
	{
		m_lActiveChallengeNotes.Clear();
		m_eChallengeActivity = ChallengeActivity.IDLE;
		m_rGameManager.SetCurrentInstrument(m_eInstrumentSoundType);

		if(m_rInstrumentSoundTypeRenderer != null)
			m_rInstrumentSoundTypeRenderer.sprite = m_sprInstrumentSoundType;

		if (m_rChallengeTitleText != null)
			m_oChallengeName.ApplyEffects( m_rChallengeTitleText );

		//if (m_rPracticeButton != null)
		//	m_rPracticeButton.m_ePlaylist = m_ePlaylistTrack;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Disable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDisable()
	{
#if UseMemoryGameManager
		if(m_rSoundsRhythmMemoryGame != null)
			m_rSoundsRhythmMemoryGame.StopPlayback();
#endif
		StopBackingTrack();
	}







	

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Draw Gizmos
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float gizmoCircleRadius = 0.2f;

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(victoryLocation + transform.localPosition, gizmoCircleRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(targetLocation + transform.localPosition, gizmoCircleRadius);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Notations From Point Onwards
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public LinkedList<Transform> GetNotations(string trName)
	{
		LinkedList<Transform> rList = new LinkedList<Transform>();
		bool found = false;
		foreach (Transform child in transform)
		{
			if (found)
				rList.AddLast(child);
			else
				found = child.name == trName;
		}
		return rList;
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Use Midi For Automatic Placement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "Init", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern void Midi__Init();
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "Dispose", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern void Midi__Dispose();
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "ReadMidiFile", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern void Midi__ReadMidiFile();
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "Empty", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern bool Midi__Empty();
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "Size", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern int Midi__Size();
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "BPM", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern int Midi__BPM();
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "NextNote", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern void Midi__NextNote();
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "GetChannel", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern int Midi__GetChannel();
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "GetKey", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern int Midi__GetKey();
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "GetStartTime", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern float Midi__GetStartTime();
	[System.Runtime.InteropServices.DllImport("MidiInputGrabber", EntryPoint = "GetEndTime", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
	static extern float Midi__GetEndTime();

	public bool UseMidiForAutoPlacement()
	{
		Midi__Init();				// Init DLL Stuff
		Midi__ReadMidiFile();       // Will open up an OpenFileDialog Window that the user shall use to select a midi
		bool bValidMidi = !Midi__Empty();
		if (bValidMidi)
		{
			// We are about to replace all children with new Notes. Remove all current children from this challenge list.
			LinkedList<GameObject> children = new LinkedList<GameObject>();
			foreach (Transform child in transform)
				children.AddLast(child.gameObject);
			foreach (GameObject child in children)
				DestroyImmediate(child);

			// If We have a link to the Rhythm Game. Set that up as well.
#if UseMemoryGameManager
			if (m_rSoundsRhythmMemoryGame != null)
			{
				m_rSoundsRhythmMemoryGame.ResizePlaylistArray(m_ePlaylistTrack, Midi__Size());
			}
#endif

			// Set BPM
			this.BPM = Midi__BPM();


			// Make Challenge Notes
			while (!Midi__Empty())
			{
#if UseMemoryGameManager
				if (m_rSoundsRhythmMemoryGame != null)
				{
					m_rSoundsRhythmMemoryGame.SetupEvent(m_ePlaylistTrack, iCurrentIndex, Midi__GetKey(), Midi__GetStartTime(), Midi__GetEndTime());
				}
#endif

				TambourineSoundsManager.SoundTypes eSoundType = GetMIDISoundTypeFromKey( Midi__GetKey() );
				float fBeatPos = ((Midi__GetStartTime() + StartDelayTime) / MetronomeTime);
				AddNewChallengeNote(eSoundType, fBeatPos);

				Midi__NextNote();
			}
		}

		Midi__Dispose();
		return bValidMidi;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add New CHallenge Note
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void AddNewChallengeNote(TambourineSoundsManager.SoundTypes eSoundType, float fBeatPos)
	{
		GameObject challengeNote = Instantiate(m_goChallengeNotePrefab, Vector3.zero, Quaternion.identity) as GameObject;
		ChallengeNoteMovement cnm = challengeNote.GetComponent<ChallengeNoteMovement>();

		int iCurrentIndex = transform.childCount + 1;
		challengeNote.transform.name = "Challenge Note - " + (iCurrentIndex < 1000 ? "0" : "") + (iCurrentIndex < 100 ? "0" : "") + (iCurrentIndex< 10 ? "0" : "") + iCurrentIndex.ToString();
		challengeNote.transform.parent = this.transform;
		challengeNote.transform.localPosition = Vector3.zero;

		cnm.m_rChallengeModeInfo = this;
		cnm.ChangeSoundType(eSoundType, fBeatPos);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Sound Type From MIDI Key
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TambourineSoundsManager.SoundTypes GetMIDISoundTypeFromKey(int iKeyID)
	{
		switch (iKeyID)
		{
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
			case 72: return TambourineSoundsManager.SoundTypes.RATTLE_TAMBOURINE_SHAKE;
			default: return TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA;
		}
	}

}
