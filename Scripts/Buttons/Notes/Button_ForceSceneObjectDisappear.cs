//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Notes Disappear
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: November 6, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is handles the input for when the player clicks an 'exit' button
//      when a note/page is displayed.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_ForceSceneObjectDisappear : Button_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ObjectTransitionAnimation m_rNotesReveal;
	public bool m_bOpenSettingsMenu = true;
	public bool m_bHideUntilOpened = false;		// Hides the button until the note has been fully revealed.



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Escape))
			OnTrigger();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();
		if(GameManager.CurrentSubscene != null)
		{
			GameManager.CurrentSubscene.HideSubscene();
			ButtonManager.ToggleAllButtons(true);
		}
        else if (m_rNotesReveal != null)
		{
			m_rNotesReveal.Disappear(m_bOpenSettingsMenu);
			ButtonManager.ToggleAllButtons(true);
		}
		SettingsMenuManager.UpdateMenuLabel();
	}
}