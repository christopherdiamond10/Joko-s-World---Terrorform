//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Button - Settings Menu Toggle
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

[CustomEditor(typeof(Button_SettingsMenuToggle))]
public class CustomEditor_ButtonSettingsMenuToggle : CustomEditor_ButtonBase<Button_SettingsMenuToggle>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		DrawNonSpriteOptions(false);

		AddSpaces(2);

		Target.m_rMenuTextLabel = DrawObjectOption("Menu Text Label Reference:", Target.m_rMenuTextLabel, "The Label on the Menu Button which will be change depending on game circumstances");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Multi-Language Text Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawMultiLanguageTextOptions()
	{
		if (Target.m_rMenuTextLabel != null)
		{
			DrawLabel("Menu Text:", EditorStyles.boldLabel);
			Target.m_oMenuLabelDisplay.dm_rTextRenderer = Target.m_rMenuTextLabel;
			DrawMultiLanguageText(Target.m_oMenuLabelDisplay, GameManager.SystemLanguages.TOTAL_LANGUAGES);

			AddSpaces(4);

			DrawLabel("Exit Text:", EditorStyles.boldLabel);
			Target.m_oExitLabelDisplay.dm_rTextRenderer = Target.m_rMenuTextLabel;
			DrawMultiLanguageText(Target.m_oExitLabelDisplay, GameManager.SystemLanguages.TOTAL_LANGUAGES);
		}
	}
}