//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Tutorial Manager - Main Instrument Area
//             Author: Christopher Diamond
//             Date: December 16, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    Deriving from the TutorialManager_Base class, this specific tutorial is 
//		made and intended for use by the Main Instrument Area. This includes
//		handling situations that users will find themselves in such as the 
//		SettingsMenu and the InstrumentSpecial; such as blowing into the 
//		microphone (Joko's Flute) or shaking the device (Joko's Tambourine).
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;

public class TutorialManager_MainInstrumentArea : TutorialManager_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~	
	public TambourineShakeDetector m_rShakeDetector;
	public InstrumentManager m_rInstrumentManager;
	public ObjectZoomEffect m_rTambourineZoomHandler;


	public Button_TambourineSound m_rRiqCenterRimButton;
	public Button_TambourineSound m_rRiqMiddleRimButton;
	public Button_TambourineSound m_rRiqOuterRimButton;

	public GameObject m_goArrowsToCymbals;
	public Button_TambourineCymbal[] m_arCymbals;
	public Button_ForceSceneObjectDisappear m_rSubsceneExitButton;

	public MultiLanguageText m_oLiteVersionKanjiraSelectText  = new MultiLanguageText();
	public MultiLanguageText m_oLiteVersionPandeiroSelectText = new MultiLanguageText();
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool  m_bPriorTambTargetsVisible;							// Store whether or not the Tambourine Targets were Visible prior to tutorial taking place
	private float m_fPriorShakeSensitivity;								// Shake Sensitivity Before Shake TutorialPoint! We will be setting it to 100% sensitivty during this tutorial point. Then changing it back to what it was when tutorial is finished.
	private float m_fPriorInstrumentSize;								// The size of the Instrument prior to tutorial. The tutorial was made with a specific size in mind, so we can't allow this to be dynamic... :/
	private InstrumentManager.InstrumentMode m_ePriorInstrumentMode;	// Which Instrument were we before we forced the Riq on the User?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override int				 NextTutorialPointID	{ get { return GetNextTutorialPointID(); } }
	protected override int				 FinalTutorialPointID	{ get { return (int)TutorialPhases.PHASES_COUNT; } }
	protected override int				 EndOfTutorialID		{ get { return (int)TutorialPhases.OPTIONS_MENU__WHERE_TO_FIND_TUTORIAL; } }
    protected override MultiLanguageText CurrentTutorialText	{ get { return GetCurrentTutorialText(); } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum TutorialPhases
	{
		WELCOME_TO_JOKOS_TAMBOURINE,
		RIQ_CENTER_RIM,
		RIQ_MIDDLE_RIM,
		RIQ_OUTER_RIM,
		RIQ_CYMBALS,
		RIQ_SHAKE,

		OPEN_OPTIONS_MENU,
		OPTIONS_MENU__TAMBOURINE_TARGETS_SELECTOR,
		OPTIONS_MENU__TAMBOURINE_SOUND_TYPE_SELECTOR,

		// Special Tutorial Points. Cannot select both... Only one. The other will be skipped.
		OPTIONS_MENU__PANDEIRO_SELECT,
		OPTIONS_MENU__KANJIRA_SELECT,

		OPTIONS_MENU__WHERE_TO_FIND_TUTORIAL,		// Show the Tutorial Button in the menu, so the user knows they can come here if they need a refresher		
		OPTIONS_MENU__CHOOSE_NEXT_DESTINATION,		// Where does the user want to go now?		Back to Instrument Game, Guide Book, or Music Challenges?


		PHASES_COUNT, // <=== Leave last!
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Unity Callback Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Start()
	{
		base.Start();

		// If MainInstrument Tutorial Completed... Turn it off and hide the fact it was ever turned on to begin with (>_<)
		if(HasCompletedTutorial)
		{
			StopAllCoroutines();
			ExitTutorial();
			VignetteManager.TransitionVignette(0.0f, 0.5f);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Next Tutorial Point ID
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int GetNextTutorialPointID()
	{
		if(CurrentTutorialPointID == (int)TutorialPhases.OPTIONS_MENU__PANDEIRO_SELECT || CurrentTutorialPointID == (int)TutorialPhases.OPTIONS_MENU__KANJIRA_SELECT)
			return (int)TutorialPhases.OPTIONS_MENU__WHERE_TO_FIND_TUTORIAL;

		return base.NextTutorialPointID;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Current Tutorial Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private MultiLanguageText GetCurrentTutorialText()
	{
		// If not full Version, show modified text warning the user about there being a limit on how long they can play the locked instruments
		if(!GameManager.IsFullVersion)
		{
			if(CurrentTutorialPointID == (int)TutorialPhases.OPTIONS_MENU__KANJIRA_SELECT)
				return m_oLiteVersionKanjiraSelectText;
			else if(CurrentTutorialPointID == (int)TutorialPhases.OPTIONS_MENU__PANDEIRO_SELECT)
				return m_oLiteVersionPandeiroSelectText;
		}
		return base.CurrentTutorialText;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Initialise Special Tutorial Info
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void InitialiseSpecialutorialInfo()
	{
		m_aSpecialTutorialPoints = new SpecialTutorialPointInfo[]
		{
			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.RIQ_CENTER_RIM,
											 onTutorialStateChange  = ChangeTambourineCenterRimTutorialPointState,
											 onTutorialPointUpdate  = UpdateNonSpecialTutorialPoint            },

			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.RIQ_MIDDLE_RIM,
											 onTutorialStateChange  = ChangeTambourineMiddleRimTutorialPointState,
											 onTutorialPointUpdate  = UpdateNonSpecialTutorialPoint            },

			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.RIQ_OUTER_RIM,
											 onTutorialStateChange  = ChangeTambourineOuterRimTutorialPointState,
											 onTutorialPointUpdate  = UpdateNonSpecialTutorialPoint            },

			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.RIQ_CYMBALS,
											 onTutorialStateChange  = ChangeTambourineCymbalsTutorialPointState,
											 onTutorialPointUpdate  = UpdateNonSpecialTutorialPoint            },

			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.RIQ_SHAKE,
											 onTutorialStateChange  = ChangeRiqShakeTutorialPointState,
											 onTutorialPointUpdate  = UpdateTambourineShakeTutorialPoint       },
		};
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Update Tambourine Shake Tutorial Point
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateTambourineShakeTutorialPoint()
	{
		switch (m_eTutorialState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT: case TutorialState.SHOWING_FOLLOW_UP_TEXT:
			{
				UpdateTextFade();
				break;
			}

			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:
			{
				if (m_rShakeDetector.HasTriggeredShake)
				{
					ContinueTutorial();
				}
				break;
			}

			default: // TutorialState.WAITING_FOR_CONTINUE_INPUT
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change Tambourine CenterRim TutorialPoint State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ChangeTambourineCenterRimTutorialPointState(TutorialState eState)
	{			
		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:
			{
				ActivateDefaultShowingTutorialTextState();
				break;
			}

			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:
			{
				ActivateDefaultWaitingForTutorialInputState();
				break;
			}

			case TutorialState.SHOWING_FOLLOW_UP_TEXT:
			{
				ActivateDefaultShowingFollowUpTextState();
				DeactivateTutorialButtonsAnimationEffects(CurrentTutorialPointID);
				break;
			}			

			default: /*WAITING_FOR_CONTINUE_INPUT*/
			{
				ActivateDefaultWaitingForContinueInputState();
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change Tambourine MiddleRim TutorialPoint State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ChangeTambourineMiddleRimTutorialPointState(TutorialState eState)
	{
		// Allow the user to keep playing all of the unlock Tambourine Sounds up to this point
		m_rRiqCenterRimButton.ActivateButton();



		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:
			{
				ActivateDefaultShowingTutorialTextState();
				break;
			}

			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:
			{
				ActivateDefaultWaitingForTutorialInputState();
				break;
			}

			case TutorialState.SHOWING_FOLLOW_UP_TEXT:
			{
				ActivateDefaultShowingFollowUpTextState();
				DeactivateTutorialButtonsAnimationEffects(CurrentTutorialPointID);
				break;
			}			

			default: /*WAITING_FOR_CONTINUE_INPUT*/
			{
				ActivateDefaultWaitingForContinueInputState();
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change Tambourine OuterRim TutorialPoint State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ChangeTambourineOuterRimTutorialPointState(TutorialState eState)
	{
		// Allow the user to keep playing all of the unlock Tambourine Sounds up to this point
		m_rRiqCenterRimButton.ActivateButton();
		m_rRiqMiddleRimButton.ActivateButton();
			


		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:
			{
				ActivateDefaultShowingTutorialTextState();
				break;
			}

			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:
			{
				ActivateDefaultWaitingForTutorialInputState();
				break;
			}

			case TutorialState.SHOWING_FOLLOW_UP_TEXT:
			{
				ActivateDefaultShowingFollowUpTextState();
				DeactivateTutorialButtonsAnimationEffects(CurrentTutorialPointID);
				break;
			}	

			default: /*WAITING_FOR_CONTINUE_INPUT*/
			{
				ActivateDefaultWaitingForContinueInputState();
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change Tambourine Cymbals TutorialPoint State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ChangeTambourineCymbalsTutorialPointState(TutorialState eState)
	{
		// Allow the user to keep playing all of the unlock Tambourine Sounds up to this point
		m_rRiqCenterRimButton.ActivateButton();
		m_rRiqMiddleRimButton.ActivateButton();
		m_rRiqOuterRimButton.ActivateButton();



		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:
			{
				ActivateDefaultShowingTutorialTextState();
				break;
			}

			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:
			{
				ActivateDefaultWaitingForTutorialInputState();
				m_goArrowsToCymbals.SetActive(true);
                break;
			}

			case TutorialState.SHOWING_FOLLOW_UP_TEXT:
			{
				// Since buttons get switched off and locked whenever the state changes. Reactivate the cymbal buttons, but turn off their collider.
				//	This will prevent them from being able to be pressed. BUT, they can still update their animation. If tapped via the TutorialPointOfInterest
				//	the cymbal will still animate correctly. Otherwise if not reactivated this way. It will pause and look weird. Additionally... We don't need the Arrows to point to the Cymbals anymore.
				m_goArrowsToCymbals.SetActive(false);
				DeactivateTutorialButtonsHighlightEffects(CurrentTutorialPointID);
				for(int i = 0; i < m_arCymbals.Length; ++i)
				{
					m_arCymbals[i].enabled = true;
					m_arCymbals[i].ButtonCollider.enabled = false;
				}

				ActivateDefaultShowingFollowUpTextState();
				break;
			}	

			default: /*WAITING_FOR_CONTINUE_INPUT*/
			{
				for(int i = 0; i < m_arCymbals.Length; ++i)
				{
					m_arCymbals[i].enabled = true;
					m_arCymbals[i].ButtonCollider.enabled = false;
				}
				ActivateDefaultWaitingForContinueInputState();
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change RiqShake TutorialPoint State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ChangeRiqShakeTutorialPointState(TutorialState eState)
	{
		// Allow the user to keep playing all of the unlock Tambourine Sounds up to this point
		m_rRiqCenterRimButton.ActivateButton();
		m_rRiqMiddleRimButton.ActivateButton();
		m_rRiqOuterRimButton.ActivateButton();
		for(int i = 0; i < m_arCymbals.Length; ++i)
			m_arCymbals[i].ActivateButton();



		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:		{ ActivateDefaultShowingTutorialTextState();		break; }
			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:	{ ActivateDefaultWaitingForTutorialInputState();	TambourineShakeDetector.CheckForShake = true;					break; }
			case TutorialState.SHOWING_FOLLOW_UP_TEXT:		{ ActivateDefaultShowingFollowUpTextState();		m_rShakeDetector.ShakeSensitivity = m_fPriorShakeSensitivity;	break; }
			default:		 /*WAITING_FOR_CONTINUE_INPUT*/ { ActivateDefaultWaitingForContinueInputState();	break; }
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnEnable()
	{
		base.OnEnable();

		float newTambSize = 0.731f;
		m_bPriorTambTargetsVisible = m_rInstrumentManager.InstrumentColoursManager.Visible;
		m_ePriorInstrumentMode = (InstrumentManager.InstrumentMode)SavedPreferenceTool.GetInt("InstrumentMode");
		m_fPriorShakeSensitivity = SavedPreferenceTool.GetFloat("SensitivityPreference", 1.0f);
		m_fPriorInstrumentSize = SavedPreferenceTool.GetFloat("SizePreference", newTambSize);

		m_rInstrumentManager.CurrentInstrumentMode = InstrumentManager.InstrumentMode.RIQ_TAMBOURINE; 
		m_rShakeDetector.ShakeSensitivity = 1.0f;
		m_rTambourineZoomHandler.SetValue(newTambSize);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Disable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnDisable()
	{
		base.OnDisable();

		m_rInstrumentManager.CurrentInstrumentMode = m_ePriorInstrumentMode;
		m_rShakeDetector.ShakeSensitivity = m_fPriorShakeSensitivity;
		m_rTambourineZoomHandler.SetValue(m_fPriorInstrumentSize);

		if (!m_bPriorTambTargetsVisible)
			m_rInstrumentManager.InstrumentColoursManager.HideTargets();

		// Make Sure the Tambourine Cymbals are still able to be hit!
		for(int i = 0; i < m_arCymbals.Length; ++i)
		{
			m_arCymbals[i].enabled = true;
			m_arCymbals[i].ButtonCollider.enabled = true;
		}
	}
}
