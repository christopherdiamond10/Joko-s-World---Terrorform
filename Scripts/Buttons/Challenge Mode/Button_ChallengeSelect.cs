//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Challenge Select
//             Author: Christopher Diamond
//             Date: October 11, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This button selects one of the Challenges to be viewed and practiced
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_ChallengeSelect : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ChallengeGameManager m_rChallengeManager;							// Challenge Game Manager, telling this button whether or not the associated challenge is available.
	public ObjectTransitionAnimation m_rChallengeGameTransitioner;				// Open Up Challenge Scene, when button is clicked (and is not currently a locked challenge) 

	public SubSceneManager m_rChallengeSelectorSubsceneManager;					// Turns off the Vignette and BGM by telling the ChallengeMenu Subscene to Hide
	public ObjectTransitionAnimation m_rChallengeSelectorAnimator;				// In case of LOCKED challenge, pass in a reference to the current SceneObject to the FullGamePlug Dialogue Window: This will then open the scene back up to where it was if the user opted NOT to purchase the full app.
	public SoundsRhythmMemoryGame.Playlist m_eChallengeID = SoundsRhythmMemoryGame.Playlist.CHALLENGE_01;

	public Color m_cButtonUnavailableColour	= new Color32(111, 111, 111, 255);	// Colour of the Button When it can NOT be clicked on
	public Color m_cNormalTextColour		= new Color32(248, 189, 108, 255);	// Colour of the Button Text When Button Can be clicked on
	public Color m_cUnavailableTextColour	= new Color32(255, 255, 255, 128);  // Colour of the Button Text When Button Can NOT be clicked on

	public SpriteRenderer m_rRequiredFeathersIcon;
	public ChallengeAwardedFeathersNotifier[] m_arAwardedFeathersNotifier = new ChallengeAwardedFeathersNotifier[3];
	public LockSymbolDisplay m_rChallengeLockedSymbol;							// Reference to the "ChallengeLockedSymbol"
	public QuickDialoguePopupManager m_rNeedMoreFeathersPopupManager;			// Popup manager for the "NEED MORE FEATHERS" Dialogue

	public FullGamePlugReveal m_rFullGamePlugScreen;							// Dialog Window which says "BUY MY GAME"; when playing with the Lite Version. Will popup dialogue when trying to access an unavailable challenge
	public MultiLanguageText m_oUnavailableText = new MultiLanguageText();      // Display "UNAVAILABLE" in more than just the English Language

	public AudioClip m_acBuzzerSound;											// Buzzer Sound Effect To Play when clicking on an Unavailable Challenge Selection
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private ButtonAvailability m_eButtonAvailability  = ButtonAvailability.AVAILABLE;
	private Coroutine m_TextChangerCoroutine;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private float TextOpacity
	{
		set
		{
			Color colour = TextRenderer.color;
			colour.a = value;
			TextRenderer.color = colour;
		}
	}

	private float FeatherOpacity
	{
		set
		{
			Color colour = m_rRequiredFeathersIcon.color;
			colour.a = value;
			m_rRequiredFeathersIcon.color = colour;
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum ButtonAvailability
	{
		UNAVAILABLE,			// Behind Paywall!
		NEED_MORE_FEATHERS,
		AVAILABLE,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		switch(m_eButtonAvailability)
		{
			case ButtonAvailability.AVAILABLE:
				OnButtonAvailableClicked();
				break;

			case ButtonAvailability.NEED_MORE_FEATHERS:
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
				OnButtonAvailableClicked();
#else
				OnButtonNeedsMoreFeathersClicked();
#endif
				break;

			default: /*UNAVAILABLE*/
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
				if(GameManager.IsFullVersion)
					OnButtonAvailableClicked();
				else
					OnButtonUnavailableClicked();
#else
				OnButtonUnavailableClicked();
#endif
				break;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Play Button Sound
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void PlayButtonSound()
	{
		// If Challenge Not available... Play Buzzer ~
		if(m_acBuzzerSound != null)
		{
			if(m_eButtonAvailability != ButtonAvailability.AVAILABLE)
				AudioSourceManager.PlayAudioClip(m_acBuzzerSound);
			else
				base.PlayButtonSound();
		}
		else
		{
			base.PlayButtonSound();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Text Information
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private IEnumerator UpdateTextInformation()
	{
		const float fTextDisplayTimer = 3.0f;
		const float fTextFadeTime = 0.5f;
		TimeTracker ttTextFadeTimer = new TimeTracker(fTextFadeTime);
		bool bShowingChallengeName = true;
		bool bRequiresMoreFeathers = (m_eButtonAvailability == ButtonAvailability.NEED_MORE_FEATHERS);
		while(true)
		{
			// Show Text
			yield return new WaitForSeconds(fTextDisplayTimer);

			// Fade out Text
			ttTextFadeTimer.Reset();
            while(!ttTextFadeTimer.TimeUp())
			{
				if(ttTextFadeTimer.Update())
				{
					TextOpacity = 0.0f;
					if(!bShowingChallengeName && bRequiresMoreFeathers)
						FeatherOpacity = 0.0f;
				}
				else
				{
					TextOpacity = Mathf.Lerp(m_cUnavailableTextColour.a, 0.0f, ttTextFadeTimer.GetCompletionPercentage());
					if(!bShowingChallengeName && bRequiresMoreFeathers)
						FeatherOpacity = (1.0f - ttTextFadeTimer.GetCompletionPercentage());
				}
				yield return new WaitForEndOfFrame();
			}


			// Change Text
			bShowingChallengeName = !bShowingChallengeName;
			if(bShowingChallengeName)
			{
				if(bRequiresMoreFeathers)
					m_rRequiredFeathersIcon.gameObject.SetActive(false);
				MultiLanguageTextComponent.ApplyEffects(TextRenderer);
			}
			else
			{
				if(bRequiresMoreFeathers)
				{
					m_rRequiredFeathersIcon.gameObject.SetActive(true);
					int iFeathers = m_rChallengeManager.GetRequiredFeathersAmountForChallenge(m_eChallengeID);
                    TextRenderer.text = " x" + (iFeathers < 10 ? "0" : "") + iFeathers.ToString();
				}
				else
				{
					m_oUnavailableText.ApplyEffects(TextRenderer);
				}
            }
			TextOpacity = 0.0f;


			// Fade in Text
			ttTextFadeTimer.Reset();
			while(!ttTextFadeTimer.TimeUp())
			{
				if(ttTextFadeTimer.Update())
				{
					TextOpacity = m_cUnavailableTextColour.a;
					if(!bShowingChallengeName && bRequiresMoreFeathers)
						FeatherOpacity = 1.0f;
				}
				else
				{
					TextOpacity = Mathf.Lerp(0.0f, m_cUnavailableTextColour.a, ttTextFadeTimer.GetCompletionPercentage());
					if(!bShowingChallengeName && bRequiresMoreFeathers)
						FeatherOpacity = ttTextFadeTimer.GetCompletionPercentage();
				}
				yield return new WaitForEndOfFrame();
			}
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Button Available Clicked
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnButtonAvailableClicked()
	{
		m_rChallengeManager.CurrentChallengeID = (int)m_eChallengeID;
		m_rChallengeSelectorSubsceneManager.HideSubscene();
		m_rChallengeGameTransitioner.Reveal(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Button "Needs More Feathers" Clicked
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnButtonNeedsMoreFeathersClicked()
	{
		if(m_rNeedMoreFeathersPopupManager != null)
			m_rNeedMoreFeathersPopupManager.ShowQuickDialoguePopup(this.transform);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Button Unavailable Clicked
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnButtonUnavailableClicked()
	{
		m_rChallengeSelectorAnimator.Disappear(false);
		m_rFullGamePlugScreen.BeginFadein(m_rChallengeSelectorSubsceneManager, m_rChallengeSelectorAnimator);
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Button Available Enabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnButtonAvailableEnabled()
	{
		if(SprRenderer != null)
			SprRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		if(TextRenderer != null)
			TextRenderer.color = m_cNormalTextColour;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Button "Needs More Feathers" Enabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnButtonNeedsMoreFeathersEnabled()
	{
		if(SprRenderer != null)
			SprRenderer.color = m_cButtonUnavailableColour;

		if(TextRenderer != null)
		{
			TextRenderer.color = m_cUnavailableTextColour;
			m_TextChangerCoroutine = StartCoroutine(UpdateTextInformation());
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Button Unavailable Enabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnButtonUnavailableEnabled()
	{
		if(SprRenderer != null)
			SprRenderer.color = m_cButtonUnavailableColour;

		if(TextRenderer != null)
		{
			TextRenderer.color = m_cUnavailableTextColour;
			m_TextChangerCoroutine = StartCoroutine(UpdateTextInformation());
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Toggle Awarded Feathers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ToggleAwardedFeathers()
	{
		int completionCount = SavedPreferenceTool.GetInt(m_eChallengeID.ToString() + "_FeatherCount", 0);
		for(int i = 0; i < m_arAwardedFeathersNotifier.Length; ++i)
		{
			if(i < completionCount)
				m_arAwardedFeathersNotifier[i].ShowActiveFeather();
			else
				m_arAwardedFeathersNotifier[i].ShowInactiveFeather();
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Toggle Lock Symbol
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ToggleLockSymbol()
	{
		m_rChallengeLockedSymbol.gameObject.SetActive( m_eButtonAvailability == ButtonAvailability.UNAVAILABLE );
	}

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnEnable()
	{
		base.OnEnable();


		// Check To See if the button CAN be Pressed
		if(m_rChallengeManager.DoesChallengeRequireFullVersion(m_eChallengeID) && !GameManager.IsFullVersion)
		{
			m_eButtonAvailability = ButtonAvailability.UNAVAILABLE;
			OnButtonUnavailableEnabled();
		}
		else if(m_rChallengeManager.GetFeathersRemainingToUnlockChallenge(m_eChallengeID) > 0)
		{
			m_eButtonAvailability = ButtonAvailability.NEED_MORE_FEATHERS;
			OnButtonNeedsMoreFeathersEnabled();
		}
		else
		{
			m_eButtonAvailability = ButtonAvailability.AVAILABLE;
			OnButtonAvailableEnabled();
		}


		// If not a TUTORIAL: Disable Required Feather Icon(s), show already awarded feathers and lock symbol if necessary
		if(m_eChallengeID != SoundsRhythmMemoryGame.Playlist.TUTORIAL)
		{
			if(m_rRequiredFeathersIcon != null)
	            m_rRequiredFeathersIcon.gameObject.SetActive(false);

			ToggleAwardedFeathers();
			ToggleLockSymbol();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Disable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnDisable()
	{
		base.OnDisable();

		if(m_TextChangerCoroutine != null)
			StopCoroutine(m_TextChangerCoroutine);
    }
}
