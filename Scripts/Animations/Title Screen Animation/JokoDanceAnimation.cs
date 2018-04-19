//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Joko's Dance Animation
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 17, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used to transition between the different states of Joko's 
//	  Dance animation. From his initial blinking, to his full-on sway animation.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JokoDanceAnimation : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Transform m_tLeftHandMusicalNotes;
	public Transform m_tRightHandMusicalNotes;
	public bool m_bFullSwayCheck = false;
	public bool m_bDeactivateFullSway = false;
	public bool m_bShowLeftMusicalNote = false;
	public bool m_bShowRightMusicalNote = false;
	public int m_iMinSwayChance = 10;
	public int m_iSwayChanceIncrement = 20;
	public int m_iMaxFails = 5;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_iCurrentFails = 0;
	private int m_iCurrentFullSwayChance = 30;
	private bool m_bStopFullSwayCheck = false;
	private bool m_bKeepNotesDisabled = false;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Animator
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Animator GetAnimator()
	{
		return this.GetComponent<Animator>(); 
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		m_iCurrentFullSwayChance = m_iMinSwayChance;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if (!m_bStopFullSwayCheck)
		{
			if (m_bFullSwayCheck)
			{
				m_bStopFullSwayCheck = true;
				m_bFullSwayCheck = false;
				if ((m_iCurrentFails == m_iMaxFails) || (Random.Range(0, 100) < m_iCurrentFullSwayChance))
				{
					GetAnimator().SetBool("FullSway", true);
					m_iCurrentFullSwayChance = m_iMinSwayChance;
					m_iCurrentFails = 0;
				}
				else
				{
					m_iCurrentFullSwayChance += m_iSwayChanceIncrement;
					m_iCurrentFails += 1;
				}
			}
		}
		else if (!m_bFullSwayCheck)
		{
			m_bStopFullSwayCheck = false;
		}

		if (m_bDeactivateFullSway)
		{
			m_bDeactivateFullSway = false;
			GetAnimator().SetBool("FullSway", false);
		}

		if (!m_bKeepNotesDisabled)
		{
			if (m_bShowLeftMusicalNote || m_bShowRightMusicalNote)
			{
				m_bKeepNotesDisabled = true;
				List<Transform> lInactiveMusicalNotes = new List<Transform>();
				Transform tNotesParent = (m_bShowLeftMusicalNote ? m_tLeftHandMusicalNotes : m_tRightHandMusicalNotes);

				foreach (Transform child in tNotesParent)
				{
					if (!child.gameObject.activeInHierarchy)
					{
						lInactiveMusicalNotes.Add(child);
					}
				}

				if (lInactiveMusicalNotes.Count > 0)
				{
					Transform theChosenOne = lInactiveMusicalNotes[Random.Range(0, lInactiveMusicalNotes.Count)];
					theChosenOne.gameObject.SetActive(true);
				}
			}
		}
		else if (!m_bShowLeftMusicalNote && !m_bShowRightMusicalNote)
		{
			m_bKeepNotesDisabled = false;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnEnable()
	{
		m_iCurrentFails = 0;
		m_bShowLeftMusicalNote = false;
		m_bShowRightMusicalNote = false;
		m_bDeactivateFullSway = false;
		m_bKeepNotesDisabled = false;
		m_bStopFullSwayCheck = false;
		GetAnimator().SetBool("FullSway", false);
	}
}
