//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - URL Opener
//             Author: Christopher Diamond
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    A custom editor is used to add additional functionality to the Unity 
//		inspector when dealing with the aforementioned class data.
//
//	  This includes the addition of adding in buttons or calling a method when a 
//		value is changed.
//	  Most importantly, a custom editor is used to make the inspector more 
//		readable and easier to edit.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Button_PlaySheetMusic))]
public class CustomEditor_ButtonPlaySheetMusic : CustomEditor_ButtonBase<Button_PlaySheetMusic>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script handles the processing for when the player hits the \"Play\"\n" +
					"button on the Sheet Music. This script, armed with an editable reference\n" +
					"to the specified track playlist, interacts with the Manager for the RhythmGame\n" +
					"and begins the game.";
		}
	}




	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawNonSpriteOptions(false);
		AddSpaces(2);
		Target.m_rChallengeGameMan	= DrawObjectOption("Challenge Mode Ref: ", Target.m_rChallengeGameMan, "Reference to the Challenge Manager. If Assigned, this button will act as the \"Begin Example\" Button, which shows an example of the challenge before the user attempts it themselves");
		if(Target.m_rChallengeGameMan == null)
		{
			Target.m_rRhythmMemoryGame = DrawObjectOption("Rhythm Memory Game Ref: ", Target.m_rRhythmMemoryGame, "Reference to the Rhythm Memory Game which will be started when this button is clicked on");
			Target.m_rNotesManager = DrawObjectOption("Notes Ref: ", Target.m_rNotesManager, "When playing the RhythmGame, this note will hide during the meantime and return once finished");
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		if(Target.m_rChallengeGameMan == null)
		{
			Target.m_ePlaylist = (SoundsRhythmMemoryGame.Playlist)EditorGUILayout.EnumPopup(new GUIContent("Assigned Playlist: ", "Which Playlist will be played via the Rhythm Game if this button is clicked on?"), Target.m_ePlaylist);
		}
		else
		{
			DrawLabel("No Editable Values available since a Challenge Mode has been assigned", Color.red);
		}
	}
}