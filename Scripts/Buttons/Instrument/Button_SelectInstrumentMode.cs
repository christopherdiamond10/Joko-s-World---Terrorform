//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Select Instrument Mode
//             Author: Christopher Diamond
//             Date: November 12, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This button, when clicked, will play a sound associated with this 
//		instrument. Futhermore, if the instrument is locked (because it is only
//		availble in the full version) it will display the Full-Game Plugin Screen.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_SelectInstrumentMode : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool m_bIsLockedInstrument = false;

	public InstrumentManager m_rInstrumentManager;
	public InstrumentManager.InstrumentMode m_eSelectedInstrument = InstrumentManager.InstrumentMode.RIQ_TAMBOURINE;
	
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		if (m_rInstrumentManager.CurrentInstrumentMode != m_eSelectedInstrument)
		{
			m_rInstrumentManager.CurrentInstrumentMode = m_eSelectedInstrument;
			if(!TutorialManager_Base.TutorialOpened)
				SettingsMenuManager.Close();
		}
	}
}
