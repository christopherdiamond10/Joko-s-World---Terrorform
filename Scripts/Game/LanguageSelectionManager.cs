//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Language Selection Manager
//             Author: Christopher Diamond
//             Date: February 15, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script does manages the Language Selection Window. Including handling
//		Language Button Selection Input and disabling visibility options for
//		non-selected languages.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class LanguageSelectionManager : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public MultiLanguageTextDisplay[] m_arVisibleMultiLanguageText = new MultiLanguageTextDisplay[1];
	public Button_LanguageSelectionToggle[] m_arLanguageSelectionButtons = new Button_LanguageSelectionToggle[(int)GameManager.SystemLanguages.TOTAL_LANGUAGES];
	public bool[] m_abAvailableLanguages = new bool[(int)GameManager.SystemLanguages.TOTAL_LANGUAGES];

	public Sprite m_rTickSprite;
	public Sprite m_rCheckboxSprite;

	public Color m_cHighlightedColour = new Color32(174, 112, 32, 255);
	public Color m_cNormalColour = new Color32(101, 65, 27, 255);
	public Color m_cUnavailableColour = new Color32(28, 28, 28, 255);
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Select Language
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SelectLanguage(Button_LanguageSelectionToggle a_rSelectedLanguageButton)
	{
		GameManager.SelectedLanguage = a_rSelectedLanguageButton.m_eLanguageSelection;
		for(int i = 0; i < m_arLanguageSelectionButtons.Length; ++i)
		{
			if (m_arLanguageSelectionButtons[i] != null)
			{
				if (m_arLanguageSelectionButtons[i] == a_rSelectedLanguageButton)
				{
					if (m_arLanguageSelectionButtons[i].m_rCheckBoxRenderer != null)
						m_arLanguageSelectionButtons[i].m_rCheckBoxRenderer.sprite = m_rTickSprite;

					if (m_arLanguageSelectionButtons[i].m_rButtonBackground != null)
						m_arLanguageSelectionButtons[i].m_rButtonBackground.material.SetColor("_Colour", m_cHighlightedColour);
				}
				else
				{
					if (m_arLanguageSelectionButtons[i].m_rCheckBoxRenderer != null)
						m_arLanguageSelectionButtons[i].m_rCheckBoxRenderer.sprite = m_rCheckboxSprite;

					if (m_arLanguageSelectionButtons[i].m_rButtonBackground != null)
					{
						if (m_abAvailableLanguages[i])
							m_arLanguageSelectionButtons[i].m_rButtonBackground.material.SetColor("_Colour", m_cNormalColour);
						else
							m_arLanguageSelectionButtons[i].m_rButtonBackground.material.SetColor("_Colour", m_cUnavailableColour);
					}
				}
			}
		}


		if(m_arVisibleMultiLanguageText != null)
		{
			for(int i = 0; i < m_arVisibleMultiLanguageText.Length; ++i)
			{
				if(m_arVisibleMultiLanguageText[i] != null)
					m_arVisibleMultiLanguageText[i].Refresh();
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is Language Available
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsLanguageAvailable(GameManager.SystemLanguages eSelectedLanguage)
	{
		return m_abAvailableLanguages[(int)eSelectedLanguage];
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* CallBack Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnEnable()
	{
		for(int i = 0; i < m_arLanguageSelectionButtons.Length; ++i)
		{
			if (m_arLanguageSelectionButtons[i] != null)
			{
				if (m_arLanguageSelectionButtons[i].m_eLanguageSelection == GameManager.SelectedLanguage)
				{
					SelectLanguage(m_arLanguageSelectionButtons[i]);
					return;
				}
			}
		}

		// Otherwise
		if (m_arLanguageSelectionButtons[0] != null)
			SelectLanguage(m_arLanguageSelectionButtons[0]);
	}
}
