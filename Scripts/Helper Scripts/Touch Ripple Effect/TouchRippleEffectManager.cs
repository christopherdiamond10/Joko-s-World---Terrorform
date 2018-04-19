//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Touch Ripple Effect Manager
//             Author: Christopher Diamond
//             Date: November 05, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script manages the Touch Ripple Effects. It also handles touches on
//		the screen. Whenever a touch is registered this script will inform itself
//		about it and show a Touch Ripple Effect where needed.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TouchRippleEffectManager : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public GameObject m_goTouchRipplePrefab;
	public int m_iStartingTouchRippleEffects = 15;



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		for (int i = 0; i < m_iStartingTouchRippleEffects; ++i)
			CreateTouchRippleEffect();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
			BeginTouchRippleEffect( Camera.main.ScreenToWorldPoint(Input.mousePosition) );
#else
			for (int i = 0; i < Input.touchCount; ++i)
			{
				if (Input.GetTouch(i).phase == TouchPhase.Began)
				{
					BeginTouchRippleEffect( Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position) );
				}
			}
#endif
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Touch Ripple Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void BeginTouchRippleEffect(Vector3 atPosition)
	{
		GameObject touchRippleEffect = GetFreeTouchRippleEffect();
		atPosition.z += 10; // Just to put it back into view space.
		touchRippleEffect.transform.position = atPosition;
		touchRippleEffect.SetActive(true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Create Touch Ripple Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private GameObject CreateTouchRippleEffect()
	{
		if (m_goTouchRipplePrefab != null)
		{
			GameObject newTouchRippleEffect = Instantiate(m_goTouchRipplePrefab) as GameObject;
			newTouchRippleEffect.transform.parent = this.transform;
			newTouchRippleEffect.SetActive(false);
			return newTouchRippleEffect;
		}
		return null;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Free Touch Ripple Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private GameObject GetFreeTouchRippleEffect()
	{
		foreach(Transform child in transform)
		{
			if(!child.gameObject.activeInHierarchy && child.GetComponent<TouchRippleEffect>() != null)
				return child.gameObject;
		}

		return CreateTouchRippleEffect();
	}
}
