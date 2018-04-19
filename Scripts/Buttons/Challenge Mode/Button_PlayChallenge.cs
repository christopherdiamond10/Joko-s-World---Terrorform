//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Play Challenge
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 23, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is handles the input for when the player clicks on the "Challenge"
//		button in the challenge mode.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_PlayChallenge : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ChallengeGameManager m_rChallengeGameMan;
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Derived Method: Start
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		if (!m_rChallengeGameMan.Active)
			m_rChallengeGameMan.BeginChallenge();
		else
			m_rChallengeGameMan.StopChallenge();
	}
}
