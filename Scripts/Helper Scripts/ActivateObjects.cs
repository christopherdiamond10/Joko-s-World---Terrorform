//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Activate Attached Objects
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: August 25, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This Script (when activated) will also activate all objects that have been
//		associated with this script. It is used with the title screen intro 
//		sequence for Joko's World.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ActivateObjects : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool m_bActivateObjects = false;
	public GameObject[] m_agoObjectsToActivate;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool m_bStopActivation = false;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if (m_bActivateObjects)
		{
			if (!m_bStopActivation)
			{
				m_bStopActivation = true;
				if (m_agoObjectsToActivate != null && m_agoObjectsToActivate.Length > 0)
				{
					foreach (GameObject Obj in m_agoObjectsToActivate)
					{
						Obj.SetActive(true);
					}
				}
			}
		}
		else
		{
			m_bStopActivation = false;
		}
	}
}
