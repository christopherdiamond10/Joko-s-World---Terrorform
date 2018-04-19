//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Tutorial Manager - Base
//             Author: Christopher Diamond
//             Date: December 11, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script deals with the Tutorial System. Enabling/Disabling buttons as
//		needed and revealing the different text components to match the current
//		point in the tutorial.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TutorialManager_Base : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public UnityEngine.UI.Text m_rTutorialTextComponent;
	public TextAnimationEffect m_rTextAnimationEffect;							// The AnimationEffectHandler for the Tutorial TextBox
	public ObjectTransitionAnimation m_rTextBoxTransitionEffect;				// Hides the TextBox when TutorialComplete
	public Collider2D m_rTextBoxCollider;                                       // This BoxCollider stops you from activating objects hidden under the TextBox. Storing it, because we need to disable/enable it depending on whether or not the follow-up text (waiting for users to actually click on the text box) is active. This BoxCollider would block that one if that's the case.
	public Button_ExitTutorial m_rTutorialExitButton;							// Button which exits out of the tutorial.
	public Button_ContinueTutorial m_rTutorialContinueButton;                   // Click this button to proceed from the 'Follow Up' Text.
	public SubSceneManager m_rOwningSubscene;									// The Subscene in which this Tutorial is explaining things for...
	public TutorialPhaseInfo[] m_arTutorialPhases = new TutorialPhaseInfo[1];   // All of the tutorial points to be used!
	public AudioSourceManager.AudioHandlerInfo m_oAudioHandlerInfo = new AudioSourceManager.AudioHandlerInfo(); // BGM to play during the Tutorial

	public float m_fTextFadeinTime = 0.3f;
	public string m_sPlayerPrefsID = "";										// Used to store 'completion' of this tutorial to external source; That way next time the tutorial does not start automatically since you already completed it.

#if UNITY_EDITOR
	public bool dm_bShowTutorialTextOptionsInInspector = false;
	public bool dm_bShowFollowUpTextOptionsInInspector = false;
	public int m_iSelectedTutorialPointID = 0;
