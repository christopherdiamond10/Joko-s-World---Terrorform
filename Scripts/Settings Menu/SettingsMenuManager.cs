//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Settings Menu manager
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: March 20, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used to reveal and hide the Settings Menu from the game.
//		For all intents and purposes, all things relating to the settings menu
//		should occur through this script. Such as opening/closing the menu.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class SettingsMenuManager : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*~ Static Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static SettingsMenuManager sm_rInstance;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Transform m_tAnimationTarget;
	public Transform m_tOutOfBoundsGameObject;
	public AnimationEffect m_aeClosedEffect;
	public AnimationEffect m_aeOpenEffect;

	public GameObject m_rgoJokosBagGlowEffect;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TransitionPhase m_eTransitionPhase = TransitionPhase.IDLE;
	private bool m_bIsOpen = false;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static bool Available	{ get { return (sm_rInstance != null) ? sm_rInstance.gameObject.activeInHierarchy : false; } }
	public static bool Opening		{ get { return (sm_rInstance != null) ? sm_rInstance.m_eTransitionPhase == TransitionPhase.OPEN : false; } }
	public static bool Opened		{ get { return (sm_rInstance != null) ? sm_rInstance.m_bIsOpen : false; } }
	public static GameObject Object { get { return (sm_rInstance != null) ? sm_rInstance.gameObject : null; } }
	public static Button_ForceSceneObjectDisappear ExitButton { get { return Object.GetComponent<Button_ForceSceneObjectDisappear>(); } }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum TransitionPhase
	{
		OPEN,
		CLOSE,
		IDLE,
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake()
	{
		sm_rInstance = this;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start() 
	{
		m_aeClosedEffect.Setup(m_tAnimationTarget);
		m_aeOpenEffect.Setup(m_tAnimationTarget);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update() 
	{
		switch (m_eTransitionPhase)
		{
			case TransitionPhase.OPEN:
			{
				if (m_aeOpenEffect.UpdateAnimation())
				{
					m_aeOpenEffect.Reset();
					OnOpen();
				}
				break;
			}

			case TransitionPhase.CLOSE:
			{
				if (m_aeClosedEffect.UpdateAnimation())
				{
					m_aeClosedEffect.Reset();
					OnClose();
				}
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Open
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void Open()
	{
		// Don't open if already opened... OBVIOUS!
		if (!Opened)
		{
			sm_rInstance.m_tAnimationTarget.gameObject.SetActive(true);
			sm_rInstance.m_eTransitionPhase = TransitionPhase.OPEN;

			if(sm_rInstance.m_rgoJokosBagGlowEffect != null)
				sm_rInstance.m_rgoJokosBagGlowEffect.SetActive(false);

			if(!TutorialManager_Base.TutorialOpened)
				VignetteManager.TransitionVignette(0.0f, 0.3f);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Close
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void Close()
	{
		// Only close if Opened!
		if (Opened)
		{
			sm_rInstance.m_eTransitionPhase = TransitionPhase.CLOSE;
			sm_rInstance.m_tOutOfBoundsGameObject.gameObject.SetActive(false);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Open
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnOpen()
	{
		m_bIsOpen = true;
		m_eTransitionPhase = TransitionPhase.IDLE;

		m_tOutOfBoundsGameObject.gameObject.SetActive(true);
		ButtonManager.ToggleAllButtonsExcept(ButtonManager.ButtonType.SETTINGS, false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Close
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnClose()
	{
		m_bIsOpen = false;
		ButtonManager.ToggleAllButtons(true);

		m_tAnimationTarget.gameObject.SetActive(false);
		m_eTransitionPhase = TransitionPhase.IDLE;


		if(m_rgoJokosBagGlowEffect != null)
			m_rgoJokosBagGlowEffect.SetActive(true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Update Menu Label
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void UpdateMenuLabel()
	{
		if (sm_rInstance != null)
		{
			Button_SettingsMenuToggle bsmt = sm_rInstance.GetComponent<Button_SettingsMenuToggle>();
			if (bsmt != null)
				bsmt.UpdateMenuLabel();
		}
	}
}
