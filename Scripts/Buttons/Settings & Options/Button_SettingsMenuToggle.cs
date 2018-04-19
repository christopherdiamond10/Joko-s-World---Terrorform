//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Settings Menu Toggle
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: February 12, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script handles the input for when the player clicks on the Settings
//      Menu button. Including opening and closing the Settings Menu when clicking 
//      on it.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_SettingsMenuToggle : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public UnityEngine.UI.Text m_rMenuTextLabel;

	public MultiLanguageText m_oMenuLabelDisplay = new MultiLanguageText(); // Shown when clicking on the Menu Button Will Open the Menu
	public MultiLanguageText m_oExitLabelDisplay = new MultiLanguageText(); // Shown when clicking on the Menu Button Will Act as an Exit Button
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private MultiLanguageText CurrentMenuLabel
	{
		get { return (((GameManager.CurrentSubscene != null) || (ObjectTransitionAnimation.CurrentlyActive != null) || (SettingsMenuManager.Opened) || (SettingsMenuManager.Opening)) ? m_oExitLabelDisplay : m_oMenuLabelDisplay); }
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();
		m_oMenuLabelDisplay.ApplyEffects(m_rMenuTextLabel);
		if (GameManager.CurrentSubscene != null)
		{
			GameManager.CurrentSubscene.HideSubscene();
		}
		else if (ObjectTransitionAnimation.CurrentlyActive != null && !ObjectTransitionAnimation.CurrentlyActive.IsLocked)
		{
			ObjectTransitionAnimation.CurrentlyActive.Disappear();
		}
		else
		{
			if (SettingsMenuManager.Opened)
			{
				ShowUnpressedSprite();
				SettingsMenuManager.Close();
			}
			else
			{
				ShowPressedSprite();
				SettingsMenuManager.Open();

				m_oExitLabelDisplay.ApplyEffects(m_rMenuTextLabel);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Menu Label
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void UpdateMenuLabel()
	{
		CurrentMenuLabel.ApplyEffects(m_rMenuTextLabel);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnEnable()
	{
		base.OnEnable();
		UpdateMenuLabel();
	}
}
