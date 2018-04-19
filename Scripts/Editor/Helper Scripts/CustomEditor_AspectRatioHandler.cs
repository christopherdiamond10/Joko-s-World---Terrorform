//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Aspect Ratio Handler
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
using UnityEditor;

[CustomEditor(typeof(AspectRatioHandler))]
public class CustomEditor_AspectRatioHandler : CustomEditor_Base<AspectRatioHandler> 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script discovers the current Aspect Ratio of the Device and attempts\n" +
					"to resize the app to fit the given dimensions. Dimensions, which, we did\n" +
					"develop primarily for.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rSceneTransform = DrawObjectOption("Scene Transform", Target.m_rSceneTransform, "Reference to the Parent Holding the Game Scenes which can be resized and affect all game scalability.");
		Target.m_rDebugTextRenderer = DrawObjectOption("Debug Text Renderer:", Target.m_rDebugTextRenderer, "Reference to a text renderer which will show what the current aspect ratio is. This can be left blank and is not required");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		EditorGUILayout.LabelField("When 16:9 Aspect Ratio:", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			Target.m_v16By09Position = EditorGUILayout.Vector3Field(new GUIContent("Position:", "Where should the Scene Position be in a 16:9 Aspect Ratio Situation?"), Target.m_v16By09Position);
			Target.m_v16By09Scale = EditorGUILayout.Vector3Field(new GUIContent("Scale:", "What should the Scene Size be in a 16:9 Aspect Ratio Situation?"), Target.m_v16By09Scale);
		}
		EditorGUI.indentLevel -= 1;

		AddSpaces(3);

		EditorGUILayout.LabelField("When 16:10 Aspect Ratio:", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			Target.m_v16By10Position = EditorGUILayout.Vector3Field(new GUIContent("Position:", "Where should the Scene Position be in a 16:10 Aspect Ratio Situation?"), Target.m_v16By10Position);
			Target.m_v16By10Scale = EditorGUILayout.Vector3Field(new GUIContent("Scale:", "What should the Scene Size be in a 16:10 Aspect Ratio Situation?"), Target.m_v16By10Scale);
		}
		EditorGUI.indentLevel -= 1;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Debug Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawDebugOptions()
	{
		if(Target.m_rSceneTransform != null)
		{
			if(GUILayout.Button("Show 16:9 UI"))
			{
				Target.m_rSceneTransform.localPosition = Target.m_v16By09Position;
				Target.m_rSceneTransform.localScale = Target.m_v16By09Scale;
			}

			if(GUILayout.Button("Show 16:10 UI"))
			{
				Target.m_rSceneTransform.localPosition = Target.m_v16By10Position;
				Target.m_rSceneTransform.localScale = Target.m_v16By10Scale;
			}
		}
	}
}