#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*. Protected Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected TutorialState m_eTutorialState = TutorialState.SHOWING_TUTORIAL_TEXT;
	
	protected SpecialTutorialPointInfo m_rCurrentSpecialTutorialPoint;
	protected SpecialTutorialPointInfo[] m_aSpecialTutorialPoints;
	protected bool m_bFreezeTutorialUpdate = false;
    protected int m_iCurrentTutorialPointID = 0;
	protected int m_iNextTutorialPointID = -1;
	protected int m_iStartingTutorialPointID = 0;		// Which tutorial point are we going to start with? This will be changed depending on where we've accessed the tutorial from. Skipping over some tutorial pieces, etc.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static bool TutorialOpened { get; protected set; }

	public bool HasCompletedTutorial									{ get { return (m_sPlayerPrefsID != "" && SavedPreferenceTool.GetBool(m_sPlayerPrefsID, false)); } }
	protected bool IsSpecialTutorialPointID								{ get { return m_rCurrentSpecialTutorialPoint != null; } }
	protected TutorialPhaseInfo CurrentTutorialInfo						{ get { return m_arTutorialPhases[ CurrentTutorialPointID ]; } }
	protected VignetteManager.VignetteInfo CurrentVignetteInfo			{ get { return CurrentTutorialInfo.rVignetteInfo; } }

	protected virtual int CurrentTutorialPointID						{ get { return m_iCurrentTutorialPointID; } set { m_iCurrentTutorialPointID = value; } }
	protected virtual int NextTutorialPointID							{ get { return (m_iNextTutorialPointID > -1 ? m_iNextTutorialPointID : CurrentTutorialPointID + 1); } }
	protected virtual int EndOfTutorialID								{ get { return FinalTutorialPointID; } }		// The "Tutorial Opened" Boolean will be switched off when this TutorialPoint is Reached. Allowing other objects to act as though the tutorial is not overriding them. This is useful (for example) for activating subscenes'
	protected virtual int FinalTutorialPointID							{ get { return 1; } }							// Must be overriden to inform this base class when tutorial areas are finished!
	protected virtual MultiLanguageText CurrentTutorialText				{ get { return CurrentTutorialInfo.rTutorialTextDisplay; } }
	protected virtual MultiLanguageText CurrentFollowupText				{ get { return CurrentTutorialInfo.rFollowUpTextDisplay; } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected delegate void SpecialTutorialUpdateCallbackMethod();
	protected delegate void SpecialTutorialStateChangeCallbackMethod(TutorialState eState);

	[System.Serializable]
	public class TutorialPhaseInfo
	{
		public MultiLanguageText rTutorialTextDisplay		= new MultiLanguageText();
		public Button_TutorialPointOfInterest[] rButtons	= new Button_TutorialPointOfInterest[1];
		public MultiLanguageText rFollowUpTextDisplay		= new MultiLanguageText();
		public VignetteManager.VignetteInfo rVignetteInfo	= new VignetteManager.VignetteInfo();
    }

	protected class SpecialTutorialPointInfo
	{
		public int tutorialID;													// ID of Special Tutorial Point
		public SpecialTutorialStateChangeCallbackMethod onTutorialStateChange;	// Method to Call when Tutorial State is changed
		public SpecialTutorialUpdateCallbackMethod onTutorialPointUpdate;       // Method to Call when Updating the Tutorial during this Special Tutorial Point
	}

	protected enum TutorialState
	{
		SHOWING_TUTORIAL_TEXT,
		WAITING_FOR_TUTORIAL_INPUT,
		SHOWING_FOLLOW_UP_TEXT,
		WAITING_FOR_CONTINUE_INPUT,
	}
	


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Start()
	{
		ResetTutorialComponents();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Update() 
	{
		if(!m_bFreezeTutorialUpdate && !VignetteManager.IsTransitioning)
		{
			// Can't show tutorial without the textbox, can we?!
			if(m_rTextBoxTransitionEffect.IsCurrentlyActive)
			{
				// If we're a special tutorial section (not adhering to normal rules), you'll have your own 'Special' update method... Call It Instead!
				if(IsSpecialTutorialPointID)
				{
					m_rCurrentSpecialTutorialPoint.onTutorialPointUpdate();
				}
				else
				{
					UpdateNonSpecialTutorialPoint();
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Non-Special Tutorial Point
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void UpdateNonSpecialTutorialPoint()
	{
		if(m_eTutorialState == TutorialState.SHOWING_TUTORIAL_TEXT || m_eTutorialState == TutorialState.SHOWING_FOLLOW_UP_TEXT)
		{
			UpdateTextFade();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Get Special Tutorial Point?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private SpecialTutorialPointInfo GetSpecialTutorialPoint()
	{
		return GetSpecialTutorialPoint(CurrentTutorialPointID);
	}

	private SpecialTutorialPointInfo GetSpecialTutorialPoint(int tutorialPointID)
	{
		if (m_aSpecialTutorialPoints != null)
		{
			for (int i = 0; i < m_aSpecialTutorialPoints.Length; ++i)
				if (tutorialPointID == m_aSpecialTutorialPoints[i].tutorialID)
					return m_aSpecialTutorialPoints[i];
		}
		return null;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Initialise Special Tutorial Info
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void InitialiseSpecialutorialInfo()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void ResetTutorial()
	{
		MoveToSpecificTutorialPoint(m_iStartingTutorialPointID);
		ResetTutorialComponents();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset Tutorial Components
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void ResetTutorialComponents()
	{
		ChangeTutorialState(TutorialState.SHOWING_TUTORIAL_TEXT);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Text Fade
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateTextFade()
	{
		if(!m_rTextAnimationEffect.IsFadeingin)
			ChangeTutorialState( (TutorialState)((int)m_eTutorialState + 1) );
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Change Tutorial State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void ChangeTutorialState(TutorialState eState)
	{
		// Lock player input to only the items that should be allowed to be touched
		LockAllButtons();

		// Use Special Tutorial State Change if it's a special tutorial point!
		m_eTutorialState = eState;
		if (IsSpecialTutorialPointID)
		{
			m_rCurrentSpecialTutorialPoint.onTutorialStateChange(eState);
		}
		else
		{
			OnNormalTutorialPointStateChange(eState);
		}		
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: On Normal Tutorial Point State Change
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnNormalTutorialPointStateChange(TutorialState eState)
	{
		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:		{ ActivateDefaultShowingTutorialTextState();		break; }
			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:	{ ActivateDefaultWaitingForTutorialInputState();	break; }
			case TutorialState.SHOWING_FOLLOW_UP_TEXT:		{ ActivateDefaultShowingFollowUpTextState();		break; }
			default:		 /*WAITING_FOR_CONTINUE_INPUT*/ { ActivateDefaultWaitingForContinueInputState();	break; }
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Default Showing Tutorial Text State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void ActivateDefaultShowingTutorialTextState()
	{
		DisableTutorialButtons();

		m_rTextBoxCollider.enabled = true;
		m_rTutorialContinueButton.gameObject.SetActive(false);
		m_rTutorialContinueButton.m_rTutorialManager = this;

		// Apply Human-Language Specific text (including fontSize, position, etc) to the Text Component. Then use that info to store TutorialTextLines
		if(CurrentTutorialText.SelectedText != "")
		{
			CurrentTutorialText.ApplyEffects(m_rTutorialTextComponent);
			m_rTextAnimationEffect.SetText( m_rTutorialTextComponent.text );
			m_rTutorialTextComponent.text = "";

			// Also make the Vignette visible if available
			VignetteManager.TransitionVignette(CurrentVignetteInfo);
        }
		// Move on to next tutorial area if there is no text to display
		else
		{
			ChangeTutorialState(TutorialState.WAITING_FOR_TUTORIAL_INPUT);
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Default Waiting for Tutorial Input State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void ActivateDefaultWaitingForTutorialInputState()
	{
		// Is there actually a tutorial here? If yes, activate tutorial buttons; Otherwise move on to Next Tutorial area
		if((CurrentTutorialInfo.rButtons.Length > 0 && CurrentTutorialInfo.rButtons[0] != null) || IsSpecialTutorialPointID)
		{
			ActivateTutorialButtons();
			ActivateTutorialButtonsHighlightEffects(CurrentTutorialPointID);
			ActivateTutorialButtonsAnimationEffects(CurrentTutorialPointID);
		}
		else
		{
			// If no Tutorial Buttons Available. Activate Continue Button
			ActivateDefaultWaitingForContinueInputState();
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Default Showing Follow-Up Text State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void ActivateDefaultShowingFollowUpTextState()
	{
		DisableTutorialButtons();


		if(CurrentFollowupText.SelectedText != "")
		{
			CurrentFollowupText.ApplyEffects(m_rTutorialTextComponent);
			m_rTextAnimationEffect.SetText( m_rTutorialTextComponent.text );
			m_rTutorialTextComponent.text = "";
		}
		else
		{
			ProceedToNextTutorialPoint();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Default Waiting for Continue Input State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void ActivateDefaultWaitingForContinueInputState()
	{
		m_rTextBoxCollider.enabled = false;
		m_rTutorialContinueButton.gameObject.SetActive(true);
		m_rTutorialContinueButton.ActivateButton();
		m_rTutorialContinueButton.m_rTutorialManager = this;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: On Point of Interest Activate
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public virtual void OnPointOfInterestActivate(int iNextTutorialPoint = -1)
	{
		m_iNextTutorialPointID = iNextTutorialPoint;
		if (m_eTutorialState == TutorialState.WAITING_FOR_TUTORIAL_INPUT)
		{
			ContinueTutorial();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Continue Tutorial
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public virtual void ContinueTutorial()
	{
		// At end of TutorialStates? Move on to next Tutorial Point
		if(m_eTutorialState == TutorialState.WAITING_FOR_CONTINUE_INPUT)
		{
			ProceedToNextTutorialPoint();
        }

		// Otherwise Move on to Next State
		else
		{
			ChangeTutorialState( (TutorialState)((int)m_eTutorialState + 1) );
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Proceed To Next Tutorial Point
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void ProceedToNextTutorialPoint()
	{
		MoveToSpecificTutorialPoint( NextTutorialPointID );
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Move To Specific Tutorial Point
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void MoveToSpecificTutorialPoint(int a_iTutorialPointID)
	{
		DisableTutorialButtons();
		if(a_iTutorialPointID >= EndOfTutorialID)
		{
			TutorialOpened = false;
		}

		// Tutorial is finished?
		if(a_iTutorialPointID >= FinalTutorialPointID)
		{
			OnTutorialComplete();
		}
		else
		{
			// Switch Current TutorialPointID && Assign Whether or not this specific point is a 'SpecialTutorial'
			CurrentTutorialPointID = a_iTutorialPointID;
			m_iNextTutorialPointID = -1;
			m_rCurrentSpecialTutorialPoint = GetSpecialTutorialPoint(a_iTutorialPointID);
			ResetTutorialComponents();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: On Tutorial Complete
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public virtual void OnTutorialComplete()
	{
		ExitTutorial();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Tutorial
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginTutorial(int iStartingTutorialID = 0)
	{
		if(iStartingTutorialID > -1 && iStartingTutorialID < FinalTutorialPointID)
		{
			m_iStartingTutorialPointID = iStartingTutorialID;
			this.gameObject.SetActive(true);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Exit Tutorial
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void ExitTutorial()
	{
		TutorialOpened = false;
		StartCoroutine(FadeoutTutorial());
		SavedPreferenceTool.SaveBool(m_sPlayerPrefsID, true); // Completed Tutorial; save that so that it doesn't start automatically again next time
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Exit Tutorial Via Button
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public virtual void OnExitTutorialViaButton()
	{
		ExitTutorial();

		if(m_rOwningSubscene != null && m_rOwningSubscene.IsSceneActive)
		{
			m_rOwningSubscene.PlaySceneBGM();
			m_rOwningSubscene.ShowSceneVignette();
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Fadein Tutorial
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected IEnumerator FadeinTutorial()
	{
		// Fade in Tutorial BGM
		AudioSourceManager.PlayAudioClip(m_oAudioHandlerInfo);

		// Fade in Vignette First
		VignetteManager.TransitionVignette(CurrentVignetteInfo);
		while(VignetteManager.IsTransitioning)
			yield return new WaitForEndOfFrame();

		// Now You can Fadein/Show the Tutorial TextBox
		m_rTextBoxTransitionEffect.Reveal(false, false);
		m_rTextBoxTransitionEffect.LockNoteObject();
		while(!m_rTextBoxTransitionEffect.IsCurrentlyActive)
			yield return new WaitForEndOfFrame();

		ResetTutorial();
        OnTutorialFadeinComplete();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Fadeout Tutorial
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected IEnumerator FadeoutTutorial()
	{
		// Fade out Tutorial BGM
		AudioSourceManager.FadeoutAudio(m_oAudioHandlerInfo);

		// Disappear/Fadeout Tutorial TextBox
		m_rTextBoxTransitionEffect.UnlockNoteObject();
		m_rTextBoxTransitionEffect.Disappear(false);
		while (m_rTextBoxTransitionEffect.IsCurrentlyActive)
			yield return new WaitForEndOfFrame();

		// Fadeout Vignette. Only if nothing else is already Transitioning it~
		if(!VignetteManager.IsTransitioning)
		{
			if(VignetteManager.CurrentColour.a > 0.05f)
			{
				float transitionTime = 0.5f;
				VignetteManager.TransitionVignette(0.0f, transitionTime);
				while(VignetteManager.IsTransitioning)
					yield return new WaitForEndOfFrame();
			}
		}

		this.gameObject.SetActive(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: On Tutorial Fadein Complete
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnTutorialFadeinComplete()
	{
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Lock All Buttons
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void LockAllButtons()
	{
		ButtonManager.UnlockButtons();
		ButtonManager.ToggleAllButtons(false);
		ButtonManager.LockButtons();

		m_rTutorialExitButton.ActivateButton();
		m_rTutorialExitButton.m_rCurrentlyRunningTutorial = this;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Unlock All Buttons
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UnlockAllButtons()
	{
		ButtonManager.UnlockButtons();
		ButtonManager.ToggleAllButtons(true);
		TambourineShakeDetector.CheckForShake = true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Activate Tutorial Buttons
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ActivateTutorialButtons()
	{
		ActivateTutorialButtons(CurrentTutorialPointID);
	}

	protected void ActivateTutorialButtons(int tutorialPointID)
	{
		foreach (Button_TutorialPointOfInterest button in m_arTutorialPhases[tutorialPointID].rButtons)
		{
			if (button != null)
			{
				button.gameObject.SetActive(true);
				button.m_rTutorialManager = this;
				button.ActivateButton();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Disable Tutorial Buttons
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DisableTutorialButtons()
	{
		DisableTutorialButtons(CurrentTutorialPointID);
	}

	protected void DisableTutorialButtons(int tutorialPointID)
	{
		if (tutorialPointID < m_arTutorialPhases.Length && tutorialPointID > -1)
		{
			foreach (Button_TutorialPointOfInterest button in m_arTutorialPhases[tutorialPointID].rButtons)
			{
				if (button != null)
				{
					button.DeactivateButton();
					button.gameObject.SetActive(false);
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Tutorial Buttons Highlight Effects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ActivateTutorialButtonsHighlightEffects(int tutorialPointID)
	{
		foreach(Button_TutorialPointOfInterest button in m_arTutorialPhases[tutorialPointID].rButtons)
		{
			if(button != null)
			{
				HighlightEffect he = button.GetComponent<HighlightEffect>();
				if(he != null)
				{
					he.enabled = true;
					he.Reset();
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Deactivate Tutorial Buttons Highlight Effects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DeactivateTutorialButtonsHighlightEffects(int tutorialPointID)
	{
		foreach(Button_TutorialPointOfInterest button in m_arTutorialPhases[tutorialPointID].rButtons)
		{
			if(button != null)
			{
				HighlightEffect he = button.GetComponent<HighlightEffect>();
				if(he != null)
				{
					he.enabled = false;
					he.Reset();
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Tutorial Buttons Animation Effects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ActivateTutorialButtonsAnimationEffects(int tutorialPointID)
	{
		foreach(Button_TutorialPointOfInterest button in m_arTutorialPhases[tutorialPointID].rButtons)
		{
			if(button != null)
			{
				AnimationEffectDesigner ae = button.GetComponent<AnimationEffectDesigner>();
				if(ae != null)
				{
					ae.enabled = true;
					ae.ResetAnimation();
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Deactivate Tutorial Buttons Animation Effects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DeactivateTutorialButtonsAnimationEffects(int tutorialPointID)
	{
		foreach(Button_TutorialPointOfInterest button in m_arTutorialPhases[tutorialPointID].rButtons)
		{
			if(button != null)
			{
				AnimationEffectDesigner ae = button.GetComponent<AnimationEffectDesigner>();
				if(ae != null)
				{
					ae.enabled = false;
					ae.ResetAnimation();
				}
			}
		}
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnEnable()
	{
		if(m_aSpecialTutorialPoints == null)
			InitialiseSpecialutorialInfo();

		FullGamePlugReveal.SupressPlugWindow = true;
		StartCoroutine(FadeinTutorial());
		TutorialOpened = true;
		m_rTutorialContinueButton.m_rTutorialManager = this;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Disable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnDisable()
	{
		// Make sure tutorial starts from the beginning next time unless otherwise stated directly.
		m_iStartingTutorialPointID = 0;

		// Turn off Now Unused Buttons
		DisableTutorialButtons();

		// Reenable Player Input
		UnlockAllButtons();

		// You can now have annoying pop-ups telling you to pay too...!
		FullGamePlugReveal.SupressPlugWindow = false;

		TutorialOpened = false;
    }
}
