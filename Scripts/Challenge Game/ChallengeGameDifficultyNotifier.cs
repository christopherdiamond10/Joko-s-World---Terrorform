//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Challenge Game Difficulty Notifier
//             Author: Christopher Diamond
//             Date: September 25, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script simply changes the difficulty notifier for the challenge mode.
//		Specific challenges interact with this script to change the difficulty
//		notification based on how difficult they are.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ChallengeGameDifficultyNotifier : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public SpriteRenderer[] m_asprRendNotifiers = new SpriteRenderer[3];

	public Sprite m_sprNonFilledDifficulty;
	public Sprite m_sprFilledDifficulty;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum DifficultyLevel
	{
		LEVEL_ONE__EASY,
		LEVEL_TWO__MEDIUM,
		LEVEL_THREE__HARD,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Change Difficulty
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ChangeDifficulty(DifficultyLevel eDifficulty)
	{
		switch (eDifficulty)
		{
			case DifficultyLevel.LEVEL_ONE__EASY:
				m_asprRendNotifiers[0].sprite = m_sprFilledDifficulty;
				m_asprRendNotifiers[1].sprite = m_sprNonFilledDifficulty;
				m_asprRendNotifiers[2].sprite = m_sprNonFilledDifficulty;
				break;
			case DifficultyLevel.LEVEL_TWO__MEDIUM:
				m_asprRendNotifiers[0].sprite = m_sprFilledDifficulty;
				m_asprRendNotifiers[1].sprite = m_sprFilledDifficulty;
				m_asprRendNotifiers[2].sprite = m_sprNonFilledDifficulty;
				break;
			default:
				m_asprRendNotifiers[0].sprite = m_sprFilledDifficulty;
				m_asprRendNotifiers[1].sprite = m_sprFilledDifficulty;
				m_asprRendNotifiers[2].sprite = m_sprFilledDifficulty;
				break;
		}
	}
}
