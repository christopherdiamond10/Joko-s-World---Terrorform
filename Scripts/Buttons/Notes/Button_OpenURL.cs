//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Open URL
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: November 27, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is handles the input for when the player clicks an button 
//      which will open a url in a web browser (PC/Mobile Devices)
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_OpenURL : Button_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ObjectTransitionAnimation m_rOwningPage;
	public ObjectTransitionAnimation m_rConfirmationWindowDisplay;
	public UnityEngine.UI.Text m_rTextDisplay;
	public Button_ReturnToSubsceneArea m_rDoNotOpenURLButton;

	public VignetteManager.VignetteInfo m_oVignetteInfo = new VignetteManager.VignetteInfo();

	public string androidURL = "";
	public string URL = "";
	public URLType m_eType = URLType.APPLE_AND_ANDROID;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private SubSceneManager m_rPreviousScene;
	private ObjectTransitionAnimation m_rPreviousNotePage;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum URLType
	{
		APPLE_AND_ANDROID,
		APPLE_OR_ANDROID,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		// Run Parental GATE on Android/iOS Devices!
#if !UNITY_EDITOR && UNITY_IOS
		Floop.FloopManager.Instance.ShowParentalGate((success) => 
		{
			if(success)
			{
#endif
				// If URL is to be used for both devices, use it:
				if(m_eType == URLType.APPLE_AND_ANDROID)
				{
					if(URL != "")
						Application.OpenURL(URL);
				}

				// Otherwise open the appropriate URL
				else
				{
#if UNITY_ANDROID
					if (androidURL != "")
						Application.OpenURL(androidURL);
#else
					if(URL != "")
						Application.OpenURL(URL);
#endif
				}
#if !UNITY_EDITOR && UNITY_IOS
			}
			else
			{
			}
		});
#endif



		if(m_rOwningPage != null)
		{
			m_rOwningPage.Disappear(false);
        }

		// Show Previous Scene/Note
		if(m_rPreviousScene != null)
		{
			if(m_rPreviousScene.IsSceneActive)
				m_rPreviousScene.ShowSceneVignette();
			else
				m_rPreviousScene.ShowSubscene();
		}
		if(m_rPreviousNotePage != null)
		{
			m_rPreviousNotePage.Reveal(false);

        }

		// Hide vignette since no one will take ownership~!
		if(m_rPreviousScene == null)
			VignetteManager.TransitionVignette(0.0f, 0.25f);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Open Confirmation Window
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void OpenConfirmationWindow(Button_OpenUrlConfirmation rOpenURLConfirmation)
	{
		OpenConfirmationWindow(rOpenURLConfirmation.m_eURLType, rOpenURLConfirmation.m_sAndroidURL, rOpenURLConfirmation.m_sAppleURL, rOpenURLConfirmation.m_oURLConfrimationDescription, rOpenURLConfirmation.m_rOwningScene, rOpenURLConfirmation.m_rOwningPage);
	}

	public void OpenConfirmationWindow(URLType eURLType, string androidURLWeblink, string appleURLWeblink, MultiLanguageText confirmationDescription, SubSceneManager previousScene = null, ObjectTransitionAnimation previousNotePage = null)
	{
		if(m_rConfirmationWindowDisplay != null)
		{
			// Hide whatever notes are currently active (if applicable)
			if (ObjectTransitionAnimation.CurrentlyActive != null)
			{
				ObjectTransitionAnimation.CurrentlyActive.Disappear(false);
			}

			// Reveal Confirmation Window & Show URL Confirmation Description
			m_rConfirmationWindowDisplay.Reveal();
			if (m_rTextDisplay != null && confirmationDescription != null)
			{
				confirmationDescription.ApplyEffects( m_rTextDisplay );
			}

			// Show Background Vignette
			VignetteManager.TransitionVignette(m_oVignetteInfo);

			// Assign URL variables as provided.
			m_eType		= eURLType;
			androidURL	= androidURLWeblink;
			URL			= appleURLWeblink;

			m_rPreviousScene = previousScene;
			m_rPreviousNotePage = previousNotePage;

			if(m_rDoNotOpenURLButton != null)
			{
				m_rDoNotOpenURLButton.PreviousScene = previousScene;
				m_rDoNotOpenURLButton.PreviousNotePage = previousNotePage;
            }
        }
	}
}
