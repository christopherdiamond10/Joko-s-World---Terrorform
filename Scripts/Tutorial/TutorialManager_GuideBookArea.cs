//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Tutorial Manager - Guide Book Area
//             Author: Christopher Diamond
//             Date: January 01, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    Deriving from the TutorialManager_Base class, this specific tutorial is 
//		made and intended for use by the Guide Book Area. This includes
//		handling situations that users will find themselves in such as 
//		Moving the pages and reizing.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TutorialManager_GuideBookArea : TutorialManager_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public MultipleNotesSheet m_rCulturalNotesPage1;
	public MultipleNotesSheet m_rCulturalNotesPage2;
	public GameObject m_goCulturalNotesNextNoteArrowButton; // The Arrow in the Cultural Notes which proceeds to the next Cultural Note

	public SubSceneManager m_rGuideBookScene;
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
		OPEN_GUIDE_BOOK,

		CULTURAL_NOTES_EXPLANATION,
		CULTURAL_NOTES_MOVE_PAGE,
		CULTURAL_NOTES_NEXT_PAGE,
		CULTURAL_NOTES_RESIZE_PAGE,
		CLOSE_GUIDE_BOOK,

		TUTORIAL_COMPLETE_TEXT,

		PHASES_COUNT, // <=== Leave last!
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Initialise Special Tutorial Info
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void InitialiseSpecialutorialInfo()
	{
		m_aSpecialTutorialPoints = new SpecialTutorialPointInfo[]
		{
			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.CULTURAL_NOTES_MOVE_PAGE,
											 onTutorialStateChange  = ChangeCulturalNotesMovePageTutorialPointState,
											 onTutorialPointUpdate  = UpdateCulturalNotesMovePageTutorialPoint			},

			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.CULTURAL_NOTES_NEXT_PAGE,
											 onTutorialStateChange  = ChangeCulturalNotesProceedToNextPageTutorialPointState,
											 onTutorialPointUpdate  = UpdateNonSpecialTutorialPoint },

			new SpecialTutorialPointInfo() { tutorialID             = (int)TutorialPhases.CULTURAL_NOTES_RESIZE_PAGE,
											 onTutorialStateChange  = ChangeCulturalNotesResizePageTutorialPointState,
											 onTutorialPointUpdate  = UpdateCulturalNotesResizePageTutorialPoint		},
		};
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update CulturalNotes MovePage Tutorial Point
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateCulturalNotesMovePageTutorialPoint()
	{
		switch (m_eTutorialState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT: case TutorialState.SHOWING_FOLLOW_UP_TEXT:
				UpdateTextFade();
				break;

			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:
				if (m_rCulturalNotesPage1.WasMoved)
					ContinueTutorial();
				break;

			default: // TutorialState.WAITING_FOR_CONTINUE_INPUT 
				break;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update CulturalNotes ResizePage Tutorial Point
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateCulturalNotesResizePageTutorialPoint()
	{
		switch (m_eTutorialState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT: case TutorialState.SHOWING_FOLLOW_UP_TEXT:
				UpdateTextFade();
				break;

			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:
				if (m_rCulturalNotesPage2.WasDoubleTapped)
					ContinueTutorial();
				break;

			default: // TutorialState.WAITING_FOR_CONTINUE_INPUT 
				break;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change CulturalNotes MovePage Tutorial State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ChangeCulturalNotesMovePageTutorialPointState(TutorialState eState)
	{
		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:
				ActivateDefaultShowingTutorialTextState();
				break;
			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:
				m_rCulturalNotesPage1.enabled = true;
				m_rCulturalNotesPage1.CanMove = true;
				ActivateDefaultWaitingForTutorialInputState();
				break;
			case TutorialState.SHOWING_FOLLOW_UP_TEXT:
				m_rCulturalNotesPage1.enabled = false;
				ActivateDefaultShowingFollowUpTextState();
				break;
			default: /*WAITING_FOR_CONTINUE_INPUT*/
				ActivateDefaultWaitingForContinueInputState();
				break;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change CulturalNotes Proceed To Next Page Tutorial State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ChangeCulturalNotesProceedToNextPageTutorialPointState(TutorialState eState)
	{
		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:
				m_rCulturalNotesPage1.enabled = true;
				m_rCulturalNotesPage1.CanMove = false;
				StartCoroutine(ReturnCulturalPageToDefaultPosition());
                ActivateDefaultShowingTutorialTextState();
				break;
			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:
				ActivateDefaultWaitingForTutorialInputState();
				break;
			case TutorialState.SHOWING_FOLLOW_UP_TEXT:
				ActivateDefaultShowingFollowUpTextState();
				break;
			default: /*WAITING_FOR_CONTINUE_INPUT*/
				ActivateDefaultWaitingForContinueInputState();
				break;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Method: Change CulturalNotes ResizePage Tutorial State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ChangeCulturalNotesResizePageTutorialPointState(TutorialState eState)
	{
		switch (eState)
		{
			case TutorialState.SHOWING_TUTORIAL_TEXT:
				ActivateDefaultShowingTutorialTextState();
				break;
			case TutorialState.WAITING_FOR_TUTORIAL_INPUT:
				m_rCulturalNotesPage2.enabled = true;
				m_rCulturalNotesPage2.CanResize = true;
				ActivateDefaultWaitingForTutorialInputState();
				break;
			case TutorialState.SHOWING_FOLLOW_UP_TEXT:
				ActivateDefaultShowingFollowUpTextState();
				break;
			default: /*WAITING_FOR_CONTINUE_INPUT*/
				ActivateDefaultWaitingForContinueInputState();
				break;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Return CulturalPage To Default Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private IEnumerator ReturnCulturalPageToDefaultPosition()
	{
		m_bFreezeTutorialUpdate = true;
		{
			yield return new WaitForSeconds(0.35f); // Stops it from happening so suddenly!

			m_rCulturalNotesPage1.ReturnToDefaultPosition();
			while(m_rCulturalNotesPage1.IsLerping)
				yield return new WaitForEndOfFrame();
		}
		m_bFreezeTutorialUpdate = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Callback Method: On Tutorial Complete
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void OnTutorialComplete()
	{
		base.OnTutorialComplete();

		// Make SubScene Active again!
		m_rGuideBookScene.ShowSubscene();
    }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnEnable()
	{
		base.OnEnable();

		// Set TutorialMode for CulturalNotes. CAN'T HAVE THEM DOING ANYTHING OUTSIDE OF PARAMETERS!
		m_goCulturalNotesNextNoteArrowButton.SetActive(false);
		m_rCulturalNotesPage1.CanMove = false;
		m_rCulturalNotesPage1.CanSingleTap = false;
		m_rCulturalNotesPage1.CanResize = false;
        m_rCulturalNotesPage1.CanSwitchNotes = false;
		m_rCulturalNotesPage1.enabled = false;

		m_rCulturalNotesPage2.CanMove = false;
		m_rCulturalNotesPage2.CanSingleTap = false;
		m_rCulturalNotesPage2.CanResize = false;
		m_rCulturalNotesPage2.CanSwitchNotes = false;
		m_rCulturalNotesPage2.enabled = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Disable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnDisable()
	{
		base.OnDisable();
		
		// Reset CulturalNotes from TutorialMode back to Normal
		m_goCulturalNotesNextNoteArrowButton.SetActive(true);
		m_rCulturalNotesPage1.CanMove = true;
		m_rCulturalNotesPage1.CanSingleTap = true;
		m_rCulturalNotesPage1.CanResize = true;
		m_rCulturalNotesPage1.CanSwitchNotes = true;
		m_rCulturalNotesPage1.enabled = true;

		m_rCulturalNotesPage2.CanMove = true;
		m_rCulturalNotesPage2.CanSingleTap = true;
		m_rCulturalNotesPage2.CanResize = true;
		m_rCulturalNotesPage2.CanSwitchNotes = true;
		m_rCulturalNotesPage2.enabled = true;
	}
}
