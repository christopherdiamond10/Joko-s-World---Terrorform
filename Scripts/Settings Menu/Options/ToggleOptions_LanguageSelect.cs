//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Toggle Options - Language Select
//             Author: Christopher Diamond
//             Date: January 31, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//	  This script interacts with the Language Selection options.
//		Allowing the user to toggle through the language we have available.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ToggleOptions_LanguageSelect : ToggleOptions_Base_ObjectTransitionAnimation
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public MultiLanguageTextDisplay[] m_arVisibleTextComponents;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Selected Index Changed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnSelectedIndexChanged(int previousIndex)
	{
		base.OnSelectedIndexChanged(previousIndex);
		GameManager.SelectedLanguage = (GameManager.SystemLanguages)SelectedIndex;

		if(m_arVisibleTextComponents != null)
		{
			for(int i = 0; i < m_arVisibleTextComponents.Length; ++i)
			{
				m_arVisibleTextComponents[i].Refresh();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void OnEnable()
	{
		LoadSavedPreference();
	}
}
