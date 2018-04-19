//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Challenge Game - Note Movement
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 13, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script keeps track of notes used in the Challenge Game. This includes
//		their movement, useful debugging options to help set them up 
//		automatically, and sprite changes as necessary.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChallengeNoteMovement : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ChallengeModeInfo m_rChallengeModeInfo;
	public AnimationEffect[] m_arToScoreAnimation = new AnimationEffect[1];
	public AnimationEffect[] m_arIntoBagAnimation = new AnimationEffect[1];

	public TambourineSoundsManager.SoundTypes m_eSoundType = TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA;

	public float m_fBeatPos = 1.0f;
	public float m_fMissedTimer = 2.5f;

	public Sprite m_sprNormalSprite;
	public Sprite m_sprSuccessSprite;
	public Sprite m_sprMissedSprite;

	public AudioClip m_acHitSound;
	public AudioClip m_acMissedSound;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Vector3 m_vStartPosition = Vector3.zero;
	
	private TimeTracker m_ttMissedMovementTracker;

	private int m_iCurrentAnimationElement = 0;
	private float m_fCurrentBeatLine = 0;
	private float m_fMissedTime = 0.0f;
	private float m_fPreviousBeatTime = 0.0f;

	private MovementPhase m_eMovementPhase = MovementPhase.MOVEMENT;
	private SuccessAnimationPhase m_eSuccessAnimationPhase = SuccessAnimationPhase.TO_SCORE;

	private TransformInterpreter m_tiTransform;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*-- Debug Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
	public float m_fLinePos = 3.0f;
	public Sprite m_sprRedAreaNormal;
	public Sprite m_sprBlueAreaNormal;
	public Sprite m_sprYellowAreaNormal;
	public Sprite m_sprCymbalNormal;
	public Sprite m_sprShakeNormal;

	public Sprite m_sprRedAreaHighlight;
	public Sprite m_sprBlueAreaHighlight;
	public Sprite m_sprYellowAreaHighlight;
	public Sprite m_sprCymbalHighlight;
	public Sprite m_sprShakeHighlight;

	public Sprite m_sprAreaSymbolMissed;
	public Sprite m_sprCymbalMissed;
	public Sprite m_sprShakeMissed;
