//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Game Manager
//             Author: Christopher Diamond
//             Date: November 12, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script maintains important values to the system. Including whether or
//		not the game is the lite/full version and the chosen language of the system.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public BuildVersion m_eBuildVersion = BuildVersion.LITE_VERSION;

#if UNITY_EDITOR
	public string[] dm_asReleasedAppName	 = new string[1] { "Instrument - Joko's World" };
	public string[] dm_asLiteVersionBundleID = new string[1] { "culturalinfusion.freeversion.jokostambourine", };
	public string[] dm_asFullVersionBundleID = new string[1] { "culturalinfusion.fullversion.jokostambourine", };

	public Sprite dm_sprLiteVersionAppIcon;
	public Sprite dm_sprFullVersionAppIcon;

	public SystemLanguages m_eSystemLanguages = SystemLanguages.ENGLISH;
	public DisplayableLanguagesTypes dm_eDisplayableLanguagesTypes = DisplayableLanguagesTypes.ALL_LANGUAGES;
	public GameObject[] m_agoRootParentsOfMultiLanguageTextComponents = new GameObject[1];
#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Static Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static BuildVersion sm_eBuildVersion = BuildVersion.LITE_VERSION;
	private static SystemLanguages sm_eSystemLanguages = SystemLanguages.ENGLISH;

	private static bool sm_bGameModeLocked = false;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static bool				IsFullVersion		{ get { return CurrentGameMode != BuildVersion.LITE_VERSION; } }
	public static SystemLanguages	SelectedLanguage	{ get { return sm_eSystemLanguages; } set { SetCurrentLanguage(value); } }
	public static int				SelectedLanguageID	{ get { return (int)sm_eSystemLanguages; } }
	public static SubSceneManager	CurrentSubscene		{ get; set; }
	public static DisplayableLanguagesTypes DisplayableLanguages
	{
#if URDU_LANGUAGE_ONLY
		get { return DisplayableLanguagesTypes.URDU_ONLY; }
#else
		get { return DisplayableLanguagesTypes.ALL_LANGUAGES; }
#endif
	}

	public static BuildVersion CurrentGameMode
	{
		get { return sm_eBuildVersion; }
		set { if (!sm_bGameModeLocked) { sm_eBuildVersion = value; sm_bGameModeLocked = true; } }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum BuildVersion
	{
		LITE_VERSION,
		FULL_VERSION,
		DEVELOPER_MODE,
	}

	public enum SystemLanguages
	{
		ENGLISH = 0,
		ARABIC,			
		CHINESE_SIMPLIFIED,
		FRENCH,
		GAELIC_SCOTTISH,			
		GERMAN,			
		INDONESIAN,		
		ITALIAN,			
		JAPANESE,		
		PERSIAN,	
		PORTUGUESE_BZ,	
		PORTUGUESE_PT,
		RUSSIAN,			
		SPANISH,	
		URDU,
		HINDI,		

		TOTAL_LANGUAGES,
	}


	public enum DisplayableLanguagesTypes
	{
		ALL_LANGUAGES,
		URDU_ONLY,

		TOTAL_OPTIONS_COUNT
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake()
	{
		CurrentGameMode = m_eBuildVersion;
		Application.targetFrameRate = 60;

#if UNITY_EDITOR // Running in Debug Mode?
		OnDebugRun();
#else           // Running on Android/iOS Device?
		OnReleaseRun();
#endif
	}
#if UNITY_EDITOR
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Debug Run
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnDebugRun()
	{
		if(m_eSystemLanguages != SystemLanguages.ENGLISH)
			sm_eSystemLanguages = m_eSystemLanguages;
		else
			m_eSystemLanguages = (SystemLanguages)Mathf.Clamp(SavedPreferenceTool.GetInt("SelectedLanguage", (int)GetRecommendedLanguage()), (int)SystemLanguages.ENGLISH, (int)SystemLanguages.TOTAL_LANGUAGES - 1);
	}
#else
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Release Run
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnReleaseRun()
	{
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		Screen.SetResolution(425, 680, false);
#endif
		FacebookHandler.InitialiseFacebook();
		sm_eSystemLanguages = (SystemLanguages)Mathf.Clamp(SavedPreferenceTool.GetInt("SelectedLanguage", (int)GetRecommendedLanguage()), (int)SystemLanguages.ENGLISH, (int)SystemLanguages.TOTAL_LANGUAGES - 1);
	}
#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Set Current Language
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static void SetCurrentLanguage(SystemLanguages eLanguage)
	{
		sm_eSystemLanguages = eLanguage;
		SavedPreferenceTool.SaveInt("SelectedLanguage", (int)sm_eSystemLanguages);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Method: Get Recommended Language
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static SystemLanguages GetRecommendedLanguage()
	{
		switch (DisplayableLanguages)
		{
			case DisplayableLanguagesTypes.URDU_ONLY:
				return SystemLanguages.URDU;

			default:
				switch (Application.systemLanguage)
				{
					case SystemLanguage.Arabic:			return SystemLanguages.ARABIC;
					case SystemLanguage.Chinese: case SystemLanguage.ChineseSimplified: case SystemLanguage.ChineseTraditional: return SystemLanguages.CHINESE_SIMPLIFIED;
					case SystemLanguage.French:			return SystemLanguages.FRENCH;
					case SystemLanguage.German:			return SystemLanguages.GERMAN;
					case SystemLanguage.Indonesian:		return SystemLanguages.INDONESIAN;
					case SystemLanguage.Italian:		return SystemLanguages.ITALIAN;
					case SystemLanguage.Japanese:		return SystemLanguages.JAPANESE;
					case SystemLanguage.Portuguese:		return SystemLanguages.PORTUGUESE_BZ;
					case SystemLanguage.Russian:		return SystemLanguages.RUSSIAN;
					case SystemLanguage.Spanish:		return SystemLanguages.SPANISH;
					default:							return SystemLanguages.ENGLISH;
				}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Methods: Get Language As String
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static string GetLanguageAsEnglishString(SystemLanguages eLanguage)
	{
		switch (eLanguage)
		{
			case SystemLanguages.ARABIC:				return "Arabic";
			case SystemLanguages.CHINESE_SIMPLIFIED:	return "Chinese";
			case SystemLanguages.FRENCH:				return "French";
			case SystemLanguages.GAELIC_SCOTTISH:		return "Gaelic";
			case SystemLanguages.GERMAN:				return "German";
			case SystemLanguages.INDONESIAN:			return "Indonesian";
			case SystemLanguages.ITALIAN:				return "Italian";
			case SystemLanguages.JAPANESE:				return "Japanese";
			case SystemLanguages.PERSIAN:				return "Persian";
			case SystemLanguages.PORTUGUESE_BZ:			return "Portuguese (BZ)";
			case SystemLanguages.PORTUGUESE_PT:			return "Portuguese (PT)";
			case SystemLanguages.RUSSIAN:				return "Russian";
			case SystemLanguages.SPANISH:				return "Spanish";
			case SystemLanguages.URDU:					return "Urdu";
			case SystemLanguages.HINDI:					return "Hindi";
			default:									return "English";
		}
	}

	public static string GetLanguageAsNativeLanguageString(SystemLanguages eLanguage)
	{
		switch (eLanguage)
		{
			case SystemLanguages.ARABIC:				return "العربية";
			case SystemLanguages.CHINESE_SIMPLIFIED:	return "中文";
			case SystemLanguages.FRENCH:				return "Français";
			case SystemLanguages.GAELIC_SCOTTISH:		return "Gàidhlig";
			case SystemLanguages.GERMAN:				return "Deutsche";
			case SystemLanguages.INDONESIAN:			return "Bahasa Indonesia";
			case SystemLanguages.ITALIAN:				return "Italiano";
			case SystemLanguages.JAPANESE:				return "日本語";
			case SystemLanguages.PERSIAN:				return "فارسي";
			case SystemLanguages.PORTUGUESE_BZ:			return "Português (BZ)";
			case SystemLanguages.PORTUGUESE_PT:			return "Português (PT)";
			case SystemLanguages.RUSSIAN:				return "Pусский";
			case SystemLanguages.SPANISH:				return "Español";
			case SystemLanguages.URDU:					return "اُردُو";
			case SystemLanguages.HINDI:					return "हिन्दी";
			default:									return "English";
		}
	}
}
