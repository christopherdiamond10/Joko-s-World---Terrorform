//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Title Screen Animation
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

[CustomEditor(typeof(TitleScreenAnimation))]
public class CustomEditor_TitleScreenAnimation : CustomEditor_Base<TitleScreenAnimation> 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script is manages the entire title screen introduction sequence.\n" +
					"Including wait times in between sequences and the consistent updates\n" +
					"for those individual sequences.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawArrayOptions("Sprites To Preload:", "m_arSpritesToPreload", "Sprites inserted into this array will be loaded as soon as the game starts. This means that the sprites in question will not have to be reloaded later; hence never having to temporarily freze the game in order to load");
		AddSpaces(2);

		Target.m_srCulturalInfusionLogo		= DrawObjectOption("Cultural Infusion Logo: ",		Target.m_srCulturalInfusionLogo,		"Reference to the Cultural Infusion Logo SpriteRenderer");
		Target.m_srBackgroundSpriteRenderer	= DrawObjectOption("Background LandscapeSprite: ",	Target.m_srBackgroundSpriteRenderer,	"Reference to the Background Landscape SpriteRenderer");
		Target.m_goJokosWorld				= DrawObjectOption("Jokos World Title: ",			Target.m_goJokosWorld,					"Reference to the Joko's World Title Logo GameObject");
		Target.m_goGameTitle				= DrawObjectOption("Game Title: ",					Target.m_goGameTitle,					"Reference to the Game Title Logo GameObject");
		Target.m_goJokosDance				= DrawObjectOption("Jokos Dance: ",					Target.m_goJokosDance,					"Reference to the Joko's Dance/Introduction GameObject");
		Target.m_goPlayButton				= DrawObjectOption("Play Button: ",					Target.m_goPlayButton,					"Reference to the Play Button GameObject");
		Target.m_goCreditsButton			= DrawObjectOption("Credits Button: ",				Target.m_goCreditsButton,				"Reference to the Credits Button GameObject");

		AddSpaces(1);
		DrawArrayOptions("Game Activation Objects: ", "m_agoGameActivationObjectsToEnable", "Which objects should be enabled once the Title Screen Introduction sequence is completed?");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_fLandscapeViewWait			 = EditorGUILayout.FloatField(new GUIContent("Landscape View Wait Time: ", "How long will we look at the Landscape before starting to move it?"), Target.m_fLandscapeViewWait);
		Target.m_fLandscapeViewAfterMoveWait = EditorGUILayout.FloatField(new GUIContent("Landscape After Move Wait Time:  ", "How long will we look at the Landscape after moving before bluring it?"), Target.m_fLandscapeViewAfterMoveWait);
		Target.m_fLandscapeBlurTime			 = EditorGUILayout.FloatField(new GUIContent("Landscape Blur Time: ", "How long will it take to blur the Landscape?"), Target.m_fLandscapeBlurTime);
		Target.m_fJokosWorldWaitTime		 = EditorGUILayout.FloatField(new GUIContent("Wait Time after Joko's World is Revealed: ", "How long will we look at the Joko's World Title Logo before showing the game title?"), Target.m_fJokosWorldWaitTime);
		Target.m_fJokosFluteWaitTime		 = EditorGUILayout.FloatField(new GUIContent("Wait Time after Game Title is Revealed: ", "How long will we look at game title before showing Joko's Dance?"), Target.m_fJokosFluteWaitTime);
		Target.m_fJokosDanceWaitTime		 = EditorGUILayout.FloatField(new GUIContent("Wait Time after Joko's Dance is Revealed: ", "How long will we watch Joko's Dance before showing the Play/Credits Button?"), Target.m_fJokosDanceWaitTime);
		Target.m_fPlayButtonWaitTime		 = EditorGUILayout.FloatField(new GUIContent("Wait Time to display Play/Credits Button: ", "How long will it take to fade in the Play/Credits Buttons?"), Target.m_fPlayButtonWaitTime);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimationOptions()
	{
		if (Target.m_srBackgroundSpriteRenderer != null)
		{
			EditorGUILayout.LabelField("Landscape Movement Animation", EditorStyles.boldLabel);
			DrawAnimationEffectOptions(ref Target.m_aLandscapeAnimation, Target.m_srBackgroundSpriteRenderer.transform);

			AddSpaces(6);
		}
	}
}
