//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Background Landscape Transition Effect
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 18, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used to transition the background states when switching from
//	  the Title->Game scene or vice versa. It handles the scaling and position.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class BackgroundLandscapeTransitionEffect : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AnimationEffect[] m_aToTitleScreenAnimation = new AnimationEffect[1];
	public AnimationEffect[] m_aToGameScreenAnimation = new AnimationEffect[1];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_iCurrentAnimationElement = 0;
	private TransitionState m_eTransitionState = TransitionState.IDLE;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsCompleted { get { return m_eTransitionState == TransitionState.IDLE; } }

	private AnimationEffect[] CurrentAnimation	
	{ 
		get { return (m_eTransitionState == TransitionState.TITLE_SCREEN) ? m_aToTitleScreenAnimation	: m_aToGameScreenAnimation; } 
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum TransitionState
	{
		IDLE,
		TITLE_SCREEN,
		GAME_SCREEN,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		for (int i = 0; i < m_aToTitleScreenAnimation.Length; ++i)
		{
			m_aToTitleScreenAnimation[i].Setup(this.transform);
		}
		for (int i = 0; i < m_aToGameScreenAnimation.Length; ++i)
		{
			m_aToGameScreenAnimation[i].Setup(this.transform);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if(m_eTransitionState != TransitionState.IDLE)
		{
			if (CurrentAnimation[m_iCurrentAnimationElement].UpdateAnimation())
			{
				m_iCurrentAnimationElement += 1;
				if (m_iCurrentAnimationElement >= CurrentAnimation.Length)
				{
					m_iCurrentAnimationElement = 0;
					CurrentAnimation[m_iCurrentAnimationElement].Reset();
					m_eTransitionState = TransitionState.IDLE;
				}
				else
				{
					CurrentAnimation[m_iCurrentAnimationElement].Reset();
				}
			}			
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Initiate Start Menu Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void InitiateStartMenuTransition()
	{
		m_eTransitionState = TransitionState.TITLE_SCREEN;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Initiate Game Scene Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void InitiateGameSceneTransition()
	{
		m_eTransitionState = TransitionState.GAME_SCREEN;
	}
}
