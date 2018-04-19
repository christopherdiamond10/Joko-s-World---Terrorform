//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Notes Movement
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 29, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used to both Move & Zoom any page (texture) that displays 
//	  notes. Such as the Cultural Notes and the Credits Page.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class NotesMovement : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public float m_fMovementSensitivity = 1.0f;
	public float m_fTapWindow = 0.08f;
	public float m_fDoubleTapWindow = 0.2f;
	public Vector3 m_vMinZoomScale;
	public Vector3 m_vMaxZoomScale;
	public Vector4 m_vMinMovementBoundaries;
	public Vector4 m_vMaxMovementBoundaries;
	public Color m_cIllegalActionColour = Color.gray;
	public GameObject[] m_agoDisableOnTouch;
	public SubSceneManager m_rSubsceneToHide;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*. Protected Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected Vector3 m_vUserStartPosition;
	protected Vector3 m_vFrameMovement;
	protected Vector3 m_vUserPreviousFramePosition;
	protected Vector3 m_vUserCurrentPosition;
	protected float m_fPreviousZoomDistance;
	protected float m_fCurrentZoomDistance;
	protected bool m_bIsTouched = false;
	protected bool m_bDoubleTapActive = false;
	protected int m_iTouchCount = 0;
	protected SpriteRenderer m_rSprRend = null;
	protected MovementMode m_eMovementMode = MovementMode.MOVE;
	protected TimeTracker m_ttDoubleTapTimer;

	protected bool m_bIsLerping = false;
	protected bool m_bIsLerpingScale = false;
	protected float m_fHeldTimer = 0.0f;
	protected Vector3 m_vLerpStart;
	protected Vector3 m_vLerpEnd;
	protected Vector3 m_vDefaultPosition;
	protected Color m_cReturnLerpStartColour = Color.white;
	protected TimeTracker m_ttLerpTimer = new TimeTracker(0.1f);

	protected const float m_fResistanceMovementMultiplier = 0.2f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool m_bCanMove = true;
	private bool m_bCanResize = true;
	private bool m_bCanSingleTap = true;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool CanMove			{ get { return m_bCanMove; }		set { m_bCanMove = value; } }
	public bool CanResize		{ get { return m_bCanResize; }		set { m_bCanResize = value; } }
	public bool CanSingleTap	{ get { return m_bCanSingleTap; }	set { m_bCanSingleTap = value; } }

	public bool IsHeld			{ get { return m_bIsTouched; } }
	public bool IsLerping		{ get { return m_bIsLerping; } }
	public bool WasMoved		{ get; private set; }
	public bool WasSingleTapped { get; private set; }
	public bool WasDoubleTapped { get; private set; }
	
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
	protected bool ScreenTouched 
	{ 
		get { return (Input.GetMouseButton(0));	} 
	}

	protected bool Triggered
	{
		get { return Input.GetMouseButtonDown(0); }
	}
#else
	protected bool ScreenTouched
	{
		get { return (Input.touchCount > 0); }
	}

	protected bool Triggered
	{
		get	{ return Input.GetTouch(0).phase == TouchPhase.Began;}
	}
