//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Joko's World Leaves Animation
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: November 14, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is handles the input for when the player clicks on the leaves
//      near Joko's World logo, in the title screen. It includes an animator variable
//      to signify which animation it will play when the button is pressed.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_JokosWorldLogoLeavesAnimation : Button_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Animator m_rLeavesAnimator;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		if (m_rLeavesAnimator != null)
		{
			m_rLeavesAnimator.Play("(Animation) Joko's World Logo (Leaves Rustling)");
		}
	}
}
