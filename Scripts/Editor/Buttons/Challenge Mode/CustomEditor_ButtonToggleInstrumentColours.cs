//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Toggle Instrument Colours
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

[CustomEditor(typeof(Button_ToggleInstrumentColours))]
public class CustomEditor_ButtonToggleInstrumentColours : CustomEditor_ButtonBase<Button_ToggleInstrumentColours>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This button, when clicked, will toggle the Instrument Colours on/off.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawNonSpriteOptions();
		AddSpaces(1);
		Target.m_rInstrumentManager = DrawObjectOption("Instrument Manager:", Target.m_rInstrumentManager, "Reference to the Instrument Manager, which has a reference to the \"Current\" Colours Manager");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Multi-Language Text Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawMultiLanguageTextOptions()
	{
		Target.m_oShowColoursText.dm_rTextRenderer = Target.TextRenderer;
		Target.m_oHideColoursText.dm_rTextRenderer = Target.TextRenderer;

		DrawLabel("Show Colours Text:", EditorStyles.boldLabel);
		DrawMultiLanguageText(Target.m_oShowColoursText, GameManager.SystemLanguages.TOTAL_LANGUAGES);

		DrawLabel("Hide Colours Text:", EditorStyles.boldLabel);
		DrawMultiLanguageText(Target.m_oHideColoursText, GameManager.SystemLanguages.TOTAL_LANGUAGES);
	}
}