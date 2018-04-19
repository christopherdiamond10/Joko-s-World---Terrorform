//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Tutorial Point Of Interest
//             Author: Christopher Diamond
//             Date: December 11, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This button overlays on top of other buttons during the tutorial in order 
//		prevent the user from clicking on things we don't want them clicking on
//		during the tutorial.
//	  When clicked, this button will both call upon the actual button that
//		needed to be pressed and allow it to do it's job, and inform the tutorial
//		manager that it's ok to move on to the next phase.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;

public class Button_TutorialPointOfInterest : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Button_Base m_rButtonToCall;
	public TutorialManager_Base m_rTutorialManager;
	public ContinueType m_eContinueType = ContinueType.NEXT_TUTORIAL_POINT;
	public int m_iSelectedNextTutorialPointID = 0;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum ContinueType
	{
		NEXT_TUTORIAL_POINT,
		SELECTIVE,
	}
	
	

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		if (m_rTutorialManager != null)
		{
			int iNextTutorialPointID = ((m_eContinueType == ContinueType.SELECTIVE) ? m_iSelectedNextTutorialPointID : -1);
            m_rTutorialManager.OnPointOfInterestActivate(iNextTutorialPointID);
		}

		if(m_rButtonToCall != null)
			m_rButtonToCall.ForceClick();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Additional Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnAdditionalTrigger()
	{
		base.OnAdditionalTrigger();
		OnTrigger();
	}
}
