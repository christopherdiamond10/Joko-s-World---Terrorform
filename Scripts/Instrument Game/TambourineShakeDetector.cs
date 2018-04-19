//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Tambourine Shake Detector
//             Author: Christopher Diamond
//             Date: December 21, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script watches out for motion in the devices gyro sensor (or accelerator).
//		When this is moved beyond the user-defined sensitivity point, it will be
//		marked as a shake of the device. This script from there plays a sound effect
//		and changes the visuals of the tambourine temporarily to reflect this.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;


public class TambourineShakeDetector : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public InstrumentManager m_rInstrumentManager;
	public TambourineSoundsManager m_rTambSoundManager;

	// Separates the Serialised Values into Android and IOS. So that they can remain the same values when converting between IOS and Android platforms in Unity
#if UNITY_ANDROID
	public float m_fAndroidMinMovementStartSpeed = 1.3f;
	public float m_fAndroidSoftShakeSpeed = 2.2f;
	public float m_fAndroidHardShakeSpeed = 3.2f;
	public float m_fAndroidMinShakeValue = 1.0f;			// For Debugging With A Range_Slider
	public float m_fAndroidMaxShakeValue = 20.0f;			// For Debugging With A Range_Slider
#else
	public float m_fiOSMinMovementStartSpeed = 1.3f;
	public float m_fiOSSoftShakeSpeed = 2.2f;
	public float m_fiOSHardShakeSpeed = 3.2f;
	public float m_fiOSMinShakeValue = 1.0f;				// For Debugging With A Range_Slider
	public float m_fiOSMaxShakeValue = 20.0f;				// For Debugging With A Range_Slider
#endif

	public int m_iRattleShakesNeeded = 3;					// The amount of consecutive shakes needed to cause a rattle sound
	public float m_fTimeBetweenRattleShakes = 0.25f;		// The time limit between consecutive shakes to cause a rattle sound

	public float m_fShakeShowTime = 1.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int m_iCurrentRattleShakes = 0;
	private bool m_bUseShakeTimer = true;				// Use Shake timer to revert the shake visual cue back to normal.
	private bool m_bSoundPlayed = false;
	
	[SerializeField] 
	private float m_fSensitivity = 1.0f;
#if !UNITY_EDITOR
	private bool m_bCheckingAcceleration = false;		// Checking for shake acceleration
	private bool m_bTimeout = false;					// For when the there is a pause between the current Shake Sound and the next check. IE right after one sound is played
#endif
	private Vector3 m_vCurrentAcceleration;
	private Vector3 m_vPreviousAcceleration;
	private TimeTracker m_ttShakeRevealTimer;
	private TimeTracker m_ttRattleTimer;
	
	private static bool sm_bUpdateShakeCheck = true;	// Check for Tambourine Shake?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*= Debug Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
	public float m_fCurrentSliderValue = 0.0f;
#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*. Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static bool CheckForShake { get { return sm_bUpdateShakeCheck; } set { sm_bUpdateShakeCheck = value; } }

	public float ShakeSensitivity { get { return (1.0f - m_fSensitivity); } set { SetSensitivity(value); } }
	public bool HasTriggeredShake { get { return m_bSoundPlayed; } }
#if UNITY_ANDROID
	public float MinMovementSpeed	{ get { return m_fAndroidMinMovementStartSpeed; }	set { m_fAndroidMinMovementStartSpeed = value; } }
	public float SoftShakeSpeed		{ get { return m_fAndroidSoftShakeSpeed; }			set { m_fAndroidSoftShakeSpeed = value; } }
	public float HardShakeSpeed		{ get { return m_fAndroidHardShakeSpeed; }			set { m_fAndroidHardShakeSpeed = value; } }
	public float MinShakeValue		{ get { return m_fAndroidMinShakeValue; }			set { m_fAndroidMinShakeValue = value; } }
	public float MaxShakeValue		{ get { return m_fAndroidMaxShakeValue; }			set { m_fAndroidMaxShakeValue = value; } }
