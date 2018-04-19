//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Quick Dialogue Pop-up Box
//             Author: Christopher Diamond
//             Date: January 24, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script animates a Dialogue Pop-up box that is meant to be displayed
//		only briefly. It will show a message and then fade away.
//	 It is in use primarily for whenever the user taps on a challenge that has
//		yet to be unlocked via feather count.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class QuickDialoguePopup : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public UnityEngine.UI.Image m_rBackgroundPanel;
	public UnityEngine.UI.Text m_rNotificationText;

	public QuickDialoguePopupManager m_rPopupDialogueManager;               // Parent Object holding this object. Will be switching parents here and there! Need to know this
	public AnimationEffect m_oFadeAwayAnimation = new AnimationEffect();
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private float TextOpacity
	{
		set
		{
			Color colour = m_rNotificationText.color;
			colour.a = value;
			m_rNotificationText.color = colour;
		}
	}
	
	

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if(m_oFadeAwayAnimation.UpdateAnimation())
		{
			DisableDialoguePopup();
		}
		else
		{
			TextOpacity = (1.0f - m_oFadeAwayAnimation.CompletionPercentage);
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Animation
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginAnimation(Transform tempParent)
	{
		transform.SetParent(tempParent, false);
		m_oFadeAwayAnimation.Setup(this.transform, imageRenderer: m_rBackgroundPanel);
		m_oFadeAwayAnimation.ShowFirstFrame();
		TextOpacity = 1.0f;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Disable Dialogue Popup
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void DisableDialoguePopup()
	{
		transform.SetParent(m_rPopupDialogueManager.transform, false);
		gameObject.SetActive(false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Disable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	/// <summary>
	/// Disable Dialogue Popup. We may be disabling the "current" parent of this dialogue pop-up. So make sure it's getting set back to the original
	/// manager if this is the case otherwise we will get astrange effect where it continues to pop-up when the non-manager parent regains focus.
	/// </summary>
	void OnDisable()
	{
		m_oFadeAwayAnimation.ForceFinish();
	}
}