//#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float MetronomeTime			{ get { return ((1.0f / m_rChallengeModeInfo.ChallengeBPM) * 60.0f); } }
	public float BeatTime				{ get { return (((m_rChallengeModeInfo.notationSquareOffset + m_fBeatPos + 1) / m_rChallengeModeInfo.ChallengeBPM) * 60.0f); } }
	public float BeatPos				{ get { return m_fCurrentBeatLine; } set { m_fCurrentBeatLine = value; transform.localPosition = new Vector3(m_rChallengeModeInfo.victoryLocation.x + m_rChallengeModeInfo.separationDifference.x * m_fCurrentBeatLine, transform.localPosition.y, transform.localPosition.z); } }
    public SpriteRenderer SprRend		{ get { return GetComponent<SpriteRenderer>(); } }
	public MovementPhase CurrentState	{ get { return m_eMovementPhase; } }
	public bool IsPractice				{ get { return CurrentState == MovementPhase.AUTO_PLAY; } }
	public bool IsChallenge				{ get { return CurrentState == MovementPhase.MOVEMENT; } }

	private TransformInterpreter.LocalPositionInterpreter LocalPosition { get { return m_tiTransform.LocalPosition; } }
	private bool MoveContinueOK { get { return IsPractice || IsChallenge; } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum MovementPhase
	{
		IDLE,
		MOVEMENT,
		SUCCESS,
		MISS,
		AUTO_PLAY,
	}

	private enum SuccessAnimationPhase
	{
		TO_SCORE,
		TO_BAG,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		m_vStartPosition = transform.localPosition;
		m_ttMissedMovementTracker = new TimeTracker(m_fMissedTimer);
		m_tiTransform = new TransformInterpreter(this);

		for (int i = 0; i < m_arToScoreAnimation.Length; ++i)
			m_arToScoreAnimation[i].Setup(this.transform);
		for (int i = 0; i < m_arIntoBagAnimation.Length; ++i)
			m_arIntoBagAnimation[i].Setup(this.transform);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		switch (m_eMovementPhase)
		{
			case MovementPhase.MOVEMENT:
			{
				if (UpdateMovement())
				{
					InvokeUnsuccessfulHit();
				}
				break;
			}

			case MovementPhase.SUCCESS:
			{
				UpdateSuccessfulHit();
				break;
			}

			case MovementPhase.MISS:
			{
				UpdateUnsuccessfulHit();
				break;
			}

			case MovementPhase.AUTO_PLAY:
			{
				//UpdateMovement();
				UpdateAutoPlay();
				break;
			}

			default:
			{
				break;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool UpdateMovement()
	{
		return BeatPos < -0.25f;
		//if (m_ttMovementTracker.Update())
		//{
		//	transform.localPosition = m_vTargetPosition;
		//	return true;
		//}

		////LocalPosition.x = Mathf.Lerp(m_vStartPosition.x, m_vTargetPosition.x, m_ttMovementTracker.GetCompletionPercentage());
  //      return false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Successful Hit!
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateSuccessfulHit()
	{
		if (m_eSuccessAnimationPhase == SuccessAnimationPhase.TO_SCORE)
		{
			if (m_arToScoreAnimation[m_iCurrentAnimationElement].UpdateAnimation())
			{
				m_iCurrentAnimationElement += 1;
				if (m_iCurrentAnimationElement >= m_arToScoreAnimation.Length)
				{
					m_iCurrentAnimationElement = 0;
					m_eSuccessAnimationPhase = SuccessAnimationPhase.TO_BAG;

					// Update Score
					m_rChallengeModeInfo.IncrementScore();

					// Reset Animations
					for (int i = 0; i < m_arToScoreAnimation.Length; ++i)
						m_arToScoreAnimation[i].Reset();
				}
			}
		}
		else
		{
			if (m_arIntoBagAnimation[m_iCurrentAnimationElement].UpdateAnimation())
			{
				m_iCurrentAnimationElement += 1;
				if (m_iCurrentAnimationElement >= m_arIntoBagAnimation.Length)
				{
					m_eMovementPhase = MovementPhase.IDLE;

					// Reset Animations
					for (int i = 0; i < m_arIntoBagAnimation.Length; ++i)
						m_arIntoBagAnimation[i].Reset();
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Unsuccessful Hit~
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateUnsuccessfulHit()
	{
		if (m_ttMissedMovementTracker.Update())
		{
			SprRend.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
			m_eMovementPhase = MovementPhase.IDLE;
		}
		else
		{
			float timeAdjustment = Time.deltaTime * 10.0f;
			m_fMissedTime += timeAdjustment;
			float xAdjustment = Mathf.Cos(m_fMissedTime) * 0.01f;
			
			float newX = m_rChallengeModeInfo.targetLocation.x + xAdjustment;	// Gradually Moving Left to Right
			float newY = LocalPosition.y - 0.1f * Time.deltaTime;				// Gradually falling

			transform.localPosition = new Vector3(newX, newY, transform.localPosition.z);
			SprRend.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - m_ttMissedMovementTracker.GetCompletionPercentage());
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Autoplay <>
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateAutoPlay()
	{
		float rightOutBoundPos = (m_rChallengeModeInfo.victoryLocation.x + (m_rChallengeModeInfo.separationDifference.x * 0.25f));

		// Now Check for actually being on the VictoryLine
		if(LocalPosition.x < rightOutBoundPos)
		{
			InvokeSuccessfulHit();

			if(m_rChallengeModeInfo.m_rSoundsRhythmMemoryGame != null)
			{
				m_rChallengeModeInfo.m_rSoundsRhythmMemoryGame.PlayNote(m_eSoundType, m_rChallengeModeInfo.MetronomeTime);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Invoke Unsuccessful Hit
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void InvokeUnsuccessfulHit()
	{
		SprRend.sprite = m_sprMissedSprite;
		AudioSourceManager.PlayAudioClip(m_acMissedSound);

		if(!TutorialManager_Base.TutorialOpened)
		{
			m_rChallengeModeInfo.ChallengeScore -= 0.01f;
			m_rChallengeModeInfo.Score -= 0.01f;
			m_rChallengeModeInfo.VisibleScore -= 0.01f;
			m_rChallengeModeInfo.RemoveNote(this);
		}

		m_eMovementPhase = MovementPhase.MISS;
		m_fMissedTime = Random.Range(0.0f, 10.0f);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Invoke Successful Hit
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void InvokeSuccessfulHit()
	{
		SprRend.sprite = m_sprSuccessSprite;
		AudioSourceManager.PlayAudioClip(m_acHitSound);

		m_rChallengeModeInfo.Score += m_rChallengeModeInfo.NoteScore;
		m_rChallengeModeInfo.RemoveNote(this);
		// !!!Change the Score During Success Animation!!! //
		
		m_eSuccessAnimationPhase = SuccessAnimationPhase.TO_SCORE;

		if(m_eMovementPhase == MovementPhase.AUTO_PLAY)
			LocalPosition.x = m_rChallengeModeInfo.victoryLocation.x;
		m_arToScoreAnimation[0].m_vStartingPosition = LocalPosition;
		
		m_eMovementPhase = MovementPhase.SUCCESS;
		m_iCurrentAnimationElement = 0;		
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin AutoPlay
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginAutoPlay()
	{
		m_eMovementPhase = MovementPhase.AUTO_PLAY;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Begin Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginMovement()
	{
		SprRend.sprite = m_sprNormalSprite;
		m_eMovementPhase = MovementPhase.MOVEMENT;
	}

	public void BeginMovement(float beatPosition)
	{
		float saved = m_fBeatPos;
		m_fBeatPos = beatPosition;
		BeatPos = beatPosition;
		BeginMovement();
		m_fBeatPos = saved;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check For Hit
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool CheckForHit()
	{
		const float allowableBeatRange = (1.0f / 32.0f);
		float currentBeatTime = m_rChallengeModeInfo.CurrentBeatPercentage;

		// Position of line to the right of the victory line... plus a little extra to account for float inaccuracies
		float earlyPosition = (m_rChallengeModeInfo.victoryLocation.x + (m_rChallengeModeInfo.separationDifference.x * 1.5f));
		float rightOutBoundPos = (m_rChallengeModeInfo.victoryLocation.x + (m_rChallengeModeInfo.separationDifference.x * 0.5f));
		if (LocalPosition.x < earlyPosition && LocalPosition.x > rightOutBoundPos) // Is Within Range of VictoryLine... Just under the BeatLine to the right of the VL but not currently on the VL?
		{
			// Successful hits are given leeway between 1/16th of the beat ratio. Basically your timing must be either perfect or you get 1/16th of a BPM to get a hit.
			// Since the TimeTracker is counting up with the beat; we need to check if it's within the ending range; So (1.0f - Our Provided Leeway).
			float beatCheck = (1.0f - allowableBeatRange);
			return (currentBeatTime > beatCheck);
		}

		// Do the same check for the left. Now we're checking for lateness.
		//float latePosition = (m_rChallengeModeInfo.victoryLocation.x - (m_rChallengeModeInfo.separationDifference.x * 1.5f));
		float leftOutBoundPos = (m_rChallengeModeInfo.victoryLocation.x - (m_rChallengeModeInfo.separationDifference.x * 0.5f));
		//if (LocalPosition.x > latePosition && LocalPosition.x < leftOutBoundPos)
		//{
		//	return (currentBeatTime < allowableBeatRange);
		//}

		//Now Check for actually being on the VictoryLine
		return (LocalPosition.x >= leftOutBoundPos && LocalPosition.x <= rightOutBoundPos);

		// BeatPos is either on the VictoryLine or the Line immediately to to right (we can handle a minor mistake)? If in a negative, then Note has gone past the victory line...
		//float allowableHitRange = (1.0f + (int)(Mathf.Max(0, (m_rChallengeModeInfo.ChallengeBPM - 100)) / 100.0f));
        //return (BeatPos <= allowableHitRange && BeatPos > -1.0f);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Check Successful Hit
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool CheckSuccessfulHit()
	{
		if(CheckForHit())
		{
			InvokeSuccessfulHit();
			return true;
		}
		return false;
    }

	public bool CheckSuccessfulHit(TambourineSoundsManager.SoundTypes eSoundType)
	{
		return (CheckMatchingSoundType(eSoundType) && CheckSuccessfulHit());
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check if SoundType Matches up to this Note.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool CheckMatchingSoundType(TambourineSoundsManager.SoundTypes eSoundType)
	{
		switch (eSoundType)
		{
			case TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA:
				return m_eSoundType == TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA;

			case TambourineSoundsManager.SoundTypes.MIDDLE_TAMBOURINE_AREA:
				return m_eSoundType == TambourineSoundsManager.SoundTypes.MIDDLE_TAMBOURINE_AREA;

			case TambourineSoundsManager.SoundTypes.OUTER_TAMBOURINE_AREA:
				return m_eSoundType == TambourineSoundsManager.SoundTypes.OUTER_TAMBOURINE_AREA;

			case TambourineSoundsManager.SoundTypes.SOFT_TAMBOURINE_SHAKE: case TambourineSoundsManager.SoundTypes.HARD_TAMBOURINE_SHAKE:
			case TambourineSoundsManager.SoundTypes.MOVE_TAMBOURINE_SOUND: case TambourineSoundsManager.SoundTypes.RATTLE_TAMBOURINE_SHAKE:
				return m_eSoundType == TambourineSoundsManager.SoundTypes.SOFT_TAMBOURINE_SHAKE || m_eSoundType == TambourineSoundsManager.SoundTypes.HARD_TAMBOURINE_SHAKE ||
					   m_eSoundType == TambourineSoundsManager.SoundTypes.MOVE_TAMBOURINE_SOUND || m_eSoundType == TambourineSoundsManager.SoundTypes.RATTLE_TAMBOURINE_SHAKE;

			default:
				return m_eSoundType == TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_1 || m_eSoundType == TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_2 ||
					   m_eSoundType == TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_3 || m_eSoundType == TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_4 ||
					   m_eSoundType == TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_5 || m_eSoundType == TambourineSoundsManager.SoundTypes.CYMBAL_SHAKER_SOUND_6;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enabled
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnEnable()
	{
		Reset();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Reset
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Reset()
	{
		m_eMovementPhase = MovementPhase.IDLE;
		m_fCurrentBeatLine = m_fBeatPos;
		SprRend.sprite = m_sprNormalSprite;


		SpriteRenderer sprRend = GetComponent<SpriteRenderer>();
		if (sprRend != null)
			sprRend.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		if (m_vStartPosition != Vector3.zero)
			transform.localPosition = m_vStartPosition;

		if (m_ttMissedMovementTracker != null)
			m_ttMissedMovementTracker.Reset();
	}







//#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Change Sound Type
	//			Doing all the automatic placements for us.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ChangeSoundType(TambourineSoundsManager.SoundTypes eSoundType, float fBeatPos)
	{
		ChangeSpriteInfo(eSoundType);

		// Move This notation to the specified Beat Position
		if (m_rChallengeModeInfo != null)
		{
			m_fBeatPos = fBeatPos;
			float newX = m_rChallengeModeInfo.victoryLocation.x + m_rChallengeModeInfo.separationDifference.x * fBeatPos;
			transform.localPosition = new Vector3(newX, transform.localPosition.y, transform.localPosition.z);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Change Sprite Information
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ChangeSpriteInfo(TambourineSoundsManager.SoundTypes eNewSoundType)
	{
		m_eSoundType = eNewSoundType;

		switch (eNewSoundType)
		{
			case TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA:
				ChangeSpriteInfo(m_sprRedAreaNormal);
				break;
			case TambourineSoundsManager.SoundTypes.MIDDLE_TAMBOURINE_AREA:
				ChangeSpriteInfo(m_sprBlueAreaNormal);
				break;
			case TambourineSoundsManager.SoundTypes.OUTER_TAMBOURINE_AREA:
				ChangeSpriteInfo(m_sprYellowAreaNormal);
				break;
			case TambourineSoundsManager.SoundTypes.SOFT_TAMBOURINE_SHAKE:
			case TambourineSoundsManager.SoundTypes.HARD_TAMBOURINE_SHAKE:
			case TambourineSoundsManager.SoundTypes.MOVE_TAMBOURINE_SOUND:
			case TambourineSoundsManager.SoundTypes.RATTLE_TAMBOURINE_SHAKE:
				ChangeSpriteInfo(m_sprShakeNormal);
				break;
			default:
				ChangeSpriteInfo(m_sprCymbalNormal);
				break;
		}
	}

	private void ChangeSpriteInfo(Sprite newNormSpr)
	{
		// Changing to a Nothing (Null)? Remove the name
		if (newNormSpr == null)
		{
			transform.name = transform.name.Replace(ChangePositionAndSpriteInformation(m_sprNormalSprite.name).Key, "");
			m_sprNormalSprite = null;
		}

		else
		{
			// Were we a null Sprite before this? Just extend the name and place where needed.
			if (m_sprNormalSprite == null)
			{
				KeyValuePair<string, float> newInfo = ChangePositionAndSpriteInformation(newNormSpr.name);
				m_sprNormalSprite = newNormSpr;
				transform.localPosition = new Vector3(transform.localPosition.x, newInfo.Value, transform.localPosition.z);
				transform.name += newInfo.Key;
			}

			// Then in that case Remove the previous extension name we gave it and replace it with the new one. Also Change its position to the appropriate line
			else
			{
				string prevName = ChangePositionAndSpriteInformation(m_sprNormalSprite.name).Key;
				KeyValuePair<string, float> newInfo = ChangePositionAndSpriteInformation(newNormSpr.name);
				m_sprNormalSprite = newNormSpr;
				transform.localPosition = new Vector3(transform.localPosition.x, newInfo.Value, transform.localPosition.z);

				if (transform.name.Contains(prevName))
				{
					transform.name = transform.name.Replace(prevName, newInfo.Key);
				}
				else
				{
					transform.name += newInfo.Key;
				}
			}
		}

		// Change the Sprite in the Renderer to match!
		SpriteRenderer sprRend = GetComponent<SpriteRenderer>();
		if (sprRend != null)
			sprRend.sprite = m_sprNormalSprite;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Change Position and Sprite Information
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private KeyValuePair<string, float> ChangePositionAndSpriteInformation(string name)
	{
		string newName = "";
		if (name == "(Challenge Game) Blue Area Icon (Not Triggered)")
		{
			if (m_rChallengeModeInfo != null)
				m_fLinePos = m_rChallengeModeInfo.blueAreaLinePos;
			newName = "~ (Blue)";
			m_sprSuccessSprite = m_sprBlueAreaHighlight;
			m_sprMissedSprite = m_sprAreaSymbolMissed;
		}
		else if (name == "(Challenge Game) Red Area Icon (Not Triggered)")
		{
			if (m_rChallengeModeInfo != null)
				m_fLinePos = m_rChallengeModeInfo.redAreaLinePos;
			newName = "~ (Red)";
			m_sprSuccessSprite = m_sprRedAreaHighlight;
			m_sprMissedSprite = m_sprAreaSymbolMissed;
		}
		else if (name == "(Challenge Game) Yellow Area Icon (Not Triggered)")
		{
			if (m_rChallengeModeInfo != null)
				m_fLinePos = m_rChallengeModeInfo.yellowAreaLinePos;
			newName = "~ (Yellow)";
			m_sprSuccessSprite = m_sprYellowAreaHighlight;
			m_sprMissedSprite = m_sprAreaSymbolMissed;
		}
		else if (name == "(Challenge Game) Shake Icon (Not Triggered)")
		{
			if (m_rChallengeModeInfo != null)
				m_fLinePos = m_rChallengeModeInfo.cymbalLinePos;
			newName = "~ (Shake)";
			m_sprSuccessSprite = m_sprShakeHighlight;
			m_sprMissedSprite = m_sprShakeMissed;
		}
		else
		{
			if (m_rChallengeModeInfo != null)
				m_fLinePos = m_rChallengeModeInfo.shakeLinePos;
			newName = "~ (Cymbal)";
			m_sprSuccessSprite = m_sprCymbalHighlight;
			m_sprMissedSprite = m_sprCymbalMissed;
		}


		// /////
		if (m_rChallengeModeInfo != null)
		{
			float newY = m_rChallengeModeInfo.victoryLocation.y + m_rChallengeModeInfo.separationDifference.y * m_fLinePos;
			return new System.Collections.Generic.KeyValuePair<string, float>(newName, newY);
		}
		else
		{
			return new System.Collections.Generic.KeyValuePair<string, float>(newName, 0.0f);
		}
	}
//#endif
}
