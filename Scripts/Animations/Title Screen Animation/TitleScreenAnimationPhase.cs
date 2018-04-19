//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Title Screen Animation Phase
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 15, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is manages a single individual Phase during the title screen
//	  sequence. This script is mainly used as a way for Unity's in-built animation
//	  system to communicate when an animation has reached a certain point.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TitleScreenAnimationPhase : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TitleScreenAnimation m_rTitleScreenAnimationPhase;
	public bool m_bMoveOnToNextPhase = false;
	public bool m_bDeactivateAnimator = false;
	public bool m_bKeepAnimatorActive = false;
	public bool m_bDestroyThisScriptOnNextPhase = true;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool AnimatorStatus { get { return GetComponent<Animator>().enabled; } set { GetComponent<Animator>().enabled = value; } }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if (m_bMoveOnToNextPhase)
		{
			m_rTitleScreenAnimationPhase.ProceedToNextPhase();		// Move on to next Title Screen Phase
			m_bMoveOnToNextPhase = false;							// Disallow continuous transitions from this script
			AnimatorStatus = m_bKeepAnimatorActive;					// Disable This GameObject's Animator if necessary
			if (m_bDestroyThisScriptOnNextPhase)
			{
				DestroyImmediate(this);								// Destroy this Instance of this script
			}
		}
		else if (m_bDeactivateAnimator)
		{
			AnimatorStatus = false;									// Disable This GameObject's Animator if necessary
			DestroyImmediate(this);									// Destroy this Instance of this script
		}
	}
}
