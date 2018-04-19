//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Challenge Game - Metronome Tracker
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 13, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script keeps track of when the Metronome sound should play in a melody.
//		It uses BPM to determine how long it needs to wait before playing 
//		another one.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;


public class MetronomeTracker : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public AudioClip m_acMetronomeSound;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool m_bActivated = false;
	private Coroutine m_MetronomeInstance = null;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool Active { get { return m_bActivated; } }




	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Start Metronome
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StartMetronome(float trackBPM)
	{
		if (m_acMetronomeSound != null)
		{
			m_bActivated = true;

			if (m_MetronomeInstance != null)
				StopCoroutine(m_MetronomeInstance);

			m_MetronomeInstance = StartCoroutine(PlayMetronome(trackBPM));
		}
		else
		{
			Debug.LogError("Audio Clip not assigned to Metronome!");
		}
	}

	public void StartMetronome(int trackBPM)
	{
		StartMetronome((float)trackBPM);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Metronome
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StopMetronome()
	{
		m_bActivated = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Metronome
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private IEnumerator PlayMetronome(float trackBPM)
	{
		float fWaitTime = (1.0f / trackBPM) * 60.0f;
		while (this.Active)
		{
			AudioSourceManager.PlayAudioClip(m_acMetronomeSound, false, 0.05f);	// Play Metronome
			yield return new WaitForSeconds(fWaitTime);				// Wait until next Beat to play next Metronome
		}
		m_MetronomeInstance = null;
    }
}
