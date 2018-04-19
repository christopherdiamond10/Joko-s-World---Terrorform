//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Open Url Confirmation Window
//             Author: Christopher Diamond
//             Date: November 06, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This button, when clicked, will transfer it's URL data over to a confirmation
//		window which asks the user if they are sure they wish to leave the app.
//	  This is intended to comply with the app/play store guidelines for the 
//		childrens section.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class Button_OpenUrlConfirmation : Button_Base 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public Button_OpenURL m_rUrlButton;

	public SubSceneManager m_rOwningScene;
	public ObjectTransitionAnimation m_rOwningPage;

	public MultiLanguageText m_oURLConfrimationDescription = new MultiLanguageText();
	public string m_sAndroidURL = "";
	public string m_sAppleURL = "";
	public Button_OpenURL.URLType m_eURLType = Button_OpenURL.URLType.APPLE_AND_ANDROID;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*-- Debug Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
	public ObjectTransitionAnimation m_rConfirmationWindowDisplay;
	public UnityEngine.UI.Text m_rTextDisplay;
#endif
	

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Trigger
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTrigger()
	{
		base.OnTrigger();

		if (m_rUrlButton != null)
			m_rUrlButton.OpenConfirmationWindow(this);
	}
}
