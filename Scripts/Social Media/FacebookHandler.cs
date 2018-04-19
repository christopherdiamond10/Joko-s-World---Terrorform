//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Facebook Handler
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: March 06, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script integrates a Facebook Sharing Button. Allowing you to set the
//		Captions & Images in the process.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Facebook.Unity;


using FBProfileDictionary = System.Collections.Generic.Dictionary<string, object>;
using FBProfile = System.Collections.Generic.Dictionary<string, string>;



public class FacebookHandler : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public MultiLanguageText m_oShareLinkName = new MultiLanguageText();
	public MultiLanguageText m_oShareLinkCaption = new MultiLanguageText();
	public string m_sShareLinkImageURL = "http://";
	public bool m_bUseCustomUrl = false;
	public string m_sCustomUrlLink = "http://";

	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Awake()
	{
		base.Awake();
		if(!FB.IsInitialized)
		{
			FacebookHandler.InitialiseFacebook();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Button Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();
		ShareAppOnFacebook();
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Log Into Facebook
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void LogIntoFacebook(bool insertShareWindow = false)
	{
		ButtonManager.ToggleAllButtons(false);
		if (insertShareWindow)
		{
			FB.Login("publish_actions", OnFacebookLoginAuthenticationWithShare);
			//FB.LogInWithPublishPermissions(new List<string>() { "publish_actions", "public_profile" } , OnFacebookLoginAuthenticationWithShare);
		}
		else
		{
			FB.Login("publish_actions", OnFacebookLoginAuthentication);
			//FB.LogInWithPublishPermissions(new List<string>() { "publish_actions", "public_profile" }, OnFacebookLoginAuthentication);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Share App On Facebook
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ShareAppOnFacebook()
	{
		// Run Parental GATE on Android/iOS Devices!
#if !UNITY_EDITOR && UNITY_IOS
		Floop.FloopManager.Instance.ShowParentalGate((success) => 
		{
			if(success)
			{
#endif
				if(FB.IsLoggedIn)
				{
					FB.Feed(
								linkName: m_oShareLinkName.SelectedText,
								linkCaption: m_oShareLinkCaption.SelectedText,
								picture: m_sShareLinkImageURL,
								link: (m_bUseCustomUrl ? m_sCustomUrlLink : "http://apps.facebook.com/" + FB.AppId + "/?challenge_brag=" + FB.UserId),
								callback: OnFacebookFeed
							);
					//FB.FeedShare(
					//			linkName: m_sShareLinkName,
					//			linkCaption: m_sShareLinkCaption,
					//			picture: new System.Uri(m_sShareLinkImageURL),
					//			link: new System.Uri((m_bUseCustomUrl ? m_sCustomUrlLink : "http://apps.facebook.com/" + FB.AppId + "/?challenge_brag=" + AccessToken.CurrentAccessToken.UserId)),
					//			callback: OnFacebookFeed
					//		);
				}
				else
				{
					LogIntoFacebook(true);
				}
#if !UNITY_EDITOR && UNITY_IOS
			}
			else
			{
			}
		});
#endif
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Callback Method: Initialise Facebook
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void InitialiseFacebook()
	{
		if (!FB.IsInitialized)
			FB.Init(OnFinishedFBInit, OnHideUnity);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Callback Method: On Hide Unity
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static void OnHideUnity(bool bUnityIsActive)
	{
		// If Facebook credentials are active, Pause unity. Otherwise make Unity Active again
		if (bUnityIsActive)
		{
			Time.timeScale = 1.0f;
			ButtonManager.ToggleAllButtons(ButtonManager.ButtonType.SETTINGS, true);
		}
		else
		{
			Time.timeScale = 0.0f;
			ButtonManager.ToggleAllButtons(ButtonManager.ButtonType.SETTINGS, false);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Callback Method: On Facebook Initialise
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static void OnFinishedFBInit()
	{
		if (FB.IsLoggedIn)
		{
		}
		else
		{
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Callback Methods: On Facebook Login Authentication
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void OnFacebookLoginAuthentication(FBResult result)
	//private void OnFacebookLoginAuthentication(ILoginResult result)
	{
		if (result.Error != null)
		{
			Debug.Log("There was an error logging into Facebook!");
			//LogIntoFacebook();	// Try again?!
			//return;				// Break out of the recursive loop!
		}
		ButtonManager.ToggleAllButtons(true);
	}

	private void OnFacebookLoginAuthenticationWithShare(FBResult result)
	//private void OnFacebookLoginAuthenticationWithShare(ILoginResult result)
	{
		if (result.Error != null)
		{
			Debug.Log("There was an error logging into Facebook!");
		}
		//if (result.Error != null)
		//{
		//	Debug.Log("There was an error logging into Facebook!");
		//	LogIntoFacebook(true);	// Try again?!
		//	return;					// Break out of the recursive loop!
		//}

		ButtonManager.ToggleAllButtons(true);
		//if (result.Error != null)
		//{
		//    if (FB.IsLoggedIn)
		//    {
		//        ShareAppOnFacebook();
		//    }
		//}
	}

	private void OnFacebookFeed(FBResult result)
	//private void OnFacebookFeed(IShareResult result)
	{
		if (result.Error != null)
		{
			Debug.Log("There was an error logging into Facebook!");
		}
		OnHideUnity(true);
	}
}