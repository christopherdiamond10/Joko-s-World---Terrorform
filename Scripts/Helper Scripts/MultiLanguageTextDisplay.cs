//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Multi Language Text
//             Author: Christopher Diamond
//             Date: November 19, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script keeps a saved copy of all the multiple languages that can be 
//		selected in the app and interacts with the Game Manager to determine which
//		is the appropriate text in it's list of languages to display.
//	
//	  There is a special CustomEditor function to interact with MultiLanguageText
//		components.
//
//
//	  There are two classes in here:
//		* Multi Language Text			: Class which is doing the work
//		* Multi Language Text Display	: derived from MonoBehaviour, can be applied
//										   to a Text object which will then change
//										   based on the selected Language											 
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

//===========================================================================
// *** Multi Language Text
//===========================================================================
[System.Serializable]
public class MultiLanguageText
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TextDisplayValues[] m_arLanguageText = new TextDisplayValues[(int)GameManager.SystemLanguages.TOTAL_LANGUAGES];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*-- Debug Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
	public UnityEngine.UI.Text dm_rTextRenderer;													// To Display Changes as they are made. Only useful for Debug/Editor Mode so can be seen in inspector.
	public string dm_sTranslationDescription = "";                                                  // When Generating the Excel Document, this description will be added along with the English Translation Text, allowing you to inform the translators' of the context regarding this English Text.
	public bool dm_bShowMultiLanguageTextComponent = false;											// Used with the Foldout option in inspector to stop the whole Multi-Language Text Component stuff from showing. It's a fairly big chunk of stuff. Might as well hide it if not using it.
	public bool dm_bShowEnglishTranslation = false;                                                 // Used with the Foldout option in inspector to show English Translation when selecting any language besides English
	public bool dm_bShowTranslationDescription = false;                                             // Used with the Foldout option in inspector to show Translation Description Option in Inspector.

	public static int dsm_iCopiedFontSize = 120;
	public static float dsm_fCopiedLineSpacing = 1.0f;
	public static Vector3 dsm_vCopiedTextPosition = Vector3.zero;
	public static TextAlignment dsm_eCopiedTextAnchor = TextAlignment.Center;