#endif


	protected Vector3 CurrentPosition
	{
		get { return Camera.main.ScreenToViewportPoint(Input.mousePosition); }
	}

	protected Vector3 CurrentPositionWithoutX
	{
		get
		{
			Vector3 v = CurrentPosition;
			v.x = 0.0f;
			return v;
		}
	}

	protected virtual bool CanMoveX
	{
		get { return !(m_eMovementMode == MovementMode.ZOOM || ZoomPercentage < 0.3f); }
	}

	protected Vector3 CurrentZoomPosition
	{
		get	{ return Camera.main.ScreenToViewportPoint(Input.GetTouch(1).position); }
	}

    protected bool IsTapped
    {
        get { return m_fHeldTimer < m_fTapWindow; }
    }

	protected float CurrentZoom
	{
		get { return Vector3.Distance(CurrentPosition, CurrentZoomPosition); }
	}

	protected bool IsLarge
	{
		get { return LocalScale.y >= Vector3.Lerp(m_vMinZoomScale, m_vMaxZoomScale, 0.5f).y; }
	}

	protected float ZoomPercentage
	{
		get { return Mathf.Max(0.0f, Mathf.Min(1.0f, (LocalScale.x - m_vMinZoomScale.x) / (m_vMaxZoomScale.x - m_vMinZoomScale.x)));	}
	}

	protected Vector3 WorldPosition
	{
		get { return this.transform.position; }
		set { this.transform.position = value; }
	}

	protected Vector3 LocalPosition
	{
		get { return this.transform.localPosition; }
		set { this.transform.localPosition = value; }
	}

	protected Vector3 LocalScale
	{
		get { return this.transform.localScale; }
		set { this.transform.localScale = value; }
	}

	protected float NorthernBoundary
	{
		get { return Mathf.Lerp(m_vMinMovementBoundaries.y, m_vMaxMovementBoundaries.y, ZoomPercentage); }
	}

	protected float SouthernBoundary
	{
		get { return Mathf.Lerp(m_vMinMovementBoundaries.w, m_vMaxMovementBoundaries.w, ZoomPercentage); }
	}

	protected float EasternBoundary
	{
		get { return Mathf.Lerp(m_vMinMovementBoundaries.x, m_vMaxMovementBoundaries.x, ZoomPercentage); }
	}

	protected float WesternBoundary
	{
		get { return Mathf.Lerp(m_vMinMovementBoundaries.z, m_vMaxMovementBoundaries.z, ZoomPercentage); }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	** Class Delcarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected enum MovementMode
	{
		MOVE,
		ZOOM,
	}

	protected enum OutOfBoundsIdentity
	{
		NORTH,
		SOUTH,
		EAST,
		WEST,
		NOT_OUT_OF_BOUNDS,
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Start()
	{
		m_ttDoubleTapTimer = new TimeTracker(m_fDoubleTapWindow);
		m_rSprRend = GetComponent<SpriteRenderer>();
		m_vDefaultPosition = LocalPosition;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void Update()
	{
		WasMoved = false;
		WasSingleTapped = false;
		WasDoubleTapped = false;

		if (IsLerping)
		{
			UpdateLerp();
		}
		else if(ScreenTouched)
		{
			UpdateScreenTouch();
		}
		else if (m_bIsTouched)
		{
			UpdateRecentUntouched();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Lerp
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateLerp()
	{
		// Update Return Lerp to NOT OUT OF BOUNDS Position/Scale. If Illegal Colour has been enabled. Lerp that back to normal also.
		if (m_ttLerpTimer.Update())
		{
			if (m_bIsLerpingScale)
			{
				LocalScale = m_vLerpEnd;
			}
			else
			{
				LocalPosition = m_vLerpEnd;
			}

			if (m_rSprRend != null)
			{
				m_rSprRend.color = Color.white;
			}

			m_ttLerpTimer.Reset();
			m_bIsLerping = false;
			CheckLimit();
		}
		else
		{
			float t = m_ttLerpTimer.GetCompletionPercentage();
			if (m_bIsLerpingScale)
			{
				LocalScale = Vector3.Slerp(m_vLerpStart, m_vLerpEnd, t);
			}
			else
			{
				LocalPosition = Vector3.Slerp(m_vLerpStart, m_vLerpEnd, t);
			}

			if (m_rSprRend != null)
			{
				m_rSprRend.color = Color.Lerp(m_cReturnLerpStartColour, Color.white, t);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Screen Touch
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateScreenTouch()
	{
		if (IsBeingTouched())
		{
			m_fHeldTimer += Time.deltaTime;

			if (m_bDoubleTapActive)
			{
				OnDoubleTap();
			}
			else if (Input.touchCount != m_iTouchCount)
			{
				OnModifiedTouchCount();
			}
			else if (!m_bIsTouched && Triggered)
			{
                OnTouch();
			}
			else if(m_bIsTouched)
			{
				// Update Zoom/Movement if no changes were made.
				if (m_eMovementMode == MovementMode.MOVE)
				{
					MoveWithTouch();
				}
				else
				{
					//ZoomWithTouch();
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Recent Untouched
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void UpdateRecentUntouched()
	{
		m_bDoubleTapActive = true;
		if (m_ttDoubleTapTimer.Update())
		{
		    m_ttDoubleTapTimer.Reset();
		    m_bDoubleTapActive = false;
		    m_bIsTouched = false;
			OnTouchRelease();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Is Being Touched?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected bool IsBeingTouched()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		return Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero).transform == this.transform;
#else
		for (int i = 0; i < Input.touchCount; ++i)
		{
			if (IsBeingTouched(i))
				return true;
		}
		return false;
#endif
	}

	protected bool IsBeingTouched(int TouchID)
	{
		return (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(TouchID).position), Vector2.zero).transform == this.transform);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Initiate Touch Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void InitiateTouchMove()
	{
		m_vUserStartPosition = WorldPosition;
		m_vUserCurrentPosition = (CanMoveX ? CurrentPosition : CurrentPositionWithoutX);
		m_vUserPreviousFramePosition = m_vUserCurrentPosition;
		m_eMovementMode = MovementMode.MOVE;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Initiate Zoom Movement
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void InitiateTouchZoom()
	{
		m_vUserStartPosition = WorldPosition;
		m_vUserCurrentPosition = m_vUserStartPosition;
		m_fCurrentZoomDistance = CurrentZoom;
		m_fPreviousZoomDistance = m_fCurrentZoomDistance;
		m_eMovementMode = MovementMode.ZOOM;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Move With Touch
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void MoveWithTouch()
	{
		if (CanMove && !IsTapped)
		{
			// Find Distance between previous and current frames. If X Scale is great enough to warrant movement on the X Axis, allow that too. Otherwise only do for Y Axis
			m_vUserPreviousFramePosition = m_vUserCurrentPosition;
			m_vUserCurrentPosition = (CanMoveX ? CurrentPosition : CurrentPositionWithoutX);
			m_vFrameMovement = m_vUserCurrentPosition - m_vUserPreviousFramePosition;

			// Move notes. Provide resistance to movement if Notes have gone out of bounds.
			OutOfBoundsIdentity eBoundary = GetOutOfBounds();
			if (eBoundary == OutOfBoundsIdentity.NOT_OUT_OF_BOUNDS)
			{
				WorldPosition += (m_vFrameMovement * m_fMovementSensitivity);
				if (m_rSprRend != null)
				{
					m_rSprRend.color = Color.white;
				}
			}
			else if(m_vFrameMovement != Vector3.zero)
			{
				WasMoved = true;
				WorldPosition += (m_vFrameMovement * m_fMovementSensitivity * m_fResistanceMovementMultiplier);

				// Now Change the Gray/Disabled Colouring of the Sprite/Note if existing
				if (m_rSprRend != null)
				{
					// Find out which Boundary we are checking
					float fBoundVal, fLocalPos;
					switch(eBoundary)
					{
						case OutOfBoundsIdentity.NORTH:
							fBoundVal = NorthernBoundary * 0.8f;
							fLocalPos = LocalPosition.y;
							break;
						case OutOfBoundsIdentity.SOUTH:
							fBoundVal = SouthernBoundary * 0.8f;
							fLocalPos = LocalPosition.y;
							break;
						case OutOfBoundsIdentity.EAST:
							fBoundVal = EasternBoundary;
							fLocalPos = LocalPosition.x;
							break;
						default:
							fBoundVal = WesternBoundary;
							fLocalPos = LocalPosition.x;
							break;
					}

					// Now find out how far we've gone out of Boundaries. Shrink Boundaries based on how big the notes are.
					float fBoundLength = (Mathf.Abs(fBoundVal * 2.0f) * (1.0f - ZoomPercentage));
					if (fBoundLength > 0.0f)
					{
						float fPercentage = Mathf.Max(0.0f, Mathf.Min(1.0f, Mathf.Abs(fLocalPos - fBoundVal) / fBoundLength));
						m_rSprRend.color = Color.Lerp(Color.white, m_cIllegalActionColour, fPercentage);
					}
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Zoom With Touch
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ZoomWithTouch()
	{
		if (CanResize)
		{
			m_fPreviousZoomDistance = m_fCurrentZoomDistance;
			m_fCurrentZoomDistance = CurrentZoom;
			float fScaleDifference = m_fCurrentZoomDistance - m_fPreviousZoomDistance;
			LocalScale += (Vector3.one * fScaleDifference * (IsValidScale(LocalScale) ? 1.0f : 0.2f));
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Double Tap
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void OnDoubleTap()
	{
		WasDoubleTapped = true;

		// Reset Double Tap Availability
		m_ttDoubleTapTimer.Reset();
		m_bDoubleTapActive = false;
		m_bIsTouched = false;

		// If at half of the full size or more, reset page to normal size)
		if (CanResize)
		{
			if (IsLarge)
			{
				LocalScale = m_vMinZoomScale;
				LocalPosition = new Vector3(m_vMinMovementBoundaries.x, m_vMinMovementBoundaries.y, LocalPosition.z);
				ToggleTouchObjects(true);
			}

			// Otherwise make page equal to half of the maximum size it can be
			else
			{
				LocalScale = Vector3.Lerp(m_vMinZoomScale, m_vMaxZoomScale, 0.5f);
				ToggleTouchObjects(true);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Modified Touch Count
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnModifiedTouchCount()
	{
		m_iTouchCount = Input.touchCount;
		if (m_iTouchCount == 2)
		{
			// Must be touched with at least the main finger before the next touch can be counted as a secondary touch
			if (IsBeingTouched(0))
			{
				if (IsBeingTouched(1))
				{
					//InitiateTouchZoom();
					//ZoomWithTouch();
				}
				else
				{
					m_bIsTouched = false;
					OnTouchRelease();
					//InitiateTouchMove();
					//MoveWithTouch();
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Touch
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void OnTouch()
	{
        m_bIsTouched = true;
		ToggleTouchObjects(false);
		InitiateTouchMove();
		MoveWithTouch();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Touch Release
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void OnTouchRelease()
	{
		// If touch was released quick enough to be considered a quick little tap (ie. you weren't intending on moving the page at all)
		if (m_fHeldTimer < m_fTapWindow && CanSingleTap)
		{
			WasSingleTapped = true;

			// If large page, make it small
			if (IsLarge)
			{
                OnDoubleTap();
			}

			// Otherwise if already small page, make it disappear
			else
			{
				if(m_rSubsceneToHide != null)
					m_rSubsceneToHide.HideSubscene();
				else
					this.GetComponent<ObjectTransitionAnimation>().Disappear(false);
			}
		}

		// Otherwise check to see that the page is not out of bounds and needs to be reset.
		else
		{
			CheckLimit();
			ToggleTouchObjects(true);
		}

		m_bIsTouched = false;
		m_fHeldTimer = 0.0f;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check Limit
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void CheckLimit()
	{
		if (!IsValidScale(LocalScale))
		{
			CorrectScaleSize();
		}
		else if (GetOutOfBounds() != OutOfBoundsIdentity.NOT_OUT_OF_BOUNDS)
		{
			CorrectMovementPosition();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Correct Scale Size
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void CorrectScaleSize()
	{
		m_bIsLerping = true;
		m_bIsLerpingScale = true;
		m_vLerpStart = LocalScale;
		m_vLerpEnd = (LocalScale.y < m_vMinZoomScale.y ? m_vMinZoomScale : m_vMaxZoomScale);

		if (m_rSprRend != null)
		{
			m_cReturnLerpStartColour = m_rSprRend.color;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Correct Movement Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void CorrectMovementPosition()
	{
		m_bIsLerping = true;
		m_bIsLerpingScale = false;
		m_vLerpStart = LocalPosition;
		m_vLerpEnd = LocalPosition;
		m_vLerpEnd.x = CorrectedXMovementPos(m_vLerpEnd.x);
		m_vLerpEnd.y = CorrectedYMovementPos(m_vLerpEnd.y);

		if (m_rSprRend != null)
		{
			m_cReturnLerpStartColour = m_rSprRend.color;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Return to Default Position
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ReturnToDefaultPosition()
	{
		m_bIsLerping = true;
		m_bIsLerpingScale = false;
		m_vLerpStart = LocalPosition;
		m_vLerpEnd = m_vDefaultPosition;
        if(m_rSprRend != null)
		{
			m_cReturnLerpStartColour = m_rSprRend.color;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is A Valid Scale?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected bool IsValidScale(Vector3 Scale)
	{
		return !(Scale.x < m_vMinZoomScale.x || Scale.y < m_vMinZoomScale.y || Scale.x > m_vMaxZoomScale.x || Scale.y > m_vMaxZoomScale.y);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is A Valid Move Area? (Get Out of Bounds)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual OutOfBoundsIdentity GetOutOfBounds()
	{
		// Find the Boundary that has been broken the most.
		float fBiggestDistance = 0.0f;
		OutOfBoundsIdentity eBrokenBoundary = OutOfBoundsIdentity.NOT_OUT_OF_BOUNDS;

		// CHECK HORIZONTAL AXIS
		if (LocalPosition.x > EasternBoundary)
		{
			float distance = Mathf.Abs(LocalPosition.x - EasternBoundary);
			if (distance > fBiggestDistance)
			{
				fBiggestDistance = distance;
				eBrokenBoundary = OutOfBoundsIdentity.EAST;
			}
		}

		else if (LocalPosition.x < WesternBoundary)
		{
			float distance = Mathf.Abs(LocalPosition.x - WesternBoundary);
			if (distance > fBiggestDistance)
			{
				fBiggestDistance = distance;
				eBrokenBoundary = OutOfBoundsIdentity.WEST;
			}
		}


		// CHECK VERTICAL AXIS
		if (LocalPosition.y < NorthernBoundary)
		{
			float distance = Mathf.Abs(LocalPosition.y - NorthernBoundary);
			if (distance > fBiggestDistance)
			{
				eBrokenBoundary = OutOfBoundsIdentity.NORTH;
			}
		}

		else if (LocalPosition.y > SouthernBoundary)
		{
			float distance = Mathf.Abs(LocalPosition.y - SouthernBoundary);
			if (distance > fBiggestDistance)
			{
				eBrokenBoundary = OutOfBoundsIdentity.SOUTH;
			}
		}

		return eBrokenBoundary;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Correct X Movement Pos
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected float CorrectedXMovementPos(float Current)
	{
		float fLeft = EasternBoundary;
		float fRight = WesternBoundary;
		return ((Current > fLeft) ? fLeft : ((Current < fRight) ? fRight : Current));
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Correct Y Movement Pos
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected float CorrectedYMovementPos(float Current)
	{
		float fTop = NorthernBoundary;
		float fBottom = SouthernBoundary;
		return ((Current < fTop) ? fTop : ((Current > fBottom) ? fBottom : Current));
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Toggle Touch Objects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void ToggleTouchObjects(bool b)
	{
		if (m_agoDisableOnTouch != null && m_agoDisableOnTouch.Length > 0)
		{
			foreach (GameObject Obj in m_agoDisableOnTouch)
			{
				if(Obj != null)
					Obj.SetActive(b);
			}
		}
	}
}
