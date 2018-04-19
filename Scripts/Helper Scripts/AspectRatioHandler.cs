//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Aspect Ratio Handler
//             Author: Christopher Diamond
//             Date: January 29, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script discovers the current Aspect Ratio of the Device and attempts
//		to resize the app to fit the given dimensions. Dimensions, which, we did
//		develop primarily for.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class AspectRatioHandler : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Transform m_rSceneTransform;
	public UnityEngine.UI.Text m_rDebugTextRenderer;

	public Vector3 m_v16By10Position;
	public Vector3 m_v16By10Scale;

	public Vector3 m_v16By09Position;
	public Vector3 m_v16By09Scale;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake()
	{
		if(m_rDebugTextRenderer != null)
		{
			if(m_rDebugTextRenderer.GetComponent<MultiLanguageTextDisplay>() != null)
				m_rDebugTextRenderer.GetComponent<MultiLanguageTextDisplay>().enabled = false;

			m_rDebugTextRenderer.text = Camera.main.aspect.ToString();
		}

		if(Camera.main.aspect >= 0.5f && Camera.main.aspect < 0.6f) // 16:9 Resolution
		{
			m_rSceneTransform.localPosition = m_v16By09Position;
			m_rSceneTransform.localScale	= m_v16By09Scale;
		}
		else
		{
			m_rSceneTransform.localPosition = m_v16By10Position;
			m_rSceneTransform.localScale	= m_v16By10Scale;
		}
		//else if(Camera.main.aspect >= 1.5f)
		//{
		//	Debug.Log("3:2");
		//}
		//else
		//{
		//	Debug.Log("4:3");
		//}
	}
}
