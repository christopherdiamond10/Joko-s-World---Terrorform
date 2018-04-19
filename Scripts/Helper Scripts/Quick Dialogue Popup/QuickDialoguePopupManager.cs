//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Quick Dialogue Popup Manager
//             Author: Christopher Diamond
//             Date: January 24, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used to interact with the Quick Dialogue Popup Boxes, telling
//		them where to animate.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class QuickDialoguePopupManager : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject m_goQuickDialoguePopupPrefab;
	public int m_iStartingQuickDialoguePopups = 15;
		
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		for (int i = 0; i < m_iStartingQuickDialoguePopups; ++i)
			CreateQuickDialoguePopupBox();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Quick Dialogue Pop-up
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ShowQuickDialoguePopup(Transform dialogueBoxParent)
	{
		GameObject quickDialoguePopupBox = GetFreeQuickDialoguePopupBox();

		quickDialoguePopupBox.SetActive(true);
		quickDialoguePopupBox.GetComponent<QuickDialoguePopup>().BeginAnimation(dialogueBoxParent);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Quick Dialogue Popup
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private GameObject CreateQuickDialoguePopupBox()
	{
		if (m_goQuickDialoguePopupPrefab != null)
		{
			GameObject newQuickDialoguePopup = Instantiate(m_goQuickDialoguePopupPrefab) as GameObject;
			newQuickDialoguePopup.transform.SetParent(this.transform, false);
			newQuickDialoguePopup.GetComponent<QuickDialoguePopup>().m_rPopupDialogueManager = this;
			newQuickDialoguePopup.SetActive(false);
			return newQuickDialoguePopup;
		}
		return null;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Free Quick Dialogue Popup Box
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private GameObject GetFreeQuickDialoguePopupBox()
	{
		if(transform.childCount > 0)
			return transform.GetChild(0).gameObject;

		return CreateQuickDialoguePopupBox();
	}
}
