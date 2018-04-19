//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Touch Ripple Effect
//             Author: Christopher Diamond
//             Date: November 05, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script merely shows the animation of the Touch Ripple Effect.
//		Resetting itself in the manager once it is complete
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TouchRippleEffect : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AnimationEffect[] m_arAnimationEffect = new AnimationEffect[1];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_iCurrentElementIndex = 0;	
	
	

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		for (int i = 0; i < m_arAnimationEffect.Length; ++i)
			m_arAnimationEffect[i].Setup(this.transform);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if (m_arAnimationEffect[m_iCurrentElementIndex].UpdateAnimation())
		{
			m_arAnimationEffect[m_iCurrentElementIndex].Reset();
			m_iCurrentElementIndex += 1;

			// If Reached End of Animation Cycle, Reset Animation and Turn Self Off in Hierachy
			if (m_iCurrentElementIndex >= m_arAnimationEffect.Length)
			{
				m_iCurrentElementIndex = 0;
				this.gameObject.SetActive(false);
			}
		}
	}
}
