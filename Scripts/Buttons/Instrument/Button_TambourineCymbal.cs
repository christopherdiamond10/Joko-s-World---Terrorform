//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Tambourine Cymbal
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: December 11, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is handles the input for when the player clicks on a Tambourine
//      Cymbal. This includes shaking the cymbal after being pressed and playing
//      the associated sound.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_TambourineCymbal : Button_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TambourineSoundsManager m_rSoundManager;
	public ChallengeGameManager m_rChallengeGameManager;
	public InstrumentManager m_rTambInstrumentManager;
	public TambourineSoundsManager.SoundTypes m_eSoundType = TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_1;
	public float m_fTotalShakeTime;
	public float m_fShakeIntervals;
	public Vector3 m_vNormalPos;
	public Vector3 m_vShakePos;
	public Vector3 m_vNormalRotation;
	public Vector3 m_vShakeRotation;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool m_bShaking = false;
	private bool m_bNormalSprite = true;
	private TimeTracker m_ttTotalShakeTimer;
	private TimeTracker m_ttShakeIntervalTimer;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	** Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool NormalMode { get { return m_bNormalSprite; } }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Start()
	{
		base.Start();

		m_ttTotalShakeTimer = new TimeTracker(m_fTotalShakeTime);
		m_ttShakeIntervalTimer = new TimeTracker(m_fShakeIntervals);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Update()
	{
		// If the Cymbal is shaking
		if (m_bShaking)
		{
			// Time to finish?
			if (m_ttTotalShakeTimer.Update())
			{
				StopCymbalShake();
			}
			// Time to show other sprite?
			else if (m_ttShakeIntervalTimer.Update())
			{
				m_ttShakeIntervalTimer.Reset();
				ToggleSprite();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();
		StartCymbalShake(m_fTotalShakeTime);


		// Challenge Mode Active? Make Sure Sound can be played before it will can be heard
		if (m_rChallengeGameManager != null && m_rChallengeGameManager.Active)
		{
			if(m_rChallengeGameManager.IsChallenge)
			{
				m_rChallengeGameManager.SuccessfulHit(m_eSoundType);
				if(m_rSoundManager != null)
				{
					AudioSourceManager.PlayAudioClip(m_rSoundManager.GetTambourineSound(m_eSoundType));
					if(m_rTambInstrumentManager != null)
						m_rTambInstrumentManager.CurrentInstrumentHitCount += 1;
                }
			}
		}

		// Otherwise Play Normally.
		else if (m_rSoundManager != null)
		{
			AudioSourceManager.PlayAudioClip(m_rSoundManager.GetTambourineSound(m_eSoundType));
			if(m_rTambInstrumentManager != null)
				m_rTambInstrumentManager.CurrentInstrumentHitCount += 1;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Additional Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnAdditionalTrigger()
	{
		base.OnAdditionalTrigger();
		OnTrigger(); // Just pretend like it was the same as the first time. Still play the Sound and show the Visual Cue :D
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reveal Pressed Sprite
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void RevealPressedSprite()
	{
		ShowPressedSprite();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reveal UnPressed Sprite
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void RevealUnpressedSprite()
	{
		ShowUnpressedSprite();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Toggle Sprite
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ToggleSprite()
	{
		if (NormalMode)
		{
			RevealPressedSprite();
		}
		else
		{
			RevealUnpressedSprite();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Show Pressed Sprite
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void ShowPressedSprite()
	{
		base.ShowPressedSprite();
		transform.localPosition = m_vShakePos;
		transform.localRotation = Quaternion.Euler(m_vShakeRotation);
		m_bNormalSprite = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Show Pressed Sprite
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void ShowUnpressedSprite()
	{
 		base.ShowUnpressedSprite();
		transform.localPosition = m_vNormalPos;
		transform.localRotation = Quaternion.Euler(m_vNormalRotation);
		m_bNormalSprite = true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start Cymbal Shake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StartCymbalShake(float howLong = 2.0f)
	{
		RevealPressedSprite();
		m_bNormalSprite = false;
		m_bShaking = true;

		m_ttTotalShakeTimer.FinishTime = howLong;
		m_ttShakeIntervalTimer.Reset();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Stop Cymbal Shake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void StopCymbalShake()
	{
		m_ttTotalShakeTimer.Reset();
		ShowUnpressedSprite();
		m_bNormalSprite = true;
		m_bShaking = false;
	}
}
