//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Notes Reveal
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 27, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used to 'Reveal' a page of notes (texture).
//	  Such as the Cultural Notes and the Credits Page.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectTransitionAnimation : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AnimationEffect[] m_aRevealAnimationEffect = new AnimationEffect[1];
	public AnimationEffect[] m_aDisappearAnimationEffect = new AnimationEffect[1];

    public Button_ForceSceneObjectDisappear[] m_arExitButtons;
    public NotesMovement m_rNotesMovement;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*. Protected Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static ObjectTransitionAnimation sm_rCurrentlyActiveNotes = null;

	protected int m_iCurrentAnimationElement = 0;
	protected bool m_bWaitForSettingsMenu = false;
	protected bool m_bAssignToSettingsButton = false;
	protected bool m_bShowSettingsMenuOnClose = true;
	protected bool m_bOpened = false;
	protected CurrentState m_eCurrentState = CurrentState.REVEAL;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static ObjectTransitionAnimation CurrentlyActive { get { return sm_rCurrentlyActiveNotes; } }

	public bool IsLocked			{ get; private set; }
	public bool IsCurrentlyActive	{ get { return m_bOpened; } }
	public float DisappearTime		{ get { float t = 0.0f; foreach (var itm in m_aDisappearAnimationEffect) { t += itm.m_fTotalAnimationTime; } return t; } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	** Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected enum CurrentState
	{
		REVEAL,
		REVERSE_REVEAL,
		IDLE,
		DISAPPEARING,
		REVERSE_DISAPPEARING,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void Start()
	{
		for (int i = 0; i < m_aRevealAnimationEffect.Length; ++i)
		{
            m_aRevealAnimationEffect[i].Setup(this.transform);
		}
		for (int j = 0; j < m_aDisappearAnimationEffect.Length; ++j)
		{
            m_aDisappearAnimationEffect[j].Setup(this.transform);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void Update()
	{
		if (m_bWaitForSettingsMenu)
		{
			if (SettingsMenuManager.Opened)// || (sm_rCurrentlyActiveNotes != null && sm_rCurrentlyActiveNotes != this))
				return;
		}
		//if (!m_bWaitForSettingsMenu || !SettingsMenuManager.Opened)
		{
			switch (m_eCurrentState)
			{
				case CurrentState.REVEAL:
				{
					UpdateRevealAnimation();
					break;
				}

				case CurrentState.REVERSE_REVEAL:
				{
					UpdateReverseRevealAnimation();
					break;
				}

				case CurrentState.DISAPPEARING:
				{
					UpdateDisappearAnimation();
					break;
				}

				case CurrentState.REVERSE_DISAPPEARING:
				{
					UpdateReverseDisappearAnimation();
					break;
				}

				default:
				{
					break;
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Reveal Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateRevealAnimation()
	{
		// Wait for the Settings Menu to Close before Revealing yourself
		if (m_aRevealAnimationEffect[m_iCurrentAnimationElement].UpdateAnimation())
		{
			m_iCurrentAnimationElement += 1;
			if (m_iCurrentAnimationElement == m_aRevealAnimationEffect.Length)
			{
				OnRevealed();
			}
			else
			{
				m_aRevealAnimationEffect[m_iCurrentAnimationElement].Reset();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Reverse Reveal Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateReverseRevealAnimation()
	{
		if (m_aRevealAnimationEffect[m_iCurrentAnimationElement].ReverseUpdate())
		{
			m_iCurrentAnimationElement -= 1;
			if (m_iCurrentAnimationElement == -1)
			{
				OnDisappeared();
			}
			else
			{
				m_aRevealAnimationEffect[m_iCurrentAnimationElement].Reset();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Disappear Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateDisappearAnimation()
	{
		if (m_aDisappearAnimationEffect[m_iCurrentAnimationElement].UpdateAnimation())
		{
			m_iCurrentAnimationElement += 1;
			if (m_iCurrentAnimationElement == m_aDisappearAnimationEffect.Length)
			{
				OnDisappeared();
			}
			else
			{
				m_aDisappearAnimationEffect[m_iCurrentAnimationElement].Reset();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Reverse Disappear Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateReverseDisappearAnimation()
	{
		if (m_aDisappearAnimationEffect[m_iCurrentAnimationElement].ReverseUpdate())
		{
			m_iCurrentAnimationElement -= 1;
			if (m_iCurrentAnimationElement == -1)
			{
				OnRevealed();
			}
			else
			{
				m_aDisappearAnimationEffect[m_iCurrentAnimationElement].Reset();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reveal
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	/// <summary>
	/// Forces the object to appear using a predefined animation.
	/// </summary>
	/// <param name="a_bCloseSettingsMenuFirst">Close Settings Menu (if opened) before proceeding?</param>
	/// <param name="a_bAssignSelfToSettings">Assign Self To Settings Menu? Doing so will cause the settings button to call this script's 'Disappear' Method when pressed.</param> 
	/// <param name="a_bReverseDisappearAnimation">Play the Disappear Animation in reverse?</param>
	public virtual void Reveal(bool a_bCloseSettingsMenuFirst = true, bool a_bAssignSelfToSettings = true, bool a_bReverseDisappearAnimation = false)
	{
		if (IsLocked)
		{
			return;
		}



		// Close the Settings Menu if Opened
		m_bAssignToSettingsButton = a_bAssignSelfToSettings;
		m_bWaitForSettingsMenu = a_bCloseSettingsMenuFirst;
		if (m_bWaitForSettingsMenu && SettingsMenuManager.Opened)
		{
			SettingsMenuManager.Close();
		}
		if (m_bAssignToSettingsButton)
		{
			// Assign self to SubScene if active
			if(GameManager.CurrentSubscene != null)
				GameManager.CurrentSubscene.SetCurrentSceneObject(this);

			if(sm_rCurrentlyActiveNotes != null)
				sm_rCurrentlyActiveNotes.Disappear(false);
		}


		// Setup Reveal variables and make this gameobject active in the Scene
		if (a_bReverseDisappearAnimation)
		{
			m_iCurrentAnimationElement = m_aDisappearAnimationEffect.Length - 1;
			m_eCurrentState = CurrentState.REVERSE_DISAPPEARING;
			m_aDisappearAnimationEffect[m_iCurrentAnimationElement].Reset();
		}
		else
		{
			m_iCurrentAnimationElement = 0;
			m_eCurrentState = CurrentState.REVEAL;
			m_aRevealAnimationEffect[m_iCurrentAnimationElement].Reset();
		}
		this.enabled = true;
		this.gameObject.SetActive(true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Disappear
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	/// <summary>
	/// Forces the object to disappear using a predefined animation.
	/// </summary>
	/// <param name="a_bShowSettingsMenuWhenClosed">Show the Settings Menu when finished aniamting close?</param>
	/// <param name="a_bReverseRevealAnimation">Play the Reveal Animation in reverse?</param>
	/// <param name="a_bFromCurrentTransform">Start Animation from the current position and Scale rather than the predefined position and Scales?</param>
	public virtual void Disappear(bool a_bShowSettingsMenuWhenClosed = true, bool a_bReverseRevealAnimation = false, bool a_bFromCurrentTransform = true)
	{
		if (IsLocked)
		{
			return;
		}



		m_bShowSettingsMenuOnClose = a_bShowSettingsMenuWhenClosed;
		m_bWaitForSettingsMenu = false;

		if (a_bReverseRevealAnimation)
		{
			m_iCurrentAnimationElement = m_aRevealAnimationEffect.Length - 1;
			m_eCurrentState = CurrentState.REVERSE_REVEAL;

			if (a_bFromCurrentTransform)
			{
				m_aRevealAnimationEffect[m_iCurrentAnimationElement].m_vStartingPosition = transform.localPosition;
				m_aRevealAnimationEffect[m_iCurrentAnimationElement].m_vStartingScale = transform.localScale;
			}
			m_aRevealAnimationEffect[m_iCurrentAnimationElement].Reset();
		}
		else
		{
			m_iCurrentAnimationElement = 0;
			m_eCurrentState = CurrentState.DISAPPEARING; 

			if (a_bFromCurrentTransform)
			{
				m_aDisappearAnimationEffect[m_iCurrentAnimationElement].m_vStartingPosition = transform.localPosition;
				m_aDisappearAnimationEffect[m_iCurrentAnimationElement].m_vStartingScale = transform.localScale;
			}
			m_aDisappearAnimationEffect[m_iCurrentAnimationElement].Reset();
		}

		


		// Stop User from being able to Move the Note(s) around
		if (m_rNotesMovement != null)
		{
			m_rNotesMovement.enabled = false;
		}



		for(int i = 0; i < m_arExitButtons.Length; ++i)
		{
			if(m_arExitButtons[i] != null)
			{
				// If the exit button isn't the Settings Menu, disable it from scene (it lives with the note in Notes Heaven now)
				if ((!a_bShowSettingsMenuWhenClosed || !SettingsMenuManager.Available) && m_arExitButtons[i].m_bHideUntilOpened == true)
				{
					if (m_arExitButtons[i].gameObject != SettingsMenuManager.Object)
					{
						m_arExitButtons[i].gameObject.SetActive(false);
					}
				}

				// Otherwise if it WAS in fact the Settings Menu button which was acting as the exit button. Just disable the exit button functionality.
				//	We Still need the Settings Menu to toggle itself... ^.~
				else
				{
					m_arExitButtons[i].enabled = false;
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Revealed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnRevealed()
	{
		// Once revealed, this note is active
		m_bOpened = true;
		if (m_bAssignToSettingsButton)
			sm_rCurrentlyActiveNotes = this;

		// Enable the Notes Movement Script and Exit Button if they exist
		m_eCurrentState = CurrentState.IDLE;
		if (m_rNotesMovement != null)
		{
			// Stop Player Input for anything besides the menu
			ButtonManager.ToggleAllButtonsExcept(ButtonManager.ButtonType.MENU, false);			
			m_rNotesMovement.enabled = true;
		}

		// Assign Self to the exit buttons. If clicked, the exit button instance will call the 'disappear' function.
		if (!TutorialManager_Base.TutorialOpened)
		{
			for (int i = 0; i < m_arExitButtons.Length; ++i)
			{
				if (m_arExitButtons[i] != null)
				{
					m_arExitButtons[i].gameObject.SetActive(true);
					m_arExitButtons[i].enabled = true;
					m_arExitButtons[i].m_rNotesReveal = this;
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Disappeared
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnDisappeared()
	{
		// Once Disappeared, this note is no longer active
		m_bOpened = false;
		if (m_bAssignToSettingsButton && sm_rCurrentlyActiveNotes == this)
			sm_rCurrentlyActiveNotes = null;

		this.enabled = false;
		this.gameObject.SetActive(false);

		// Reactivate Player Input
		ButtonManager.ToggleAllButtons(true);
		TambourineShakeDetector.CheckForShake = true;

		// If we came from the Settings Menu, make it open up once again
		if (m_bShowSettingsMenuOnClose && SettingsMenuManager.Available)
		{
			SettingsMenuManager.Open();
		}
		SettingsMenuManager.UpdateMenuLabel();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Disabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void OnDisable()
	{
		if (sm_rCurrentlyActiveNotes == this)
			sm_rCurrentlyActiveNotes = null;

		SettingsMenuManager.UpdateMenuLabel();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Lock Object
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void LockNoteObject()
	{
		IsLocked = true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Unlock Object
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void UnlockNoteObject()
	{
		IsLocked = false;
	}
}
