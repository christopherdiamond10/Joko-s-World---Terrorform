//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Title Screen Animation
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 15, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is manages the entire title screen introduction sequence.
//		Including wait times in between sequences and the consistent updates 
//		for those individual sequences.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;


public class TitleScreenAnimation : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Sprite[] m_arSpritesToPreload;
	public SpriteRenderer m_srCulturalInfusionLogo;
	public SpriteRenderer m_srBackgroundSpriteRenderer;
	public GameObject m_goJokosWorld;
	public GameObject m_goGameTitle;
	public GameObject m_goJokosDance;
	public GameObject m_goPlayButton;
	public GameObject m_goCreditsButton;
	public GameObject[] m_agoGameActivationObjectsToEnable;

	public float m_fLandscapeBlurDistance		= 0.001f;	// How Much Blur are we aiming for


	public float m_fLandscapeBlurTime			= 1.0f;
	public float m_fLandscapeViewWait			= 2.0f;
	public float m_fLandscapeViewAfterMoveWait	= 1.5f;
	public float m_fJokosWorldWaitTime			= 0.0f;
	public float m_fJokosFluteWaitTime			= 0.0f;
	public float m_fJokosDanceWaitTime			= 0.0f;
	public float m_fPlayButtonWaitTime			= 0.0f;

	public AnimationEffect[] m_aLandscapeAnimation	= new AnimationEffect[1];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TimeTracker m_ttLandscapeViewWaitTimer;
	private TimeTracker m_ttLanscapeBlurTimer;
	private TimeTracker m_ttLandscapeAfterMoveWaitTimer;
	private TimeTracker m_ttJokosWorldWaitTimer;
	private TimeTracker m_ttJokosFluteWaitTimer;
	private TimeTracker m_ttJokosDanceWaitTimer;
	private TimeTracker m_ttPlayButtonWaitTimer;

	private int			m_iCurrentElement = 0;
	private SpriteRenderer[] m_arPreloadedSpriteRenderers;

	private AnimationPhase m_eAnimationPhase = AnimationPhase.BEGIN;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	** Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum AnimationPhase
	{
		BEGIN,
		LOAD_PRELOADED_SPRITES,
		REMOVE_PRELOADED_SPRITES,
		//SPLASH_SCREEN_REVEAL,
		//SPLASH_SCREEN_WAIT,
		SPLASH_SCREEN_FADE,
		LANDSCAPE_WAIT,
		LANDSCAPE_MOVE,
		LANDSCAPE_BLUR,
		LANDSCAPE_AFTERMOVE_WAIT,
		JOKOS_WORLD_INTRO,
		JOKOS_WORLD_INTRO_WAIT,
		GAME_TITLE_INTRO,
		GAME_TITLE_INTRO_WAIT,
		JOKOS_DANCE_INTRO,
		JOKOS_DANCE_INTRO_WAIT,
		PLAYBUTTON_INTRO,
		PLAYBUTTON_INTRO_WAIT,

		GAME_ACTIVATION,
		
		IDLE,
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		// Setup Everything necessary and deactivate player input
		m_eAnimationPhase = AnimationPhase.LOAD_PRELOADED_SPRITES;
		SetupTimeTrackers();
		SetupAnimations();
		ButtonManager.ToggleAllButtonsExcept(ButtonManager.ButtonType.SETTINGS, false);

		// Deactivate everything associated with the title screen intro except the first thing to be displayed... The Cultural Infusion Logo. We shall reactivate everything one by one as they are needed
		m_srCulturalInfusionLogo.gameObject.SetActive(true);
		m_srBackgroundSpriteRenderer.gameObject.SetActive(false);
		m_goJokosWorld.SetActive(false);
		m_goGameTitle.SetActive(false);
		m_goJokosDance.SetActive(false);
		m_goPlayButton.SetActive(false);
		m_goCreditsButton.SetActive(false);

		GetComponent<AudioSource>().Play();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup TimeTrackers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetupTimeTrackers()
	{
		if (m_fLandscapeViewWait > 0.0f)			{ m_ttLandscapeViewWaitTimer		= new TimeTracker(m_fLandscapeViewWait);			}
		if (m_fLandscapeBlurTime > 0.0f)			{ m_ttLanscapeBlurTimer				= new TimeTracker(m_fLandscapeBlurTime);			}
		if (m_fLandscapeViewAfterMoveWait > 0.0f)	{ m_ttLandscapeAfterMoveWaitTimer	= new TimeTracker(m_fLandscapeViewAfterMoveWait);	}
		if (m_fJokosWorldWaitTime > 0.0f)			{ m_ttJokosWorldWaitTimer			= new TimeTracker(m_fJokosWorldWaitTime);			}
		if (m_fJokosFluteWaitTime > 0.0f)			{ m_ttJokosFluteWaitTimer			= new TimeTracker(m_fJokosFluteWaitTime);			}
		if (m_fJokosDanceWaitTime > 0.0f)			{ m_ttJokosDanceWaitTimer			= new TimeTracker(m_fJokosDanceWaitTime);			}
		if (m_fPlayButtonWaitTime > 0.0f)			{ m_ttPlayButtonWaitTimer			= new TimeTracker(m_fPlayButtonWaitTime);			}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Animations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetupAnimations()
	{
		for (int i = 0; i < m_aLandscapeAnimation.Length; ++i)	{ m_aLandscapeAnimation[i].Setup(m_srBackgroundSpriteRenderer.transform); }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
#if UNITY_EDITOR // If I'm impatient, I shall just skip everything
		if (m_eAnimationPhase <= AnimationPhase.GAME_ACTIVATION && Input.GetKeyDown(KeyCode.Space))
		{
			ForceFinish();
		}
#endif

		switch (m_eAnimationPhase)
		{
			case AnimationPhase.LOAD_PRELOADED_SPRITES:     { UpdateLoadPreloadedSprites();		break; }
			case AnimationPhase.REMOVE_PRELOADED_SPRITES:	{ UpdateRemovePreloadedSprites();	break; }
			case AnimationPhase.LANDSCAPE_WAIT:				{ UpdateLandscapeWait();			break; }
			case AnimationPhase.LANDSCAPE_MOVE:				{ UpdateLandscapeMove();			break; }
			case AnimationPhase.LANDSCAPE_BLUR:				{ UpdateBackgroundBlur();			break; }
			case AnimationPhase.LANDSCAPE_AFTERMOVE_WAIT:	{ UpdateLandscapeAfterMoveWait();		break; }
			case AnimationPhase.JOKOS_WORLD_INTRO_WAIT:		{ UpdateJokosWorldLogoWait();			break; }
			case AnimationPhase.GAME_TITLE_INTRO_WAIT:		{ UpdateJokosInstrumentNameLogoWait();	break; }
			case AnimationPhase.JOKOS_DANCE_INTRO_WAIT:		{ UpdateJokosDanceWait();			break; }
			case AnimationPhase.PLAYBUTTON_INTRO_WAIT:		{ UpdatePlayButtonWait();			break; }
			case AnimationPhase.GAME_ACTIVATION:			{ UpdateGameActivationWait();		break; }
			default:										{									break; }
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Force Finish
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ForceFinish()
	{
		// Activate and do everything for all TitleScreen Animation Phase remaining
		while(m_eAnimationPhase <= AnimationPhase.GAME_ACTIVATION)
		{
			ProceedToNextPhase();
		}

		// Force all animations and their individual frames to finish
		foreach (AnimationEffect Effect in m_aLandscapeAnimation)
		{
			Effect.ForceFinish();
		}
		m_srBackgroundSpriteRenderer.material.SetFloat("_Distance", m_fLandscapeBlurDistance);
		UpdateGameActivationWait();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Load Preloaded Sprites
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateLoadPreloadedSprites()
	{
		if(m_arSpritesToPreload != null && m_arSpritesToPreload.Length > 0)
		{
			m_arPreloadedSpriteRenderers = new SpriteRenderer[m_arSpritesToPreload.Length];
			for(int i = 0; i < m_arSpritesToPreload.Length; ++i)
			{
				if(m_arSpritesToPreload[i] != null)
				{
					GameObject temp = new GameObject("Temp ~ " + ((i + 1) < 10 ? "0" : "") + (i + 1).ToString());
					temp.transform.SetParent(this.transform, false);
					temp.transform.localPosition = Vector3.zero;
					temp.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
					m_arPreloadedSpriteRenderers[i] = temp.AddComponent<SpriteRenderer>();
					m_arPreloadedSpriteRenderers[i].sprite = m_arSpritesToPreload[i];
				}
			}
		}
		ProceedToNextPhase();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Remove Preloaded Sprites
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateRemovePreloadedSprites()
	{
		if(m_arPreloadedSpriteRenderers != null)
		{
			for(int i = 0; i < m_arPreloadedSpriteRenderers.Length; ++i)
			{
				if(m_arPreloadedSpriteRenderers[i] != null)
					Destroy(m_arPreloadedSpriteRenderers[i].gameObject);
			}
		}
		ProceedToNextPhase();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Landscape Wait
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateLandscapeWait()
	{
		if (m_ttLandscapeViewWaitTimer != null)
		{
			if (m_ttLandscapeViewWaitTimer.Update())
			{
				m_ttLandscapeViewWaitTimer.Reset();
				ProceedToNextPhase();
			}
		}
		else
		{
			ProceedToNextPhase();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Landscape Move
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateLandscapeMove()
	{
		if (m_aLandscapeAnimation[m_iCurrentElement].UpdateAnimation())
		{
			m_iCurrentElement += 1;
			if (!(m_iCurrentElement < m_aLandscapeAnimation.Length))
			{
				m_iCurrentElement = 0;
				ProceedToNextPhase();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Background Blur
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateBackgroundBlur()
	{
		if (m_ttLanscapeBlurTimer.Update())
		{
			m_ttLanscapeBlurTimer.Reset();
			m_srBackgroundSpriteRenderer.material.SetFloat("_Distance", m_fLandscapeBlurDistance);
			ProceedToNextPhase();
		}
		else
		{
			m_srBackgroundSpriteRenderer.material.SetFloat( "_Distance", Mathf.Lerp(0.0f, m_fLandscapeBlurDistance, m_ttLanscapeBlurTimer.GetCompletionPercentage()) );
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Landscape After Move Wait
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateLandscapeAfterMoveWait()
	{
		if (m_ttLandscapeAfterMoveWaitTimer != null)
		{
			if (m_ttLandscapeAfterMoveWaitTimer.Update())
			{
				m_ttLandscapeAfterMoveWaitTimer.Reset();
				ProceedToNextPhase();
			}
		}
		else
		{
			ProceedToNextPhase();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Jokos World Logo Wait
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateJokosWorldLogoWait()
	{
		if (m_ttJokosWorldWaitTimer != null)
		{
			if (m_ttJokosWorldWaitTimer.Update())
			{
				m_ttJokosWorldWaitTimer.Reset();
				ProceedToNextPhase();
			}
		}
		else
		{
			ProceedToNextPhase();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Jokos Instrument Name Logo Wait
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateJokosInstrumentNameLogoWait()
	{
		if (m_ttJokosFluteWaitTimer != null)
		{
			if (m_ttJokosFluteWaitTimer.Update())
			{
				m_ttJokosFluteWaitTimer.Reset();
				ProceedToNextPhase();
			}
		}
		else
		{
			ProceedToNextPhase();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Jokos Dance Wait
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateJokosDanceWait()
	{
		if (m_ttJokosDanceWaitTimer != null)
		{
			if (m_ttJokosDanceWaitTimer.Update())
			{
				m_ttJokosDanceWaitTimer.Reset();
				ProceedToNextPhase();
			}
		}
		else
		{
			ProceedToNextPhase();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Play Button Wait
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdatePlayButtonWait()
	{
		if (m_ttPlayButtonWaitTimer != null)
		{
			if (m_ttPlayButtonWaitTimer.Update())
			{
				m_ttPlayButtonWaitTimer.Reset();
				ProceedToNextPhase();
			}
		}
		else
		{
			ProceedToNextPhase();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Game Activation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateGameActivationWait()
	{
		m_eAnimationPhase = AnimationPhase.IDLE;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Game
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ActivateGame()
	{
		SavedPreferenceTool.SaveInt("SeenTitleIntroSequence", 1);
		ButtonManager.ToggleAllButtons(true);

		if (m_agoGameActivationObjectsToEnable != null && m_agoGameActivationObjectsToEnable.Length > 0)
		{
			foreach (GameObject Obj in m_agoGameActivationObjectsToEnable)
			{
				if(Obj != null)
					Obj.SetActive(true);
			}
		}
		ProceedToNextPhase();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Proceed To Next Phase
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ProceedToNextPhase()
	{
		m_eAnimationPhase += 1;
		switch (m_eAnimationPhase)
		{
			case AnimationPhase.LANDSCAPE_WAIT:				{ m_srBackgroundSpriteRenderer.gameObject.SetActive(true); m_srCulturalInfusionLogo.gameObject.SetActive(false); m_aLandscapeAnimation[0].SetPosition(0.0f); break; }
			case AnimationPhase.JOKOS_WORLD_INTRO:			{ m_goJokosWorld.SetActive(true);		break; }
			case AnimationPhase.GAME_TITLE_INTRO:			{ m_goGameTitle.SetActive(true);		break; }
			case AnimationPhase.JOKOS_DANCE_INTRO:			{ m_goJokosDance.SetActive(true);		ProceedToNextPhase();	break; }
			case AnimationPhase.PLAYBUTTON_INTRO:			{ m_goCreditsButton.SetActive(true); m_goPlayButton.SetActive(true); break; }
			case AnimationPhase.GAME_ACTIVATION:			{ ActivateGame();						break; }
			default:										{										break; }
		}
	}
}
