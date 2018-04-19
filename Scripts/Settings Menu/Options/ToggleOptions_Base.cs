//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Toggle Options - Base
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 21, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is simply a base class to handle operations associated with
//      toggling options in the system/game.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

[System.Serializable]
public class ToggleOptions_Base<T> : MonoBehaviour 
	where T : MonoBehaviour
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Public Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public bool m_bSavePreference = true;
    public string m_sSavedKeyPreference = "";

    public T[] m_arSelectableOptions = new T[1];
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*- Private Instance Variables
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private int m_iSelectedIndex = 0;
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	*+ Attr_Accessors
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected bool SelectedNextOption { get; private set; }

    protected int SelectedIndex
    {
        get { return m_iSelectedIndex; }
		set { ChangeSelectedIndex(value); }
    }

    protected T SelectedItem
    {
        get { return m_arSelectableOptions[m_iSelectedIndex]; }
    }



    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Derived Method: Start
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected virtual void Start()
    {
		LoadSavedPreference();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Select Next Option
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public virtual void SelectNextOption()
    {
		SelectedNextOption = true;
        SelectedIndex += 1;
		SavePreference();
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Select Previous Option
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public virtual void SelectPreviousOption()
    {
		SelectedNextOption = false;
        SelectedIndex -= 1;
		SavePreference();        
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Selected Index Changed
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnSelectedIndexChanged(int previousIndex)
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Load Saved Index
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnLoadSavedIndex(int previousIndex)
	{
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* New Method: Save Preference
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected void SavePreference()
    {
        if (m_bSavePreference)
        {
			SavedPreferenceTool.SaveInt(m_sSavedKeyPreference, SelectedIndex);
        }
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Load Saved Preference
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void LoadSavedPreference()
	{
		if (m_bSavePreference)
		{
			int newIndex = SavedPreferenceTool.GetInt(m_sSavedKeyPreference, m_iSelectedIndex);
			if (newIndex != m_iSelectedIndex)
			{
				int previousIndex = m_iSelectedIndex;
				m_iSelectedIndex = newIndex;
				OnLoadSavedIndex(previousIndex);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Change Selected Index
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ChangeSelectedIndex(int newIndex)
	{
		if (newIndex > m_arSelectableOptions.Length - 1)
			newIndex = 0;
		else if (newIndex < 0)
			newIndex = (m_arSelectableOptions.Length - 1);

		if (newIndex != m_iSelectedIndex)
		{
			int previousIndex = m_iSelectedIndex;
			m_iSelectedIndex = newIndex;
			OnSelectedIndexChanged(previousIndex);
		}
	}
}
