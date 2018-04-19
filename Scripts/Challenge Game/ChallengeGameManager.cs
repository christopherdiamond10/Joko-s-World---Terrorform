//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Challenge Game - Challenge Game Manager
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 20, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script simply keeps track of and integrates with the challenge game.
//		Allowing other scripts to interact with the current challenge.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ChallengeGameManager : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ChallengeGameScoreManager m_rScoreManager;
	public ChallengeResultsManager m_rResultsManager;
	public MetronomeTracker m_rMetronomeTracker;
	public InstrumentManager m_rInstrumentManager;
	public ChallengeModeInfo[] m_arChallenges = new ChallengeModeInfo[(int)SoundsRhythmMemoryGame.Playlist.TOTAL_PLAYLISTS];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_iCurrentScore = 0;
	private int m_iCurrentChallenge = 0;
	
	private InstrumentManager.InstrumentMode m_eInstrumentBeforeChange = InstrumentManager.InstrumentMode.RIQ_TAMBOURINE;

	private TimeTracker m_ttFinishWaitTimer = new TimeTracker(1.0f);
	private ChallengePhase m_eChallengePhase = ChallengePhase.INACTIVE;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool Active
	{
		get { return gameObject.activeInHierarchy && CurrentChallenge.Active; }
	}

	public bool IsPractice
	{
		get { return CurrentChallenge.IsPracticeMode; }
	}

	public bool IsChallenge
	{
		get { return CurrentChallenge.IsChallengeMode; }
	}

	public ChallengeModeInfo CurrentChallenge
	{
		get { return m_arChallenges[m_iCurrentChallenge]; }
	}

	public int CurrentChallengeID
	{
		get { return m_iCurrentChallenge; }
		set { if (value != m_iCurrentChallenge && value < m_arChallenges.Length && value >= 0) ChangeCurrentChallengeSelection(value); }
	}

	public string CurrentChallengeIDAsString
	{
		get { return "Challenge " + ((CurrentChallengeID / 5) + 1).ToString() + "." + ((CurrentChallengeID % 5) + 1).ToString(); } // return "Challenge " + ((CurrentChallengeID + 1) < 10 ? "0" : "") + (CurrentChallengeID + 1).ToString(); }
	}

	public string CurrentChallengeName
	{
		get { return CurrentChallenge.m_oChallengeName.EnglishTranslation; }
	}

	public int CurrentScore
	{
		get { return m_iCurrentScore; }
		set { ModifyScore(value); }
	}

	public float CurrentTrackBPM
	{
		get { return CurrentChallenge.ChallengeBPM; }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum ChallengePhase
	{
		INACTIVE,
		PLAYING,
		STOPPING,
		RESULTS,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void Update()
	{
		switch (m_eChallengePhase)
		{
			case ChallengePhase.PLAYING:
				UpdatePlayingPhase();
				break;
			case ChallengePhase.STOPPING:
				UpdateStoppingPhase();
				break;
			case ChallengePhase.RESULTS:
				UpdateResultsPhase();
				break;
			default:
				break;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Playing Phase
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdatePlayingPhase()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Stopping Phase
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateStoppingPhase()
	{
		if (m_ttFinishWaitTimer.Update())
		{
			CurrentChallenge.StopChallenge();
			m_eChallengePhase = ChallengePhase.INACTIVE;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Results Phase
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateResultsPhase()
	{
		if (m_ttFinishWaitTimer.Update())
		{
			CurrentChallenge.StopChallenge();
			m_rResultsManager.BeginFadein(CurrentChallenge.m_ePlaylistTrack, m_iCurrentScore, CheckForUnlockedItems());
			m_eChallengePhase = ChallengePhase.INACTIVE;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Does Challenge Require Full Version?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool DoesChallengeRequireFullVersion(SoundsRhythmMemoryGame.Playlist eChallengeID)
	{
		return m_arChallenges[ (int)eChallengeID ].RequiresFullVersion;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Required Feathers To Unlock Challenge
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public int GetRequiredFeathersAmountForChallenge(SoundsRhythmMemoryGame.Playlist eChallengeID)
	{
		return m_arChallenges[ (int)eChallengeID ].RequiredFeathers;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: How Many Feathers Remaining are Required To Unlock Challenge?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public int GetFeathersRemainingToUnlockChallenge(SoundsRhythmMemoryGame.Playlist eChallengeID)
	{
		return m_arChallenges[ (int)eChallengeID ].RemainingRequiredFeathers;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check for Any Unlocked Items
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private ChallengeResultsManager.UnlockedItems CheckForUnlockedItems()
	{
		const int TOTAL_FEATHERS_PER_CHALLENGE	= 3;	// How many Feather can I be awarded in total per challenge?
		const int TOTAL_CHALLENGES_PER_BLOCK	= 5;	// How many challenges are there between challenge blocks? (Challenge 01-05 in Beginner, Challenge 06-10 in Student, etc)
		const int TOTAL_CHALLENGE_BLOCKS		= 5;	// How Many Challenge Blocks (Difficulty Tiers) are there? (Beginner, Student, Virtuoso, Maestro, Ustad) = 5
		const int TOTAL_FEATHERS_PER_BLOCK		= (TOTAL_FEATHERS_PER_CHALLENGE * TOTAL_CHALLENGES_PER_BLOCK);  // How many feathers in total can I earn per challenge block?
		const int TOTAL_OBTAINABLE_FEATHERS		= (TOTAL_FEATHERS_PER_BLOCK * TOTAL_CHALLENGE_BLOCKS);			// How many Feathers in total can I earn in this game/app?

		// You can't unlock anything if no feathers were obtained~
		if(ChallengeFeathersInfo.NewlyObtainedFeathers > 0)
		{
			int iPreviousFeatherCount = ChallengeFeathersInfo.PreviouslyAccumulatedFeathers;
			int iCurrentFeatherCount = ChallengeFeathersInfo.AccumulatedFeathers;

			// If our feather count has hit the max: Show Game Ending
			if(iCurrentFeatherCount >= TOTAL_OBTAINABLE_FEATHERS)
			{
				return ChallengeResultsManager.UnlockedItems.UNLOCKED_GAME_ENDING;
			}

			// Go Through All Challenges. Find out if they were just unlocked. Keeping in mind that we can't unlock some challenges in the Lite Version
			for(int i = ((int)SoundsRhythmMemoryGame.Playlist.CHALLENGE_25); i >= (int)SoundsRhythmMemoryGame.Playlist.CHALLENGE_01; --i)
			{
				if(m_arChallenges[i].IsChallengeAvailable)
				{
					int iRequiredFeathers = m_arChallenges[i].RequiredFeathers;
					if(iPreviousFeatherCount < iRequiredFeathers && iCurrentFeatherCount >= iRequiredFeathers)
					{
						return ((i % TOTAL_CHALLENGES_PER_BLOCK == 0) ? ChallengeResultsManager.UnlockedItems.UNLOCKED_NEW_CHALLENGE_TIER : ChallengeResultsManager.UnlockedItems.UNLOCKED_NEW_CHALLENGE);
					}
				}
			}
		}
		// If we got here, nothing new was unlocked~
		return ChallengeResultsManager.UnlockedItems.NO_UNLOCKS;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Current Instrument
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetCurrentInstrument(InstrumentManager.InstrumentMode newInstrument)
	{
		m_rInstrumentManager.CurrentInstrumentMode = newInstrument;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Challenge Practice
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginChallengePractice()
	{
        if (!CurrentChallenge.IsChallengeMode && !CurrentChallenge.IsPracticeMode)
        {
            CurrentChallenge.StartAutoPlay();
			m_eChallengePhase = ChallengePhase.PLAYING;
			StartMetronome(CurrentTrackBPM);
			ButtonManager.ToggleAllButtonsExcept(ButtonManager.ButtonType.GAME, false);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Challenge
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginChallenge()
	{
        if (!CurrentChallenge.IsChallengeMode && !CurrentChallenge.IsPracticeMode)
        {
            CurrentChallenge.StartChallenge();
			m_eChallengePhase = ChallengePhase.PLAYING;
			StartMetronome(CurrentTrackBPM);
			ButtonManager.ToggleAllButtonsExcept(ButtonManager.ButtonType.GAME, false);
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Stop Challenge
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StopChallenge()
	{
		StopMetronome();
        CurrentChallenge.StopChallenge();
		ButtonManager.ToggleAllButtons(true);
	}

	public void StopChallenge(bool showResults)
	{
		StopMetronome();
        m_ttFinishWaitTimer.Reset();
		ButtonManager.ToggleAllButtons(true);

		if (TutorialManager_Base.TutorialOpened)
			m_eChallengePhase = ChallengePhase.STOPPING;
		else
			m_eChallengePhase = (showResults ? ChallengePhase.RESULTS : ChallengePhase.STOPPING);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Start Metronome
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StartMetronome(float trackBPM)
	{
		//m_rMetronomeTracker.StartMetronome(trackBPM);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Stop Metronome
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StopMetronome()
	{
		m_rMetronomeTracker.StopMetronome();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Successful Hit?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool SuccessfulHit(TambourineSoundsManager.SoundTypes eSoundType)
	{
		return CurrentChallenge.CheckSuccessfulHit(eSoundType);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Change Current Challenge Selection
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ChangeCurrentChallengeSelection(int a_iCurrentChallenge)
	{
		if(CurrentChallenge != null && CurrentChallenge.gameObject != null)
			CurrentChallenge.gameObject.SetActive(false);
		
		m_iCurrentChallenge = a_iCurrentChallenge;
		CurrentChallenge.gameObject.SetActive(true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Modify Score
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ModifyScore(int newScore)
	{
		newScore = Mathf.Min(100, Mathf.Max(0, newScore));
		//if (newScore != m_iCurrentScore)
		{
			if (m_rScoreManager != null)
			{
				if (newScore > m_iCurrentScore)
					m_rScoreManager.BeginIncrementAnimation(newScore.ToString() + "%");
				else
					m_rScoreManager.BeginDecrementAnimation(newScore.ToString() + "%");
			}
			m_iCurrentScore = newScore;
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnEnable()
	{
		m_eInstrumentBeforeChange = m_rInstrumentManager.CurrentInstrumentMode;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Disabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnDisable()
	{
		m_rInstrumentManager.CurrentInstrumentMode = m_eInstrumentBeforeChange;
		StopMetronome();
	}
}
