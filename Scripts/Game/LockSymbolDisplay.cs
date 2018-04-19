//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Lock Symbol Display
//             Author: Christopher Diamond
//             Date: November 13, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script updates the lock symbol Animation. Additionally hiding the 
//		Lock Symbol if the app is in full version.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class LockSymbolDisplay : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AnimationEffect m_aeStartAnimation			= new AnimationEffect();
	public AnimationEffect[] m_aaeLoopedAnimationCycle	= new AnimationEffect[1];
	public AnimationEffect m_aeEndAnimation				= new AnimationEffect();

	public int m_iLoopedAnimationCycleLoopCount			= 6;
	public float m_fWaitTimeBetweenRepeats				= 4.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_iCurrentAnimationIndex = 0;
	private int m_iCurrentLoopCount = 0;

	private AnimationPhase m_eAnimationPhase = AnimationPhase.START;
	private TimeTracker m_ttWaitTimer;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum AnimationPhase
	{
		START,
		LOOPING,
		END,
		WAIT,
	}
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		// Hide if full version
		if (GameManager.IsFullVersion)
		{
			this.gameObject.SetActive(false);
			m_eAnimationPhase = AnimationPhase.WAIT;
		}

		// Otherwise setup Animation sequences
		else
		{
			m_ttWaitTimer = new TimeTracker(m_fWaitTimeBetweenRepeats);

			m_aeStartAnimation.Setup(this.transform);
			m_aeEndAnimation.Setup(this.transform);
			for (int i = 0; i < m_aaeLoopedAnimationCycle.Length; ++i)
			{
				m_aaeLoopedAnimationCycle[i].Setup(this.transform);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		switch (m_eAnimationPhase)
		{
			case AnimationPhase.START:		{ UpdateStartAnimation();  break; }
			case AnimationPhase.LOOPING:	{ UpdateLoopedAnimation(); break; }
			case AnimationPhase.END:		{ UpdateEndAnimation();	   break; }
			default:						{ UpdateWaitTime();		   break; }
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Start Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateStartAnimation()
	{
		if (m_aeStartAnimation.UpdateAnimation())
		{
			m_aeStartAnimation.Reset();
			m_eAnimationPhase = AnimationPhase.LOOPING;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Looped Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateLoopedAnimation()
	{
		if (m_aaeLoopedAnimationCycle[m_iCurrentAnimationIndex].UpdateAnimation())
		{
			m_aaeLoopedAnimationCycle[m_iCurrentAnimationIndex].Reset();

			// If finished this animation frame, move on to next. If no more animation frames remain... reset back to first.
			m_iCurrentAnimationIndex += 1;
			if (m_iCurrentAnimationIndex >= m_aaeLoopedAnimationCycle.Length)
			{
				m_iCurrentAnimationIndex = 0;

				// Increment loop count. We have just finished a loop after all. Should we exceed the loop limit. Move on to next animation phase.
				m_iCurrentLoopCount += 1;
				if (m_iCurrentLoopCount >= m_iLoopedAnimationCycleLoopCount)
				{
					m_iCurrentLoopCount = 0;
					m_eAnimationPhase = AnimationPhase.END;
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update End Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateEndAnimation()
	{
		if (m_aeEndAnimation.UpdateAnimation())
		{
			m_aeEndAnimation.Reset();
			m_eAnimationPhase = AnimationPhase.WAIT;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Wait Time
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateWaitTime()
	{
		if (m_ttWaitTimer.Update())
		{
			m_ttWaitTimer.Reset();
			m_eAnimationPhase = AnimationPhase.START;
		}
	}
}
