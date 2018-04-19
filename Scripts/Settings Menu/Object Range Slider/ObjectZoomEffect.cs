//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Object Zoom Effect
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: February 13, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used to adjust the scale of objects based on a min and max 
//		value. It is to be used in conjunction with other scripts which will give 
//		it the required instructions to do so.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ObjectZoomEffect : ObjectRangeSlider_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Vector3 m_vMinPos;
	public Vector3 m_vMaxPos;
	public Vector3 m_vMinScale;
	public Vector3 m_vMaxScale;
	public Transform m_tObjectToScale;


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Set Value
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void SetValue(float percentage)
	{
		base.SetValue(percentage);

		// Set Object/Transform Scale
		percentage = Mathf.Max(0.0f, Mathf.Min(percentage, 1.0f));
		m_tObjectToScale.localPosition = Vector3.Lerp(m_vMinPos, m_vMaxPos, percentage);
		m_tObjectToScale.localScale = Vector3.Lerp(m_vMinScale, m_vMaxScale, percentage);
	}
}
