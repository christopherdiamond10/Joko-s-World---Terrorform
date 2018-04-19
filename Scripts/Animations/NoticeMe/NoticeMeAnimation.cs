//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Notice Me - Animation
//             Author: Christopher Diamond
//             Date: March 23, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script updates an animation intended to be used to direct the player's
//		attention to it. "Notice Me"
//	  It includes a start/end and loop animation giving you flexibility
//		to determine how the start animation blends into the loop blending into the
//		end animation.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class NoticeMeAnimation : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AnimationEffect m_aeStartAnimation			= new AnimationEffect();
	public AnimationEffect[] m_aaeLoopedAnimationCycle	= new AnimationEffect[1];
	public AnimationEffect m_aeEndAnimation				= new AnimationEffect();

	public int m_iLoopedAnimationCycleLoopCount			= 8;
	public float m_fWaitTimeBetweenRepeats				= 3.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected int m_iCurrentAnimationIndex = 0;
	protected int m_iCurrentLoopCount = 0;

	protected AnimationPhase m_eAnimationPhase = AnimationPhase.START;
	protected TimeTracker m_ttWaitTimer;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected enum AnimationPhase
	{
		START,
		LOOPING,
		END,
		WAIT,
		STOPPED,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Start()
	{
		m_ttWaitTimer = new TimeTracker(m_fWaitTimeBetweenRepeats);
		SetupAnimations();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Setup Animations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void SetupAnimations()
	{
		m_aeStartAnimation.Setup(this.transform);
		m_aeEndAnimation.Setup(this.transform);
		for (int i = 0; i < m_aaeLoopedAnimationCycle.Length; ++i)
		{
			m_aaeLoopedAnimationCycle[i].Setup(this.transform);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Update()
	{
		switch (m_eAnimationPhase)
		{
			case AnimationPhase.START:		{ UpdateStartAnimation();  break; }
			case AnimationPhase.LOOPING:	{ UpdateLoopedAnimation(); break; }
			case AnimationPhase.END:		{ UpdateEndAnimation();	   break; }
			case AnimationPhase.WAIT:		{ UpdateWaitTime();		   break; }
			default: 						{ break; } 
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Start Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void UpdateStartAnimation()
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
	protected virtual void UpdateLoopedAnimation()
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
	protected virtual void UpdateEndAnimation()
	{
		if (m_aeEndAnimation.UpdateAnimation())
		{
			m_aeEndAnimation.Reset();
			m_eAnimationPhase = AnimationPhase.WAIT;
			OnAnimationEnd();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Wait Time
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void UpdateWaitTime()
	{
		if (m_ttWaitTimer.Update())
		{
			m_ttWaitTimer.Reset();
			if(IsAllowedToAnimate())
			{
				m_eAnimationPhase = AnimationPhase.START;
				OnAnimationStart();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Allowed To Animate?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual bool IsAllowedToAnimate()
	{
		return true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Animation Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnAnimationStart()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Animation End
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnAnimationEnd()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Start Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public virtual void StartAnimation()
	{
		m_eAnimationPhase = AnimationPhase.START;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public virtual void StopAnimation()
	{
		for (int i = 0; i < m_aaeLoopedAnimationCycle.Length; ++i) 
		{
			m_aaeLoopedAnimationCycle [i].Reset();
		}
		m_aeStartAnimation.Reset();
		m_aeEndAnimation.Reset();

		m_aeEndAnimation.ShowLastFrame();
		m_eAnimationPhase = AnimationPhase.STOPPED;
	}
}
