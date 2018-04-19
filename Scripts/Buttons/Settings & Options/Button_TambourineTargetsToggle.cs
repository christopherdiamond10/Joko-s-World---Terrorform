//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Tambourine Targets Toggle
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: February 12, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script handles the input for showing/hiding the Tambourine Targets.
//		(the areas on the tambourine where you can strike)
//
//		This button is hidden away inside of the settings menu.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_TambourineTargetsToggle : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TambourineTargetsManager m_rTambTargetsManager;

	public SpriteRenderer m_sprNormalTambourineIcon;
	public SpriteRenderer m_sprTargetTambourineIcon;
	public Color m_cDisabledColour;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Start()
	{
		base.Start();
		ToggleButtonSprite();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		ToggleGameObject();
		ToggleButtonSprite();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Toggle Game Object
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ToggleGameObject()
	{
		m_rTambTargetsManager.ToggleTargets();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Toggle Button Sprite
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ToggleButtonSprite()
	{
		if (m_rTambTargetsManager.Visible)
		{
			m_sprNormalTambourineIcon.color = m_cDisabledColour;
			m_sprTargetTambourineIcon.color = Color.white;
		}
		else
		{
			m_sprNormalTambourineIcon.color = Color.white;
			m_sprTargetTambourineIcon.color = m_cDisabledColour;
		}
	}
}
