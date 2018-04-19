//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Display Text Over Time
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: July 31, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This Script allows you to make text appear over time like it often does 
//		when a character is talking in an RolePlaying Game. The text will appear
//		one character at a time until the text is complete.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class DisplayTextOverTime : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public UnityEngine.UI.Text m_rText;
	public float m_fCharacterWaitTime = 0.1f;
	public Animator m_rJokosSpeechAnimator = null;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_iCurrentTextElement = 1;
	private bool m_bHasText = false;
	private string m_sFullText = "";
	private TimeTracker m_ttCharacterWaitTimer;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake()
	{
		m_ttCharacterWaitTimer = new TimeTracker(m_fCharacterWaitTime);
		m_sFullText = m_rText.text;
		m_rText.text = "";
		m_bHasText = (m_rText.text != m_sFullText);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if (m_bHasText)
		{
			if (m_iCurrentTextElement < m_sFullText.Length)
			{
				// If it's time for next character, show it.
				if (m_ttCharacterWaitTimer.Update())
				{
					m_ttCharacterWaitTimer.Reset();
					m_rText.text = m_sFullText.Substring(0, m_iCurrentTextElement);
					m_iCurrentTextElement += 1;
					if (m_iCurrentTextElement >= m_sFullText.Length)
					{
						OnFullyDisplayed();
					}
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Display Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void DisplayText(string newText)
	{
		m_iCurrentTextElement = 0;
		m_sFullText = newText;
		m_rText.text = "";
		m_bHasText = true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnEnable()
	{
		// Simply Resetting Text
		m_iCurrentTextElement = 0;
		m_rText.text = "";

		if (m_rJokosSpeechAnimator != null)
		{
			m_rJokosSpeechAnimator.SetBool("StopSpeech", false);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Fully Displayed (Text)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnFullyDisplayed()
	{
		m_rText.text = m_sFullText;
		if (m_rJokosSpeechAnimator != null)
		{
			m_rJokosSpeechAnimator.SetBool("StopSpeech", true);
		}
	}
}