#else
	public float MinMovementSpeed	{ get { return m_fiOSMinMovementStartSpeed; }	set { m_fiOSMinMovementStartSpeed = value; } }
	public float SoftShakeSpeed		{ get { return m_fiOSSoftShakeSpeed; }			set { m_fiOSSoftShakeSpeed = value; } }
	public float HardShakeSpeed		{ get { return m_fiOSHardShakeSpeed; }			set { m_fiOSHardShakeSpeed = value; } }
	public float MinShakeValue		{ get { return m_fiOSMinShakeValue; }			set { m_fiOSMinShakeValue = value; } }
	public float MaxShakeValue		{ get { return m_fiOSMaxShakeValue; }			set { m_fiOSMaxShakeValue = value; } }
#endif

	private float AccelCX	{ get { return m_vCurrentAcceleration.x; } }
	private float AccelCY	{ get { return m_vCurrentAcceleration.y; } }
	private float AccelCZ	{ get { return m_vCurrentAcceleration.z; } }
	private float AccelCMag { get { return Mathf.Abs(AccelCX) + Mathf.Abs(AccelCY) + Mathf.Abs(AccelCZ); } }
	private float AccelPX	{ get { return m_vPreviousAcceleration.x; } }
	private float AccelPY	{ get { return m_vPreviousAcceleration.y; } }
	private float AccelPZ	{ get { return m_vPreviousAcceleration.z; } }
	private float AccelPMag { get { return Mathf.Abs(AccelPX) + Mathf.Abs(AccelPY) + Mathf.Abs(AccelPZ); } }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		m_ttShakeRevealTimer = new TimeTracker(m_fShakeShowTime);
		m_ttRattleTimer = new TimeTracker(m_fTimeBetweenRattleShakes);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if(CheckForShake)
		{
#if UNITY_EDITOR
			UpdateShakeReveal();			// Update Shake Visual Cue (If it is showing)
			m_ttRattleTimer.Update();		// Update Rattle Timer (if the next shake exceeds this timer, then rattle must be reset)

			if (Input.GetKeyDown(KeyCode.LeftControl))
			{
				// If we did not cause a rattle sound
				if (!PerformRattleShakeCheck())
				{
					// Make a shake sound
					PerformTambShakeCheck();
				}
			}
			else
			{
				m_bSoundPlayed = false;
			}
#else
			UpdateAcceleration();		// Update Movement Speed (Check Device Gyroscope)
			UpdateShakeReveal();		// Update Shake Visual Cue (If it is showing)
			m_ttRattleTimer.Update();	// Update Rattle Timer (if the next shake exceeds this timer, then rattle must be reset)

			// If we aren't already checking acceleration
			if (!m_bCheckingAcceleration)
			{
				// Have we at least STARTED moving? 
				if (IsMoving())
				{
					// GOOD. Make movement start noise!
					AudioSourceManager.PlayAudioClip( m_rTambSoundManager.GetTambourineSound(TambourineSoundsManager.SoundTypes.MOVE_TAMBOURINE_SOUND) );
					m_bCheckingAcceleration = true;
				}
			}

			// If we are checking acceleration and it has been long enough since the last noise to make a new one. Check for shake
			if (m_bCheckingAcceleration && !m_bTimeout)
			{
				PerformTambShakeCheck();
			}

			// No movement? Don;t bother checking for shake.
			if (!IsMoving())
			{
				m_bSoundPlayed = false;
				m_bCheckingAcceleration = false;
			}
#endif
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Perform Rattle Shake Check
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool PerformRattleShakeCheck()
	{
		// Increment Current Consecutive Shakes Count
		m_iCurrentRattleShakes += 1;

		// If the user took too long to make another shake, reset the rattle count
		if (m_ttRattleTimer.TimeUp())
		{
			m_iCurrentRattleShakes = 1;
		}

		// Reset Rattle timer (it would be unfair if we didn't reset the timer and allow the user as much time as possible to make another shake occur) 
		m_ttRattleTimer.Reset();

		// Got enough shakes? Awesome. Make the Rattle Sound
		if (m_iCurrentRattleShakes >= m_iRattleShakesNeeded)
		{
			if (!m_bSoundPlayed)
			{
				AudioSourceManager.PlayAudioClip( m_rTambSoundManager.GetTambourineSound(TambourineSoundsManager.SoundTypes.RATTLE_TAMBOURINE_SHAKE) );
				ShowShakenTambourine();
			}
			m_bSoundPlayed = true;
			return true;
		}
		return false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Perform Tambourine Shake Check
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void PerformTambShakeCheck()
	{
#if UNITY_EDITOR
		// Play Shake Sound unless it has already been played
		if (!m_bSoundPlayed)
		{
			AudioSourceManager.PlayAudioClip( m_rTambSoundManager.GetTambourineSound(TambourineSoundsManager.SoundTypes.HARD_TAMBOURINE_SHAKE) );
			ShowShakenTambourine();
		}
		m_bSoundPlayed = true;
#else
		// If Hard Shake, then make hard shake sound if Rattle hasn't occured
		if (IsHardShake())
		{
			if (!PerformRattleShakeCheck())
			{
				if (!m_bSoundPlayed)
				{
					AudioSourceManager.PlayAudioClip( m_rTambSoundManager.GetTambourineSound(TambourineSoundsManager.SoundTypes.HARD_TAMBOURINE_SHAKE) );
					ShowShakenTambourine();
				}
				m_bSoundPlayed = true;
			}
		}

		// If Soft Shake, then make soft shake sound if Rattle hasn't occured
		else if (IsSoftShake())
		{
			if (!PerformRattleShakeCheck())
			{
				if (!m_bSoundPlayed)
				{
					AudioSourceManager.PlayAudioClip( m_rTambSoundManager.GetTambourineSound(TambourineSoundsManager.SoundTypes.SOFT_TAMBOURINE_SHAKE) );
					ShowShakenTambourine();
				}
				m_bSoundPlayed = true;
			}
		}
#endif
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Acceleration
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateAcceleration()
	{
		m_vPreviousAcceleration = m_vCurrentAcceleration;
		m_vCurrentAcceleration = Input.acceleration;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Shake Reveal
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateShakeReveal()
	{
		// Show the tambourine facing towards the player once the timer is complete.
		if (m_bUseShakeTimer)
		{
			if (m_ttShakeRevealTimer.Update())
			{
				m_bUseShakeTimer = false;
				ShowNormalTambourine();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Shakened Tambourine
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ShowShakenTambourine(bool bUseShakeTimer = true, bool bHideTargets = false)
	{
		// Reset Shake Visual Cue Timer
		if (bUseShakeTimer)
		{
			if (m_ttShakeRevealTimer != null)
			{
				m_bUseShakeTimer = true;
				m_ttShakeRevealTimer.Reset();
			}
		}

		// Now show the tambourine for its shaken form
		m_rInstrumentManager.ShowSpecialInstrumentState();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Normal Tambourine
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ShowNormalTambourine(bool bHideTargets = false)
	{
		// Now show the tambourine for its normal form
		m_rInstrumentManager.ShowNormalInstrumentState();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Moving?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool IsMoving()
	{
		return (AccelCMag > MinMovementSpeed);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is a Soft Shake?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool IsSoftShake()
	{
		return (AccelPMag > SoftShakeSpeed) && (AccelCMag < SoftShakeSpeed);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is a Hard Shake?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool IsHardShake()
	{
		return (AccelPMag > HardShakeSpeed) && (AccelCMag < HardShakeSpeed);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Sensitivity
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetSensitivity(float percentage)
	{
		m_fSensitivity = (1.0f - percentage); // In this case, the slider is supposed to make it MORE Sensitive the higher it is. So we'll just reverse the input to achieve this.

		HardShakeSpeed = Mathf.Lerp(MinShakeValue, MaxShakeValue, m_fSensitivity) * 3.0f;
		SoftShakeSpeed = (HardShakeSpeed * 0.75f);		// Soft Shake is 75% of Hard Shake Requirement
		MinMovementSpeed = (SoftShakeSpeed * 0.75f);
	}
}
