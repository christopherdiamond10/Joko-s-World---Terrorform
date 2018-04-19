//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Tutorial Manager - Music Challenges Area
//             Author: Christopher Diamond
//             Date: January 12, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    Deriving from the TutorialManager_Base class, this specific tutorial is 
//		made and intended for use by the Music Challenges Area. This includes
//		handling situations that users will find themselves in such as 
//		Moving and hitting a note at the correct time. 
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TutorialManager_MusicChallengesArea : TutorialManager_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ObjectZoomEffect m_rInstrumentZoomHandler;
	public SettingsMenuManager m_rSettingsMenuManager; // Gotta Turn off the Menu when at a certain point in the tutorial.

	public Vector3 m_vChallengeModeTextBoxPosition;
	public float m_fPositionLerpTime = 3.0f;

	public ChallengeGameManager m_rChallengeGameManager;
	public AnimationEffectDesigner m_rArrowToExampleButton;
	public Button_ChallengeSelect[] m_arChallengeSelectionButtons;

	public ChallengeNotesInfo m_oRedTambourineAreaInfo = new ChallengeNotesInfo();
	public ChallengeNotesInfo m_oBlueTambourineAreaInfo = new ChallengeNotesInfo();
	public ChallengeNotesInfo m_oGreenTambourineAreaInfo = new ChallengeNotesInfo();
	public ChallengeNotesInfo m_oTambourineJingleInfo = new ChallengeNotesInfo();
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Vector3 m_vSavedOriginalTextBoxPosition;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override int EndOfTutorialID			{ get { return (int)TutorialPhases.TUTORIAL_COMPLETE_TEXT; } }
    protected override int FinalTutorialPointID		{ get { return (int)TutorialPhases.PHASES_COUNT; } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum TutorialPhases
	{
		OPEN_OPTIONS_MENU,
		OPEN_MUSIC_CHALLENGES,

		MUSIC_CHALLENGES_EXPLANATION,
		OPEN_CHALLENGES_TUTORIAL,
		
		CHALLENGE_MODE_EXPLANATION,
		PLAY_TUTORIAL_EXAMPLE,
		PLAY_TUTORIAL_CHALLENGE,
		HIT_ALL_NOTES,

		TUTORIAL_COMPLETE_TEXT,

		PHASES_COUNT, // <=== Leave last!
	}

	[System.Serializable]
	public class ChallengeNotesInfo
	{
		public ChallengeNoteMovement challengeNote;
		public Button_Base instrumentButton;
		public AnimationEffectDesigner instrumentHighlight;
		public MultiLanguageText firstAttemptText = new MultiLanguageText();
		public MultiLanguageText retryAttemptText = new MultiLanguageText();
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Initialise Special Tutorial Info
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void InitialiseSpecialutorialInfo()
	{
		m_aSpecialTutorialPoints = new SpecialTutorialPointInfo[]
		{
			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.CHALLENGE_MODE_EXPLANATION,
											 onTutorialStateChange  = ChangeChallengeModeExplanationState,
											 onTutorialPointUpdate  = UpdateNonSpecialTutorialPoint				},

			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.PLAY_TUTORIAL_EXAMPLE,
											 onTutorialStateChange  = ChangePlayTutorialExampleState,
											 onTutorialPointUpdate  = UpdateActiveChallengeState               },

			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.HIT_ALL_NOTES,
											 onTutorialStateChange  = ChangeHitAllNotesState,
											 onTutorialPointUpdate  = UpdateActiveChallengeState               },
		};
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Update Active Challenge State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateActiveChallengeState()
	{
		if(m_eTutorialState == TutorialState.WAITING_FOR_CONTINUE_INPUT)
		{
			if(!m_rChallengeGameManager.Active)
			{
				ContinueTutorial();
				m_oAudioHandlerInfo.SetVolume(m_oAudioHandlerInfo.m_fMaxVolume, 1.5f);
			}
        }
		else
		{
			UpdateNonSpecialTutorialPoint();
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change ChallengeMode Explanation State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ChangeChallengeModeExplanationState(TutorialState eState)
	{
		OnNormalTutorialPointStateChange(eState);
		if(eState == TutorialState.SHOWING_TUTORIAL_TEXT)
		{
			if(m_iStartingTutorialPointID < (int)TutorialPhases.CHALLENGE_MODE_EXPLANATION)
			{
				m_rSettingsMenuManager.gameObject.SetActive(false);
				StartCoroutine(ChangeTutorialTextBoxPositionToChallengeMode());
			}
        }
		else if(eState == TutorialState.WAITING_FOR_TUTORIAL_INPUT)
		{
			ActivateDefaultWaitingForContinueInputState();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change PlayTutorialExample State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ChangePlayTutorialExampleState(TutorialState eState)
	{
		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:		{ ActivateDefaultShowingTutorialTextState();		break; }
			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:  { ActivateDefaultWaitingForTutorialInputState(); break; }// m_rArrowToExampleButton.gameObject.SetActive(true); break; }
			case TutorialState.SHOWING_FOLLOW_UP_TEXT:		{ ContinueTutorial(); m_oAudioHandlerInfo.SetVolume(0.0f, 0.25f); CurrentTutorialInfo.rButtons[0].gameObject.SetActive(false); break; }//m_rArrowToExampleButton.gameObject.SetActive(false); break; }
			default:		 /*WAITING_FOR_CONTINUE_INPUT*/ { 													break; }
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change HitAllNotes State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ChangeHitAllNotesState(TutorialState eState)
	{
		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:		{ ContinueTutorial(); break; }
			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:	{ ContinueTutorial(); break; }
			case TutorialState.SHOWING_FOLLOW_UP_TEXT:		{ ContinueTutorial(); m_oAudioHandlerInfo.SetVolume(0.0f, 0.25f); StartCoroutine(ManagePlayAllNotesState());	break; }
			default:		 /*WAITING_FOR_CONTINUE_INPUT*/ { 																	break; }
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Change TextBox Position to Challenge Mode Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private IEnumerator ChangeTutorialTextBoxPositionToChallengeMode()
	{
		m_bFreezeTutorialUpdate = true;
		{
			Vector3 vStartPos = m_rTextBoxTransitionEffect.transform.localPosition;
            TimeTracker ttLerpTimer = new TimeTracker(m_fPositionLerpTime);
			while(!ttLerpTimer.TimeUp())
			{
				if(ttLerpTimer.Update())
				{
					m_rTextBoxTransitionEffect.transform.localPosition = m_vChallengeModeTextBoxPosition;
				}
				else
				{
					m_rTextBoxTransitionEffect.transform.localPosition = Vector3.Lerp(vStartPos, m_vChallengeModeTextBoxPosition, ttLerpTimer.GetCompletionPercentage());
					yield return new WaitForEndOfFrame();
                }
			}
		}
		m_bFreezeTutorialUpdate = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Manage 'PlayAllNotes' State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private IEnumerator ManagePlayAllNotesState()
	{
		const float RESET_POSITION = 4.0f;
		ChallengeNotesInfo[] aChallengeNotes = new ChallengeNotesInfo[] { m_oRedTambourineAreaInfo, m_oBlueTambourineAreaInfo, m_oGreenTambourineAreaInfo, m_oTambourineJingleInfo };

		m_bFreezeTutorialUpdate = true;
		{
			// Go through each ChallengeNote starting from the first. Only move on to the next challenge note once it has been a confirmed successful hit
			for(int i = 0; i < aChallengeNotes.Length; ++i)
			{
				// Show FirstAttempt Text and activate the Corresponding tutorial button plus the highlight effect to help guide the user
				aChallengeNotes[i].firstAttemptText.ApplyEffects( m_rTutorialTextComponent );
				m_rTextAnimationEffect.SetText(m_rTutorialTextComponent.text, TextAnimationEffect.FadeinAnimationType.INSTANT);
				aChallengeNotes[i].instrumentHighlight.gameObject.SetActive(true);
				aChallengeNotes[i].instrumentButton.ActivateButton();

				// Don't continue UNTIL we have a confirmed hit!
				while(aChallengeNotes[i].challengeNote.CurrentState != ChallengeNoteMovement.MovementPhase.SUCCESS)
				{
					if(aChallengeNotes[i].challengeNote.CurrentState == ChallengeNoteMovement.MovementPhase.MISS)
					{
						// If the note was missed, show the 'Retry' text. Also iterate through each remaining note and push them back so that the challenge can be attempted again
						aChallengeNotes[i].retryAttemptText.ApplyEffects( m_rTutorialTextComponent );
						m_rTextAnimationEffect.SetText(m_rTutorialTextComponent.text, TextAnimationEffect.FadeinAnimationType.INSTANT);
						m_rChallengeGameManager.StartMetronome(m_rChallengeGameManager.CurrentTrackBPM);
						for(int j = i, startindex = 1; j < aChallengeNotes.Length; ++j, ++startindex)
						{
							aChallengeNotes[j].challengeNote.BeginMovement(RESET_POSITION * startindex);
						}
                    }
					yield return new WaitForEndOfFrame();
                }

				// Alright! Good Job User! Turn off the corresponding button and it's highlight effect. It there are still notes after this, the 'for' loop will kick in and repeat for others.
				aChallengeNotes[i].instrumentHighlight.gameObject.SetActive(false);
				aChallengeNotes[i].instrumentButton.DeactivateButton();
            }
		}
		m_bFreezeTutorialUpdate = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Change Tutorial State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void ChangeTutorialState(TutorialState eState)
	{
		base.ChangeTutorialState(eState);

		foreach(Button_ChallengeSelect selector in m_arChallengeSelectionButtons)
		{
			selector.enabled = true;
			selector.ButtonCollider.enabled = false;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Callback Method: On Exit Tutorial Via Button
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void OnExitTutorialViaButton()
	{
		base.OnExitTutorialViaButton();

		if(CurrentTutorialPointID >= (int)TutorialPhases.CHALLENGE_MODE_EXPLANATION && CurrentTutorialPointID <= (int)TutorialPhases.TUTORIAL_COMPLETE_TEXT)
		{
			m_rOwningSubscene.ShowSubscene();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnEnable()
	{
		float newTambSize = 0.731f;
		m_rInstrumentZoomHandler.SetValue(newTambSize);

		m_vSavedOriginalTextBoxPosition = m_rTextBoxTransitionEffect.m_aRevealAnimationEffect[0].m_vEndPosition;
		if(m_iStartingTutorialPointID >= (int)TutorialPhases.CHALLENGE_MODE_EXPLANATION && m_iStartingTutorialPointID <= (int)TutorialPhases.TUTORIAL_COMPLETE_TEXT)
		{
			m_rSettingsMenuManager.gameObject.SetActive(false);
			m_rTextBoxTransitionEffect.m_aRevealAnimationEffect[0].m_vEndPosition = m_vChallengeModeTextBoxPosition;
		}
		base.OnEnable();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Disable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnDisable()
	{
		base.OnDisable();

		m_rSettingsMenuManager.gameObject.SetActive(true);

		float newTambSize = 0.731f;
		m_rInstrumentZoomHandler.SetValue(SavedPreferenceTool.GetFloat("SizePreference", newTambSize));
		m_rTextBoxTransitionEffect.m_aRevealAnimationEffect[0].m_vEndPosition = m_vSavedOriginalTextBoxPosition;

		if(m_oRedTambourineAreaInfo.instrumentHighlight != null)	m_oRedTambourineAreaInfo.instrumentHighlight.gameObject.SetActive(false);
		if(m_oBlueTambourineAreaInfo.instrumentHighlight != null)	m_oBlueTambourineAreaInfo.instrumentHighlight.gameObject.SetActive(false);
		if(m_oGreenTambourineAreaInfo.instrumentHighlight != null)	m_oGreenTambourineAreaInfo.instrumentHighlight.gameObject.SetActive(false);
		if(m_oTambourineJingleInfo.instrumentHighlight != null)		m_oTambourineJingleInfo.instrumentHighlight.gameObject.SetActive(false);
	}
}
