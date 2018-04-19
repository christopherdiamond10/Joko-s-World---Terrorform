//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Challenge Game - Challenge Game Score Manager
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 20, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script simply keeps track of the Challenge Game's Score and associated
//		images. This includes the ability to change said items and provide 
//		animations
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ChallengeGameScoreManager : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AnimationEffect[] m_arIncrementAnimation = new AnimationEffect[1];
	public AnimationEffect[] m_arDecrementAnimation = new AnimationEffect[1];

	public UnityEngine.UI.Text m_rUIText;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_iCurrentAnimation = 0;
	private string m_sScoreText = "100%";
	private AnimationPhase m_eAnimationPhase = AnimationPhase.IDLE;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum AnimationPhase
	{
		IDLE,
		INCREMENT,
		DECREMENT,
	}

	public enum ScoreResult
	{
		TERRIBLE	= 0,
		OKAY		= 50,
		GOOD		= 80,
		GREAT		= 95,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Get Score Result
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static ScoreResult GetScoreResult(int a_iScore)
	{
		return	(a_iScore >= (int)ScoreResult.GREAT) ?	ScoreResult.GREAT :
				(a_iScore >= (int)ScoreResult.GOOD)	 ?	ScoreResult.GOOD  :
				(a_iScore >= (int)ScoreResult.OKAY)	 ?	ScoreResult.OKAY  :
														ScoreResult.TERRIBLE;
	}




	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		for (int i = 0; i < m_arIncrementAnimation.Length; ++i)
			m_arIncrementAnimation[i].Setup(this.transform);
		for (int i = 0; i < m_arDecrementAnimation.Length; ++i)
			m_arDecrementAnimation[i].Setup(this.transform);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		switch (m_eAnimationPhase)
		{
			case AnimationPhase.INCREMENT:
				UpdateIncrementAnimation();
				break;
			case AnimationPhase.DECREMENT:
				UpdateDecrementAnimation();
				break;
			default:
				break;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Increment Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateIncrementAnimation()
	{
		if (m_arIncrementAnimation[m_iCurrentAnimation].UpdateAnimation())
		{
			m_arIncrementAnimation[m_iCurrentAnimation].Reset();
			m_iCurrentAnimation += 1;

			// Update Score!
			if (m_rUIText != null && m_iCurrentAnimation == 1) // On Wait Animation?
				m_rUIText.text = m_sScoreText;

			if (m_iCurrentAnimation >= m_arIncrementAnimation.Length)
			{
				m_eAnimationPhase = AnimationPhase.IDLE;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Decrement Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateDecrementAnimation()
	{
		if (m_arDecrementAnimation[m_iCurrentAnimation].UpdateAnimation())
		{
			m_arDecrementAnimation[m_iCurrentAnimation].Reset();
			m_iCurrentAnimation += 1;

			// Update Score!
			if (m_rUIText != null && m_iCurrentAnimation == 1) // On Wait Animation?
				m_rUIText.text = m_sScoreText;

			if (m_iCurrentAnimation >= m_arDecrementAnimation.Length)
			{
				m_eAnimationPhase = AnimationPhase.IDLE;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Increment Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginIncrementAnimation(string score)
	{
		m_eAnimationPhase = AnimationPhase.INCREMENT;
		m_iCurrentAnimation = 0;

		m_sScoreText = score;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Decrement Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginDecrementAnimation(string score)
	{
		m_eAnimationPhase = AnimationPhase.DECREMENT;
		m_iCurrentAnimation = 0;

		m_sScoreText = score;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnEnable()
	{
		m_eAnimationPhase = AnimationPhase.IDLE;
		m_arDecrementAnimation[0].ShowFirstFrame();
	}
}
