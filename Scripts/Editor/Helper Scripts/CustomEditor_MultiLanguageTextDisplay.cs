//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Mul-Language Text Display
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

[CustomEditor(typeof(MultiLanguageTextDisplay))]
public class CustomEditor_MultiLanguageTextDisplay : CustomEditor_Base<MultiLanguageTextDisplay>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script keeps a saved copy of all the multiple languages that can be\n" +
					"selected in the app and interacts with the Game Manager to determine which\n" +
					"is the appropriate text in it's list of languages to display.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Multi-Language Text Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void  DrawMultiLanguageTextOptions()
	{
		Target.m_oMultiLanguageText.dm_rTextRenderer = Target.GetComponent<UnityEngine.UI.Text>();
        DrawMultiLanguageText(Target.m_oMultiLanguageText, GameManager.SystemLanguages.TOTAL_LANGUAGES);
		if (Target.m_oMultiLanguageText.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text == "")
		{
			Target.m_oMultiLanguageText.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text = Target.m_oMultiLanguageText.dm_rTextRenderer.text;
			Target.m_oMultiLanguageText.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].fontSize = Target.m_oMultiLanguageText.dm_rTextRenderer.fontSize;
        }
	}
}