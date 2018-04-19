//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Skip Title Intro Sequence
//             Author: Christopher Diamond
//             Date: November 06, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This button, when clicked, will simply skip past the Title Screen intro
//		sequence.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_SkipTitleIntroSequence : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TitleScreenAnimation m_rTitleScreenAnimation;

	public AnimationEffect[] m_arDisplayAnimation = new AnimationEffect[1];
	public AnimationEffect[] m_arTextDisplayAnimation = new AnimationEffect[1];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_iCurrentAnimationID = 0;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Start()
	{
		base.Start();
		if (SavedPreferenceTool.GetInt("SeenTitleIntroSequence", 0) == 0)
		{
			this.gameObject.SetActive(false);
		}
		else
		{
			for (int i = 0; i < m_arDisplayAnimation.Length; ++i)
			{
				m_arDisplayAnimation[i].Setup(this.transform);
			}
			for (int i = 0; i < m_arTextDisplayAnimation.Length; ++i)
			{
				m_arTextDisplayAnimation[i].Setup(TextRenderer.transform);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Update()
	{
		base.Update();

		m_arTextDisplayAnimation[m_iCurrentAnimationID].UpdateAnimation();
		if (m_arDisplayAnimation[m_iCurrentAnimationID].UpdateAnimation())
		{
			m_iCurrentAnimationID += 1;
			if (m_iCurrentAnimationID >= m_arDisplayAnimation.Length)
			{
				this.gameObject.SetActive(false);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();
		if (m_rTitleScreenAnimation != null)
		{
			m_rTitleScreenAnimation.ForceFinish();
			this.gameObject.SetActive(false);
		}
	}
}
