//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Full-Game Plug Reveal
//             Author: Christopher Diamond
//             Date: November 06, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script fades in/out the Full-Game Plug Screen.
//		including making sure that other pages have been hidden before displaying.
//
//	  This script also handles input for the buttons located inside of 
//		the Plug.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class FullGamePlugReveal : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject	  m_goSettingsMenu;
	public SpriteRenderer m_sprPageBackground;
	public SpriteRenderer m_sprJokosInstrumentDisplay;
	public UnityEngine.UI.Text m_rFullGamePlugTextRenderer;
	public SpriteRenderer[] m_aSelectableButtons = new SpriteRenderer[2];

	public float m_fRevealAnimationSpeed = 1.0f;
	public float m_fDisappearAnimationSpeed = 2.0f;
	public MultiLanguageText m_oFullGamePlugTextDescription = new MultiLanguageText();
	public MultiLanguageText m_oURLConfirmationDescription = new MultiLanguageText();
	public VignetteManager.VignetteInfo m_oVignetteInfo = new VignetteManager.VignetteInfo();

	public AnimationEffect m_rSettingsMenuAE			= new AnimationEffect();
	public AnimationEffect m_rPageBackgroundAE			= new AnimationEffect();
	public AnimationEffect m_rJokosInstrumentDisplayAE	= new AnimationEffect();
	public AnimationEffect[] m_rButtonSpritesAE			= new AnimationEffect[2] { new AnimationEffect(), new AnimationEffect() };

	// Full Game Download Location Info
	public Button_OpenURL m_rOpenURLConfirmationWindow;
	public string[] m_asFullGameAppleURL		= new string[(int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT];
	public string[] m_asFullGameAndroidURL		= new string[(int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool m_bFadeinResults = true;

	private SubSceneManager m_rPreviousScene;
	private ObjectTransitionAnimation m_rPreviousNotePage;
	private TransitionPhase m_eTransitionPhase = TransitionPhase.IDLE;
	private ExitSelection m_eExitSelection = ExitSelection.EXIT;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static bool SupressPlugWindow { get; set; }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum TransitionPhase
	{
		SETTINGS_MENU				= 0,
		BACKGROUND_PAGE				= 1,
		JOKOS_INSTRUMENTS_DISPLAY	= 2,
		DESCRIPTION_TEXT			= 3,
		SELECTABLE_BUTTONS			= 4,

		IDLE,
	}

	public enum ExitSelection
	{
		EXIT,
		OPEN_URL_CONFIRMATION,
	}


	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		m_rSettingsMenuAE.Setup(m_goSettingsMenu);		
		m_rPageBackgroundAE.Setup(m_sprPageBackground.gameObject);
		m_rJokosInstrumentDisplayAE.Setup(m_sprJokosInstrumentDisplay.gameObject);
		m_rButtonSpritesAE[0].Setup(m_aSelectableButtons[0].gameObject);
		m_rButtonSpritesAE[1].Setup(m_aSelectableButtons[1].gameObject);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{	
		switch (m_eTransitionPhase)
		{
			case TransitionPhase.SETTINGS_MENU:				{ UpdateSettingsMenuTransition();	break; }
			case TransitionPhase.BACKGROUND_PAGE:			{ UpdateBackgroundPageTransition();	break; }
			case TransitionPhase.JOKOS_INSTRUMENTS_DISPLAY: { UpdateJokosInstrumentsDisplayTransition(); break; }
			case TransitionPhase.DESCRIPTION_TEXT:			{ UpdateDescriptionTextTransition();	break; }
			case TransitionPhase.SELECTABLE_BUTTONS:		{ UpdateResultsButtonsTransition(); break; }
			default:										{ break; }
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Settings Menu Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateSettingsMenuTransition()
	{
		// Wait For Settings Menu to close before making it disappear.
		if (!SettingsMenuManager.Opened)
		{
			if (m_bFadeinResults)
			{
				if (m_rSettingsMenuAE.UpdateAnimation(m_fRevealAnimationSpeed))
				{
					m_rSettingsMenuAE.Reset();
					m_eTransitionPhase = TransitionPhase.BACKGROUND_PAGE;
					m_sprPageBackground.gameObject.SetActive(true);

					m_goSettingsMenu.GetComponent<Button_ForceSceneObjectDisappear>().enabled = false; // There's nothing to disable/disappear, Buddy!
					m_goSettingsMenu.SetActive(false);
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
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Background Page Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateBackgroundPageTransition()
	{
		if (m_bFadeinResults)
		{
			if (m_rPageBackgroundAE.UpdateAnimation(m_fRevealAnimationSpeed))
			{
				m_rPageBackgroundAE.Reset();
				m_eTransitionPhase = TransitionPhase.JOKOS_INSTRUMENTS_DISPLAY;
				m_sprJokosInstrumentDisplay.gameObject.SetActive(true);
			}
		}
		else
		{
			if (m_rPageBackgroundAE.ReverseUpdate(m_fDisappearAnimationSpeed))
			{
				m_rPageBackgroundAE.Reset();
				m_eTransitionPhase = TransitionPhase.SETTINGS_MENU;
				m_goSettingsMenu.SetActive(true);
				m_sprPageBackground.gameObject.SetActive(false);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Joko's Instruments Display Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateJokosInstrumentsDisplayTransition()
	{
		if (m_bFadeinResults)
		{
			if (m_rJokosInstrumentDisplayAE.UpdateAnimation(m_fRevealAnimationSpeed))
			{
				m_rJokosInstrumentDisplayAE.Reset();
				m_eTransitionPhase = TransitionPhase.DESCRIPTION_TEXT;
			}
		}
		else
		{
			if (m_rJokosInstrumentDisplayAE.ReverseUpdate(m_fDisappearAnimationSpeed))
			{
				m_rJokosInstrumentDisplayAE.Reset();
				m_eTransitionPhase = TransitionPhase.BACKGROUND_PAGE;
				m_sprJokosInstrumentDisplay.gameObject.SetActive(false);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Description Text Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateDescriptionTextTransition()
	{
		if (m_bFadeinResults)
		{
			m_rFullGamePlugTextRenderer.gameObject.SetActive(true);
			m_oFullGamePlugTextDescription.ApplyEffects( m_rFullGamePlugTextRenderer );

			m_eTransitionPhase = TransitionPhase.SELECTABLE_BUTTONS;
			m_aSelectableButtons[0].gameObject.SetActive(true);
			m_aSelectableButtons[1].gameObject.SetActive(true);
		}
		else
		{
			m_rFullGamePlugTextRenderer.text = "";
			m_rFullGamePlugTextRenderer.gameObject.SetActive(false);
			m_eTransitionPhase = TransitionPhase.JOKOS_INSTRUMENTS_DISPLAY;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Selectable Buttons Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateResultsButtonsTransition()
	{
		if (m_bFadeinResults)
		{
			m_rButtonSpritesAE[1].UpdateAnimation(m_fRevealAnimationSpeed);
			if (m_rButtonSpritesAE[0].UpdateAnimation(m_fRevealAnimationSpeed))
			{
				m_rButtonSpritesAE[1].Reset();
				m_rButtonSpritesAE[0].Reset();
				OnFadein();
			}
		}
		else
		{
			m_rButtonSpritesAE[1].ReverseUpdate(m_fDisappearAnimationSpeed);
			if (m_rButtonSpritesAE[0].ReverseUpdate(m_fDisappearAnimationSpeed))
			{
				m_rButtonSpritesAE[1].Reset();
				m_rButtonSpritesAE[0].Reset();
				m_eTransitionPhase = TransitionPhase.DESCRIPTION_TEXT;
				m_aSelectableButtons[0].gameObject.SetActive(false);
				m_aSelectableButtons[1].gameObject.SetActive(false);
			}
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Begin Fadein
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginFadein(SubSceneManager previousScene)
	{
		BeginFadein(previousScene, null);
    }

	public void BeginFadein(ObjectTransitionAnimation previousNote)
	{
		BeginFadein(null, previousNote);
	}

	public void BeginFadein(SubSceneManager previousScene, ObjectTransitionAnimation previousNote)
	{
		m_rPreviousScene = previousScene;
		m_rPreviousNotePage = previousNote;
		if (!SupressPlugWindow)
		{
			// Show Vignette
			VignetteManager.TransitionVignette(m_oVignetteInfo);

			if (SettingsMenuManager.Opened)
				SettingsMenuManager.Close();

			// Toggle animation and disable user input for now
			ShowFirstAnimationFrames();
			m_eTransitionPhase = TransitionPhase.SETTINGS_MENU;
			m_bFadeinResults = true;
			ButtonManager.ToggleAllButtons(false);
		}
		else
		{
			if (previousNote != null)
				previousNote.Reveal(false);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Fadeout
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginFadeout(ExitSelection eSelection)
	{
		m_bFadeinResults = false; // Fadeout
		m_eTransitionPhase = TransitionPhase.SELECTABLE_BUTTONS;
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
		ButtonManager.ToggleAllButtons(true);

		if (m_eExitSelection == ExitSelection.EXIT)
		{
			// Do Something when "Maybe Later" (Do not purchase full game) is clicked?
			if(m_rPreviousScene != null)
			{
				if(m_rPreviousScene.IsSceneActive)
					m_rPreviousScene.ShowSceneVignette();
				else
					m_rPreviousScene.ShowSubscene();
			}
			else
			{
				VignetteManager.TransitionVignette(0.0f, 0.25f);
			}

			if (m_rPreviousNotePage != null)
				m_rPreviousNotePage.Reveal();
		}
		else
		{
			// Open URL Confirmation Window (Purchase Full Game)
			m_rOpenURLConfirmationWindow.OpenConfirmationWindow(Button_OpenURL.URLType.APPLE_OR_ANDROID, m_asFullGameAndroidURL[(int)GameManager.DisplayableLanguages], m_asFullGameAppleURL[(int)GameManager.DisplayableLanguages], m_oURLConfirmationDescription, m_rPreviousScene, m_rPreviousNotePage);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show First Animation Frames
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ShowFirstAnimationFrames()
	{
		m_rFullGamePlugTextRenderer.text = "";
		m_rPageBackgroundAE.ShowFirstFrame();
		m_rJokosInstrumentDisplayAE.ShowFirstFrame();
		m_rButtonSpritesAE[0].ShowFirstFrame();
		m_rButtonSpritesAE[1].ShowFirstFrame();			
	}
}
