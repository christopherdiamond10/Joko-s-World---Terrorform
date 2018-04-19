//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Multi Language Text File Handler
//             Author: Christopher Diamond
//             Date: February 08, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script acts as a middle man to produce excel files from the Multi-Language
//		Text Components and then reads those same excel files back in and auto-fills
//		language data once the translators are finished with it.
//
//	This class is an extension of the GameManager Custom Editor
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ArabicSupport;


public class Utility_MultiLanguageTextFileHandler : CustomEditor_Base<GameManager> 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static string m_sErrorMessage = "";



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Generate File
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public static void GenerateFile(LinkedList<CustomEditor_GameManager.MultiLanguageTextInstanceInfo> a_lMultiLanguageTextItems, GameManager.SystemLanguages a_eSelectedLanguage)
	{
		string saveFilename = EditorUtility.SaveFilePanel("Save File", @"C:\", "Multi-Language Text Generator", "xml");
		if(saveFilename != "")
		{
			string xmlString = CreateExcelDocument(a_lMultiLanguageTextItems, a_eSelectedLanguage);
			using(System.IO.StreamWriter textWriter = new System.IO.StreamWriter(saveFilename))
			{
				foreach(string line in xmlString.Split(new char[] { '\n' }))
					textWriter.WriteLine(line);
			}
			EditorUtility.DisplayDialog("Success", "Text File Generation Successful\n\n\nFind the Generated File at: " + saveFilename, "Okay");
		}
		else
		{
			Debug.Log("TEXT FILE GENERATION CANCELLED");
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Read From File
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void ReadFromFile(LinkedList<CustomEditor_GameManager.MultiLanguageTextInstanceInfo> a_lMultiLanguageTextItems, GameManager.SystemLanguages a_eSelectedLanguage)
	{
		string openFilename = (Application.dataPath + "/Multi-Language-Urdu-Updated.xml");
		if (!System.IO.File.Exists(openFilename))
		{
			openFilename = EditorUtility.OpenFilePanel("Open File", @"C:\", "xml");
		}
		if(openFilename != "")
		{
			using(System.IO.StreamReader textReader = new System.IO.StreamReader(openFilename))
			{
				m_sErrorMessage = "";
				Dictionary<string, string>  translationData = ReadExcelDocument(textReader.ReadToEnd().Replace("\r", "").Replace("\n", ""));
				if(translationData != null)
				{
					LinkedList<string> lUnTranslatedComponenets = new LinkedList<string>();
					LinkedList<Component> lDirtyComponentsList = new LinkedList<Component>();
					foreach(CustomEditor_GameManager.MultiLanguageTextInstanceInfo mlti in a_lMultiLanguageTextItems)
					{
						MultiLanguageText textComponent = mlti.rInstance;
						if(!(textComponent.m_arLanguageText.Length > (int)a_eSelectedLanguage))
						{
							ResizeArray(ref textComponent.m_arLanguageText, (int)a_eSelectedLanguage + 1);
						}

						string sTranslation = Regex.Replace(textComponent.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text.Replace("\n", "").Replace("\r", ""), @"<color=#[a-zA-Z0-9]+?>(.*?)</color>",
										m =>
										{
											return (m.Groups.Count > 1 && m.Groups[1].Value != "") ? string.Format("{0}", m.Groups[1].Value) : "";
										},
										RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(" ", "").ToUpper();
						if(translationData.ContainsKey(sTranslation))
						{
							// Assign Translation Fixes and Alignment!
							switch(a_eSelectedLanguage)
							{
								case GameManager.SystemLanguages.ARABIC: case GameManager.SystemLanguages.PERSIAN: case GameManager.SystemLanguages.URDU:
									sTranslation = RTLService.RTL.Convert(Regex.Replace(translationData[sTranslation], "<color=#[a-zA-Z0-9]+?>", "").Replace("</color>", ""));
									//textComponent.m_arLanguageText[(int)a_eSelectedLanguage].textAlignment = TextAlignment.Right;
									break;
								case GameManager.SystemLanguages.CHINESE_SIMPLIFIED: case GameManager.SystemLanguages.JAPANESE:
									sTranslation = translationData[sTranslation];
									break;
								default:
									textComponent.m_arLanguageText[(int)a_eSelectedLanguage].chosenFont = textComponent.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].chosenFont;
									sTranslation = translationData[sTranslation];
								break;
							}
							textComponent.m_arLanguageText[(int)a_eSelectedLanguage].text = sTranslation;

							if(textComponent.m_arLanguageText[(int)a_eSelectedLanguage].fontPosition == Vector3.zero)
								textComponent.m_arLanguageText[(int)a_eSelectedLanguage].fontPosition = textComponent.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].fontPosition;

							if(textComponent.m_arLanguageText[(int)a_eSelectedLanguage].fontSize == 0)
								textComponent.m_arLanguageText[(int)a_eSelectedLanguage].fontSize = textComponent.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].fontSize;

							if(textComponent.m_arLanguageText[(int)a_eSelectedLanguage].lineSpacing == 0.0f)
								textComponent.m_arLanguageText[(int)a_eSelectedLanguage].lineSpacing = textComponent.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].lineSpacing;

							if(!lDirtyComponentsList.Contains(mlti.rOwningParent))
								lDirtyComponentsList.AddLast( mlti.rOwningParent );
						}
						else
						{
							if(sTranslation != "")
								lUnTranslatedComponenets.AddLast(mlti.sDisplayLabel);
						}
					}

					if(lUnTranslatedComponenets.Count > 0)
					{
						string msg = "Non-Translated Components Count: " + lUnTranslatedComponenets.Count + "\n";
						foreach(string s in lUnTranslatedComponenets)
							msg += s + "\n";
						Debug.LogWarning(msg);
					}

					foreach(Component c in lDirtyComponentsList)
						if(c != null)
							EditorUtility.SetDirty(c);

					//EditorUtility.DisplayDialog("Success", "Load Successful. Check your Multi-Language Text Components to make sure everything imported correctly and looks nice.", "Okay");
				}

				if(m_sErrorMessage != "")
				{
					Debug.LogError(m_sErrorMessage);
				}
            }
		}
		else
		{
			Debug.Log("TEXT FILE LOAD CANCELLED");
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Create Word Document
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static string CreateWordDocument(LinkedList<CustomEditor_GameManager.MultiLanguageTextInstanceInfo> a_lMultiLanguageTextItems)
	{
		LinkedList<string> lAlreadyAddedText = new LinkedList<string>();
		const string COL1_WIDTH = "4068";
		const string COL2_WIDTH = "5400";
		const string FONT_SIZE = "18";

		string xmlString =  "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n" +
							"<?mso-application progid=\"Word.Document\"?>\n" +
							"<w:wordDocument xmlns:aml=\"http://schemas.microsoft.com/aml/2001/core\" xmlns:dt=\"uuid:C2F41010-65B3-11d1-A29F-00AA00C14882\" xmlns:ve=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" xmlns:w=\"http://schemas.microsoft.com/office/word/2003/wordml\" xmlns:wx=\"http://schemas.microsoft.com/office/word/2003/auxHint\" xmlns:wsp=\"http://schemas.microsoft.com/office/word/2003/wordml/sp2\" xmlns:sl=\"http://schemas.microsoft.com/schemaLibrary/2003/core\" w:macrosPresent=\"no\" w:embeddedObjPresent=\"no\" w:ocxPresent=\"no\" xml:space=\"preserve\"><w:ignoreSubtree w:val=\"http://schemas.microsoft.com/office/word/2003/wordml/sp2\"/><o:DocumentProperties><o:Author>CDA</o:Author><o:LastAuthor>CDA</o:LastAuthor><o:Revision>2</o:Revision><o:TotalTime>5</o:TotalTime><o:Created>2016-01-26T20:22:00Z</o:Created><o:LastSaved>2016-01-26T20:22:00Z</o:LastSaved><o:Pages>1</o:Pages><o:Words>42</o:Words><o:Characters>244</o:Characters><o:Lines>2</o:Lines><o:Paragraphs>1</o:Paragraphs><o:CharactersWithSpaces>285</o:CharactersWithSpaces><o:Version>12</o:Version></o:DocumentProperties><w:fonts><w:defaultFonts w:ascii=\"Calibri\" w:fareast=\"MS Mincho\" w:h-ansi=\"Calibri\" w:cs=\"Times New Roman\"/><w:font w:name=\"Times New Roman\"><w:panose-1 w:val=\"02020603050405020304\"/><w:charset w:val=\"00\"/><w:family w:val=\"Roman\"/><w:pitch w:val=\"variable\"/><w:sig w:usb-0=\"E0002AFF\" w:usb-1=\"C0007841\" w:usb-2=\"00000009\" w:usb-3=\"00000000\" w:csb-0=\"000001FF\" w:csb-1=\"00000000\"/></w:font><w:font w:name=\"MS Mincho\"><w:altName w:val=\"ＭＳ 明朝\"/><w:panose-1 w:val=\"02020609040205080304\"/><w:charset w:val=\"80\"/><w:family w:val=\"Modern\"/><w:pitch w:val=\"fixed\"/><w:sig w:usb-0=\"E00002FF\" w:usb-1=\"6AC7FDFB\" w:usb-2=\"00000012\" w:usb-3=\"00000000\" w:csb-0=\"0002009F\" w:csb-1=\"00000000\"/></w:font><w:font w:name=\"MS Gothic\"><w:altName w:val=\"ＭＳ ゴシック\"/><w:panose-1 w:val=\"020B0609070205080204\"/><w:charset w:val=\"80\"/><w:family w:val=\"Modern\"/><w:pitch w:val=\"fixed\"/><w:sig w:usb-0=\"E00002FF\" w:usb-1=\"6AC7FDFB\" w:usb-2=\"00000012\" w:usb-3=\"00000000\" w:csb-0=\"0002009F\" w:csb-1=\"00000000\"/></w:font><w:font w:name=\"Cambria Math\"><w:panose-1 w:val=\"02040503050406030204\"/><w:charset w:val=\"01\"/><w:family w:val=\"Roman\"/><w:notTrueType/><w:pitch w:val=\"variable\"/><w:sig w:usb-0=\"00000000\" w:usb-1=\"00000000\" w:usb-2=\"00000000\" w:usb-3=\"00000000\" w:csb-0=\"00000000\" w:csb-1=\"00000000\"/></w:font><w:font w:name=\"Cambria\"><w:panose-1 w:val=\"02040503050406030204\"/><w:charset w:val=\"00\"/><w:family w:val=\"Roman\"/><w:pitch w:val=\"variable\"/><w:sig w:usb-0=\"E00002FF\" w:usb-1=\"400004FF\" w:usb-2=\"00000000\" w:usb-3=\"00000000\" w:csb-0=\"0000019F\" w:csb-1=\"00000000\"/></w:font><w:font w:name=\"Calibri\"><w:panose-1 w:val=\"020F0502020204030204\"/><w:charset w:val=\"00\"/><w:family w:val=\"Swiss\"/><w:pitch w:val=\"variable\"/><w:sig w:usb-0=\"E00002FF\" w:usb-1=\"4000ACFF\" w:usb-2=\"00000001\" w:usb-3=\"00000000\" w:csb-0=\"0000019F\" w:csb-1=\"00000000\"/></w:font><w:font w:name=\"Verdana\"><w:panose-1 w:val=\"020B0604030504040204\"/><w:charset w:val=\"00\"/><w:family w:val=\"Swiss\"/><w:pitch w:val=\"variable\"/><w:sig w:usb-0=\"A10006FF\" w:usb-1=\"4000205B\" w:usb-2=\"00000010\" w:usb-3=\"00000000\" w:csb-0=\"0000019F\" w:csb-1=\"00000000\"/></w:font><w:font w:name=\"@MS Mincho\"><w:panose-1 w:val=\"02020609040205080304\"/><w:charset w:val=\"80\"/><w:family w:val=\"Modern\"/><w:pitch w:val=\"fixed\"/><w:sig w:usb-0=\"E00002FF\" w:usb-1=\"6AC7FDFB\" w:usb-2=\"00000012\" w:usb-3=\"00000000\" w:csb-0=\"0002009F\" w:csb-1=\"00000000\"/></w:font><w:font w:name=\"@MS Gothic\"><w:panose-1 w:val=\"020B0609070205080204\"/><w:charset w:val=\"80\"/><w:family w:val=\"Modern\"/><w:pitch w:val=\"fixed\"/><w:sig w:usb-0=\"E00002FF\" w:usb-1=\"6AC7FDFB\" w:usb-2=\"00000012\" w:usb-3=\"00000000\" w:csb-0=\"0002009F\" w:csb-1=\"00000000\"/></w:font></w:fonts><w:styles><w:versionOfBuiltInStylenames w:val=\"7\"/><w:latentStyles w:defLockedState=\"off\" w:latentStyleCount=\"267\"><w:lsdException w:name=\"Normal\"/><w:lsdException w:name=\"heading 1\"/><w:lsdException w:name=\"heading 2\"/><w:lsdException w:name=\"heading 3\"/><w:lsdException w:name=\"heading 4\"/><w:lsdException w:name=\"heading 5\"/><w:lsdException w:name=\"heading 6\"/><w:lsdException w:name=\"heading 7\"/><w:lsdException w:name=\"heading 8\"/><w:lsdException w:name=\"heading 9\"/><w:lsdException w:name=\"toc 1\"/><w:lsdException w:name=\"toc 2\"/><w:lsdException w:name=\"toc 3\"/><w:lsdException w:name=\"toc 4\"/><w:lsdException w:name=\"toc 5\"/><w:lsdException w:name=\"toc 6\"/><w:lsdException w:name=\"toc 7\"/><w:lsdException w:name=\"toc 8\"/><w:lsdException w:name=\"toc 9\"/><w:lsdException w:name=\"caption\"/><w:lsdException w:name=\"Title\"/><w:lsdException w:name=\"Default Paragraph Font\"/><w:lsdException w:name=\"Subtitle\"/><w:lsdException w:name=\"Strong\"/><w:lsdException w:name=\"Emphasis\"/><w:lsdException w:name=\"Table Grid\"/><w:lsdException w:name=\"Placeholder Text\"/><w:lsdException w:name=\"No Spacing\"/><w:lsdException w:name=\"Light Shading\"/><w:lsdException w:name=\"Light List\"/><w:lsdException w:name=\"Light Grid\"/><w:lsdException w:name=\"Medium Shading 1\"/><w:lsdException w:name=\"Medium Shading 2\"/><w:lsdException w:name=\"Medium List 1\"/><w:lsdException w:name=\"Medium List 2\"/><w:lsdException w:name=\"Medium Grid 1\"/><w:lsdException w:name=\"Medium Grid 2\"/><w:lsdException w:name=\"Medium Grid 3\"/><w:lsdException w:name=\"Dark List\"/><w:lsdException w:name=\"Colorful Shading\"/><w:lsdException w:name=\"Colorful List\"/><w:lsdException w:name=\"Colorful Grid\"/><w:lsdException w:name=\"Light Shading Accent 1\"/><w:lsdException w:name=\"Light List Accent 1\"/><w:lsdException w:name=\"Light Grid Accent 1\"/><w:lsdException w:name=\"Medium Shading 1 Accent 1\"/><w:lsdException w:name=\"Medium Shading 2 Accent 1\"/><w:lsdException w:name=\"Medium List 1 Accent 1\"/><w:lsdException w:name=\"Revision\"/><w:lsdException w:name=\"List Paragraph\"/><w:lsdException w:name=\"Quote\"/><w:lsdException w:name=\"Intense Quote\"/><w:lsdException w:name=\"Medium List 2 Accent 1\"/><w:lsdException w:name=\"Medium Grid 1 Accent 1\"/><w:lsdException w:name=\"Medium Grid 2 Accent 1\"/><w:lsdException w:name=\"Medium Grid 3 Accent 1\"/><w:lsdException w:name=\"Dark List Accent 1\"/><w:lsdException w:name=\"Colorful Shading Accent 1\"/><w:lsdException w:name=\"Colorful List Accent 1\"/><w:lsdException w:name=\"Colorful Grid Accent 1\"/><w:lsdException w:name=\"Light Shading Accent 2\"/><w:lsdException w:name=\"Light List Accent 2\"/><w:lsdException w:name=\"Light Grid Accent 2\"/><w:lsdException w:name=\"Medium Shading 1 Accent 2\"/><w:lsdException w:name=\"Medium Shading 2 Accent 2\"/><w:lsdException w:name=\"Medium List 1 Accent 2\"/><w:lsdException w:name=\"Medium List 2 Accent 2\"/><w:lsdException w:name=\"Medium Grid 1 Accent 2\"/><w:lsdException w:name=\"Medium Grid 2 Accent 2\"/><w:lsdException w:name=\"Medium Grid 3 Accent 2\"/><w:lsdException w:name=\"Dark List Accent 2\"/><w:lsdException w:name=\"Colorful Shading Accent 2\"/><w:lsdException w:name=\"Colorful List Accent 2\"/><w:lsdException w:name=\"Colorful Grid Accent 2\"/><w:lsdException w:name=\"Light Shading Accent 3\"/><w:lsdException w:name=\"Light List Accent 3\"/><w:lsdException w:name=\"Light Grid Accent 3\"/><w:lsdException w:name=\"Medium Shading 1 Accent 3\"/><w:lsdException w:name=\"Medium Shading 2 Accent 3\"/><w:lsdException w:name=\"Medium List 1 Accent 3\"/><w:lsdException w:name=\"Medium List 2 Accent 3\"/><w:lsdException w:name=\"Medium Grid 1 Accent 3\"/><w:lsdException w:name=\"Medium Grid 2 Accent 3\"/><w:lsdException w:name=\"Medium Grid 3 Accent 3\"/><w:lsdException w:name=\"Dark List Accent 3\"/><w:lsdException w:name=\"Colorful Shading Accent 3\"/><w:lsdException w:name=\"Colorful List Accent 3\"/><w:lsdException w:name=\"Colorful Grid Accent 3\"/><w:lsdException w:name=\"Light Shading Accent 4\"/><w:lsdException w:name=\"Light List Accent 4\"/><w:lsdException w:name=\"Light Grid Accent 4\"/><w:lsdException w:name=\"Medium Shading 1 Accent 4\"/><w:lsdException w:name=\"Medium Shading 2 Accent 4\"/><w:lsdException w:name=\"Medium List 1 Accent 4\"/><w:lsdException w:name=\"Medium List 2 Accent 4\"/><w:lsdException w:name=\"Medium Grid 1 Accent 4\"/><w:lsdException w:name=\"Medium Grid 2 Accent 4\"/><w:lsdException w:name=\"Medium Grid 3 Accent 4\"/><w:lsdException w:name=\"Dark List Accent 4\"/><w:lsdException w:name=\"Colorful Shading Accent 4\"/><w:lsdException w:name=\"Colorful List Accent 4\"/><w:lsdException w:name=\"Colorful Grid Accent 4\"/><w:lsdException w:name=\"Light Shading Accent 5\"/><w:lsdException w:name=\"Light List Accent 5\"/><w:lsdException w:name=\"Light Grid Accent 5\"/><w:lsdException w:name=\"Medium Shading 1 Accent 5\"/><w:lsdException w:name=\"Medium Shading 2 Accent 5\"/><w:lsdException w:name=\"Medium List 1 Accent 5\"/><w:lsdException w:name=\"Medium List 2 Accent 5\"/><w:lsdException w:name=\"Medium Grid 1 Accent 5\"/><w:lsdException w:name=\"Medium Grid 2 Accent 5\"/><w:lsdException w:name=\"Medium Grid 3 Accent 5\"/><w:lsdException w:name=\"Dark List Accent 5\"/><w:lsdException w:name=\"Colorful Shading Accent 5\"/><w:lsdException w:name=\"Colorful List Accent 5\"/><w:lsdException w:name=\"Colorful Grid Accent 5\"/><w:lsdException w:name=\"Light Shading Accent 6\"/><w:lsdException w:name=\"Light List Accent 6\"/><w:lsdException w:name=\"Light Grid Accent 6\"/><w:lsdException w:name=\"Medium Shading 1 Accent 6\"/><w:lsdException w:name=\"Medium Shading 2 Accent 6\"/><w:lsdException w:name=\"Medium List 1 Accent 6\"/><w:lsdException w:name=\"Medium List 2 Accent 6\"/><w:lsdException w:name=\"Medium Grid 1 Accent 6\"/><w:lsdException w:name=\"Medium Grid 2 Accent 6\"/><w:lsdException w:name=\"Medium Grid 3 Accent 6\"/><w:lsdException w:name=\"Dark List Accent 6\"/><w:lsdException w:name=\"Colorful Shading Accent 6\"/><w:lsdException w:name=\"Colorful List Accent 6\"/><w:lsdException w:name=\"Colorful Grid Accent 6\"/><w:lsdException w:name=\"Subtle Emphasis\"/><w:lsdException w:name=\"Intense Emphasis\"/><w:lsdException w:name=\"Subtle Reference\"/><w:lsdException w:name=\"Intense Reference\"/><w:lsdException w:name=\"Book Title\"/><w:lsdException w:name=\"Bibliography\"/><w:lsdException w:name=\"TOC Heading\"/></w:latentStyles><w:style w:type=\"paragraph\" w:default=\"on\" w:styleId=\"Normal\"><w:name w:val=\"Normal\"/><w:rsid w:val=\"00D67F50\"/><w:pPr><w:spacing w:after=\"200\" w:line=\"276\" w:line-rule=\"auto\"/></w:pPr><w:rPr><wx:font wx:val=\"Calibri\"/><w:sz w:val=\"22\"/><w:sz-cs w:val=\"22\"/><w:lang w:val=\"EN-US\" w:fareast=\"JA\" w:bidi=\"AR-SA\"/></w:rPr></w:style><w:style w:type=\"paragraph\" w:styleId=\"Heading1\"><w:name w:val=\"heading 1\"/><wx:uiName wx:val=\"Heading 1\"/><w:basedOn w:val=\"Normal\"/><w:next w:val=\"Normal\"/><w:link w:val=\"Heading1Char\"/><w:rsid w:val=\"000E68FF\"/><w:pPr><w:keepNext/><w:keepLines/><w:spacing w:before=\"480\" w:after=\"0\"/><w:outlineLvl w:val=\"0\"/></w:pPr><w:rPr><w:rFonts w:ascii=\"Cambria\" w:fareast=\"MS Gothic\" w:h-ansi=\"Cambria\"/><wx:font wx:val=\"Cambria\"/><w:b/><w:b-cs/><w:color w:val=\"365F91\"/><w:sz w:val=\"28\"/><w:sz-cs w:val=\"28\"/></w:rPr></w:style><w:style w:type=\"paragraph\" w:styleId=\"Heading2\"><w:name w:val=\"heading 2\"/><wx:uiName wx:val=\"Heading 2\"/><w:basedOn w:val=\"Normal\"/><w:next w:val=\"Normal\"/><w:link w:val=\"Heading2Char\"/><w:rsid w:val=\"000E68FF\"/><w:pPr><w:keepNext/><w:keepLines/><w:spacing w:before=\"200\" w:after=\"0\"/><w:outlineLvl w:val=\"1\"/></w:pPr><w:rPr><w:rFonts w:ascii=\"Cambria\" w:fareast=\"MS Gothic\" w:h-ansi=\"Cambria\"/><wx:font wx:val=\"Cambria\"/><w:b/><w:b-cs/><w:color w:val=\"4F81BD\"/><w:sz w:val=\"26\"/><w:sz-cs w:val=\"26\"/></w:rPr></w:style><w:style w:type=\"character\" w:default=\"on\" w:styleId=\"DefaultParagraphFont\"><w:name w:val=\"Default Paragraph Font\"/></w:style><w:style w:type=\"table\" w:default=\"on\" w:styleId=\"TableNormal\"><w:name w:val=\"Normal Table\"/><wx:uiName wx:val=\"Table Normal\"/><w:rPr><wx:font wx:val=\"Calibri\"/><w:lang w:val=\"EN-US\" w:fareast=\"JA\" w:bidi=\"AR-SA\"/></w:rPr><w:tblPr><w:tblInd w:w=\"0\" w:type=\"dxa\"/><w:tblCellMar><w:top w:w=\"0\" w:type=\"dxa\"/><w:left w:w=\"108\" w:type=\"dxa\"/><w:bottom w:w=\"0\" w:type=\"dxa\"/><w:right w:w=\"108\" w:type=\"dxa\"/></w:tblCellMar></w:tblPr></w:style><w:style w:type=\"list\" w:default=\"on\" w:styleId=\"NoList\"><w:name w:val=\"No List\"/></w:style><w:style w:type=\"paragraph\" w:styleId=\"Header\"><w:name w:val=\"header\"/><wx:uiName wx:val=\"Header\"/><w:basedOn w:val=\"Normal\"/><w:link w:val=\"HeaderChar\"/><w:rsid w:val=\"000E68FF\"/><w:pPr><w:tabs><w:tab w:val=\"center\" w:pos=\"4680\"/><w:tab w:val=\"right\" w:pos=\"9360\"/></w:tabs><w:spacing w:after=\"0\" w:line=\"240\" w:line-rule=\"auto\"/></w:pPr><w:rPr><wx:font wx:val=\"Calibri\"/></w:rPr></w:style><w:style w:type=\"character\" w:styleId=\"HeaderChar\"><w:name w:val=\"Header Char\"/><w:basedOn w:val=\"DefaultParagraphFont\"/><w:link w:val=\"Header\"/><w:rsid w:val=\"000E68FF\"/></w:style><w:style w:type=\"paragraph\" w:styleId=\"Footer\"><w:name w:val=\"footer\"/><wx:uiName wx:val=\"Footer\"/><w:basedOn w:val=\"Normal\"/><w:link w:val=\"FooterChar\"/><w:rsid w:val=\"000E68FF\"/><w:pPr><w:tabs><w:tab w:val=\"center\" w:pos=\"4680\"/><w:tab w:val=\"right\" w:pos=\"9360\"/></w:tabs><w:spacing w:after=\"0\" w:line=\"240\" w:line-rule=\"auto\"/></w:pPr><w:rPr><wx:font wx:val=\"Calibri\"/></w:rPr></w:style><w:style w:type=\"character\" w:styleId=\"FooterChar\"><w:name w:val=\"Footer Char\"/><w:basedOn w:val=\"DefaultParagraphFont\"/><w:link w:val=\"Footer\"/><w:rsid w:val=\"000E68FF\"/></w:style><w:style w:type=\"character\" w:styleId=\"Heading1Char\"><w:name w:val=\"Heading 1 Char\"/><w:basedOn w:val=\"DefaultParagraphFont\"/><w:link w:val=\"Heading1\"/><w:rsid w:val=\"000E68FF\"/><w:rPr><w:rFonts w:ascii=\"Cambria\" w:fareast=\"MS Gothic\" w:h-ansi=\"Cambria\" w:cs=\"Times New Roman\"/><w:b/><w:b-cs/><w:color w:val=\"365F91\"/><w:sz w:val=\"28\"/><w:sz-cs w:val=\"28\"/></w:rPr></w:style><w:style w:type=\"character\" w:styleId=\"Heading2Char\"><w:name w:val=\"Heading 2 Char\"/><w:basedOn w:val=\"DefaultParagraphFont\"/><w:link w:val=\"Heading2\"/><w:rsid w:val=\"000E68FF\"/><w:rPr><w:rFonts w:ascii=\"Cambria\" w:fareast=\"MS Gothic\" w:h-ansi=\"Cambria\" w:cs=\"Times New Roman\"/><w:b/><w:b-cs/><w:color w:val=\"4F81BD\"/><w:sz w:val=\"26\"/><w:sz-cs w:val=\"26\"/></w:rPr></w:style></w:styles><w:shapeDefaults><o:shapedefaults v:ext=\"edit\" spidmax=\"3074\"/><o:shapelayout v:ext=\"edit\"><o:idmap v:ext=\"edit\" data=\"1\"/></o:shapelayout></w:shapeDefaults><w:docPr><w:view w:val=\"print\"/><w:zoom w:percent=\"120\"/><w:doNotEmbedSystemFonts/><w:defaultTabStop w:val=\"720\"/><w:punctuationKerning/><w:characterSpacingControl w:val=\"DontCompress\"/><w:optimizeForBrowser/><w:validateAgainstSchema/><w:saveInvalidXML w:val=\"off\"/><w:ignoreMixedContent w:val=\"off\"/><w:alwaysShowPlaceholderText w:val=\"off\"/><w:footnotePr><w:footnote w:type=\"separator\"><w:p wsp:rsidR=\"001C4094\" wsp:rsidRDefault=\"001C4094\" wsp:rsidP=\"000E68FF\"><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:line-rule=\"auto\"/></w:pPr><w:r><w:separator/></w:r></w:p></w:footnote><w:footnote w:type=\"continuation-separator\"><w:p wsp:rsidR=\"001C4094\" wsp:rsidRDefault=\"001C4094\" wsp:rsidP=\"000E68FF\"><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:line-rule=\"auto\"/></w:pPr><w:r><w:continuationSeparator/></w:r></w:p></w:footnote></w:footnotePr><w:endnotePr><w:endnote w:type=\"separator\"><w:p wsp:rsidR=\"001C4094\" wsp:rsidRDefault=\"001C4094\" wsp:rsidP=\"000E68FF\"><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:line-rule=\"auto\"/></w:pPr><w:r><w:separator/></w:r></w:p></w:endnote><w:endnote w:type=\"continuation-separator\"><w:p wsp:rsidR=\"001C4094\" wsp:rsidRDefault=\"001C4094\" wsp:rsidP=\"000E68FF\"><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:line-rule=\"auto\"/></w:pPr><w:r><w:continuationSeparator/></w:r></w:p></w:endnote></w:endnotePr><w:compat><w:breakWrappedTables/><w:snapToGridInCell/><w:wrapTextWithPunct/><w:useAsianBreakRules/><w:dontGrowAutofit/><w:useFELayout/></w:compat><wsp:rsids><wsp:rsidRoot wsp:val=\"000E68FF\"/><wsp:rsid wsp:val=\"000E68FF\"/><wsp:rsid wsp:val=\"001C4094\"/><wsp:rsid wsp:val=\"00287E95\"/><wsp:rsid wsp:val=\"00441421\"/><wsp:rsid wsp:val=\"005C2800\"/><wsp:rsid wsp:val=\"006713DF\"/><wsp:rsid wsp:val=\"00780078\"/><wsp:rsid wsp:val=\"00AD22BD\"/><wsp:rsid wsp:val=\"00D67F50\"/><wsp:rsid wsp:val=\"00E44711\"/><wsp:rsid wsp:val=\"00EF70DE\"/></wsp:rsids></w:docPr>\n" +
							"\t<w:body>\n" +
							"\t\t<w:tbl><w:tblPr><w:tblW w:w=\"0\" w:type=\"auto\"/><w:tblBorders><w:top w:val=\"single\" w:sz=\"4\" wx:bdrwidth=\"10\" w:space=\"0\" w:color=\"auto\"/><w:left w:val=\"single\" w:sz=\"4\" wx:bdrwidth=\"10\" w:space=\"0\" w:color=\"auto\"/><w:bottom w:val=\"single\" w:sz=\"4\" wx:bdrwidth=\"10\" w:space=\"0\" w:color=\"auto\"/><w:right w:val=\"single\" w:sz=\"4\" wx:bdrwidth=\"10\" w:space=\"0\" w:color=\"auto\"/><w:insideH w:val=\"single\" w:sz=\"4\" wx:bdrwidth=\"10\" w:space=\"0\" w:color=\"auto\"/><w:insideV w:val=\"single\" w:sz=\"4\" wx:bdrwidth=\"10\" w:space=\"0\" w:color=\"auto\"/></w:tblBorders><w:tblLayout w:type=\"Fixed\"/></w:tblPr><w:tblGrid><w:gridCol w:w=\"" + COL1_WIDTH + "\"/><w:gridCol w:w=\"" + COL2_WIDTH + "\"/></w:tblGrid>\n";

		foreach(CustomEditor_GameManager.MultiLanguageTextInstanceInfo displayedText in a_lMultiLanguageTextItems)
		{
			if(displayedText.rInstance.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text != "")
			{
				if(lAlreadyAddedText.Contains(displayedText.rInstance.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text.ToUpper()))
				{
					continue;
				}
				else
				{
					lAlreadyAddedText.AddLast(displayedText.rInstance.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text.ToUpper());
				}



				xmlString += "\t\t\t<w:tr wsp:rsidR=\"000E68FF\" wsp:rsidRPr=\"00E44711\" wsp:rsidTr=\"000E68FF\">\n";
				xmlString += "\t\t\t\t<w:tc><w:tcPr><w:tcW w:w=\"" + COL1_WIDTH + "\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr>\n";
				xmlString += "\t\t\t\t<w:p wsp:rsidR=\"000E68FF\" wsp:rsidRPr=\"00E44711\" wsp:rsidRDefault=\"00441421\"><w:pPr><w:rPr><w:rFonts w:ascii=\"Calibri\" w:fareast=\"MS Mincho\" w:h-ansi=\"Calibri\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr>\n";
				string[] colourDifferences = Regex.Split(displayedText.rInstance.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text, @"(<color=#[0-9a-zA-Z]{6}[0-9a-zA-Z]?[0-9a-zA-Z]?>.+?</color>)", RegexOptions.Multiline);
				foreach(string colourPart in colourDifferences)
				{
					string[] lineDifferences = Regex.Replace(colourPart, @"<color=#[a-zA-Z0-9]+>", "").Replace("</color>", "").Split(new char[] { '\n', '\r' });
					string colourCode = "";
					Match match = Regex.Match(colourPart, @"<color=#([0-9a-zA-Z]{6})[0-9a-zA-Z]?[0-9a-zA-Z]?>", RegexOptions.Multiline);
					if(match.Success)
						colourCode = match.Groups[1].Value;
						
					for(int i = 0; i < lineDifferences.Length; ++i)
					{
						xmlString += "\t\t\t\t\t<w:r wsp:rsidRPr=\"00D35B32\"><w:rPr><w:rFonts w:ascii=\"Calibri\" w:fareast=\"MS Mincho\" w:h-ansi=\"Calibri\" w:cs=\"Times New Roman\"/>";
						xmlString += "<w:sz w:val=\"" + FONT_SIZE + "\"/>";				// Defining the Size of the Text in Word
						if(colourCode != "")
							xmlString += "<w:color w:val=\"" + colourCode + "\"/>";		// Define the Colour of the Text in Word
						xmlString += "</w:rPr>";
						if(i > 0)
							xmlString += "<w:br/>";
						xmlString += "<w:t>";
						xmlString += lineDifferences[i];
						xmlString += "</w:t></w:r>\n";
					}
				}
				xmlString += "\t\t\t\t</w:p>\n";
				xmlString += "\t\t\t</w:tc>\n";
				xmlString += "\t\t\t<w:tc><w:tcPr><w:tcW w:w=\"" + COL2_WIDTH + "\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr>\n";
				xmlString += "\t\t\t\t<w:p wsp:rsidR=\"000E68FF\" wsp:rsidRPr=\"00E44711\" wsp:rsidRDefault=\"00441421\"><w:pPr><w:rPr><w:rFonts w:ascii=\"Calibri\" w:fareast=\"MS Mincho\" w:h-ansi=\"Calibri\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r wsp:rsidRPr=\"00E44711\"><w:rPr><w:rFonts w:ascii=\"Calibri\" w:fareast=\"MS Mincho\" w:h-ansi=\"Calibri\" w:cs=\"Times New Roman\"/></w:rPr><w:t></w:t></w:r></w:p></w:tc>\n";
				xmlString += "\t\t\t</w:tr>\n";
			}
		}
		xmlString += "\t\t</w:tbl>\n";
		xmlString += "\t</w:body>\n";
		xmlString += "</w:wordDocument>";

		return xmlString;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Create Excel Document
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static string CreateExcelDocument(LinkedList<CustomEditor_GameManager.MultiLanguageTextInstanceInfo> a_lMultiLanguageTextItems, GameManager.SystemLanguages a_eSelectedLanguage)
	{
		LinkedList<string> lAlreadyAddedText = new LinkedList<string>();
		const string columnWidth = "250";
		const float defaultRowHeight = 22.5f;
		const string englishLabel = "ENGLISH TEXT:&#10;&#10;";
		const string englishLabelExplanation = "The text we're looking to get translated. Please do not change any of the text (even if there is something wrong with it gramatically) as the exact English in these boxes will be identified via script and will not work if you change anything";
		const string contextLabel = "CONTEXT FOR TEXT:&#10;&#10;";
		const string contextLabelExplanation = "Please check the context before translating as it may help you better understand the tone we are looking for.";
		string translationLabel = "Translation:&#10;&#10;";
		string translationLabelExplanation = "Please insert the language translation for " + GameManager.GetLanguageAsEnglishString(a_eSelectedLanguage) + " into the boxes below. We also ask that when you come across </Font><Font html:Color=\"#F26D7D\">coloured</Font><Font html:Color=\"#000000\"> English words that you also </Font><Font html:Color=\"#F26D7D\">colour</Font><Font html:Color=\"#000000\"> the translated phrase in the same colour.&#10;&#10;Please also remember that this app is intended for children. Child friendly text is preferred.";
		string xmlString = "<?xml version=\"1.0\"?>\n<?mso-application progid=\"Excel.Sheet\"?>\n<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\nxmlns:o=\"urn:schemas-microsoft-com:office:office\"\n xmlns:x=\"urn:schemas-microsoft-com:office:excel\"\nxmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"\n xmlns:html=\"http://www.w3.org/TR/REC-html40\">\n <DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">\n  <Author>CDA</Author>\n  <LastAuthor>CDA</LastAuthor>\n  <Created>2016-01-28T05:58:23Z</Created>\n  <Version>12.00</Version>\n </DocumentProperties>\n <ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">\n  <WindowHeight>12780</WindowHeight>\n  <WindowWidth>28695</WindowWidth>\n  <WindowTopX>120</WindowTopX>\n  <WindowTopY>60</WindowTopY>\n  <ProtectStructure>False</ProtectStructure>\n  <ProtectWindows>False</ProtectWindows>\n </ExcelWorkbook>\n <Styles>\n <Style ss:ID=\"Default\" ss:Name=\"Normal\">\n <Alignment ss:Vertical=\"Bottom\"/>\n <Borders/>\n <Font ss:FontName=\"Calibri\" x:Family=\"Swiss\" ss:Size=\"11\" ss:Color=\"#000000\"/>\n <Interior/>\n <NumberFormat/>\n <Protection/>\n </Style>\n <Style ss:ID=\"s65\">\n <Alignment ss:Horizontal=\"Left\" ss:Vertical=\"Center\" ss:WrapText=\"1\"/>\n </Style>\n </Styles>\n <Worksheet ss:Name=\"Sheet1\">\n";

		// Telling the Translator what the boxes are for~
		string identityString = "   <Row ss:AutoFitHeight=\"0\" ss:Height=\"150\">\n";
		identityString += "    <Cell ss:StyleID=\"s65\"><ss:Data ss:Type=\"String\" xmlns=\"http://www.w3.org/TR/REC-html40\">";
		identityString += "<B><Font html:Color=\"#000000\">" + contextLabel + "</Font></B><Font html:Color=\"#000000\">" + contextLabelExplanation + "</Font></ss:Data></Cell>\n";
		identityString += "    <Cell ss:StyleID=\"s65\"><ss:Data ss:Type=\"String\" xmlns=\"http://www.w3.org/TR/REC-html40\">";
		identityString += "<B><Font html:Color=\"#000000\">" + englishLabel  + "</Font></B><Font html:Color=\"#000000\">" + englishLabelExplanation  + "</Font></ss:Data></Cell>\n";
		identityString += "    <Cell ss:StyleID=\"s65\"><ss:Data ss:Type=\"String\" xmlns=\"http://www.w3.org/TR/REC-html40\">";
		identityString += "<B><Font html:Color=\"#000000\">" + translationLabel + "</Font></B><Font html:Color=\"#000000\">" + translationLabelExplanation + "</Font></ss:Data></Cell>\n   </Row>\n";

		string dataString = "";
		int indexID = 0;
		foreach(CustomEditor_GameManager.MultiLanguageTextInstanceInfo displayedText in a_lMultiLanguageTextItems)
		{
			if(displayedText.rInstance.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text != "")
			{
				if(lAlreadyAddedText.Contains(displayedText.rInstance.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text.ToUpper()))
				{
					continue;
				}
				else
				{
					lAlreadyAddedText.AddLast(displayedText.rInstance.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text.ToUpper());
					indexID += 1;
				}

				string rowHeight = (defaultRowHeight * (1 * Mathf.CeilToInt(Mathf.Max((float)displayedText.rInstance.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text.Split(new char[] { '\n' }).Length, ((float)displayedText.rInstance.dm_sTranslationDescription.Length / 47.0f))))).ToString();
				dataString += "   <Row ss:AutoFitHeight=\"0\" ss:Height=\"" + rowHeight + "\">\n";
				{

					// ~~~ TEXT CONTEXT ~~~ //
					{
						dataString += "    <Cell ss:StyleID=\"s65\"><ss:Data ss:Type=\"String\" xmlns=\"http://www.w3.org/TR/REC-html40\"><Font html:Color=\"#000000\">";
						dataString += displayedText.rInstance.dm_sTranslationDescription.Replace("\n", "&#10;").Replace("\"", "&quot;");
						dataString += "</Font></ss:Data></Cell>\n";
					}

					// ~~~ ENGLISH TEXT ~~~ //
					{
						dataString += "    <Cell ss:StyleID=\"s65\"><ss:Data ss:Type=\"String\" xmlns=\"http://www.w3.org/TR/REC-html40\">";
						string[] colourDifferences = Regex.Split(displayedText.rInstance.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text, @"(<color=#[0-9a-zA-Z]{6}[0-9a-zA-Z]?[0-9a-zA-Z]?>.+?</color>)", RegexOptions.Multiline);
						foreach(string colourPart in colourDifferences)
						{
							string[] lineDifferences = Regex.Replace(colourPart, @"<color=#[a-zA-Z0-9]+>", "").Replace("</color>", "").Split(new char[] { '\n', '\r' });
							string colourCode = "000000";
							Match match = Regex.Match(colourPart, @"<color=#([0-9a-zA-Z]{6})[0-9a-zA-Z]?[0-9a-zA-Z]?>", RegexOptions.Multiline);
							if(match.Success)
								colourCode = match.Groups[1].Value;

							for(int i = 0; i < lineDifferences.Length; ++i)
							{
								dataString += "<Font html:Color=\"#" + colourCode + "\">";     // Define the Colour of the Text in Excel
								if(i > 0)
									dataString += "&#10;";
								dataString += lineDifferences[i];
								dataString += "</Font>";
							}
						}
						dataString += "</ss:Data></Cell>\n";
					}
				}
				dataString += "   </Row>\n";
			}
		}


		string tableInfo = "  <Table x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\">\n";
		for(int i = 0; i < 3; ++i)
		{
			tableInfo += "   <Column ss:AutoFitWidth=\"0\" ss:Width=\"" + columnWidth + "\"/>\n";
		}

		xmlString += tableInfo;
		xmlString += identityString;
		xmlString += dataString;


		xmlString += "  </Table>\n  <WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">\n   <PageSetup>\n    <Header x:Margin=\"0.3\"/>\n    <Footer x:Margin=\"0.3\"/>\n    <PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/>\n   </PageSetup>\n   <Unsynced/>\n   <Print>\n    <ValidPrinterInfo/>\n    <PaperSizeIndex>9</PaperSizeIndex>\n    <HorizontalResolution>600</HorizontalResolution>\n    <VerticalResolution>600</VerticalResolution>\n   </Print>\n   <Selected/>\n   <Panes>\n    <Pane>\n     <Number>3</Number>\n     <ActiveRow>1</ActiveRow>\n     <ActiveCol>2</ActiveCol>\n    </Pane>\n   </Panes>\n   <ProtectObjects>False</ProtectObjects>\n   <ProtectScenarios>False</ProtectScenarios>\n  </WorksheetOptions>\n </Worksheet>\n </Workbook>";
		return xmlString;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Read Excel Document
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static Dictionary<string, string> ReadExcelDocument(string a_sTextInput)
	{
		const string tableCheckRegexPattern = "<Table.*?>(.+)</Table>";
		const string rowCheckRegexPattern = "<Row.*?>(.+?)</Row>";
		const string cellCheckRegexPattern = "<Cell.*?><(?:ss:)?Data.*?>(.*?)</(?:ss:)?Data>";
		const string colourCheckRegexPattern = "<Font.*?html:Color=\"#([a-zA-Z0-9]+?)\".*?>(.*?)</Font>";
		Match tableCheck = Regex.Match(a_sTextInput, tableCheckRegexPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
		if(tableCheck.Success)
		{
			string sTableContents = tableCheck.Groups[1].Value;
			Dictionary<string, string> englishToOtherLanguage = new Dictionary<string, string>();
			MatchCollection rowMatches = Regex.Matches(sTableContents, rowCheckRegexPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
			int iTranslationID = 1;
			foreach(Match row in rowMatches)
			{
				MatchCollection cellMatches = Regex.Matches(row.Value, cellCheckRegexPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
				const byte iEnglishTextLocation = 1;
				const byte iLanguageTranslationLocation = 2;

				// Replace HTML Code with UNITY UI Code (e.g "<Font html:Color="#000000">"		=>		"<color=#000000>" || &quot; => ")
				if(cellMatches.Count < 3)
				{
					m_sErrorMessage = ("Issue in Document at Line " + iTranslationID + ": Could not follow reading protocol. Line ignored\n");
				}
				else
				{
					string sEnglishText = Regex.Replace(cellMatches[iEnglishTextLocation].Value, colourCheckRegexPattern,
											m => 
											{
												return (m.Groups.Count > 1 && m.Groups[1].Value != "") ? string.Format("{0}", m.Groups[2].Value) : "";
											},
											RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&#10;", "");

					string sTranslation = Regex.Replace(cellMatches[iLanguageTranslationLocation].Value, colourCheckRegexPattern,
											m => 
											{
												if(m.Groups.Count > 1 && m.Groups[1].Value != "")
												{
													if(m.Groups[1].Value != "000000" && m.Groups[1].Value != "00000000")
														return string.Format("<color=#{0}>{1}</color>", m.Groups[1].Value, m.Groups[2].Value);
													else
														return string.Format("{0}", m.Groups[2].Value);
												}
												return "";
											},
											RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&#10;", "\n");


					// Remove Remaining HTML Tags that are of no use to us.
					sEnglishText = Regex.Replace(Regex.Replace(sEnglishText, "</(?:ss:)?Data>", ""), "<Cell.*?><(?:ss:)?Data.*?>", "").ToUpper().Replace(" ", "");
					sTranslation = Regex.Replace(Regex.Replace(sTranslation, "</(?:ss:)?Data>", ""), "<Cell.*?><(?:ss:)?Data.*?>", "");


					if(!englishToOtherLanguage.ContainsKey(sEnglishText))
					{
						englishToOtherLanguage.Add(sEnglishText, sTranslation);
					}
					iTranslationID += 1;
                }
			}

			return englishToOtherLanguage;
		}
		return null;
	}
}
