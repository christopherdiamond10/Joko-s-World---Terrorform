//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Joko's Title Screen Showcase
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: January 29, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used to randomise Joko's image appearance in the title 
//	  screen. Meaning he'll move from the left side of the screen to the right
//	  of the screen with his different instrument showcases.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JokosTitleScreenShowcase : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public const int LEFT_SIDE_ANIMATION_ID = 0;
	public const int MIDDLE_SIDE_ANIMATION_ID = 1;
	public const int RIGHT_SIDE_ANIMATION_ID = 2;


	public AnimationEffect[] m_aLeftAnimation	= new AnimationEffect[1];
	public AnimationEffect[] m_aMiddleAnimation = new AnimationEffect[1];
	public AnimationEffect[] m_aRightAnimation	= new AnimationEffect[1];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int			m_iCurrentShowcase			= RIGHT_SIDE_ANIMATION_ID;
	private int			m_iCurrentAnimationElement	= 0;
	private List<int>	m_lAvailableSpots			= new List<int>();
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private AnimationEffect[] CurrentAnimation
	{
		get
		{
			switch (m_iCurrentShowcase)
			{
				case LEFT_SIDE_ANIMATION_ID:	return m_aLeftAnimation;
				case RIGHT_SIDE_ANIMATION_ID:	return m_aRightAnimation;
				default:						return m_aMiddleAnimation;
			}
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		CreateAvailabilityList();

		for (int i = 0; i < m_aLeftAnimation.Length; ++i)
		{
			m_aLeftAnimation[i].Setup(this.transform);
		}
		for (int i = 0; i < m_aMiddleAnimation.Length; ++i)
		{
			m_aMiddleAnimation[i].Setup(this.transform);
		}
		for (int i = 0; i < m_aRightAnimation.Length; ++i)
		{
			m_aRightAnimation[i].Setup(this.transform);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if (CurrentAnimation[m_iCurrentAnimationElement].UpdateAnimation())
		{
			m_iCurrentAnimationElement += 1;
			if (m_iCurrentAnimationElement == CurrentAnimation.Length)
			{
				m_iCurrentAnimationElement = 0;
				CurrentAnimation[m_iCurrentAnimationElement].Reset();
				ShowNextShowcase();
			}
			else
			{
				CurrentAnimation[m_iCurrentAnimationElement].Reset();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Next Showcase
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ShowNextShowcase()
	{
		// Randomise showcase
		m_iCurrentShowcase = m_lAvailableSpots[Random.Range(0, m_lAvailableSpots.Count)];

		// Reset Availability for next time 
		CreateAvailabilityList();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Availability List
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CreateAvailabilityList()
	{
		m_lAvailableSpots.Clear();
		if (m_iCurrentShowcase != LEFT_SIDE_ANIMATION_ID)	{ m_lAvailableSpots.Add(LEFT_SIDE_ANIMATION_ID); }
		if (m_iCurrentShowcase != MIDDLE_SIDE_ANIMATION_ID) { m_lAvailableSpots.Add(MIDDLE_SIDE_ANIMATION_ID); }
		if (m_iCurrentShowcase != RIGHT_SIDE_ANIMATION_ID)	{ m_lAvailableSpots.Add(RIGHT_SIDE_ANIMATION_ID); }
	}


}
