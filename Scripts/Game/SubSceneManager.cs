//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Sub Scene Manager
//             Author: Christopher Diamond
//             Date: January 06, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is called upon to Enter, Exit and Update SubScene Areas'
//		These include for example: the Guide Book & Music Challenges Selection.
//
//	  This script will handle Vignette and Audio for the Scene.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class SubSceneManager : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TutorialManager_Base m_rAssignedSubsceneTutorial;
	public ObjectTransitionAnimation m_rStartSceneObjectTransitionAnimation;

	public AudioSourceManager.AudioHandlerInfo m_oSubsceneAudioHandler = new AudioSourceManager.AudioHandlerInfo();
	public VignetteManager.VignetteInfo m_oVignetteInfo = new VignetteManager.VignetteInfo();
	public Button_ForceSceneObjectDisappear[] m_arExitButtons;

	public int m_iTutorialStartPoint = 0;
	public float m_fFadeinAudioTime = 3.0f;

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private ObjectTransitionAnimation m_rCurrentSceneObject;

	static int sm_iNecessaryTapsUntilAdIsShown = 2;
	static int sm_iCurrentTapsCount = 0; 
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsSceneActive { get; private set; }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{	
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{	
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Subscene
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ShowSubscene()
	{
#if URDU_LANGUAGE_ONLY
		CheckForAdOK();
#endif

		if (m_rAssignedSubsceneTutorial != null && !m_rAssignedSubsceneTutorial.HasCompletedTutorial)
		{
			m_rAssignedSubsceneTutorial.BeginTutorial( m_iTutorialStartPoint );
        }

		if(!TutorialManager_Base.TutorialOpened)
		{
			ShowSceneVignette();
			PlaySceneBGM();
        }

		if (m_rStartSceneObjectTransitionAnimation != null)
		{
			m_rStartSceneObjectTransitionAnimation.Reveal();
			SetCurrentSceneObject(m_rStartSceneObjectTransitionAnimation);
		}

		IsSceneActive = true;
		GameManager.CurrentSubscene = this;

		// Assign Self to the exit buttons. If clicked, the exit button instance will call the 'disappear' function.
		for(int i = 0; i < m_arExitButtons.Length; ++i)
		{
			if(m_arExitButtons[i] != null)
			{
				m_arExitButtons[i].gameObject.SetActive(true);
				m_arExitButtons[i].enabled = true;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Hide Subscene
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void HideSubscene()
	{
		AudioSourceManager.FadeoutAudio(m_oSubsceneAudioHandler);

		if(!TutorialManager_Base.TutorialOpened)
		{
			VignetteManager.TransitionVignette(0.0f, m_oVignetteInfo.transitionTime, m_oVignetteInfo.orderInLayer);
		}

		if (m_rCurrentSceneObject != null)
		{
			m_rCurrentSceneObject.Disappear(false);
		}

		IsSceneActive = false;
		if (GameManager.CurrentSubscene == this)
		{
			GameManager.CurrentSubscene = null;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Current Scene Object
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetCurrentSceneObject(ObjectTransitionAnimation sceneObject)
	{
		m_rCurrentSceneObject = sceneObject;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Lower BGM Volume For Set Time
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void LowerBGMVolumeForSetTime(float howLong, float fadeoutTime = 0.0f)
	{
		int audioHandlerID = m_oSubsceneAudioHandler.AudioHandlerID;
		float startVolume = AudioSourceManager.GetSelectedAudioVolume(m_oSubsceneAudioHandler.AudioHandlerID);
		float endVolume = 0.005f;
        AudioSourceManager.FadeoutAudioThenFadeBackIn(audioHandlerID, startVolume, endVolume, m_oSubsceneAudioHandler.m_fMaxVolume, fadeoutTime, howLong);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Play Scene BGM
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void PlaySceneBGM()
	{
		if(m_oSubsceneAudioHandler != null)
		{
			AudioSourceManager.PlayAudioClip(m_oSubsceneAudioHandler);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Scene Vignette
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ShowSceneVignette()
	{
		VignetteManager.TransitionVignette(m_oVignetteInfo);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check If Ad Should Show
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static void CheckForAdOK()
	{
		sm_iCurrentTapsCount += 1;
		if (sm_iCurrentTapsCount >= sm_iNecessaryTapsUntilAdIsShown)
		{
			sm_iCurrentTapsCount = 0;
			sm_iNecessaryTapsUntilAdIsShown = Random.Range(2, 6);
			AdsManager.ShowAdvertisement();
		}
	}
}
