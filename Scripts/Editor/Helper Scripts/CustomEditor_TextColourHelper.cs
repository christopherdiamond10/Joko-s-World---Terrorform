//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Text Colour Helper
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
using System.Text.RegularExpressions;

[CustomEditor(typeof(TextColourHelper))]
public class CustomEditor_TextColourHelper : CustomEditor_Base<TextColourHelper>
{
	string m_sArabicTextToConvert = "";
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override InspectorRegion[] AdditionalRegions
	{
		get { return new InspectorRegion[] { new InspectorRegion() { label = "~Text Helper~ (Helping to Colour Text)", representingDrawMethod = DrawTextHelperOptions } }; }
	}

	private string ColourCode
	{
		get
		{
			int r = Mathf.CeilToInt(Target.dm_cChosenColour.r * 255.0f);
			int g = Mathf.CeilToInt(Target.dm_cChosenColour.g * 255.0f);
			int b = Mathf.CeilToInt(Target.dm_cChosenColour.b * 255.0f);
			int a = Mathf.CeilToInt(Target.dm_cChosenColour.a * 255.0f);
			if(a == 255) // No need to give Alpha Colour if no difference
				return (r.ToString("X2") + g.ToString("X2") + b.ToString("X2"));
			else
				return (r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + a.ToString("X2"));
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawTextHelperOptions()
	{
		if(Target.dm_rAssociatedTextRenderer == null)
			Target.dm_rAssociatedTextRenderer = Target.GetComponent<UnityEngine.UI.Text>();

		Target.dm_rAssociatedTextRenderer = DrawObjectOption("Text Renderer:", Target.dm_rAssociatedTextRenderer, "The text renderer to apply values to");
		if(Target.dm_rAssociatedTextRenderer != null)
		{
			AddSpaces(2);
			Target.dm_sHighlightedText = EditorGUILayout.TextField(new GUIContent("Highlighting Text:", "The text that will be changing colours using the colour option below. Do not insert any colour tags already defined; just the text that should be highlighted"), Target.dm_sHighlightedText);
			EditorGUI.indentLevel += 2;
			Color newColour = EditorGUILayout.ColorField(new GUIContent("Highlighting Colour:", "The colour which this text will be highlited with. Also changes the Colour Code below"), Target.dm_cChosenColour);
			EditorGUI.indentLevel -= 2;
			if(newColour != Target.dm_cChosenColour)
			{
				Target.dm_cChosenColour = newColour;
				if(Target.dm_sHighlightedText != "")
				{
					string regexPattern = @"(?:<color=#[0-9a-zA-Z]+>)?" + Target.dm_sHighlightedText + @"(?:</color>)?";
					string replacementText = "<color=#" + ColourCode + ">" + Target.dm_sHighlightedText + "</color>";
					Target.dm_rAssociatedTextRenderer.text = Regex.Replace(Target.dm_rAssociatedTextRenderer.text, regexPattern, replacementText, RegexOptions.IgnoreCase);
				}
			}

			EditorGUI.indentLevel += 3;
			EditorGUILayout.TextField(new GUIContent("Color Code:", "The Colour Code that can be used to change text colour"), ColourCode);
			EditorGUI.indentLevel -= 3;

			AddSpaces(3);

			EditorGUILayout.LabelField("Arabic Text Support:");
			m_sArabicTextToConvert = EditorGUILayout.TextField(new GUIContent("Arabic Text To Convert:", "Input the Arabic Text you wish to convert into proper arabic text"), m_sArabicTextToConvert);
			if(m_sArabicTextToConvert != "")
			{
				EditorGUI.indentLevel += 3;
				EditorGUILayout.TextField(new GUIContent("Converted Text (ArbSup):", "Text converted into proper arabic"), ArabicSupport.ArabicFixer.Fix(m_sArabicTextToConvert));
				EditorGUILayout.TextField(new GUIContent("Converted Text (RTL):", "Text converted into proper arabic"), RTLService.RTL.Convert(m_sArabicTextToConvert));
				EditorGUI.indentLevel -= 3;
			}
        }
	}
}