#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public string EnglishTranslation	{ get { return m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text; } set { m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text = value; } }
	public string SelectedText			{ get { return GetText(GameManager.SelectedLanguage); } }
	public Font SelectedFont			{ get { return GetFont(GameManager.SelectedLanguage); } }
	public int SelectedFontSize			{ get { return GetFontSize(GameManager.SelectedLanguage); } }
	public float SelectedLineSpacing	{ get { return GetLineSpacing(GameManager.SelectedLanguage); } }
	public Vector3 SelectedFontPosition { get { return GetPosition(GameManager.SelectedLanguage); } }
	public TextAnchor SelectedAlignment { get { return GetTextAlignment(GameManager.SelectedLanguage); } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	[System.Serializable]
	public class TextDisplayValues
	{
		public Font chosenFont;
		public Vector3 fontPosition;
		public string text = "";
		public int fontSize = 0;
		public float lineSpacing = 1.0f;
		public TextAlignment textAlignment = TextAlignment.Center;
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Apply Effects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ApplyEffects(UnityEngine.UI.Text uiTextRenderer)
	{
		if (uiTextRenderer != null)
		{
			uiTextRenderer.font = SelectedFont;
			uiTextRenderer.text = SelectedText;
			uiTextRenderer.rectTransform.localPosition = SelectedFontPosition;
			int fontSize = SelectedFontSize;
			uiTextRenderer.resizeTextForBestFit = (fontSize == 0);
			uiTextRenderer.fontSize = fontSize;
			uiTextRenderer.alignment = GetTextAlignment(uiTextRenderer);

			float lineSpacing = SelectedLineSpacing;
            if(lineSpacing > 0.0f)
				uiTextRenderer.lineSpacing = lineSpacing;

			uiTextRenderer.horizontalOverflow = ((GameManager.SelectedLanguage == GameManager.SystemLanguages.ARABIC || GameManager.SelectedLanguage == GameManager.SystemLanguages.PERSIAN || GameManager.SelectedLanguage == GameManager.SystemLanguages.URDU) ? HorizontalWrapMode.Overflow : HorizontalWrapMode.Wrap);

		}
	}

	public void ApplyEffects(UnityEngine.UI.Text uiTextRenderer, GameManager.SystemLanguages selectedLanguage)
	{
		if (uiTextRenderer != null)
		{
			uiTextRenderer.font = GetFont(selectedLanguage);
			uiTextRenderer.text = GetText(selectedLanguage);
			uiTextRenderer.rectTransform.localPosition = GetPosition(selectedLanguage);
			int fontSize = GetFontSize(selectedLanguage);
			uiTextRenderer.resizeTextForBestFit = (fontSize == 0);
			uiTextRenderer.fontSize = fontSize;
			uiTextRenderer.alignment = GetTextAlignment(selectedLanguage, uiTextRenderer);

			float lineSpacing = GetLineSpacing(selectedLanguage);
			if (lineSpacing > 0.0f)
				uiTextRenderer.lineSpacing = lineSpacing;

			uiTextRenderer.horizontalOverflow = ((selectedLanguage == GameManager.SystemLanguages.ARABIC || selectedLanguage == GameManager.SystemLanguages.PERSIAN || selectedLanguage == GameManager.SystemLanguages.URDU) ? HorizontalWrapMode.Overflow : HorizontalWrapMode.Wrap);

		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Get Language Instance
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TextDisplayValues GetLanguageInstance()
	{
		return GetLanguageInstance(GameManager.SelectedLanguage);
	}

	private TextDisplayValues GetLanguageInstance(GameManager.SystemLanguages eSelectedLanguage)
	{
		if((int)eSelectedLanguage >= m_arLanguageText.Length || m_arLanguageText[(int)eSelectedLanguage] == null)
			return null;
		return m_arLanguageText[(int)eSelectedLanguage];
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private string GetText(GameManager.SystemLanguages eSelectedLanguage)
	{
		// Show Error if LanguageID doesn't exist or does not have a valid value (only if debugging). If not debugging, show English version... Better than nothing?!
		TextDisplayValues instance = GetLanguageInstance(eSelectedLanguage);
		if (instance == null)
		{
#if UNITY_EDITOR
			Debug.LogError(GameManager.SelectedLanguage.ToString() + " NOT Available");
			return (GameManager.SelectedLanguage.ToString() + " NOT Available");
#else
			if (m_arLanguageText.Length > (int)GameManager.SystemLanguages.ENGLISH && m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH] != null)
				return m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text;
			return (GameManager.SelectedLanguage.ToString() + " NOT Available");
#endif
		}
		return instance.text; 
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Font
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Font GetFont(GameManager.SystemLanguages eSelectedLanguage)
	{
		TextDisplayValues instance = GetLanguageInstance(eSelectedLanguage);
		if (instance != null)
		{
			return ((instance.chosenFont != null) ? instance.chosenFont : Resources.GetBuiltinResource<Font>("Arial.ttf"));
		}
		return Resources.GetBuiltinResource<Font>("Arial.ttf");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Get Text Alignment
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TextAnchor GetTextAlignment(UnityEngine.UI.Text uiTextRenderer = null)
	{
		return GetTextAlignment(GameManager.SelectedLanguage, uiTextRenderer);
	}

	public TextAnchor GetTextAlignment(GameManager.SystemLanguages eSelectedLanguage, UnityEngine.UI.Text uiTextRenderer = null)
	{
		TextDisplayValues instance = GetLanguageInstance(eSelectedLanguage);
		if(instance != null)
		{
			if(uiTextRenderer != null)
			{
				switch(uiTextRenderer.alignment)
				{
				case TextAnchor.UpperCenter: case TextAnchor.UpperLeft: case TextAnchor.UpperRight:
					switch(instance.textAlignment)
					{
						case TextAlignment.Center:	return TextAnchor.UpperCenter;
						case TextAlignment.Left:	return TextAnchor.UpperLeft;
						default:					return TextAnchor.UpperRight;
					}
				case TextAnchor.MiddleCenter: case TextAnchor.MiddleLeft: case TextAnchor.MiddleRight:
					switch(instance.textAlignment)
					{
						case TextAlignment.Center:	return TextAnchor.MiddleCenter;
						case TextAlignment.Left:	return TextAnchor.MiddleLeft;
						default:					return TextAnchor.MiddleRight;
					}
				default: // LOWER!
					switch(instance.textAlignment)
					{
						case TextAlignment.Center:	return TextAnchor.LowerCenter;
						case TextAlignment.Left:	return TextAnchor.LowerLeft;
						default:					return TextAnchor.LowerRight;
					}
				}
			}

			else
			{
				switch(instance.textAlignment)
				{
					case TextAlignment.Center:	return TextAnchor.UpperCenter;
					case TextAlignment.Left:	return TextAnchor.UpperLeft;
					default:					return TextAnchor.UpperRight;
				}
			}
		}

		return TextAnchor.UpperCenter;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get FontSize
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private int GetFontSize(GameManager.SystemLanguages eSelectedLanguage)
	{
		TextDisplayValues instance = GetLanguageInstance(eSelectedLanguage);
		if (instance != null)
		{
			return instance.fontSize;
		}
		else
		{
			if (m_arLanguageText.Length > (int)GameManager.SystemLanguages.ENGLISH && m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH] != null)
				return m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].fontSize;
			return 0;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Line Spacing
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private float GetLineSpacing(GameManager.SystemLanguages eSelectedLanguage)
	{
		TextDisplayValues instance = GetLanguageInstance(eSelectedLanguage);
		if(instance != null)
		{
			return instance.lineSpacing;
		}
		else
		{
			if(m_arLanguageText.Length > (int)GameManager.SystemLanguages.ENGLISH && m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH] != null)
				return m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].lineSpacing;
			return 0.0f;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Vector3 GetPosition(GameManager.SystemLanguages eSelectedLanguage)
	{
		TextDisplayValues instance = GetLanguageInstance(eSelectedLanguage);
		if (instance != null)
		{
			return instance.fontPosition;
		}
		return Vector3.zero;
	}
}



//===========================================================================
// *** Multi Language Text Display
//===========================================================================
public class MultiLanguageTextDisplay : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public MultiLanguageText m_oMultiLanguageText = new MultiLanguageText();



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Refresh
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Refresh()
	{
		UnityEngine.UI.Text rText = GetComponent<UnityEngine.UI.Text>();
		if(rText != null)
		{
			m_oMultiLanguageText.ApplyEffects(rText);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnEnable()
	{
		Refresh();
    }
}
