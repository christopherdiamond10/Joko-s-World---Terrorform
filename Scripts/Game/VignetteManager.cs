//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Vignette Manager
//             Author: Christopher Diamond
//             Date: January 05, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script interacts with the Vignette located within the Game, changing
//		sort order, colour/transparency, etc
//
//	  Other scripts wishing to change the Vignette should go through this script
//		to do so.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class VignetteManager : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public SpriteRenderer m_rVignetteRenderer;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static VignetteManager sm_rSelfInstance;

	private static OnTransitionCompleteCallback sm_OnCompleteCallback;
	private static TimeTracker sm_ttVignetteFadeTimer = new TimeTracker(0.25f);

	private static Color sm_cPreviousColour		 = Color.white;
	private static Color sm_cTransitioningColour = Color.white;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static bool IsTransitioning	 { get; private set; }
	public static Color CurrentColour	 { get { return Vignette.color; }		set { Vignette.color = value; } }
	public static float CurrentAlpha	 { get { return CurrentColour.a; }		set { Color colour = CurrentColour; colour.a = value; CurrentColour = colour; } }
	public static int	CurrentSortOrder { get { return Vignette.sortingOrder; } set { Vignette.sortingOrder = value; } }

	private static SpriteRenderer Vignette { get { return (sm_rSelfInstance != null ? sm_rSelfInstance.m_rVignetteRenderer : Camera.main.GetComponent<VignetteManager>().m_rVignetteRenderer); } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public delegate void OnTransitionCompleteCallback();

	[System.Serializable]
	public class VignetteInfo
	{
		public Color newColour							= Color.white;
		public float transitionTime						= 0.25f;
		public int orderInLayer							= -1;
		public OnTransitionCompleteCallback callback	= null;
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake()
	{
		sm_rSelfInstance = this;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if(IsTransitioning)
		{
			if(sm_ttVignetteFadeTimer.Update())
			{
				IsTransitioning = false;

				if(sm_OnCompleteCallback != null)
					sm_OnCompleteCallback();
			}

			CurrentColour = Color.Lerp(sm_cPreviousColour, sm_cTransitioningColour, sm_ttVignetteFadeTimer.GetCompletionPercentage());
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Static Methods: Transition Vignette
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static void TransitionVignette(VignetteInfo info)
	{
		TransitionVignette(info.newColour, info.transitionTime, info.orderInLayer, info.callback);
	}

	public static void TransitionVignette(float newAlphaColour, float transitionTime, int orderInLayer = -1, OnTransitionCompleteCallback callback = null)
	{
		Color newColour = CurrentColour;
		newColour.a = newAlphaColour;
		TransitionVignette(newColour, transitionTime, orderInLayer, callback);
	}

	public static void TransitionVignette(Color newColour, float transitionTime, int orderInLayer = -1, OnTransitionCompleteCallback callback = null)
	{
		if (Vignette != null) 
		{
			sm_OnCompleteCallback = callback;

			sm_cPreviousColour = CurrentColour;
			sm_cTransitioningColour = newColour;

			if (orderInLayer > -1)
				Vignette.sortingOrder = orderInLayer;

			sm_ttVignetteFadeTimer.FinishTime = transitionTime;
			sm_ttVignetteFadeTimer.Reset ();
			IsTransitioning = true;
		}
	}
}
