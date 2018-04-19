//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Challenge Results Manager
//             Author: Christopher Diamond
//             Date: October 09, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script fades in/out the Results Screen for the Challenge Game; 
//		including removing the Challenge Game Options and settings menu.
//
//	  This script also handles input for the buttons located inside of 
//		the results screen.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ChallengeResultsManager : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ObjectTransitionAnimation m_rChallengeGameManagerTransition;
	public SubSceneManager m_rChallengeGameSelectionScene;
	public ChallengeGameManager m_rChallengeGameManager;
	public SettingsMenuManager m_rSettingsMenu;
	public Sprite m_sprDisabledFeatherSprite;
	public Sprite m_sprEnabledFeatherSprite;

	public VignetteManager.VignetteInfo m_oVignetteOptions	= new VignetteManager.VignetteInfo();
	public SpriteRenderer m_sprPageBackground;
	public SpriteRenderer m_sprJokosReaction;
	public SpriteRenderer[] m_aSprFeathers					= new SpriteRenderer[3];

	public SpriteRenderer m_sprChallengeFeatherNotifier;
	public UnityEngine.UI.Text m_rEncouragementTextRenderer;
	public TextAnimationEffect m_rEncouragementTextAnimation;
	public UnityEngine.UI.Text m_rChallengeFeatherNotificationText;
	public UnityEngine.UI.Image m_imgExperienceBarBackground;
	public UnityEngine.UI.Image m_imgExperienceBar;

	public QuickDialoguePopupManager m_rFeathersPopupManager;
	public AnimationEffectDesigner m_rUnlockedItemsNotifierBounceAE;
	public UnityEngine.UI.Text m_rUnlockedItemsText;
	public SpriteRenderer[] m_aResultsButtons				= new SpriteRenderer[3];


	public JokosReaction[] m_arReactions					= new JokosReaction[4]	 { new JokosReaction(), new JokosReaction(), new JokosReaction(), new JokosReaction() };
	public AnimationEffect m_rSettingsMenuAE				= new AnimationEffect();
	public AnimationEffect m_rPageBackgroundAE				= new AnimationEffect();
	public AnimationEffect m_rJokosReactionAE				= new AnimationEffect();
	public AnimationEffect[] m_rFeatherSpritesAE			= new AnimationEffect[3] { new AnimationEffect(), new AnimationEffect(), new AnimationEffect() };
	public AnimationEffect[] m_rChallengeFeatherNotifierAE	= new AnimationEffect[2] { new AnimationEffect(), new AnimationEffect() };
	public AnimationEffect[] m_rExperienceBarAE				= new AnimationEffect[2] { new AnimationEffect(), new AnimationEffect() };
	public AnimationEffect m_rUnlockedItemsNotifierAE		= new AnimationEffect();
	public AnimationEffect[] m_rButtonSpritesAE				= new AnimationEffect[3] { new AnimationEffect(), new AnimationEffect(), new AnimationEffect() };

	public MultiLanguageText[] m_aoUnlockedItemsText		= new MultiLanguageText[(int)UnlockedItems.NO_UNLOCKS];

	public AudioClip m_acFeatherGETSoundEffect;
	public AudioClip m_acChallengeNotifierWhooshSFX;
	public AudioClip m_acDissapointmentSFX;
	public AudioClip m_acExcitedAudioClip;
	public float m_fRevealAnimationSpeed = 1.0f;
	public float m_fDisappearAnimationSpeed = 2.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private ReactionMode m_eReactionMode = ReactionMode.DEFEATED;
	private Coroutine m_ShowAwardedFeathersCoroutine;

	private float m_fAcquiredScore = 0.0f;
	private bool m_bFadeinResults = true;
	private SoundsRhythmMemoryGame.Playlist m_eCurrentChallengeID = SoundsRhythmMemoryGame.Playlist.CHALLENGE_01;
	private TransitionPhase m_eTransitionPhase = TransitionPhase.IDLE;
	private ExitSelection m_eExitSelection = ExitSelection.EXIT;
	private UnlockedItems m_eUnlockedItem = UnlockedItems.NO_UNLOCKS;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	[System.Serializable]
	public class JokosReaction
	{
		public Sprite reactionImage;
		public MultiLanguageText encouragementText = new MultiLanguageText();
	}

	public enum ReactionMode
	{
		DEFEATED	= 0,
		OKAY		= 1,
		GOOD		= 2,
		GREAT		= 3,
	}

	private enum TransitionPhase
	{
		CHALLENGE_MODE			= 0,
		SETTINGS_MENU			= 1,
		RESULTS_BG				= 2,
		RESULTS_PAGE			= 3,
		JOKOS_REACTION			= 4,
		RESULTS_FEATHERS		= 5,
		CHALLENGE_FEATHERS		= 6,
		REACTION_TEXT			= 7,
		EXP_BAR					= 8,
		RECEIVED_FEATHERS_POPUP = 9,
		UNLOCK_NOTIFICATION		= 10,
		RESULTS_BUTTONS			= 11,

		IDLE,
	}

	public enum UnlockedItems
	{
		UNLOCKED_NEW_CHALLENGE,
		UNLOCKED_NEW_CHALLENGE_TIER,
		UNLOCKED_GAME_ENDING,

		NO_UNLOCKS // <= Leave Last!
	}

	public enum ExitSelection
	{
		EXIT,
		RETRY,
		NEXT_CHALLENGE,
	}
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{	
		// Setup Animations
		m_rSettingsMenuAE.Setup(m_rSettingsMenu.gameObject);		
		m_rPageBackgroundAE.Setup(m_sprPageBackground.gameObject);
		m_rJokosReactionAE.Setup(m_sprJokosReaction.gameObject);

		m_rFeatherSpritesAE[0].Setup(m_aSprFeathers[0].gameObject);
		m_rFeatherSpritesAE[1].Setup(m_aSprFeathers[1].gameObject);
		m_rFeatherSpritesAE[2].Setup(m_aSprFeathers[2].gameObject);

		m_rChallengeFeatherNotifierAE[0].Setup(m_sprChallengeFeatherNotifier.gameObject);
		m_rChallengeFeatherNotifierAE[1].Setup(m_rChallengeFeatherNotificationText.gameObject);

		m_rExperienceBarAE[0].Setup(m_imgExperienceBarBackground.gameObject);
		m_rExperienceBarAE[1].Setup(m_imgExperienceBar.gameObject);

		m_rUnlockedItemsNotifierAE.Setup(m_rUnlockedItemsNotifierBounceAE.gameObject);

		m_rButtonSpritesAE[0].Setup(m_aResultsButtons[0].gameObject);
		m_rButtonSpritesAE[1].Setup(m_aResultsButtons[1].gameObject);
		m_rButtonSpritesAE[2].Setup(m_aResultsButtons[2].gameObject);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		switch (m_eTransitionPhase)
		{
			case TransitionPhase.CHALLENGE_MODE:		{ UpdateChallengeModeTransition();			break; }
			case TransitionPhase.SETTINGS_MENU:			{ UpdateSettingsMenuTransition();			break; }
			case TransitionPhase.RESULTS_BG:			{ UpdateResultsBGTransition();				break; }
			case TransitionPhase.RESULTS_PAGE:			{ UpdateResultsPageTransition();			break; }
			case TransitionPhase.JOKOS_REACTION:		{ UpdateJokosReactionTransition();			break; }
			case TransitionPhase.RESULTS_FEATHERS:		{ UpdateJokosFeathersTransition();			break; }
			case TransitionPhase.CHALLENGE_FEATHERS:	{ UpdateChallengeFeathersNotification();	break; }
			case TransitionPhase.REACTION_TEXT:			{ UpdateReactionTextTransition();			break; }
			case TransitionPhase.EXP_BAR:				{ UpdateEXPBarTransition();					break; }
			case TransitionPhase.UNLOCK_NOTIFICATION:	{ UpdateUnlockedItemsNotification();		break; }
            case TransitionPhase.RESULTS_BUTTONS:		{ UpdateResultsButtonsTransition();			break; }
			default:									{											break; }
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Challenge Mode Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateChallengeModeTransition()
	{
		m_rChallengeGameManagerTransition.Disappear(false);
		m_eTransitionPhase = TransitionPhase.SETTINGS_MENU;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Settings Menu Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateSettingsMenuTransition()
	{
		if (m_bFadeinResults)
		{
			if (m_rSettingsMenuAE.UpdateAnimation(m_fRevealAnimationSpeed))
			{
				m_rSettingsMenuAE.Reset();
				m_eTransitionPhase = TransitionPhase.RESULTS_BG;
				VignetteManager.TransitionVignette(m_oVignetteOptions);

				m_rSettingsMenu.GetComponent<Button_ForceSceneObjectDisappear>().enabled = false; // There's nothing to disable/disappear, Buddy!
				m_rSettingsMenu.gameObject.SetActive(false);
			}
		}
		else
		{
			if (m_rSettingsMenuAE.ReverseUpdate(m_fDisappearAnimationSpeed))
			{
				m_rSettingsMenuAE.Reset();
				OnFadeout();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Results BG Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateResultsBGTransition()
	{
		if (m_bFadeinResults)
		{
			if (!VignetteManager.IsTransitioning)
			{
				m_eTransitionPhase = TransitionPhase.RESULTS_PAGE;
				m_sprPageBackground.gameObject.SetActive(true);
			}
		}
		else
		{
			if(!VignetteManager.IsTransitioning)
			{
				m_eTransitionPhase = TransitionPhase.SETTINGS_MENU;
				m_rSettingsMenu.gameObject.SetActive(true);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Results Page Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateResultsPageTransition()
	{
		if (m_bFadeinResults)
		{
			if (m_rPageBackgroundAE.UpdateAnimation(m_fRevealAnimationSpeed))
			{
				m_rPageBackgroundAE.Reset();
				m_eTransitionPhase = TransitionPhase.JOKOS_REACTION;
				m_sprJokosReaction.gameObject.SetActive(true);
			}
		}
		else
		{
			if (m_rPageBackgroundAE.ReverseUpdate(m_fDisappearAnimationSpeed))
			{
				m_rPageBackgroundAE.Reset();
				m_eTransitionPhase = TransitionPhase.RESULTS_BG;
				VignetteManager.TransitionVignette(0.0f, m_oVignetteOptions.transitionTime);
				m_sprPageBackground.gameObject.SetActive(false);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Joko's Reaction Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateJokosReactionTransition()
	{
		if (m_bFadeinResults)
		{
			if (m_rJokosReactionAE.UpdateAnimation(m_fRevealAnimationSpeed))
			{
				m_rJokosReactionAE.Reset();
				m_eTransitionPhase = TransitionPhase.RESULTS_FEATHERS;
				m_aSprFeathers[0].gameObject.SetActive(true);
				m_aSprFeathers[1].gameObject.SetActive(true);
				m_aSprFeathers[2].gameObject.SetActive(true);
			}
		}
		else
		{
			if (m_rJokosReactionAE.ReverseUpdate(m_fDisappearAnimationSpeed))
			{
				m_rJokosReactionAE.Reset();
				m_eTransitionPhase = TransitionPhase.RESULTS_PAGE;
				m_sprJokosReaction.gameObject.SetActive(false);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Joko's Feathers Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateJokosFeathersTransition()
	{
		if (m_bFadeinResults)
		{
			m_rFeatherSpritesAE[2].UpdateAnimation(m_fRevealAnimationSpeed);
			m_rFeatherSpritesAE[1].UpdateAnimation(m_fRevealAnimationSpeed);
			if (m_rFeatherSpritesAE[0].UpdateAnimation(m_fRevealAnimationSpeed))
			{
				m_rFeatherSpritesAE[2].Reset();
				m_rFeatherSpritesAE[1].Reset();
				m_rFeatherSpritesAE[0].Reset();

				m_sprChallengeFeatherNotifier.gameObject.SetActive(true);
				m_rChallengeFeatherNotificationText.gameObject.SetActive(true);
				m_eTransitionPhase = TransitionPhase.CHALLENGE_FEATHERS;
			}
		}
		else
		{
			m_rFeatherSpritesAE[2].ReverseUpdate(m_fDisappearAnimationSpeed);
			m_rFeatherSpritesAE[1].ReverseUpdate(m_fDisappearAnimationSpeed);
			if (m_rFeatherSpritesAE[0].ReverseUpdate(m_fDisappearAnimationSpeed))
			{
				m_rFeatherSpritesAE[2].Reset();
				m_rFeatherSpritesAE[1].Reset();
				m_rFeatherSpritesAE[0].Reset();
				m_eTransitionPhase = TransitionPhase.JOKOS_REACTION;
				m_aSprFeathers[0].gameObject.SetActive(false);
				m_aSprFeathers[1].gameObject.SetActive(false);
				m_aSprFeathers[2].gameObject.SetActive(false);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Challenge Feathers Notification
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateChallengeFeathersNotification()
	{
		if (m_bFadeinResults)
		{
			m_rChallengeFeatherNotifierAE[1].UpdateAnimation(m_fRevealAnimationSpeed);
			if (m_rChallengeFeatherNotifierAE[0].UpdateAnimation(m_fRevealAnimationSpeed))
			{
				m_rChallengeFeatherNotifierAE[1].Reset();
				m_rChallengeFeatherNotifierAE[0].Reset();
				m_eTransitionPhase = TransitionPhase.REACTION_TEXT;
			}

		}
		else
		{
			m_rChallengeFeatherNotifierAE[1].ReverseUpdate(m_fDisappearAnimationSpeed);
			if (m_rChallengeFeatherNotifierAE[0].ReverseUpdate(m_fDisappearAnimationSpeed))
			{
				m_rChallengeFeatherNotifierAE[1].Reset();
				m_rChallengeFeatherNotifierAE[0].Reset();
				m_eTransitionPhase = TransitionPhase.RESULTS_FEATHERS;
				m_sprChallengeFeatherNotifier.gameObject.SetActive(false);
				m_rChallengeFeatherNotificationText.gameObject.SetActive(false);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Reaction Text Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateReactionTextTransition()
	{
		if (m_bFadeinResults)
		{
			m_rEncouragementTextRenderer.gameObject.SetActive(true);
			m_arReactions[(int)m_eReactionMode].encouragementText.ApplyEffects( m_rEncouragementTextRenderer );
			m_rEncouragementTextAnimation.SetText( m_rEncouragementTextRenderer.text );

			if(ChallengeFeathersInfo.NewlyObtainedFeathers > 0)
			{
				m_ShowAwardedFeathersCoroutine = StartCoroutine(ShowRewardedFeathersPopup(ChallengeFeathersInfo.NewlyObtainedFeathers));
				m_eTransitionPhase = TransitionPhase.RECEIVED_FEATHERS_POPUP;
			}
			else
			{
				m_eTransitionPhase = TransitionPhase.EXP_BAR;
				m_imgExperienceBarBackground.gameObject.SetActive(true);
				m_imgExperienceBar.gameObject.SetActive(true);
			}			

			m_aResultsButtons[0].gameObject.SetActive(true);
			m_aResultsButtons[1].gameObject.SetActive(true);
			m_aResultsButtons[2].gameObject.SetActive(true);
		}
		else
		{
			m_rEncouragementTextRenderer.gameObject.SetActive(false);
			m_rEncouragementTextRenderer.text = "";
			m_eTransitionPhase = TransitionPhase.CHALLENGE_FEATHERS;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Rewarded Feathers Pop-up
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private IEnumerator ShowRewardedFeathersPopup(int iAwardedFeathersCount)
	{
		float fWaitTimeBetweenFeathers = 0.5f;
		int iAccFeathers = 0;;
		for(int i = 1; i <= iAwardedFeathersCount; ++i)
		{
			if(m_acFeatherGETSoundEffect != null)
			{
				AudioSourceManager.PlayAudioClip(m_acFeatherGETSoundEffect);
            }
            m_rFeathersPopupManager.ShowQuickDialoguePopup(this.transform);

			iAccFeathers = ChallengeFeathersInfo.PreviouslyAccumulatedFeathers + i;
			m_rChallengeFeatherNotificationText.text = "x" + (iAccFeathers < 10 ? "0" : "") + iAccFeathers.ToString();

			yield return new WaitForSeconds(fWaitTimeBetweenFeathers);
        }
		m_eTransitionPhase = TransitionPhase.EXP_BAR;
		m_imgExperienceBarBackground.gameObject.SetActive(true);
		m_imgExperienceBar.gameObject.SetActive(true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update EXP Bar Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateEXPBarTransition()
	{
		if (m_bFadeinResults)
		{
			if (m_rExperienceBarAE[0].IsCompleted())
			{
				if (m_rExperienceBarAE[1].UpdateAnimation(m_fRevealAnimationSpeed))
				{
					m_imgExperienceBar.fillAmount = m_fAcquiredScore;
					m_rExperienceBarAE[0].Reset();
					m_rExperienceBarAE[1].Reset();

					// PLAY HAPPY/UNHAPPY SFX!
					if(m_eReactionMode == ReactionMode.DEFEATED || m_eReactionMode == ReactionMode.OKAY)
					{
						if(m_acDissapointmentSFX != null)
							AudioSourceManager.PlayAudioClip(m_acDissapointmentSFX);
					}
					else
					{
						if(m_acExcitedAudioClip != null)
							AudioSourceManager.PlayAudioClip(m_acExcitedAudioClip);
					}

					// SHOWING 'UNLOCKED ITEM' INFO?
					if(m_eUnlockedItem == UnlockedItems.NO_UNLOCKS)
					{
						m_eTransitionPhase = TransitionPhase.RESULTS_BUTTONS;
						m_aResultsButtons[0].gameObject.SetActive(true);
						m_aResultsButtons[1].gameObject.SetActive(true);
						m_aResultsButtons[2].gameObject.SetActive(true);
					}
					else
					{
						m_eTransitionPhase = TransitionPhase.UNLOCK_NOTIFICATION;
						m_rUnlockedItemsNotifierBounceAE.gameObject.SetActive(true);
						m_aoUnlockedItemsText[(int)m_eUnlockedItem].ApplyEffects( m_rUnlockedItemsText );
						if(m_acChallengeNotifierWhooshSFX != null)
							AudioSourceManager.PlayAudioClip(m_acChallengeNotifierWhooshSFX);
                    }
                }
				else
				{
					m_imgExperienceBar.fillAmount = Mathf.Lerp(0, m_fAcquiredScore, m_rExperienceBarAE[1].CompletionPercentage);
				}
			}
			else
			{
				m_rExperienceBarAE[0].UpdateAnimation(m_fRevealAnimationSpeed);
			}
		}
		else
		{
			if (m_rExperienceBarAE[1].IsCompleted())
			{
				if (m_rExperienceBarAE[0].ReverseUpdate(m_fDisappearAnimationSpeed))
				{
					m_imgExperienceBar.fillAmount = 0.0f;
					m_rExperienceBarAE[0].Reset();
					m_rExperienceBarAE[1].Reset();
					m_eTransitionPhase = TransitionPhase.REACTION_TEXT;
					m_imgExperienceBarBackground.gameObject.SetActive(false);
					m_imgExperienceBar.gameObject.SetActive(false);
				}
			}
			else
			{
				m_rExperienceBarAE[1].ReverseUpdate(m_fDisappearAnimationSpeed);
				m_imgExperienceBar.fillAmount = Mathf.Lerp(m_fAcquiredScore, 0, m_rExperienceBarAE[1].CompletionPercentage);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Unlocked Items Notification
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateUnlockedItemsNotification()
	{
		if(m_bFadeinResults)
		{
			if(m_rUnlockedItemsNotifierAE.UpdateAnimation())
			{
				m_rUnlockedItemsNotifierAE.Reset();
				m_rUnlockedItemsNotifierBounceAE.enabled = true;
				m_eTransitionPhase = TransitionPhase.RESULTS_BUTTONS;
			}
		}
		else
		{
			if(m_rUnlockedItemsNotifierAE.ReverseUpdate())
			{
				m_rUnlockedItemsNotifierAE.Reset();
				m_rUnlockedItemsNotifierBounceAE.enabled = false;
				m_rUnlockedItemsNotifierBounceAE.gameObject.SetActive(false);
				m_eTransitionPhase = TransitionPhase.EXP_BAR;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Results Buttons Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateResultsButtonsTransition()
	{
		if (m_bFadeinResults)
		{
			m_rButtonSpritesAE[2].UpdateAnimation(m_fRevealAnimationSpeed);
			m_rButtonSpritesAE[1].UpdateAnimation(m_fRevealAnimationSpeed);
			if (m_rButtonSpritesAE[0].UpdateAnimation(m_fRevealAnimationSpeed))
			{
				m_rButtonSpritesAE[2].Reset();
				m_rButtonSpritesAE[1].Reset();
				m_rButtonSpritesAE[0].Reset();
				OnFadein();
			}
		}
		else
		{
			m_rButtonSpritesAE[2].ReverseUpdate(m_fDisappearAnimationSpeed);
			m_rButtonSpritesAE[1].ReverseUpdate(m_fDisappearAnimationSpeed);
			if (m_rButtonSpritesAE[0].ReverseUpdate(m_fDisappearAnimationSpeed))
			{
				m_rButtonSpritesAE[2].Reset();
				m_rButtonSpritesAE[1].Reset();
				m_rButtonSpritesAE[0].Reset();
				m_eTransitionPhase = (m_eUnlockedItem == UnlockedItems.NO_UNLOCKS ? TransitionPhase.EXP_BAR : TransitionPhase.UNLOCK_NOTIFICATION);
				m_aResultsButtons[0].gameObject.SetActive(false);
				m_aResultsButtons[1].gameObject.SetActive(false);
				m_aResultsButtons[2].gameObject.SetActive(false);
			}
		}
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Fadein
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginFadein(SoundsRhythmMemoryGame.Playlist a_eChallengeID, int a_iScore, UnlockedItems a_eUnlockedItems)
	{
		m_eCurrentChallengeID = a_eChallengeID;
		m_eUnlockedItem = a_eUnlockedItems;

		int iAccFeathers = ChallengeFeathersInfo.PreviouslyAccumulatedFeathers;
        m_rChallengeFeatherNotificationText.text = "x" + (iAccFeathers < 10 ? "0" :"") + iAccFeathers.ToString();

		// Stop Coroutine if active!
		if(m_ShowAwardedFeathersCoroutine != null)
		{
			StopCoroutine(m_ShowAwardedFeathersCoroutine);
        }

		// Joko's Reaction Settings
		ChallengeGameScoreManager.ScoreResult eScoreResult = ChallengeGameScoreManager.GetScoreResult(a_iScore);
		m_eReactionMode =	(eScoreResult == ChallengeGameScoreManager.ScoreResult.GREAT) ? ReactionMode.GREAT :
							(eScoreResult == ChallengeGameScoreManager.ScoreResult.GOOD)  ? ReactionMode.GOOD  :
							(eScoreResult == ChallengeGameScoreManager.ScoreResult.OKAY)  ? ReactionMode.OKAY  :
																							ReactionMode.DEFEATED;
		// EXP Bar Settings
		m_fAcquiredScore = ((float)a_iScore / 100);
		m_imgExperienceBar.fillMethod = (a_iScore > 99 ? UnityEngine.UI.Image.FillMethod.Radial180 : UnityEngine.UI.Image.FillMethod.Horizontal);
		m_sprJokosReaction.sprite = m_arReactions[(int)m_eReactionMode].reactionImage;

		// Feather Options
		m_aSprFeathers[0].sprite = (eScoreResult != ChallengeGameScoreManager.ScoreResult.TERRIBLE ? m_sprEnabledFeatherSprite : m_sprDisabledFeatherSprite);
		m_aSprFeathers[1].sprite = ((eScoreResult == ChallengeGameScoreManager.ScoreResult.GOOD || eScoreResult == ChallengeGameScoreManager.ScoreResult.GREAT) ? m_sprEnabledFeatherSprite : m_sprDisabledFeatherSprite);
		m_aSprFeathers[2].sprite = (eScoreResult == ChallengeGameScoreManager.ScoreResult.GREAT ? m_sprEnabledFeatherSprite : m_sprDisabledFeatherSprite);

		// Toggle animation and disable user input for now
		ShowFirstAnimationFrames();
		m_eTransitionPhase = TransitionPhase.CHALLENGE_MODE;
		m_bFadeinResults = true;
		ButtonManager.ToggleAllButtons(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Fadeout
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginFadeout(ExitSelection eSelection)
	{
		m_bFadeinResults = false; // Fadeout
		m_eTransitionPhase = TransitionPhase.RESULTS_BUTTONS;
		m_eExitSelection = eSelection;
		ButtonManager.ToggleAllButtons(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Fadein Complete
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnFadein()
	{
		m_eTransitionPhase = TransitionPhase.IDLE;
		ButtonManager.ToggleAllButtons(ButtonManager.ButtonType.MENU, true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Fadeout
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnFadeout()
	{
		m_eTransitionPhase = TransitionPhase.IDLE;

		switch(m_eExitSelection)
		{
			case ExitSelection.RETRY:
				OnRetrySelected();
				break;

			case ExitSelection.NEXT_CHALLENGE:
				OnNextChallengeSelected();
				break;

			default:     /* ...EXIT */
				OnExitSelected();
				break;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Retry Selected
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnRetrySelected()
	{
		m_rChallengeGameManagerTransition.Reveal(false);
		ButtonManager.ToggleAllButtons(true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On NextChallenge Selected
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnNextChallengeSelected()
	{
		// Return to ChallengeSelection if just Passed FinalChallenge
		if(m_eCurrentChallengeID != SoundsRhythmMemoryGame.Playlist.CHALLENGE_25)
		{
			SoundsRhythmMemoryGame.Playlist eNextChallenge = (SoundsRhythmMemoryGame.Playlist)((int)m_eCurrentChallengeID + 1);
			if((m_rChallengeGameManager.DoesChallengeRequireFullVersion(eNextChallenge) && !GameManager.IsFullVersion) || m_rChallengeGameManager.GetFeathersRemainingToUnlockChallenge(eNextChallenge) > 0)
			{
				m_rChallengeGameSelectionScene.ShowSubscene();
				ButtonManager.ToggleAllButtons(ButtonManager.ButtonType.MENU, true);
				return;
			}
			else
			{
				m_rChallengeGameManager.CurrentChallengeID += 1;
				m_rChallengeGameManagerTransition.Reveal(false);
				ButtonManager.ToggleAllButtons(true);
				return;
			}
		}
		else
		{
			OnExitSelected();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Exit Selected
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnExitSelected()
	{
		m_rChallengeGameSelectionScene.ShowSubscene();
		ButtonManager.ToggleAllButtons(ButtonManager.ButtonType.MENU, true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show First Animation Frames
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ShowFirstAnimationFrames()
	{
		m_rEncouragementTextRenderer.text = "";
		VignetteManager.CurrentAlpha = 0.0f;
		m_rPageBackgroundAE.ShowFirstFrame();
		m_rJokosReactionAE.ShowFirstFrame();
		m_rChallengeFeatherNotifierAE[0].ShowFirstFrame();
		m_rChallengeFeatherNotifierAE[1].ShowFirstFrame();
		m_rExperienceBarAE[0].ShowFirstFrame();
		m_rExperienceBarAE[1].ShowFirstFrame();
		m_rFeatherSpritesAE[0].ShowFirstFrame();
		m_rFeatherSpritesAE[1].ShowFirstFrame();
		m_rFeatherSpritesAE[2].ShowFirstFrame();
		m_rButtonSpritesAE[0].ShowFirstFrame();
		m_rButtonSpritesAE[1].ShowFirstFrame();
		m_rButtonSpritesAE[2].ShowFirstFrame();				
	}
}
