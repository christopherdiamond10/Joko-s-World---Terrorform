//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Tambourine Targets Manager
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

[CustomEditor(typeof(TambourineTargetsManager))]
public class CustomEditor_TambourineTargetsManager : CustomEditor_Base<TambourineTargetsManager>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script manages the targets on the Tambourine. Revealing the separate\n" +
					"targets to other scripts so that they can be edited without having multiple\n" +
					"scripts give conflicting information about them.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		for(int i = 0; i < Target.m_arTambourineTargets.Length; ++i)
			if(Target.m_arTambourineTargets[i] == null)
				Target.m_arTambourineTargets[i] = new TambourineTargetsManager.TambTargetInfo();

		Target.m_sprShakenTambourineTargets = DrawObjectOption("Shaken Tambourine Sprite: ", Target.m_sprShakenTambourineTargets, "Image to show when the tambourine is shaken");
		Target.m_arTambourineTargets[0].sprColouredTambourineTarget	= DrawObjectOption("Red (Center) Target Sprite Renderer: ", Target.m_arTambourineTargets[0].sprColouredTambourineTarget, "Red Tambourine Target Renderer");
		Target.m_arTambourineTargets[1].sprColouredTambourineTarget = DrawObjectOption("Blue (Middle) Target Sprite: ", Target.m_arTambourineTargets[1].sprColouredTambourineTarget, "Blue Tambourine Target Renderer");
		Target.m_arTambourineTargets[2].sprColouredTambourineTarget = DrawObjectOption("Green (Outer) Target Sprite: ", Target.m_arTambourineTargets[2].sprColouredTambourineTarget, "Green Tambourine Target Renderer");

		AddSpaces(2);

		Target.m_arTambourineTargets[0].sprHighlightedTambourineTarget = DrawObjectOption("Red (Center) Highlighted Target Sprite Renderer: ", Target.m_arTambourineTargets[0].sprHighlightedTambourineTarget, "Highlighted-Red Tambourine Target Renderer");
		Target.m_arTambourineTargets[1].sprHighlightedTambourineTarget = DrawObjectOption("Blue (Middle) Highlighted Target Sprite: ", Target.m_arTambourineTargets[1].sprHighlightedTambourineTarget, "Highlighted-Blue Tambourine Target Renderer");
        Target.m_arTambourineTargets[2].sprHighlightedTambourineTarget = DrawObjectOption("Green (Outer) Highlighted Target Sprite: ", Target.m_arTambourineTargets[2].sprHighlightedTambourineTarget, "Highlighted-Green Tambourine Target Renderer");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		Target.m_fTargetHighlightTime = EditorGUILayout.FloatField(new GUIContent("Highlight Time for Targets: ", "How long will it take for the Targets to fadeout when the user taps on a drum area"), Target.m_fTargetHighlightTime);
	}
}
