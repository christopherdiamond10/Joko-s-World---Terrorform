//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Highlight Effect
//             Author: Christopher Diamond
//             Date: December 10, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script interacts with the 'SolidColour' shader changing the displayed
//		material colour. Creating a HighlightEffect.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;


public class HighlightEffect : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ColourAnimationEffect[] m_arColourAnimation = new ColourAnimationEffect[1];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_iCurrentAnimationID = 0;

	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		for(int i = 0; i < m_arColourAnimation.Length; ++i)
			m_arColourAnimation[i].Setup( GetComponent<SpriteRenderer>().material );

		Reset();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update() 
	{
		if (m_arColourAnimation[m_iCurrentAnimationID].UpdateAnimation())
		{
			m_arColourAnimation[m_iCurrentAnimationID].Reset();

			m_iCurrentAnimationID += 1;
			if (m_iCurrentAnimationID >= m_arColourAnimation.Length)
			{
				m_iCurrentAnimationID = 0;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Reset()
	{
		m_iCurrentAnimationID = 0;
		if(m_arColourAnimation != null && m_arColourAnimation.Length > 0)
		{
			m_arColourAnimation[m_iCurrentAnimationID].Reset();
			m_arColourAnimation[m_iCurrentAnimationID].ShowFirstFrame();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void OnEnable()
	{
		Reset();
	}
}
