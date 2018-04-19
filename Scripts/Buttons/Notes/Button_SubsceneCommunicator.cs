//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Subscene Communicator
//             Author: Christopher Diamond
//             Date: January 07, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script handles opening and closing a specific SubScene.
//		if the SubScene is inactive, then it will be opened.
//		if the SubScene is active, then it will be closed.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;

public class Button_SubsceneCommunicator : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public SubSceneManager m_rSubScene;
	public SubSceneManager m_rUrduSubscene;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private SubSceneManager SubScene
	{
		get
		{
			if (GameManager.DisplayableLanguages == GameManager.DisplayableLanguagesTypes.URDU_ONLY)
				return m_rUrduSubscene != null ? m_rUrduSubscene : m_rSubScene;
			return m_rSubScene;
		} 
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();
		if (SubScene.IsSceneActive)
		{
			SubScene.HideSubscene();
		}
		else
		{
			SubScene.ShowSubscene();
		}
	}
}
