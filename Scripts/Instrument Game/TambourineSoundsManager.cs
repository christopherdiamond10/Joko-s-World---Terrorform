//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Tambourine Sounds Manager
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: February 12, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script manages the Tambourine sounds and their playback. This is heavily
//		used in conjunction with the SoundBank and SoundRhythmGame.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;




//===========================================================================
// *** TAMBOURINE SOUNDS MANAGER
//===========================================================================
public class TambourineSoundsManager : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public InstrumentManager m_rInstrumentManager;

	// SoundBank
	public AudioClip[] m_aacNormalTambourineSounds		= new AudioClip[(int)SoundTypes.RATTLE_TAMBOURINE_SHAKE + 1];
	public AudioClip[] m_aacPandeiroTambourineSounds	= new AudioClip[(int)SoundTypes.RATTLE_TAMBOURINE_SHAKE + 1];
	public AudioClip[] m_aacKanjiraTambourineSounds		= new AudioClip[(int)SoundTypes.RATTLE_TAMBOURINE_SHAKE + 1];
	public AudioClip[] m_aacRiqTambourineSounds			= new AudioClip[(int)SoundTypes.RATTLE_TAMBOURINE_SHAKE + 1];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum SoundTypes
	{
		CENTER_TAMBOURINE_AREA,
		MIDDLE_TAMBOURINE_AREA,
		OUTER_TAMBOURINE_AREA,
		CYMBAL_SHAKER_SOUND_1,
		CYMBAL_SHAKER_SOUND_2,
		CYMBAL_SHAKER_SOUND_3,
		CYMBAL_SHAKER_SOUND_4,
		CYMBAL_SHAKER_SOUND_5,
		CYMBAL_SHAKER_SOUND_6,
		SOFT_TAMBOURINE_SHAKE,
		HARD_TAMBOURINE_SHAKE,
		MOVE_TAMBOURINE_SOUND,
		RATTLE_TAMBOURINE_SHAKE,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void Start()
	{	
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Get Tambourine Sound
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public AudioClip GetTambourineSound(SoundTypes eSoundType)
    {
        return GetTambourineSound(m_rInstrumentManager.CurrentInstrumentMode, eSoundType);
    }

    public AudioClip GetTambourineSound(InstrumentManager.InstrumentMode eTambourineType, SoundTypes eSoundType)
	{
		// Full Version of App!
		//if(GameManager.IsFullVersion)
		//{
			switch (eTambourineType)
			{
				case InstrumentManager.InstrumentMode.NORMAL_TAMBOURINE:	return m_aacNormalTambourineSounds[(int)eSoundType];
				case InstrumentManager.InstrumentMode.PANDEIRO_TAMBOURINE:	return m_aacPandeiroTambourineSounds[(int)eSoundType];
				case InstrumentManager.InstrumentMode.KANJIRA_TAMBOURINE:	return m_aacKanjiraTambourineSounds[(int)eSoundType];
				default:													return m_aacRiqTambourineSounds[(int)eSoundType];
			}
		//}

		// Lite Version Only!
		//return m_aacRiqTambourineSounds[(int)eSoundType];
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Play Tambourine Sound
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public int PlayTambourineSound(SoundTypes eSoundType)
	{
		return PlayTambourineSound(m_rInstrumentManager.CurrentInstrumentMode, eSoundType);
    }

	public int PlayTambourineSound(InstrumentManager.InstrumentMode eTambourineType, SoundTypes eSoundType)
	{
		return AudioSourceManager.PlayAudioClip( GetTambourineSound(eTambourineType, eSoundType) );
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Highlight Sound Object
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void HighlightSoundObject(SoundTypes eSound)
	{
		switch(eSound)
		{
			case SoundTypes.CENTER_TAMBOURINE_AREA:
				m_rInstrumentManager.InstrumentColoursManager.ShowCenterTambourineTarget();
				break;
			case SoundTypes.MIDDLE_TAMBOURINE_AREA:
				m_rInstrumentManager.InstrumentColoursManager.ShowMiddleTambourineTarget();
				break;
			case SoundTypes.OUTER_TAMBOURINE_AREA:
				m_rInstrumentManager.InstrumentColoursManager.ShowOuterTambourineTarget();
				break;

			case SoundTypes.CYMBAL_SHAKER_SOUND_1: case SoundTypes.CYMBAL_SHAKER_SOUND_2: case SoundTypes.CYMBAL_SHAKER_SOUND_3:
			case SoundTypes.CYMBAL_SHAKER_SOUND_4: case SoundTypes.CYMBAL_SHAKER_SOUND_5: case SoundTypes.CYMBAL_SHAKER_SOUND_6:
				m_rInstrumentManager.CurrentAutoPlayCymbal.StartCymbalShake();
				break;

			case SoundTypes.MOVE_TAMBOURINE_SOUND: case SoundTypes.RATTLE_TAMBOURINE_SHAKE:
			case SoundTypes.SOFT_TAMBOURINE_SHAKE: case SoundTypes.HARD_TAMBOURINE_SHAKE:
				m_rInstrumentManager.ShowSpecialInstrumentState();
				break;

			default:
				break;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Revert Sound Object
	//----------------------------------------------------
	// Called after a sound in the playlist has been played.
	// Makes the other sounds stop showing visual cues
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void RevertSoundObject(SoundTypes eSound)
	{
		switch(eSound)
		{
			case SoundTypes.CENTER_TAMBOURINE_AREA:
				m_rInstrumentManager.InstrumentColoursManager.HideCenterTambourineTarget();
				break;
			case SoundTypes.MIDDLE_TAMBOURINE_AREA:
				m_rInstrumentManager.InstrumentColoursManager.HideMiddleTambourineTarget();
				break;
			case SoundTypes.OUTER_TAMBOURINE_AREA:
				m_rInstrumentManager.InstrumentColoursManager.HideOuterTambourineTarget();
				break;

			case SoundTypes.CYMBAL_SHAKER_SOUND_1: case SoundTypes.CYMBAL_SHAKER_SOUND_2: case SoundTypes.CYMBAL_SHAKER_SOUND_3:
			case SoundTypes.CYMBAL_SHAKER_SOUND_4: case SoundTypes.CYMBAL_SHAKER_SOUND_5: case SoundTypes.CYMBAL_SHAKER_SOUND_6:
				m_rInstrumentManager.CurrentAutoPlayCymbal.StopCymbalShake();
				break;

			case SoundTypes.MOVE_TAMBOURINE_SOUND: case SoundTypes.RATTLE_TAMBOURINE_SHAKE:
			case SoundTypes.SOFT_TAMBOURINE_SHAKE: case SoundTypes.HARD_TAMBOURINE_SHAKE:
				m_rInstrumentManager.ShowNormalInstrumentState();
				break;

			default:
				break;
		}	
	}
}
