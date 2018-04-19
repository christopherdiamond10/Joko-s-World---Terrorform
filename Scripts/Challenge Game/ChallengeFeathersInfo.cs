//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Challenge Feathers Info
//             Author: Christopher Diamond
//             Date: October 16, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script informs the user regarding the amount of feathers that they 
//		have accumulated. It also spins the feather around to give it more 
//		attention.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class ChallengeFeathersInfo : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public SpriteRenderer m_sprNotificationFeather;
	public UnityEngine.UI.Text m_rNotificationText;
	public float m_fFeatherRotationSpeed = 1.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static int PreviouslyAccumulatedFeathers
	{
		get { return SavedPreferenceTool.GetInt("PreviouslyAccumulatedFeathers", SavedPreferenceTool.GetInt("AccumulatedChallengeFeathers")); }
	}

	public static int AccumulatedFeathers
	{
		get { return SavedPreferenceTool.GetInt("AccumulatedChallengeFeathers"); }
		set { SavedPreferenceTool.SaveInt("PreviouslyAccumulatedFeathers", AccumulatedFeathers); SavedPreferenceTool.SaveInt("AccumulatedChallengeFeathers", value); }
	}

	public static int NewlyObtainedFeathers
	{
		get;
		set;
	}

	private float FeatherRotation
	{
		get { return m_sprNotificationFeather.transform.localRotation.eulerAngles.y; }
		set { m_sprNotificationFeather.transform.localRotation = Quaternion.Euler(m_sprNotificationFeather.transform.localRotation.eulerAngles.x, value, m_sprNotificationFeather.transform.localRotation.eulerAngles.z); }
	}
	
	
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		LoadAccumulatedFeathers();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if (m_sprNotificationFeather != null)
		{
			FeatherRotation += m_fFeatherRotationSpeed * Time.deltaTime;
			if (FeatherRotation > 360.0f)
				FeatherRotation -= 360.0f;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: On Enable
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void OnEnable()
	{
		LoadAccumulatedFeathers();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Load Accumulated Feathers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void LoadAccumulatedFeathers()
	{
		if (m_rNotificationText != null)
		{
			int iFeathersCount = AccumulatedFeathers;
			m_rNotificationText.text = "x" + (iFeathersCount < 10 ? "0" : "") + iFeathersCount.ToString();
		}
	}
}
