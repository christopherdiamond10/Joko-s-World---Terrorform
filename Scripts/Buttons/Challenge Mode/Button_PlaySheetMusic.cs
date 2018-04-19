//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Play Sheet Music
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: April 16, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script handles the processing for when the player hits the "Play" 
//		button on the Sheet Music. This script, armed with an editable reference
//		to the specified track playlist, interacts with the Manager for the RhythmGame
//		and begins the game. 
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_PlaySheetMusic : Button_Base
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public SoundsRhythmMemoryGame m_rRhythmMemoryGame;
    public MultipleNotesManager m_rNotesManager;
    public ChallengeGameManager m_rChallengeGameMan;
    public SoundsRhythmMemoryGame.Playlist m_ePlaylist = SoundsRhythmMemoryGame.Playlist.CHALLENGE_01;



    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Derived Method: On Trigger
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected override void OnTrigger()
    {
		if ((m_rRhythmMemoryGame != null && m_rRhythmMemoryGame.IsPlaying) || (m_rChallengeGameMan != null && m_rChallengeGameMan.Active))
			StopRhythmGame();
		else
			StartRhythmGame();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Start Rhythm Game
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StartRhythmGame()
	{
		if (m_rChallengeGameMan != null)
		{
			m_rChallengeGameMan.BeginChallengePractice();
		}
		else
		{
			m_rRhythmMemoryGame.PlayThroughRhythmList(m_ePlaylist, m_rNotesManager);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Stop Rhythm Game
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void StopRhythmGame()
	{
		if (m_rChallengeGameMan != null)
		{
			m_rChallengeGameMan.StopChallenge();
		}
		else
		{
			m_rRhythmMemoryGame.StopPlayback();
        }
	}
}