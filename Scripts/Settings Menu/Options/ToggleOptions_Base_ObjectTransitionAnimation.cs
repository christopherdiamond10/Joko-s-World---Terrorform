//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             ToggleOptions - Object Transition Animation
//             Author: Christopher Diamond
//             Date: September 22, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script simply extends the base class for the ToggleOptions to include
//		Support for ObjectTransitionAnimation's. Basically a 'fadein/fadeout' kind
//		of script used to animate objects in the game.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ToggleOptions_Base_ObjectTransitionAnimation : ToggleOptions_Base<ObjectTransitionAnimation>   
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Start()
	{
		base.Start();

		// Reveal SelectedItem if it hasn't been revealed already in the base 'Start'. It will reveal in the base Start if there has been a saved preference.
		if (!m_bSavePreference)
		{
			SelectedItem.Reveal(false, false);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Selected Index Changed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnSelectedIndexChanged(int previousIndex)
	{
		// Fadeout Previous Option
		m_arSelectableOptions[previousIndex].Disappear(false, !SelectedNextOption, false);

		base.OnSelectedIndexChanged(previousIndex);

		// Fadein New Option
		SelectedItem.Reveal(false, false, !SelectedNextOption);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Load Saved Index
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnLoadSavedIndex(int previousIndex)
	{
		OnSelectedIndexChanged(previousIndex);
	}
}
