//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             ToggleOptions - TambourineTargets
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 22, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script interacts with the tambourine targets option. Setting the 
//      Targets on/off.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;


public class ToggleOptions_TambourineTargets : ToggleOptions_Base_ObjectTransitionAnimation
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public InstrumentManager m_rInstrumentsManager;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: On Selected Index Changed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnSelectedIndexChanged(int previousIndex)
	{
		base.OnSelectedIndexChanged(previousIndex);
		if(SelectedIndex == 0)
			m_rInstrumentsManager.InstrumentColoursManager.HideTargets();
		else
			m_rInstrumentsManager.InstrumentColoursManager.ShowTargets();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void OnEnable()
	{
		SelectedIndex = (m_rInstrumentsManager.InstrumentColoursManager.Visible ? 1 : 0);
	}
}
