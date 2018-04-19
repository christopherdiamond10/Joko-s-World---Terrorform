//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Movement Via Circle
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: July 31, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This Script allows you to move an object as if it were rotation around a
//		circle. Also giving you constraints so that it doesn't move out of bounds
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class MovementViaCircle : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool m_bMoveHorizontally = true;
	public bool m_bMoveVertically = true;
	public bool m_bStartWithOppositeDirection = false;

	public float m_fHorizontalSpeed = 1.0f;
	public float m_fVerticalSpeed = 1.0f;

	public float m_fHorizontalMovementBoundary = 5.0f;
	public float m_fVerticalMovementBoundary = 5.0f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private Vector3 m_vStartPosition;
	private TransformInterpreter m_tiInstance;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Attr Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private TransformInterpreter.LocalPositionInterpreter LocalPosition { get { return m_tiInstance.LocalPosition; } }



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start () 
	{
		m_tiInstance = new TransformInterpreter(this);
		m_vStartPosition = LocalPosition;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update () 
	{
		if (m_bMoveHorizontally)
		{
			LocalPosition.x = (m_vStartPosition.x + (Mathf.Cos(Time.realtimeSinceStartup * m_fHorizontalSpeed) * (m_bStartWithOppositeDirection ? -m_fHorizontalMovementBoundary : m_fHorizontalMovementBoundary)));
		}

		if (m_bMoveVertically)
		{
			LocalPosition.y = (m_vStartPosition.y + (Mathf.Sin(Time.realtimeSinceStartup * m_fVerticalSpeed) * (m_bStartWithOppositeDirection ? -m_fVerticalMovementBoundary : m_fVerticalMovementBoundary)));
		}
	}
}
