//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Button - Range Slider
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: December 4, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is handles the input for when the player clicks and holds on the
//      Slider Symbol. It includes the code to handle the movement of the slider
//      and the boundaries of the slider.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Button_RangeSlider : Button_Base
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool m_bSaveOption = false;
	public string m_sSavedOptionKey = "";
	public float m_fMinPos;
	public float m_fMaxPos;
	public string m_sLabel;
	public Text m_rTextObj;
	public ObjectRangeSlider_Base m_rObjRangeSliderEffect;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	float SliderPosition
	{
		get { return transform.localPosition.x; }
		set { SetLocalPosition(value); }
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Start()
	{
		base.Start();
		KeepPressedEvenWithNoContact = true;

		// If saving the slider into an external save system
		if (m_bSaveOption)
		{
			SetPosition(SavedPreferenceTool.GetFloat(m_sSavedOptionKey, GetPercentage()));
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void Update()
	{
		// Update without button release, this will let the Player touch the button and move the slider whilst their finger isn't directly over the button itself...
		//				but instead ANYWHERE on the screen.
		//base.UpdateSingleTouchWithoutButtonRelease();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Touch
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnTouch()
	{
		if(Triggered)
	        base.OnTouch();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Held
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnHeld()
	{
		base.OnHeld();
		float currentPercentage = GetPercentage();

		// Update Slider Position based on touch
		SetWorldPosition(TouchPosition.x);
		if (SliderPosition < m_fMinPos)
		{
			SliderPosition = m_fMinPos;
		}
		else if(SliderPosition > m_fMaxPos)
		{
			SliderPosition = m_fMaxPos;
		}

		// Set the value of the slider to whatever needs to know about it
		m_rObjRangeSliderEffect.SetValue(GetPercentage());


		// If Slider Position Has Changed. Show updated text and save to storage if asked to do so.
		if (currentPercentage != GetPercentage())
		{
			if (m_rTextObj != null)
			{
				m_rTextObj.text = m_sLabel + ((int)(GetPercentage() * 100.0f)).ToString() + "%";
			}
		}
	}
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //	* Derived Method: On Release
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected override void OnRelease()
    {
        base.OnRelease();
        
        if (m_bSaveOption)
        {
			SavedPreferenceTool.SaveFloat(m_sSavedOptionKey, GetPercentage());
        }
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Position
	//----------------------------------------------------
	//	: Uses a percentage out of 0-1 to determine where
	//	  the slider should be
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetPosition(float Percentage)
	{
		Vector3 Pos = transform.localPosition;
		Pos.x = Mathf.Lerp(m_fMinPos, m_fMaxPos, Percentage);
		transform.localPosition = Pos;

		if (m_rTextObj != null)
		{
			m_rTextObj.text = m_sLabel + Mathf.CeilToInt(GetPercentage() * 100.0f).ToString() + "%";
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Local Position
	//----------------------------------------------------
	//	: Uses a value to just throw the slider position
	//	  to a location. Does NOT check boundaries.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetLocalPosition(float Zone)
	{
		Vector3 Pos = transform.localPosition;
		Pos.x = Zone;
		transform.localPosition = Pos;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set World Position
	//----------------------------------------------------
	//	: Uses a value to just throw the slider position
	//	  to a location. Does NOT check boundaries.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void SetWorldPosition(float Zone)
	{
		Vector3 Pos = transform.position;
		Pos.x = Zone;
		transform.position = Pos;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Percentage
	//----------------------------------------------------
	//	: Gets the percentage from the leftmost point on the
	//	  slider to the rightmost, returning 0 for left most
	//	  and 1 for rightmost.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float GetPercentage()
	{
		float Total		= (m_fMaxPos * 2);
		float Current	= (m_fMinPos - SliderPosition);
		return Mathf.Abs(Current / Total);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived method: On Enable
	//----------------------------------------------------
	//	: Checks to make sure that the slider is still in 
	//		the correct position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void OnEnable()
	{
		base.OnEnable();

		// Place into correct area of slider
		if (m_bSaveOption)
		{
			SetPosition(SavedPreferenceTool.GetFloat(m_sSavedOptionKey, GetPercentage()));
		}
	}
}
