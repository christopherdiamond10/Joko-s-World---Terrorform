//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Multiple Notes Manager
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: March 20, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used in conjunction with multiple notes, so as to add extra
//		functionality to the Notes scripts including the ability to support managing
//		more than one note.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class MultipleNotesManager : ObjectTransitionAnimation
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public MultipleNotesManager m_rPreviousNote;
	public MultipleNotesManager m_rNextNote;

	public AnimationEffect[] m_aeLeftRevealAnimation = new AnimationEffect[1];
	public AnimationEffect[] m_aeRightRevealAnimation = new AnimationEffect[1];
	public AnimationEffect[] m_aeLeftDisappearAnimation = new AnimationEffect[1];
	public AnimationEffect[] m_aeRightDisappearAnimation = new AnimationEffect[1];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private MultipleNotesManager m_rTransitioningNote;			// The Next note we will be showing after we disappear
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Att_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool HasPreviousNote { get { return m_rPreviousNote != null; } }
	public bool HasNextNote		{ get { return m_rNextNote != null; } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum TransitioningNote
	{
		PREVIOUS_NOTE,
		NEXT_NOTE,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void Awake()
	{
		m_aRevealAnimationEffect = m_aeRightRevealAnimation;

		foreach (AnimationEffect AE in m_aeLeftRevealAnimation) { AE.Setup(transform); }
		foreach (AnimationEffect AE in m_aeRightRevealAnimation) { AE.Setup(transform); }
		foreach (AnimationEffect AE in m_aeLeftDisappearAnimation) { AE.Setup(transform); }
		foreach (AnimationEffect AE in m_aeRightDisappearAnimation) { AE.Setup(transform); }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Disappear
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Disappear(TransitioningNote a_rTransitioningNote)
	{
		// If transitioning to prev note in the list, then set it up
		if (a_rTransitioningNote == TransitioningNote.PREVIOUS_NOTE)
		{
			m_rTransitioningNote = m_rPreviousNote;
			this.m_aDisappearAnimationEffect = m_aeRightDisappearAnimation;
			m_rTransitioningNote.m_aRevealAnimationEffect = m_rTransitioningNote.m_aeLeftRevealAnimation;
		}

		// Likewise if it's the next note in the list
		else
		{
			m_rTransitioningNote = m_rNextNote;
			this.m_aDisappearAnimationEffect = m_aeLeftDisappearAnimation;
			m_rTransitioningNote.m_aRevealAnimationEffect = m_rTransitioningNote.m_aeRightRevealAnimation;
		}


		ButtonManager.ToggleAllButtons(false);
		base.Disappear(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Revealed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnRevealed()
	{
		base.OnRevealed();
		m_rTransitioningNote = null;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Disappeared
	//----------------------------------------------------
	//	: Pretty much the exact same as the base version 
	//	  of this function. Except that the player input 
	//	  is only reactivated if there is no other note 
	//	  to display.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnDisappeared()
	{		
		sm_rCurrentlyActiveNotes = null;
		m_bOpened = false;

		this.gameObject.SetActive(false);
		if (m_rTransitioningNote != null)
		{
			m_rTransitioningNote.Reveal();
		}
		else
		{
			TambourineShakeDetector.CheckForShake = true;
			if (m_bShowSettingsMenuOnClose && SettingsMenuManager.Available)
			{
				SettingsMenuManager.Open();
			}
			else
			{
				ButtonManager.ToggleAllButtons(true);
			}
		}
	}
}
