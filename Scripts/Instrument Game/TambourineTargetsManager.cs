//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Tambourine Targets Manager
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: July 18, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script manages the targets on the Tambourine. Revealing the separate
//		targets to other scripts so that they can be edited without having multiple
//		scripts give conflicting information about them.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TambourineTargetsManager : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public TambTargetInfo[] m_arTambourineTargets = new TambTargetInfo[3];
	public SpriteRenderer m_sprShakenTambourineTargets;

	public float m_fTargetHighlightTime = 0.25f;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*. Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool Visible { get; private set; }

	private TambTargetInfo CenterTargetInfo { get { return m_arTambourineTargets[0]; } }
	private TambTargetInfo MiddleTargetInfo { get { return m_arTambourineTargets[1]; } }
	private TambTargetInfo OuterTargetInfo	{ get { return m_arTambourineTargets[2]; } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	[System.Serializable]
	public class TambTargetInfo
	{
		public SpriteRenderer sprColouredTambourineTarget;		// Normal Coloured Tambourine Target: Used as Main Target Display
		public SpriteRenderer sprHighlightedTambourineTarget;	// White Coloured Tambourine Target: Used as a secondary to highlight touched areas on the Tambourine whenever the Main/Coloured Tambourine target is in view
		public bool bHighlightTarget = false;
		public TimeTracker ttHighlightTimer;
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		for(int i = 0; i < m_arTambourineTargets.Length; ++i)
			m_arTambourineTargets[i].ttHighlightTimer = new TimeTracker(m_fTargetHighlightTime);
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		UpdateTargetHighlightEffects();
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Target Highlight Effects
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateTargetHighlightEffects()
	{
		for(int i = 0; i < m_arTambourineTargets.Length; ++i)
		{
			if(m_arTambourineTargets[i].bHighlightTarget)
			{
				SpriteRenderer highlightableTarget = (Visible ? m_arTambourineTargets[i].sprHighlightedTambourineTarget : m_arTambourineTargets[i].sprColouredTambourineTarget);
                if(m_arTambourineTargets[i].ttHighlightTimer.Update())
				{
					m_arTambourineTargets[i].bHighlightTarget = false;
                    highlightableTarget.enabled = false;
				}
				else
				{
					Color currentColour = highlightableTarget.color;
					currentColour.a = (1.0f - m_arTambourineTargets[i].ttHighlightTimer.GetCompletionPercentage());
					highlightableTarget.color = currentColour;
					if(Visible)
					{
						highlightableTarget.material.SetColor("_Colour", currentColour);
                    }
                }
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Show Individual Targets
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ShowCenterTambourineTarget()	{ if(Visible) RevealHighlightedTarget(CenterTargetInfo);	else RevealColouredTarget(CenterTargetInfo); }
	public void HideCenterTambourineTarget()	{ if(Visible) HideHighlightedTarget(CenterTargetInfo);		else { HideColouredTarget(CenterTargetInfo); HideHighlightedTarget(CenterTargetInfo); } }

	public void ShowMiddleTambourineTarget()	{ if(Visible) RevealHighlightedTarget(MiddleTargetInfo);	else RevealColouredTarget(MiddleTargetInfo); }
	public void HideMiddleTambourineTarget()	{ if(Visible) HideHighlightedTarget(MiddleTargetInfo);		else { HideColouredTarget(MiddleTargetInfo); HideHighlightedTarget(MiddleTargetInfo); } }

	public void ShowOuterTambourineTarget()		{ if(Visible) RevealHighlightedTarget(OuterTargetInfo);		else RevealColouredTarget(OuterTargetInfo);  }
	public void HideOuterTambourineTarget()		{ if(Visible) HideHighlightedTarget(OuterTargetInfo);		else { HideColouredTarget(OuterTargetInfo); HideHighlightedTarget(OuterTargetInfo); } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Reveal/Hide Coloured Target
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void RevealColouredTarget(TambTargetInfo targetInfo)
	{
		targetInfo.sprColouredTambourineTarget.enabled = true;
		targetInfo.sprColouredTambourineTarget.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}

	private void HideColouredTarget(TambTargetInfo targetInfo)
	{
		targetInfo.sprColouredTambourineTarget.enabled = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Reveal/Hide Highlighted Target
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void RevealHighlightedTarget(TambTargetInfo targetInfo)
	{
		targetInfo.sprHighlightedTambourineTarget.enabled = true;
		targetInfo.sprHighlightedTambourineTarget.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		targetInfo.sprHighlightedTambourineTarget.material.SetColor("_Colour", targetInfo.sprHighlightedTambourineTarget.color);
	}

	private void HideHighlightedTarget(TambTargetInfo targetInfo)
	{
		targetInfo.sprHighlightedTambourineTarget.enabled = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Targets
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ShowTargets()
	{
		RevealColouredTarget(CenterTargetInfo);
		RevealColouredTarget(MiddleTargetInfo);
		RevealColouredTarget(OuterTargetInfo);
		Visible = true;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Hide Targets
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void HideTargets()
	{
		Visible = false;
		HideColouredTarget(CenterTargetInfo);
		HideColouredTarget(MiddleTargetInfo);
		HideColouredTarget(OuterTargetInfo);

		m_sprShakenTambourineTargets.enabled = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show Shaken Tambourine Targets
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ShowShakenTambourineTargets()	
	{
		m_sprShakenTambourineTargets.enabled = true;
		HideColouredTarget(CenterTargetInfo);
		HideColouredTarget(MiddleTargetInfo);
        HideColouredTarget(OuterTargetInfo);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Hide Shaken Tambourine Targets
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void HideShakenTambourineTargets()	
	{
		m_sprShakenTambourineTargets.enabled = false;
		RevealColouredTarget(CenterTargetInfo);
		RevealColouredTarget(MiddleTargetInfo);
		RevealColouredTarget(OuterTargetInfo);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Begin Highlight Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void BeginHighlightEffect(TambourineSoundsManager.SoundTypes eDrumArea)
	{
		if(eDrumArea == TambourineSoundsManager.SoundTypes.CENTER_TAMBOURINE_AREA || eDrumArea == TambourineSoundsManager.SoundTypes.MIDDLE_TAMBOURINE_AREA || eDrumArea == TambourineSoundsManager.SoundTypes.OUTER_TAMBOURINE_AREA)
		{
			m_arTambourineTargets[(int)eDrumArea].bHighlightTarget = true;
			m_arTambourineTargets[(int)eDrumArea].ttHighlightTimer.Reset();

			if(Visible)
			{
				m_arTambourineTargets[(int)eDrumArea].sprHighlightedTambourineTarget.enabled = true;
				m_arTambourineTargets[(int)eDrumArea].sprHighlightedTambourineTarget.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				m_arTambourineTargets[(int)eDrumArea].sprHighlightedTambourineTarget.material.SetColor("_Colour", m_arTambourineTargets[(int)eDrumArea].sprHighlightedTambourineTarget.color);
            }
			else
			{
				m_arTambourineTargets[(int)eDrumArea].sprColouredTambourineTarget.enabled = true;
				m_arTambourineTargets[(int)eDrumArea].sprColouredTambourineTarget.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}
		}
	}


	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Refresh
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void Refresh()
	{
		if (Visible)
			ShowTargets();
		else
			HideTargets();
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Toggle Targets
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ToggleTargets()
	{
		if (Visible) 
			HideTargets(); 
		else 
			ShowTargets(); 
	}
}
