//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Notice Me - Main Game Menu
//             Author: Christopher Diamond
//             Date: March 23, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script updates an animation intended to make the users 'notice' the
//		Main Game Menu.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class NoticeMe_MainGameMenu : NoticeMeAnimation
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Is Allowed To Animate?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override bool IsAllowedToAnimate()
	{
		// Only Shake Joko's Bag if there is nothing else that should be grabbing the Player's Attention
		return (!SettingsMenuManager.Opened && GameManager.CurrentSubscene == null && ObjectTransitionAnimation.CurrentlyActive == null);
	}
}
