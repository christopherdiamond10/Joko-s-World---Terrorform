//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Main Camera Transition
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: October 14, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used to transition between the two main scenes of the app
//	  (Title Screen & Game). As such, it handles the fade out/in and 
//	  movement/scaling of the scene during transitions.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class MainCameraTransition : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject m_goTitleSceneObject;
	public GameObject m_goGameSceneObject;
	public SceneFadeEffect m_rTitleSceneFadeEffect;
	public SceneFadeEffect m_rGameSceneFadeEffect;
	public BackgroundLandscapeTransitionEffect m_rBackgroundLandscapeTransitionEffect;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TransitionPhase m_ePreviousScene = TransitionPhase.SHOWSTART;
	private TransitionPhase m_eCurrentScene = TransitionPhase.SHOWSTART;
	private TransitionPhase m_eTransitionPhase = TransitionPhase.IDLE;
	private TransitionPoint m_eTransitionPoint = TransitionPoint.FADEOUT_START;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum TransitionPhase
	{
		SHOWSTART,
		SHOWGAME,
		IDLE,
	}

	private enum TransitionPoint
	{
		FADEOUT_START,
		FADEOUT,
		BACKGROUND_TRANSITION_START,
		BACKGROUND_TRANSITION,
		FADEIN_START,
		FADEIN,
	}

	private GameObject CurrentSceneObject
	{
		get { return m_eTransitionPhase == TransitionPhase.SHOWSTART ? m_goTitleSceneObject : m_goGameSceneObject; }
	}

	private SceneFadeEffect CurrentSceneFadeEffect
	{
		get { return m_eTransitionPhase == TransitionPhase.SHOWSTART ? m_rTitleSceneFadeEffect : m_rGameSceneFadeEffect; }
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if (m_eTransitionPhase != TransitionPhase.IDLE)
		{
			switch (m_eTransitionPoint)
			{
				case TransitionPoint.FADEOUT_START:					{ OnFadeoutBegin();					break; }
				case TransitionPoint.FADEOUT:						{ UpdateSceneFadeout();				break; }
				case TransitionPoint.BACKGROUND_TRANSITION_START:	{ OnBackgroundTransitionStart();	break; }
				case TransitionPoint.BACKGROUND_TRANSITION:			{ UpdateBackgroundTransition();		break; }
				case TransitionPoint.FADEIN_START:					{ OnFadeinBegin();					break; }
				case TransitionPoint.FADEIN:						{ UpdateFadein();					break; }
				default:											{									break; }
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Fadeout Begin
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnFadeoutBegin()
	{
		// Start Fadeout of scene and move on to the "Fadeout Wait" phase
		CurrentSceneFadeEffect.InitiateFadeOut();
		m_eTransitionPoint = TransitionPoint.FADEOUT;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Scene Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateSceneFadeout()
	{
		// If the scene has finished fading out, move on to next pahse
		if (CurrentSceneFadeEffect.IsCompleted)
		{
			CurrentSceneObject.SetActive(false);
			m_eTransitionPoint = TransitionPoint.BACKGROUND_TRANSITION_START;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Background Landscape Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnBackgroundTransitionStart()
	{
		// Remember that at the moment, these are actually reversed. This is to save code. So SHOWGAME actually means to ShowTitle Screen at this point.
		if (m_eCurrentScene == TransitionPhase.SHOWGAME)
		{
			m_rBackgroundLandscapeTransitionEffect.InitiateGameSceneTransition();
			
		}
		else
		{
			m_rBackgroundLandscapeTransitionEffect.InitiateStartMenuTransition();
		}
		m_eTransitionPoint = TransitionPoint.BACKGROUND_TRANSITION;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Background Landscape Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateBackgroundTransition()
	{
		// If the background transition has been completed, reverse the transition phase to be the opposite. This other scene will now be fading in
		if (m_rBackgroundLandscapeTransitionEffect.IsCompleted)
		{
			m_eTransitionPoint = TransitionPoint.FADEIN_START;
			m_eTransitionPhase = m_eCurrentScene;// (m_eTransitionPhase == TransitionPhase.SHOWSTART ? TransitionPhase.SHOWGAME : TransitionPhase.SHOWSTART);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Scene Fade in Begin
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnFadeinBegin()
	{
		CurrentSceneObject.SetActive(true);
		CurrentSceneFadeEffect.InitiateFadeIn();
		m_eTransitionPoint = TransitionPoint.FADEIN;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Scene Fade In
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateFadein()
	{
		if (CurrentSceneFadeEffect.IsCompleted)
		{
			m_eTransitionPhase = TransitionPhase.IDLE;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Scene Transition
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void TransitionGame(TransitionPhase eTransition)
	{
		m_ePreviousScene = m_eCurrentScene;
		m_eCurrentScene = eTransition;
		// Due to the way this script has been made, we actually start with the opposite of what we intended. We fade out that scene THEN Show (Fadein) the scene we actually intended
		m_eTransitionPhase = m_ePreviousScene;
		m_eTransitionPoint = TransitionPoint.FADEOUT_START;
	}
}
