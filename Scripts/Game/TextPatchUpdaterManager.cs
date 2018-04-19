//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Text Patch Updater Manager
//             Author: Christopher Diamond
//             Date: November 19, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script gets data from a txt file online which may contain updated text
//		to be displayed (among perhaps other things like font size or positions if
//		deemed necessary, though most likely not).
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class TextPatchUpdaterManager : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static string sm_sTextURL = "http://www.christopherdiamond.net/files/Jokos-World/TambourineAppEditables.txt";
	private static string sm_sResultingText = "";
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static string SelectedLanguage { get { return GameManager.GetLanguageAsEnglishString(GameManager.SelectedLanguage); } }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum PatchSelection
	{
		FACEBOOK_TITLE,
		FACEBOOK_CAPTION,
		FACEBOOK_LINK,
	}
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake()
	{
		StartCoroutine(ReceiveUpdatedTextPatch());
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Receive Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private IEnumerator ReceiveUpdatedTextPatch()
	{
		WWW www = new WWW(sm_sTextURL);
		yield return www;
		sm_sResultingText = www.text;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Updated Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static string GetUpdatedText(PatchSelection ePatchSelection, string defaultText = "")
	{
		// Has updated text from latest text file?
		if (sm_sResultingText != "")
		{
			switch (ePatchSelection)
			{
				case PatchSelection.FACEBOOK_TITLE:		return GetRegexMatch("FacebookTitle" + SelectedLanguage + "=\"(.+)\"", defaultText);
				case PatchSelection.FACEBOOK_CAPTION:	return GetRegexMatch("FacebookCaption" + SelectedLanguage + "=\"(.+)\"", defaultText);
				case PatchSelection.FACEBOOK_LINK:		return GetRegexMatch("FacebookLink" + SelectedLanguage + "=\"(.+)\"", defaultText);
				default:								return defaultText;
			}
		}

		// Don't have anything for you. Whatever was provided will have to do.
		else
		{
			return defaultText;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Regex Match
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static string GetRegexMatch(string regexMatchPattern, string defaultText = "")
	{
		Match match = Regex.Match(sm_sResultingText, regexMatchPattern, RegexOptions.IgnoreCase);
		if (match.Success)
		{
			return match.Groups[1].Value;
		}
		return defaultText;
	}
}
