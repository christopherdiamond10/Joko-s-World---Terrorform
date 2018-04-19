//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Instrument Manager
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: December 31, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script manages the different intruments in the app. Applying the
//		different interpretations of sprites and special abilities as
//		needed. All scripts that need to interact with an instrument should
//		go through this script.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class InstrumentManager : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public InstrumentDetails[]			m_aInstrumentDetails			= new InstrumentDetails[		(int)InstrumentMode.NORMAL_TAMBOURINE + 1 ];
	public GameObject[]					m_agoTambourineCymbals			= new GameObject[				(int)InstrumentMode.NORMAL_TAMBOURINE + 1 ];
	public TambourineTargetsManager[]	m_arTambourineTargets			= new TambourineTargetsManager[	(int)InstrumentMode.NORMAL_TAMBOURINE + 1 ];
	public Button_TambourineCymbal[]	m_arTambourineAutoPlayCymbals	= new Button_TambourineCymbal[	(int)InstrumentMode.NORMAL_TAMBOURINE + 1 ];

	public FullGamePlugReveal m_rFullGamePopup;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private InstrumentMode m_eCurrentInstrumentMode = InstrumentManager.InstrumentMode.RIQ_TAMBOURINE;
	private uint[] m_aiInstrumentHitCount = new uint[ (int)InstrumentMode.NORMAL_TAMBOURINE + 1 ];
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Accessors
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public InstrumentMode CurrentInstrumentMode
	{
		get { return m_eCurrentInstrumentMode; }
		set { SetInstrumentMode(value); }
	}

	public uint CurrentInstrumentHitCount
	{
		get { return m_aiInstrumentHitCount[(int)CurrentInstrumentMode]; }
		set { SetCurrentInstrumentTapCount(value); }
	}

	public TambourineTargetsManager InstrumentColoursManager
	{
		get { return m_arTambourineTargets[(int)CurrentInstrumentMode]; }
	}

	public Button_TambourineCymbal CurrentAutoPlayCymbal
	{
		get { return m_arTambourineAutoPlayCymbals[(int)CurrentInstrumentMode]; }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	[System.Serializable]
	public class InstrumentDetails
	{
		public GameObject holder;				// Object to Activate/Deactivate when enabling this Instrument
		public SpriteRenderer renderer;			// Sprite renderer for object

		public Sprite normalSprite;				// Sprite used for instrument during normal state 
		public Sprite specialSprite;			// Sprite used for instrument during special state (eg: Tambourine Shaking)

		public Vector3 normalPosition;			// Position of GameObject during normal state
		public Vector3 specialPosition;         // Position of GameObject during special state
		public Vector3 normalRotation;			// Rotation of GameObject during normal state
		public Vector3 specialRotation;			// Rotation of GameObject during special state
		public Vector3 normalScale;             // Scale of GameObject during normal state
		public Vector3 specialScale;			// Scale of GameObject during special state
	}

	public enum InstrumentMode
	{
		RIQ_TAMBOURINE		= 0,
		PANDEIRO_TAMBOURINE = 1,
		KANJIRA_TAMBOURINE	= 2,
		NORMAL_TAMBOURINE	= 3,
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*-- Debug/Editor Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
	public InstrumentMode dm_eSelectedInspectorInstrument = InstrumentMode.RIQ_TAMBOURINE;
#endif



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Awake
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Awake()
	{
		// Switch to the appropriate instrument type, saved during the last game. Can only change Instruments permanently in full version of app~
		InstrumentMode eSavedInstrumentMode = m_eCurrentInstrumentMode;
		//if(GameManager.IsFullVersion)
			eSavedInstrumentMode = (InstrumentMode)SavedPreferenceTool.GetInt("InstrumentMode", (int)InstrumentMode.RIQ_TAMBOURINE);

		if(eSavedInstrumentMode != m_eCurrentInstrumentMode)
			SetInstrumentMode(eSavedInstrumentMode);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{	
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Instrument Mode
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetInstrumentMode(InstrumentMode eMode)
	{
		if(m_eCurrentInstrumentMode != eMode)
		{
			// Disable Currently Active Instrument
			DisableInstrument(m_eCurrentInstrumentMode);
			bool bActivateTargets = m_arTambourineTargets[(int)m_eCurrentInstrumentMode].Visible;

			// Make the switch, saving to memory so that the app uses this same instrument next time app is loaded
			m_eCurrentInstrumentMode = eMode;
			SavedPreferenceTool.SaveInt("InstrumentMode", (int)eMode);

			// Activate next instrument and put it into the normal display state
			ActivateInstrument(eMode);
			ShowNormalInstrumentState();
			if(bActivateTargets)
				m_arTambourineTargets[(int)eMode].ShowTargets();
			else
				m_arTambourineTargets[(int)eMode].HideTargets();
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Current Instrument Tap Count
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void SetCurrentInstrumentTapCount(uint count)
	{
		m_aiInstrumentHitCount[(int)CurrentInstrumentMode] = count;

		// Don't Own Full Version? Have an annoying popup to "convince" you.
		if(!GameManager.IsFullVersion && CurrentInstrumentMode != InstrumentMode.RIQ_TAMBOURINE)
		{
			const uint ALLOWABLE_TAPS_PER_LOCKED_INSTRUMENT = 20;
			if(count >= ALLOWABLE_TAPS_PER_LOCKED_INSTRUMENT)
			{
				m_rFullGamePopup.BeginFadein(ObjectTransitionAnimation.CurrentlyActive);
			}
		}
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Show Normal Instrument State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ShowNormalInstrumentState()
	{
		ShowNormalInstrumentState(CurrentInstrumentMode);
	}

	public void ShowNormalInstrumentState(InstrumentMode eWhichInstrument)
	{
		InstrumentDetails instrument = m_aInstrumentDetails[(int)eWhichInstrument];
		if(instrument != null)
		{
			instrument.renderer.sprite					= instrument.normalSprite;
			instrument.holder.transform.localPosition	= instrument.normalPosition;
			instrument.holder.transform.localRotation	= Quaternion.Euler(instrument.normalRotation);
			instrument.holder.transform.localScale		= instrument.normalScale;

			if(m_agoTambourineCymbals[(int)eWhichInstrument] != null)
			{
				m_agoTambourineCymbals[(int)eWhichInstrument].SetActive(true);
				foreach(Transform cymbal in m_agoTambourineCymbals[(int)eWhichInstrument].transform)
					cymbal.GetComponent<Button_TambourineCymbal>().enabled = true;
			}

			if(m_arTambourineTargets[(int)eWhichInstrument] != null && m_arTambourineTargets[(int)eWhichInstrument].Visible)
				m_arTambourineTargets[(int)eWhichInstrument].HideShakenTambourineTargets();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Show Special Instrument State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ShowSpecialInstrumentState()
	{
		ShowSpecialInstrumentState(CurrentInstrumentMode);
	}

	public void ShowSpecialInstrumentState(InstrumentMode eWhichInstrument)
	{
		InstrumentDetails instrument = m_aInstrumentDetails[(int)eWhichInstrument];
		if(instrument != null)
		{
			instrument.renderer.sprite					= instrument.specialSprite;
			instrument.holder.transform.localPosition	= instrument.specialPosition;
			instrument.holder.transform.localRotation	= Quaternion.Euler(instrument.specialRotation);
			instrument.holder.transform.localScale		= instrument.specialScale;

			if(m_agoTambourineCymbals[(int)eWhichInstrument] != null)
				m_agoTambourineCymbals[(int)eWhichInstrument].SetActive(false);

			if(m_arTambourineTargets[(int)eWhichInstrument] != null && m_arTambourineTargets[(int)eWhichInstrument].Visible)
				m_arTambourineTargets[(int)eWhichInstrument].ShowShakenTambourineTargets();
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Toggle Instrument State
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ToggleInstrumentState()
	{
		ToggleInstrumentState(CurrentInstrumentMode);
    }

	public void ToggleInstrumentState(InstrumentMode eWhichInstrument)
	{
		InstrumentDetails instrument = m_aInstrumentDetails[(int)eWhichInstrument];
		if(instrument != null)
		{
			if(instrument.renderer.sprite == instrument.specialSprite)
				ShowNormalInstrumentState(eWhichInstrument);
			else
				ShowSpecialInstrumentState(eWhichInstrument);
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Activate Instrument
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void ActivateInstrument(InstrumentMode eWhichInstrument)
	{
		InstrumentDetails instrument = m_aInstrumentDetails[(int)eWhichInstrument];
		if(instrument != null)
			instrument.holder.SetActive(true);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Disable Instrument
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void DisableInstrument(InstrumentMode eWhichInstrument)
	{
		InstrumentDetails instrument = m_aInstrumentDetails[(int)eWhichInstrument];
		if(instrument != null)
			instrument.holder.SetActive(false);
	}
}
