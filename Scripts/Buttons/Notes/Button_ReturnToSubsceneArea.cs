//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Return To Subscene Area
//             Author: Christopher Diamond
//             Date: January 31, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This button keeps track of a prior scene and transitions back to 
//		it when clicked.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_ReturnToSubsceneArea : Button_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ObjectTransitionAnimation m_rOwningPage;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public SubSceneManager				PreviousScene		{ get; set; }
	public ObjectTransitionAnimation	PreviousNotePage	{ get; set; }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		if(m_rOwningPage != null)
		{
			m_rOwningPage.Disappear(false);
        }

		// Show Previous Scene/Note
		if(PreviousScene != null)
		{
			if(PreviousScene.IsSceneActive)
				PreviousScene.ShowSceneVignette();
			else
				PreviousScene.ShowSubscene();
		}
		if(PreviousNotePage != null)
		{
			PreviousNotePage.Reveal(false);
		}

		// Hide Vignette if no one is going to own it :/
		if(PreviousScene == null)
			VignetteManager.TransitionVignette(0.0f, 0.25f);
	}
}
