//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Toggle Options - Tambourine Type
//             Author: Christopher Diamond
//             Date: September 22, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script interacts with the Tambourine Type options. Allowing you to 
//		switch the output sounds form the Tambourine with another type.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ToggleOptions_TambourineType : ToggleOptions_Base_ObjectTransitionAnimation
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public InstrumentManager m_rInstrumentManager;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: On Selected Index Changed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnSelectedIndexChanged(int previousIndex)
	{
		base.OnSelectedIndexChanged(previousIndex);
		m_rInstrumentManager.CurrentInstrumentMode =	(SelectedIndex == 0 ? InstrumentManager.InstrumentMode.RIQ_TAMBOURINE :
														(SelectedIndex == 1 ? InstrumentManager.InstrumentMode.PANDEIRO_TAMBOURINE :
														InstrumentManager.InstrumentMode.KANJIRA_TAMBOURINE));
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void OnEnable()
	{
		LoadSavedPreference();
	}
}
