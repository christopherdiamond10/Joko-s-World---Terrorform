// *******************************************************
// Copyright 2013 Daikon Forge, all rights reserved under 
// US Copyright Law and international treaties
// *******************************************************
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//		Modified By Christopher Diamond. 
//
//	-	Updating original script to reflect new Unity SceneManager Code.
//	-	Additionally Changing a forced save to a choice dialogue box asking the
//			User on play whether they wish to save the modifed scene or not.
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class Utility_SaveSceneOnPlay 
{
	static Utility_SaveSceneOnPlay()
	{

		EditorApplication.playmodeStateChanged = () =>
		{

			if( EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying )
			{
				
				Debug.Log( "Auto-Saving scene before entering Play mode: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

				if(UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes())
				{
					EditorApplication.SaveAssets();
				}
			}

		};

	}
}
