//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Tutorial Manager - Guide Book Area
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

[CustomEditor(typeof(TutorialManager_GuideBookArea))]
public class CustomEditor_TutorialManagerGuideBookArea : CustomEditor_TutorialManager_Base<TutorialManager_GuideBookArea>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "Deriving from the TutorialManager_Base class, this specific tutorial is\n" +
					"made and intended for use by the Guide Book Area. This includes\n" +
					"handling situations that users will find themselves in such as\n" +
					"Moving the pages and reizing.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		base.DrawInspectorOptions();
		AddSpaces(2);
		Target.m_rCulturalNotesPage1				= DrawObjectOption("Cultural Notes Page 1 Ref: ", Target.m_rCulturalNotesPage1, "Reference to the first page of the Cultural Notes. This will have blocks applied to it so that it can be used in the tutorial without having the user have the ability to go out of parameters and screwing up the tutorial");
		Target.m_rCulturalNotesPage2				= DrawObjectOption("Cultural Notes Page 2 Ref: ", Target.m_rCulturalNotesPage2, "Reference to the second page of the Cultural Notes. This will have blocks applied to it so that it can be used in the tutorial without having the user have the ability to go out of parameters and screwing up the tutorial");
		Target.m_goCulturalNotesNextNoteArrowButton = DrawObjectOption("Cultural Notes Page 1 'Next CulturalNote' Arrow", Target.m_goCulturalNotesNextNoteArrowButton, "Reference to the arrow/button which moves on to the next Cultural Note... the one used with Cultural Note Page 1");
		AddSpaces(1);
		Target.m_rGuideBookScene					= DrawObjectOption("Guide Book Scene Ref: ", Target.m_rGuideBookScene, "After the Tutorial is finished it will request that the GuideBook Subscene re-open itself; to save the user the hassle of doing so themselves");
	}
}