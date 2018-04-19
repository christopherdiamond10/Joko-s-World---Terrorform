//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Scene Fade Effect
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: September 18, 2014
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is used to fadein/fadeout a scene. It goes through all available
//	  sprite renderers on initialisation then fades out/in all of those sprites
//	  when the appropriate function is called.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections.Generic;

public class SceneFadeEffect : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public float m_fTransitionTime = 3.0f;
	public AudioSource m_asSoundPlayer;

#if UNITY_EDITOR
	public SceneFadeEffect m_rConnectingScene;	// Scene we are connected to!
#endif
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool m_bCompleted = false;
	private TimeTracker m_ttTransitionTimer;
	private List<SpriteRenderer> m_lSprRends = new List<SpriteRenderer>();
	private List<UnityEngine.UI.Text> m_lTextRends = new List<UnityEngine.UI.Text>();
	private List<UnityEngine.UI.Image> m_lImgRends = new List<UnityEngine.UI.Image>();
	private TransitionState m_eTransitionState = TransitionState.IDLE;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Reader
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public bool IsCompleted { get { return m_bCompleted; } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private enum TransitionState
	{
		IDLE,
		FADEIN,
		FADEOUT,
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start()
	{
		m_ttTransitionTimer = new TimeTracker(m_fTransitionTime);
		CycleThroughChildren(transform);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Update
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Update()
	{
		if (m_eTransitionState != TransitionState.IDLE)
		{
			UpdateFadeEffect();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Update Fade Effect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void UpdateFadeEffect()
	{
		// If the fadein timer has completed (or I got impatient on the PC); Show the Scene Fully Opaque / Fully Transparent
#if UNITY_EDITOR
		if (m_ttTransitionTimer.Update() || Input.GetKeyDown(KeyCode.Space))
#else
		if(m_ttTransitionTimer.Update())
#endif
		{
			if (m_eTransitionState == TransitionState.FADEIN)
			{
				OnFadeIn();
			}
			else
			{
				OnFadeout();
			}
			m_eTransitionState = TransitionState.IDLE;
		}

		// Otherwise Fade in/out scene according to the completion percentage of the timer.
		else
		{
			float t = Mathf.Lerp(0.0f, 1.0f, (m_eTransitionState == TransitionState.FADEIN ? m_ttTransitionTimer.GetCompletionPercentage() : 1.0f - m_ttTransitionTimer.GetCompletionPercentage()));
			foreach (SpriteRenderer sr in m_lSprRends)
			{
				// Make Sure the Sprite Renderer is still valid
				if (sr != null)
				{
					Color colour = sr.color;
					colour.a = t;
					sr.color = colour;
				}
			}
			foreach (UnityEngine.UI.Text tr in m_lTextRends)
			{
				if (tr != null)
				{
					Color colour = tr.color;
					colour.a = t;
					tr.color = colour;
				}
			}
			foreach (UnityEngine.UI.Image ir in m_lImgRends)
			{
				if (ir != null)
				{
					Color colour = ir.color;
					colour.a = t;
					ir.color = colour;
				}
			}

			// If there is a Sound Player for BGM, fade in/out the audio volume also
			if (m_asSoundPlayer != null)
			{
				m_asSoundPlayer.volume = Mathf.Lerp(0.0f, 1.0f, t);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Cycle Through Children (Get Sprite Renderers)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void CycleThroughChildren(Transform Parent)
	{
		// Recursively go through ALL children in the Scene
		foreach (Transform child in Parent)
		{
			CycleThroughChildren(child);
		}

		// If this particular object (Remember it's a recursive function) has a sprite renderer, add it to the sprite renderer list for Fadeout/Fadein.
		SpriteRenderer sr = Parent.GetComponent<SpriteRenderer>();
		if (sr != null)
		{
			m_lSprRends.Add(sr);
		}

		UnityEngine.UI.Text tr = Parent.GetComponent<UnityEngine.UI.Text>();
		if (tr != null)
		{
			m_lTextRends.Add(tr);
		}

		UnityEngine.UI.Image ir = Parent.GetComponent<UnityEngine.UI.Image>();
		if (ir != null)
		{
			m_lImgRends.Add(ir);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Initiate FadeIn
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void InitiateFadeIn()
	{
		m_eTransitionState = TransitionState.FADEIN;
		ButtonManager.ToggleAllButtons(false);
		m_bCompleted = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Initiate FadeOut
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void InitiateFadeOut()
	{
		m_eTransitionState = TransitionState.FADEOUT;
		ButtonManager.ToggleAllButtons(false);
		m_bCompleted = false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On FadeIn
	//---------------------------------------------------
	//	: Is also called by the Unity Inspector Button
	//	  for fading in a scene.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void OnFadeIn()
	{
#if UNITY_EDITOR
		if (m_lSprRends.Count == 0 || m_ttTransitionTimer == null)
			Start(); // In case we call this from the Editor whilst creating the game; We must make sure that the start function has done its job. This isn't a concern when the game is released (or not in Debug Mode)
#endif

		// We've completed, so make sure everyone knows about it. Oh and Giving the Player the ability to press things might be a good idea too...
		m_bCompleted = true;
		ButtonManager.ToggleAllButtons(true);

		// Reset Fade Effect
		m_ttTransitionTimer.Reset();
		m_eTransitionState = TransitionState.IDLE;

		// Show all SpriteRenders with Full Opacity
		foreach (SpriteRenderer sr in m_lSprRends)
		{
			// Make Sure the Sprite Renderer is still valid
			if (sr != null)
			{
				Color colour = sr.color;
				colour.a = 1.0f;
				sr.color = colour;
			}
		}
		foreach (UnityEngine.UI.Text tr in m_lTextRends)
		{
			if (tr != null)
			{
				Color colour = tr.color;
				colour.a = 1.0f;
				tr.color = colour;
			}
		}
		foreach (UnityEngine.UI.Image ir in m_lImgRends)
		{
			if (ir != null)
			{
				Color colour = ir.color;
				colour.a = 1.0f;
				ir.color = colour;
			}
		}

		// And AudioPlayer as well (Full Volume)
		if (m_asSoundPlayer != null)
		{
			m_asSoundPlayer.volume = 1.0f;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: On Fadeout
	//---------------------------------------------------
	//	: Is also called by the Unity Inspector Button
	//	  for fading out a scene.
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void OnFadeout()
	{
#if UNITY_EDITOR
		if (m_lSprRends.Count == 0 || m_ttTransitionTimer == null)
			Start();	// In case we call this from the Editor whilst creating the game; We must make sure that the start function has done its job. This isn't a concern when the game is released (or not in Debug Mode)
#endif

		// We've completed, so make sure everyone knows about it. Oh and Giving the Player the ability to press things might be a good idea too...
		m_bCompleted = true;
		ButtonManager.ToggleAllButtons(true);

		// Reset Fade Effect
		m_ttTransitionTimer.Reset();
		m_eTransitionState = TransitionState.IDLE;

		// Show all SpriteRenders with Full Transparency
		foreach (SpriteRenderer sr in m_lSprRends)
		{
			// Make Sure the Sprite Renderer is still valid
			if (sr != null)
			{
				Color colour = sr.color;
				colour.a = 0.0f;
				sr.color = colour;
			}
		}
		foreach (UnityEngine.UI.Text tr in m_lTextRends)
		{
			if (tr != null)
			{
				Color colour = tr.color;
				colour.a = 0.0f;
				tr.color = colour;
			}
		}
		foreach (UnityEngine.UI.Image ir in m_lImgRends)
		{
			if (ir != null)
			{
				Color colour = ir.color;
				colour.a = 0.0f;
				ir.color = colour;
			}
		}

		// And AudioPlayer as well (Muted)
		if (m_asSoundPlayer != null)
		{
			m_asSoundPlayer.volume = 0.0f;
		}

		gameObject.SetActive(false);
	}
}
