//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Base Class
//             Author: Christopher Diamond
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//		This Script is a base class for all New Custom Editors. It adds some 
//			additional functionality that should be readily available in all
//			Custom Editor scripts.
//
//    A custom editor is used to add additional functionality to the Unity 
//		inspector when dealing with the aforementioned class data.
//
//	  This includes the addition of adding in buttons or calling a method when a 
//		value is changed.
//	  Most importantly, a custom editor is used to make the inspector more 
//		readable and easier to edit.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Reflection;
using UnityEditor;

public class CustomEditor_Base<K> : Editor          // K for Klass
						 where K : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*. Protected Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static string sm_sOpenPathDirectory = @"C:\";
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private bool m_bShowDescription = false;
	private static string m_sArabicText;
	private static string m_sArabicRTLText;
	private static string m_sArabicFixText;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected K Target { get { return target as K; } }
	protected Transform transform { get { return Target.transform; } }
	protected virtual string ScriptDescription { get { return ""; } }
	protected virtual InspectorRegion[] AdditionalRegions { get { return null; } }
	protected Rect RectPos { get { return new Rect(232, GUILayoutUtility.GetLastRect().y, GUILayoutUtility.GetLastRect().width - 232, GUILayoutUtility.GetLastRect().height); } }

	protected virtual Color MainFontColour { get { return new Color32(61, 84, 47, 255); } }
	protected virtual Color SecondaryFontColour { get { return new Color32(137, 107, 47, 255); } }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected delegate void CallbackMethod();

	protected struct InspectorRegion
	{
		public string label;
		public CallbackMethod representingDrawMethod;
	}

	protected enum BooleanState
	{
		TRUE,
		FALSE
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: On Inspector GUI
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public override void OnInspectorGUI()
	{
		// Get it away from the top of the inspector so it's easier to read!
		AddSpaces(2);

		// Draw Script Description, if it exists
		DrawScriptDescription();

		// Draw Predefined Regions
		if (DoesInspectorRegionExist("DrawInspectorOptions")) { DrawNewInspectorRegion("~Inspector Options~ (Applying References To The Script)", DrawInspectorOptions); }
		if (DoesInspectorRegionExist("DrawEditableValuesOptions")) { DrawNewInspectorRegion("~Editable Script Values~ (Editable Values In The Script)", DrawEditableValuesOptions); }

		// Draw Additional Regions... If any... (probably won't happen often!)
		InspectorRegion[] additionalRegions = AdditionalRegions;
		if (additionalRegions != null)
			for (int i = 0; i < additionalRegions.Length; ++i)
				DrawNewInspectorRegion(additionalRegions[i]);

		// Draw Remaining Predefined Regions.
		if (DoesInspectorRegionExist("DrawAudioHandlerInfoOptions")) { DrawNewInspectorRegion("~Audio Options~ (Edit Audio Information)", DrawAudioHandlerInfoOptions); }
		if (DoesInspectorRegionExist("DrawAnimationOptions")) { DrawNewInspectorRegion("~Animation Options~ (Shows Simple Animation Options)", DrawAnimationOptions); }
		if (DoesInspectorRegionExist("DrawAnimatorOptions")) { DrawNewInspectorRegion("~Animator Options~ (To Be Used With Unity's 2D Animation System)", DrawAnimatorOptions); }
		if (DoesInspectorRegionExist("DrawVignetteInfoOptions")) { DrawNewInspectorRegion("~Vignette Text Options~ (Apply a Background Vignette)", DrawVignetteInfoOptions); }
		if (DoesInspectorRegionExist("DrawMultiLanguageTextOptions")) { DrawNewInspectorRegion("~Multi-Language Text Options~ (Applying Values to Account for Other Languages)", DrawMultiLanguageTextOptions); }
		if (DoesInspectorRegionExist("DrawDebugOptions")) { DrawNewInspectorRegion("~Debug Options~ (To Help With Testing)", DrawDebugOptions); }

		// Reserialise Script Instance if things have been changed.
		if (GUI.changed)
		{
			EditorUtility.SetDirty(Target);
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		}
	}

	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Does Inspector Region Exist?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected bool DoesInspectorRegionExist(string methodName)
	{
		MethodInfo methodInfo = this.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		return (methodInfo != null ? methodInfo.DeclaringType != typeof(CustomEditor_Base<K>) : false);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Draw New Inspector Region
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawNewInspectorRegion(string label, CallbackMethod InspectorDrawMethod)
	{
		EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
		EditorGUI.indentLevel += 1;
		{
			InspectorDrawMethod();
			AddSpaces(3);
		}
		EditorGUI.indentLevel -= 1;
	}

	protected void DrawNewInspectorRegion(InspectorRegion newInspectorRegion)
	{
		DrawNewInspectorRegion(newInspectorRegion.label, newInspectorRegion.representingDrawMethod);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Script Description
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void DrawScriptDescription()
	{
		if (ScriptDescription != "")
		{
			m_bShowDescription = EditorGUILayout.Foldout(m_bShowDescription, new GUIContent("Description", "Reveal description of this script"), EditorStyles.foldout);
			if (m_bShowDescription)
			{
				DrawSplitter();
				string[] textlines = ScriptDescription.Split(new char[] { '\n', });
				GUIStyle s = new GUIStyle();
				s.alignment = TextAnchor.UpperCenter;
				s.normal.textColor = new Color32(36, 68, 196, 255);
				foreach (string line in textlines)
				{
					AddSpaces(2);
					Rect pos = GetScaledRect();
					EditorGUI.LabelField(pos, line, s);
				}
				AddSpaces(2);
				DrawSplitter();
			}
			else
			{
				AddSpaces(1);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Inpector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void DrawInspectorOptions()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void DrawEditableValuesOptions()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void DrawAnimationOptions()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Animator Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void DrawAnimatorOptions()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Audio Handler Info Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void DrawAudioHandlerInfoOptions()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Vignette Info Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void DrawVignetteInfoOptions()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Multi-Language Text Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void DrawMultiLanguageTextOptions()
	{
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Debug Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected virtual void DrawDebugOptions()
	{
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Add Spaces
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void AddSpaces(int count = 3)
	{
		for (int i = 0; i < count; ++i)
		{
			EditorGUILayout.Space();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Is a Secondary Component?
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	/// <summary>
	/// Determines whether or not this component is a secondary component.
	/// Basically if this component is listed with an ID of 2, 4, 6, 8, 10, etc. it 
	/// will be considered a secondary component. It doesn't mean anything; it's just
	/// a way of identifying whether or not the colours of the font/labels in this component
	/// should change colours
	/// </summary>
	/// <returns>True if secondary component</returns>
	protected bool IsSecondaryComponent()
	{
		Component[] components = Target.gameObject.GetComponents<Component>();
		for (int i = 0; i < components.Length; ++i)
		{
			if (components[i] == Target)
				return (i % 2 != 0);
		}
		return false;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get Scaled Rect
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static Rect GetScaledRect()
	{
		float y = GUILayoutUtility.GetLastRect().y;
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		Rect scale = GUILayoutUtility.GetLastRect();
		scale.y = y;
		scale.height = 15;
		return scale;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Splitter
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawSplitter()
	{
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Int Field
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected int DrawIntField(string name, int currentValue, string tooltip = "")
	{
		GUIContent Label = (tooltip != "" ? new GUIContent(name, tooltip) : new GUIContent(name));
		return EditorGUILayout.IntField(Label, currentValue);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Float Field
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected float DrawFloatField(string name, float currentValue, string tooltip = "")
	{
		GUIContent Label = (tooltip != "" ? new GUIContent(name, tooltip) : new GUIContent(name));
		return EditorGUILayout.FloatField(Label, currentValue);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Toggle Field
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected bool DrawToggleField(string name, bool currentValue, string tooltip = "", bool useEnumPopup = true)
	{
		GUIContent Label = (tooltip != "" ? new GUIContent(name, tooltip) : new GUIContent(name));
		if (useEnumPopup)
			return ((BooleanState)EditorGUILayout.EnumPopup(Label, (currentValue ? BooleanState.TRUE : BooleanState.FALSE)) == BooleanState.TRUE);
		else
			return EditorGUILayout.Toggle(Label, currentValue);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Draw Object Option
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected T DrawObjectOption<T>(string name, T obj, string tooltip = "", bool showRedTextIfNull = true) where T : UnityEngine.Object
	{
		GUIContent Label = (tooltip != "" ? new GUIContent(name, tooltip) : new GUIContent(name));

		// Can't do Template Specialization in C#. So have to do this hacky copy-paste job instead. If it's a Sprite show all text on the same line as Sprite
		//  Since Unity now shows the sprites in the inspector rather than the object box (llike it used to).
		if (typeof(T) == typeof(Sprite))
		{
			T val = (T)EditorGUILayout.ObjectField(" ", obj, typeof(T), true);

			if (showRedTextIfNull && obj == null)
			{
				GUIStyle s = new GUIStyle();
				s.normal.textColor = new Color(1.0f, 0.0f, 0.0f, 0.75f);
				EditorGUI.LabelField(new Rect(15 * EditorGUI.indentLevel, GUILayoutUtility.GetLastRect().y + 22, 300, GUILayoutUtility.GetLastRect().height), Label, s);
			}
			else
			{
				EditorGUI.LabelField(new Rect(15 * EditorGUI.indentLevel, GUILayoutUtility.GetLastRect().y + 22, 300, GUILayoutUtility.GetLastRect().height), Label);
			}

			return val;
		}
		else if (showRedTextIfNull && obj == null)
		{
			GUIStyle s = new GUIStyle();
			s.normal.textColor = new Color(1.0f, 0.0f, 0.0f, 0.75f);
			T val = (T)EditorGUILayout.ObjectField(" ", obj, typeof(T), true);
			EditorGUI.LabelField(new Rect(15 * EditorGUI.indentLevel, GUILayoutUtility.GetLastRect().y, 300, GUILayoutUtility.GetLastRect().height), Label, s);
			return val;
		}
		else
		{
			return (T)EditorGUILayout.ObjectField(Label, obj, typeof(T), true);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Audio Clip Option
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected AudioClip DrawAudioClipOption(string name, AudioClip audioClip, string tooltip = "")
	{
		AudioClip returningValue = DrawObjectOption(name, audioClip, tooltip);
		if (returningValue != null)
		{
			Rect pos = new Rect(GUILayoutUtility.GetLastRect().width / 2, GUILayoutUtility.GetLastRect().y + GUILayoutUtility.GetLastRect().height + 2, GUILayoutUtility.GetLastRect().width / 2, GUILayoutUtility.GetLastRect().height);
			if (GUI.Button(pos, new GUIContent("Play Sound", "Plays the associated sound")))
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(returningValue);
			}
			AddSpaces(3);
		}
		return returningValue;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Label
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawLabel(string text, bool bUseCustomColour = false)
	{
		if (bUseCustomColour)
			DrawLabel(text, IsSecondaryComponent() ? SecondaryFontColour : MainFontColour);
		else
			EditorGUILayout.LabelField(text);
	}

	protected void DrawLabel(string text, GUIStyle s)
	{
		EditorGUILayout.LabelField(text, s);
	}

	protected void DrawLabel(string text, Color colour)
	{
		GUIStyle s = new GUIStyle();
		s.normal.textColor = colour;
		DrawLabel(text, s);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Changeable Number Option (INT)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected int DrawChangeableIntegerOption(string label, int currentNumber, int changingAmount = 1, string tooltip = "", bool indentLabel = true)
	{
		if (label != "")
		{
			if (indentLabel) { EditorGUI.indentLevel += 1; }
			EditorGUILayout.LabelField(new GUIContent(label, tooltip));
			if (indentLabel) { EditorGUI.indentLevel -= 1; }
		}
		return DrawChangeableIntegerOption(GetScaledRect(), "", currentNumber, changingAmount, "", false);
	}

	protected int DrawChangeableIntegerOption(Rect drawPos, string label, int curentNumber, int changingAmount = 1, string tooltip = "", bool indentLabel = true)
	{
		if (label != "")
		{
			if (indentLabel) { EditorGUI.indentLevel += 1; }
			EditorGUILayout.LabelField(new GUIContent(label, tooltip));
			if (indentLabel) { EditorGUI.indentLevel -= 1; }
		}
		float bw = 20; // Button Width
		float tw = 30; // Text Box width
		Rect pos = drawPos;
		pos.x = ((pos.x + pos.width) - (50.0f + tw));
		pos.width = bw;
		if (GUI.Button(pos, "<"))
		{
			curentNumber -= changingAmount;
		}

		pos.x += pos.width + 5;
		pos.width = tw;
		int currentEditorIndent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		curentNumber = int.Parse(EditorGUI.TextField(pos, curentNumber.ToString()));
		EditorGUI.indentLevel = currentEditorIndent;

		pos.x += pos.width + 5;
		pos.width = bw;
		if (GUI.Button(pos, ">"))
		{
			curentNumber += changingAmount;
		}
		return curentNumber;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Draw Changeable Number Option (FLOAT)
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected float DrawChangeableFloatOption(string label, float currentNumber, float changingAmount = 0.1f, string tooltip = "", bool indentLabel = true)
	{
		if (label != "")
		{
			if (indentLabel) { EditorGUI.indentLevel += 1; }
			EditorGUILayout.LabelField(new GUIContent(label, tooltip));
			if (indentLabel) { EditorGUI.indentLevel -= 1; }
		}
		return DrawChangeableFloatOption(GetScaledRect(), "", currentNumber, changingAmount, "", false);
	}

	protected float DrawChangeableFloatOption(Rect drawPos, string label, float curentNumber, float changingAmount = 0.1f, string tooltip = "", bool indentLabel = true)
	{
		if (label != "")
		{
			if (indentLabel) { EditorGUI.indentLevel += 1; }
			EditorGUILayout.LabelField(new GUIContent(label, tooltip));
			if (indentLabel) { EditorGUI.indentLevel -= 1; }
		}
		float bw = 20; // Button Width
		float tw = 50; // Text Box width
		Rect pos = drawPos;
		pos.x = ((pos.x + pos.width) - (50.0f + tw));
		pos.width = bw;
		if (GUI.Button(pos, "<"))
		{
			curentNumber -= changingAmount;
		}

		pos.x += pos.width + 5;
		pos.width = tw;
		int currentEditorIndent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		curentNumber = float.Parse(EditorGUI.TextField(pos, curentNumber.ToString()));
		EditorGUI.indentLevel = currentEditorIndent;

		pos.x += pos.width + 5;
		pos.width = bw;
		if (GUI.Button(pos, ">"))
		{
			curentNumber += changingAmount;
		}
		return curentNumber;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Audio Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawAudioOptions(AudioSourceManager.AudioHandlerInfo audioInfo)
	{
		audioInfo.m_acAudioToPlay = DrawObjectOption("AudioClip To Play:", audioInfo.m_acAudioToPlay, "The Desired AudioClip To be played, whether that be BGM or SFX");
		if (audioInfo.m_acAudioToPlay != null)
		{
			AudioSource rAudioSource = ((Camera.main != null && Camera.main.GetComponent<AudioSource>() != null) ? Camera.main.GetComponent<AudioSource>() : null);
			EditorGUI.indentLevel += 1;
			{
				bool bNewIsLooping = DrawToggleField("Loop Audio:", audioInfo.m_bLoopAudio, "Will the Audio Loop?", true);
				if (bNewIsLooping != audioInfo.m_bLoopAudio)
				{
					audioInfo.m_bLoopAudio = bNewIsLooping;
					if (rAudioSource != null && rAudioSource.clip == audioInfo.m_acAudioToPlay)
						rAudioSource.loop = bNewIsLooping;
				}
				float fNewVolume = EditorGUILayout.Slider(new GUIContent("Play Volume:", "Max volume of the AudioClip whilst playing"), audioInfo.m_fMaxVolume, 0.05f, 1.0f);
				if (fNewVolume != audioInfo.m_fMaxVolume)
				{
					audioInfo.m_fMaxVolume = fNewVolume;
					if (rAudioSource != null && rAudioSource.clip == audioInfo.m_acAudioToPlay)
						rAudioSource.volume = fNewVolume;
				}
				AddSpaces(2);
				audioInfo.m_bRandomiseTrackStartPosition = DrawToggleField("Randomise Track Start Position: ", audioInfo.m_bRandomiseTrackStartPosition, "Should the Track Start from a Random Point when played?", true);
				if (!audioInfo.m_bRandomiseTrackStartPosition)
				{
					EditorGUI.indentLevel += 1;
					audioInfo.m_iStartTrackPosition = EditorGUILayout.IntSlider(new GUIContent("Audio Start Position:", "using PCM Samples, The Audio Track will start from the Specified Position. The value is clamped to be within range of the Specified Audio Clip"), audioInfo.m_iStartTrackPosition, 0, audioInfo.m_acAudioToPlay.samples - 1);// Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Audio Start Position:", "using PCM Samples, The Audio Track will start from the Specified Position. The value is clamped to be within range of the Specified Audio Clip"), audioInfo.m_iStartTrackPosition), 0, audioInfo.m_acAudioToPlay.samples);
					EditorGUI.indentLevel -= 1;
				}
				audioInfo.m_bFadeinAudioUponPlaying = DrawToggleField("Fade-in Audio When Playing:", audioInfo.m_bFadeinAudioUponPlaying, "When the call is made to begin playing this audio, shall we fade-in the Audio \n(From Vol: 0.00 : To Vol: " + audioInfo.m_fMaxVolume.ToString("0.00") + ")?");
				audioInfo.m_fFadeinAudioTime = EditorGUILayout.FloatField(new GUIContent("Fade-in Time:", "How long will it take to fade-in the Audio, assuming we ever do so"), audioInfo.m_fFadeinAudioTime);
				audioInfo.m_fFadeoutAudioTime = EditorGUILayout.FloatField(new GUIContent("Fade-out Time:", "How long will it take to fade-out the Audio, assuming we ever do so"), audioInfo.m_fFadeoutAudioTime);

				if (rAudioSource != null)
				{
					AddSpaces(1);
					Rect drawPos = GetScaledRect();
					float buttonWidth = 150.0f;
					drawPos.x += (drawPos.width - buttonWidth);
					drawPos.width = buttonWidth;
					drawPos.height = 18.0f;
					if (GUI.Button(drawPos, new GUIContent("Play Audio", "Plays the audio using the values above")))
					{
						rAudioSource.Stop();
						rAudioSource.clip = audioInfo.m_acAudioToPlay;
						rAudioSource.loop = audioInfo.m_bLoopAudio;
						rAudioSource.volume = audioInfo.m_fMaxVolume;
						rAudioSource.Play();
						int trackStartPosition = (audioInfo.m_bRandomiseTrackStartPosition ? Random.Range(0, (audioInfo.m_acAudioToPlay.samples / audioInfo.m_acAudioToPlay.channels)) : audioInfo.m_iStartTrackPosition);
						rAudioSource.timeSamples = trackStartPosition;
					}
					if (rAudioSource.isPlaying)
					{
						drawPos.x -= (10.0f + buttonWidth);
						if (GUI.Button(drawPos, new GUIContent("Stop Audio", "Stops playing whatever audio is currently playing in the Editor")))
						{
							rAudioSource.Stop();
						}
					}
					AddSpaces(2);
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Vignette Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawVignetteOptions(VignetteManager.VignetteInfo vignetteInfo)
	{
		if (Camera.main != null && Camera.main.GetComponent<VignetteManager>() != null)
		{
			SpriteRenderer vignetteRenderer = Camera.main.GetComponent<VignetteManager>().m_rVignetteRenderer;
			if (vignetteRenderer != null)
			{
				AddSpaces(1);
				Rect drawPos = GetScaledRect();
				float buttonWidth = 150.0f;
				drawPos.x += (drawPos.width - buttonWidth);
				drawPos.width = buttonWidth;
				drawPos.height = 18.0f;
				if (GUI.Button(drawPos, new GUIContent("Show Vignette", "Show the vignette with the values set below")))
				{
					vignetteRenderer.color = vignetteInfo.newColour;
					vignetteRenderer.sortingOrder = vignetteInfo.orderInLayer;
				}
				drawPos.x -= (10.0f + buttonWidth);
				if (GUI.Button(drawPos, new GUIContent("Hide Vignette", "Hides the Vignette from view by changing it's alpha value to zero")))
				{
					Color colour = vignetteRenderer.color;
					colour.a = 0.0f;
					vignetteRenderer.color = colour;
				}
				AddSpaces(2);

				EditorGUI.indentLevel += 1;
				{
					vignetteInfo.transitionTime = EditorGUILayout.FloatField(new GUIContent("Vignette Transition Time:", "How long will it take the vignette to transition to the new desired colour and location?"), vignetteInfo.transitionTime);

					Color newColour = EditorGUILayout.ColorField(new GUIContent("Vignette Colour: ", "The Desired Colour of the Vignette"), vignetteInfo.newColour);
					if (newColour != vignetteInfo.newColour)
					{
						vignetteInfo.newColour = newColour;
						vignetteRenderer.color = newColour;
					}

					int newOrderInLayer = Mathf.Clamp(DrawChangeableIntegerOption("Sorting Order:", vignetteInfo.orderInLayer, tooltip: "The order in the default layer in which the vignette appears in. By changing this value, the vignette may draw above or below other objects"), -1, 100);
					if (newOrderInLayer != vignetteInfo.orderInLayer)
					{
						vignetteInfo.orderInLayer = newOrderInLayer;
						vignetteRenderer.sortingOrder = newOrderInLayer;
					}
				}
				EditorGUI.indentLevel -= 1;
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Multi Language Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	/// <summary>
	/// This method displays the Multi-Language Text Component. As it's widely used, it was a good idea to have it in the
	/// base Custom_Editor for easier access. The first part of the component will show the current English display. The second
	/// will display the translation for the other selected language. Keeping the English one above it so that translation can
	/// be done easier. Note that although there may be an enum to select which language you're trying to edit/translate to.
	/// The enum will be clamped to stop you from choosing either ENGLISH or TOTAL_LANGUAGES as there is already an option for
	/// English Text and selecting TOTAL_LANGUAGES will break the system.
	/// </summary>
	/// <param name="rMultiLanguageText">Reference to the Multi-Language Text Class that we are going to be editing</param>
	/// <param name="eDefaultLangauge">Pass in the language we are editing. Whatever you pass in will be the ONLY language we will be able to translate to/edit in the inspector. If you wish to be able to select which language via enum, keep the param set to the default: TOTAL_LANGUAGES</param>
	/// <param name="bForceAutoEnglishComplete">Whenever the Enlgish text field is left blank, the system will parse through the Provided TextRenderer and automatically copy components to auto-fill Multi-Language Text Fields. Avoid this if you want a Multi-Lanugage Text component that can be blank/unfilled</param>
	protected void DrawMultiLanguageText(MultiLanguageText rMultiLanguageText, GameManager.SystemLanguages eDefaultLangauge, bool bForceAutoEnglishComplete = true)
	{
		if (Camera.main != null && Camera.main.GetComponent<GameManager>() != null)
		{
			GameManager.SystemLanguages eSelectedSystemLanguage = Camera.main.GetComponent<GameManager>().m_eSystemLanguages;

			// Resize Array if Less than Total Languages
			if (rMultiLanguageText.m_arLanguageText.Length != (int)GameManager.SystemLanguages.TOTAL_LANGUAGES)
				ResizeArray(ref rMultiLanguageText.m_arLanguageText, (int)GameManager.SystemLanguages.TOTAL_LANGUAGES);

			// Get Text Reference
			rMultiLanguageText.dm_rTextRenderer = DrawObjectOption("Text Renderer: ", rMultiLanguageText.dm_rTextRenderer, "Object which changes to the text fields in this section will be applied to during edits. You will see any changes you make on this provided TextRenderer if it's visible in the Scene/Game window(s)", true);



			DrawLabel("Selected Language: ");
			GameManager.SystemLanguages newSelectedLanguage = eSelectedSystemLanguage;
			Rect drawRect = GetScaledRect();
			float width = drawRect.width;

			// Draw "<" Button
			{
				drawRect.x += (drawRect.width / 2.5f);
				drawRect.width = 20.0f;
				if (GUI.Button(drawRect, new GUIContent("<")))
				{
					if ((int)newSelectedLanguage > 0)
						newSelectedLanguage = (GameManager.SystemLanguages)((int)newSelectedLanguage - 1);
				}
			}

			// Draw Enum Popup Box
			{
				drawRect.x += 10.0f;
				drawRect.width = (width - (drawRect.x + 30.0f));

				newSelectedLanguage = (GameManager.SystemLanguages)Mathf.Clamp((int)((GameManager.SystemLanguages)EditorGUI.EnumPopup(drawRect, newSelectedLanguage)), (int)GameManager.SystemLanguages.ENGLISH, (int)GameManager.SystemLanguages.TOTAL_LANGUAGES - 1);
			}


			// Draw ">" Button
			{
				drawRect.x += (drawRect.width + 10.0f);
				drawRect.width = 20.0f;
				if (GUI.Button(drawRect, new GUIContent(">")))
				{
					if ((int)newSelectedLanguage < (int)(GameManager.SystemLanguages.TOTAL_LANGUAGES - 1))
						newSelectedLanguage = (GameManager.SystemLanguages)((int)newSelectedLanguage + 1);
				}
			}

			if (newSelectedLanguage != eSelectedSystemLanguage)
			{
				SetGameLanguage(newSelectedLanguage);
				if (rMultiLanguageText.dm_rTextRenderer != null)
					rMultiLanguageText.ApplyEffects(rMultiLanguageText.dm_rTextRenderer, newSelectedLanguage);
			}

			if (rMultiLanguageText.m_arLanguageText[(int)newSelectedLanguage] == null)
				rMultiLanguageText.m_arLanguageText[(int)newSelectedLanguage] = new MultiLanguageText.TextDisplayValues();

			AddSpaces(5);
			DrawMultiLanguageText(rMultiLanguageText, rMultiLanguageText.m_arLanguageText[(int)newSelectedLanguage], newSelectedLanguage, bForceAutoEnglishComplete);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Multi Language Text
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawMultiLanguageText(MultiLanguageText rMultiLanguageText, MultiLanguageText.TextDisplayValues rTextDisplay, GameManager.SystemLanguages eChosenLanguage, bool bForceAutoEnglishComplete)
	{
		DrawLabel(GameManager.GetLanguageAsEnglishString(eChosenLanguage) + " Text: ", EditorStyles.boldLabel);

	#region DRAW COPY/PASTE OPTIONS
		// Draw Copy/Paste Options
		{
			AddSpaces(1);
			Rect drawpos = GetScaledRect();
			float buttonWidth = 300.0f;
			drawpos.width = buttonWidth;
			drawpos.height = 18.0f;
			if (GUI.Button(drawpos, new GUIContent("Copy Language Settings", "Copies the Language settings currently selected which can be pasted into other area as needed")))
			{
				MultiLanguageText.dsm_vCopiedTextPosition = rTextDisplay.fontPosition;
				MultiLanguageText.dsm_iCopiedFontSize = rTextDisplay.fontSize;
				MultiLanguageText.dsm_fCopiedLineSpacing = rTextDisplay.lineSpacing;
				MultiLanguageText.dsm_eCopiedTextAnchor = rTextDisplay.textAlignment;
			}
			drawpos.x += (10 + buttonWidth);
			if (GUI.Button(drawpos, new GUIContent("Paste Language Settings", "Pastes the Language settings into the currently selected MultiLanguage. Overwritting everything in the process")))
			{
				rTextDisplay.fontPosition = MultiLanguageText.dsm_vCopiedTextPosition;
				rTextDisplay.fontSize = MultiLanguageText.dsm_iCopiedFontSize;
				rTextDisplay.lineSpacing = MultiLanguageText.dsm_fCopiedLineSpacing;
				rTextDisplay.textAlignment = MultiLanguageText.dsm_eCopiedTextAnchor;

				if(rMultiLanguageText.dm_rTextRenderer != null)
					rMultiLanguageText.ApplyEffects(rMultiLanguageText.dm_rTextRenderer);
			}
			AddSpaces(3);
		}
	#endregion

	#region DRAW ENGLISH TRANSLATION OPTIONS (IF NOT EDITING ENGLISH)
		// Show option to See English Translation
		if (eChosenLanguage != GameManager.SystemLanguages.ENGLISH)
		{
			AddSpaces(1);
			Rect rectpos = GetScaledRect();
			rectpos.x += 60.0f;  //((rectpos.width / 2) - (rMultiLanguageText.dm_bShowEnglishTranslation ? 75.0f : 107.0f));
			rectpos.width = 160.0f;
			GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
			foldoutStyle.normal.textColor = new Color32(36, 68, 196, 255);
			rMultiLanguageText.dm_bShowEnglishTranslation = EditorGUI.Foldout(rectpos, rMultiLanguageText.dm_bShowEnglishTranslation, (rMultiLanguageText.dm_bShowEnglishTranslation ? "Hide English Translation" : "Show English Translation"), true, foldoutStyle);
			if (rMultiLanguageText.dm_bShowEnglishTranslation)
			{
				AddSpaces(2);
				DrawSplitter();
                string[] textlines = rMultiLanguageText.m_arLanguageText[(int)GameManager.SystemLanguages.ENGLISH].text.Split(new char[] { '\n', }, System.StringSplitOptions.RemoveEmptyEntries);
				GUIStyle s = new GUIStyle();
				s.alignment = TextAnchor.UpperCenter;
				s.normal.textColor = new Color32(36, 68, 196, 255);
				foreach (string line in textlines)
				{
					AddSpaces(2);
					Rect pos = GetScaledRect();
					EditorGUI.LabelField(pos, line, s);
				}
				AddSpaces(2);
				DrawSplitter();
            }
			else
			{
				AddSpaces(3);
			}
			AddSpaces(2);
			if (eChosenLanguage == GameManager.SystemLanguages.URDU)
			{
				if (GUILayout.Button("Replace With Corrected Text"))
				{
					System.Collections.Generic.LinkedList<CustomEditor_GameManager.MultiLanguageTextInstanceInfo> acquiredMultiLanguageTextInstance = new System.Collections.Generic.LinkedList<CustomEditor_GameManager.MultiLanguageTextInstanceInfo>();
					CustomEditor_GameManager.MultiLanguageTextInstanceInfo mlti = new CustomEditor_GameManager.MultiLanguageTextInstanceInfo();
					string text = rMultiLanguageText.m_arLanguageText[(int)eChosenLanguage].text;
					mlti.rInstance = rMultiLanguageText;
					acquiredMultiLanguageTextInstance.AddLast(mlti);
					Utility_MultiLanguageTextFileHandler.ReadFromFile(acquiredMultiLanguageTextInstance, eChosenLanguage);
					rMultiLanguageText.dm_rTextRenderer.text = rMultiLanguageText.m_arLanguageText[(int)eChosenLanguage].text;
					//rMultiLanguageText.m_arLanguageText[(int)eChosenLanguage].text = text;



					// Show Saved Values
					rMultiLanguageText.dm_rTextRenderer.rectTransform.localPosition = rTextDisplay.fontPosition;
					rMultiLanguageText.dm_rTextRenderer.font = rTextDisplay.chosenFont;
					rMultiLanguageText.dm_rTextRenderer.fontSize = rTextDisplay.fontSize;
					rMultiLanguageText.dm_rTextRenderer.lineSpacing = rTextDisplay.lineSpacing;
					rMultiLanguageText.dm_rTextRenderer.text = rTextDisplay.text;
					rMultiLanguageText.dm_rTextRenderer.alignment = rMultiLanguageText.GetTextAlignment(eChosenLanguage, rMultiLanguageText.dm_rTextRenderer);
					AddSpaces(4);
				}
			}
		}
	#endregion

	#region DRAW TRANSLATION DESCRIPTION OPTIONS (IF EDITING ENGLISH)
		// Otherwise if Enlgish, Show Option to Enter Translation Description!
		else
		{
			AddSpaces(1);
			Rect rectpos = GetScaledRect();
			rectpos.x += 60.0f;
			rectpos.width = 160.0f;
			GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
			foldoutStyle.normal.textColor = new Color32(36, 68, 196, 255);
			rMultiLanguageText.dm_bShowTranslationDescription = EditorGUI.Foldout(rectpos, rMultiLanguageText.dm_bShowTranslationDescription, (rMultiLanguageText.dm_bShowTranslationDescription ? "Hide Translation Description" : "Show Translation Description (Inform Translators' about the context of this Text)"), true, foldoutStyle);
			if(rMultiLanguageText.dm_bShowTranslationDescription)
			{
				AddSpaces(3);
				Rect scale = GetScaledRect();
				int lineCount = rMultiLanguageText.dm_sTranslationDescription.Split(new char[]{'\n'}).Length;
				if(lineCount < 2)
					lineCount = 2;
				scale.height = 13.5f * lineCount;
				AddSpaces((int)(lineCount * 2f));

				rMultiLanguageText.dm_sTranslationDescription = EditorGUI.TextArea(scale, rMultiLanguageText.dm_sTranslationDescription);
			}
        }
	#endregion


		#region DRAW POSITION OPTIONS
		// Draw Position Options
		{
			AddSpaces(3);
			Vector3 pos = rTextDisplay.fontPosition;
			Rect rectpos = GetScaledRect();
			rectpos.x = ((rectpos.x + rectpos.width) - 95.0f);
			rectpos.width = 90.0f;
			pos.x = EditorGUI.FloatField(rectpos, rTextDisplay.fontPosition.x);
			rectpos.x -= 25.0f;
			rectpos.y -= 2.25f;
			EditorGUI.LabelField(rectpos, new GUIContent("X :", "X Position of the font when rendering in this language"));

			AddSpaces(3);
			rectpos = GetScaledRect();
			rectpos.x = ((rectpos.x + rectpos.width) - 95.0f);
			rectpos.width = 90.0f;
			pos.y = EditorGUI.FloatField(rectpos, rTextDisplay.fontPosition.y);
			rectpos.x -= 25.0f;
			rectpos.y -= 2.25f;
			EditorGUI.LabelField(rectpos, new GUIContent("Y :", "Y Position of the font when rendering in this language"));

			AddSpaces(3);
			Rect buttonpos = rectpos = GetScaledRect();
			rectpos.x = ((rectpos.x + rectpos.width) - 95.0f);
			rectpos.width = 90.0f;
			pos.z = EditorGUI.FloatField(rectpos, rTextDisplay.fontPosition.z);
			rectpos.x -= 25.0f;
			rectpos.y -= 2.25f;
			EditorGUI.LabelField(rectpos, new GUIContent("Z :", "Z Position of the font when rendering in this language"));
			if (pos != rTextDisplay.fontPosition)
			{
				rTextDisplay.fontPosition = pos;
				if (rMultiLanguageText.dm_rTextRenderer != null)
					rMultiLanguageText.dm_rTextRenderer.rectTransform.localPosition = rTextDisplay.fontPosition;
			}


			buttonpos.x += ((buttonpos.width / 2) - 110);
			buttonpos.y -= 32;
			EditorGUI.LabelField(buttonpos, "Font Position:", EditorStyles.boldLabel);
			if (rMultiLanguageText.dm_rTextRenderer != null)
			{
				buttonpos.y += 20;
				buttonpos.x -= 75.0f;
				buttonpos.width = 135.0f;
				buttonpos.height = 18.0f;
				if (GUI.Button(buttonpos, new GUIContent("Copy Current Values", "Copy the current values that the TextRenderer is using... i.e. Does it look nice now? Then copy those values!")))
				{
					rTextDisplay.fontPosition = rMultiLanguageText.dm_rTextRenderer.rectTransform.localPosition;
				}
				buttonpos.x += 140.0f;
				if (GUI.Button(buttonpos, new GUIContent("Show Saved Values", "Apply the saved values (to the right) to the TextRenderer... i.e. The text position will be changed depending on what those values are (to the right)")))
				{
					rMultiLanguageText.dm_rTextRenderer.rectTransform.localPosition = rTextDisplay.fontPosition;
					rMultiLanguageText.dm_rTextRenderer.font = rTextDisplay.chosenFont;
					rMultiLanguageText.dm_rTextRenderer.fontSize = rTextDisplay.fontSize;
					rMultiLanguageText.dm_rTextRenderer.lineSpacing = rTextDisplay.lineSpacing;
                    rMultiLanguageText.dm_rTextRenderer.text = rTextDisplay.text;
					rMultiLanguageText.dm_rTextRenderer.alignment = rMultiLanguageText.GetTextAlignment(eChosenLanguage, rMultiLanguageText.dm_rTextRenderer);
				}
			}


			AddSpaces(5);
		}
		#endregion

		#region DRAW ALIGNMENT OPTIONS
		// Draw Alignment Options
		{
			Rect rectpos = GetScaledRect();
			float width = 200.0f;
			rectpos.x = ((rectpos.x + rectpos.width) - width);
			rectpos.width = width;
			TextAlignment newAlignment = (TextAlignment)EditorGUI.EnumPopup(rectpos, rTextDisplay.textAlignment);
			if(newAlignment != rTextDisplay.textAlignment)
			{
				rTextDisplay.textAlignment = newAlignment;
				if(rMultiLanguageText.dm_rTextRenderer != null)
				{
					rMultiLanguageText.dm_rTextRenderer.alignment = rMultiLanguageText.GetTextAlignment(eChosenLanguage, rMultiLanguageText.dm_rTextRenderer);
				}
			}
			rectpos.x -= 72.5f;
			EditorGUI.LabelField(rectpos, new GUIContent("Alignment:", "What will be the alignment of this text in this Selected Language"));
			AddSpaces(3);
		}
		#endregion

		#region DRAW FONT OPTIONS
		// Draw Font Options
		{
			int iNewFontSize = DrawChangeableIntegerOption("", rTextDisplay.fontSize);
			Rect rectpos = GetScaledRect();
			rectpos.x = ((rectpos.x + rectpos.width) - 153.5f);
			rectpos.y -= 6.5f;
			EditorGUI.LabelField(rectpos, new GUIContent("FontSize:", "Size of the font used when rendering in this language"));

			rectpos.x -= 90.0f;
			rectpos.width = 80.0f;
            float fNewLineSpacing = DrawChangeableFloatOption(rectpos, "", rTextDisplay.lineSpacing, 0.01f);
			rectpos.x -= 113.0f;
			rectpos.width = 180.0f;
			EditorGUI.LabelField(rectpos, new GUIContent("LineSpacing:", "Line Spacing used when rendering in this langauge"));


			rectpos.x = 97.0f;
			rectpos.width = 150.0f;
			Font rNewFontChoice = (Font)EditorGUI.ObjectField(rectpos, new GUIContent("", "Font to use when rendering in this language"), rTextDisplay.chosenFont, typeof(Font), true);
			rectpos.x -= 68.0f;
			rectpos.height = 20.0f;
			rectpos.width = 160.0f;
			EditorGUI.LabelField(rectpos, new GUIContent("FontType:", "Font to use when rendering in this language"));
			if (iNewFontSize != rTextDisplay.fontSize || rNewFontChoice != rTextDisplay.chosenFont || fNewLineSpacing != rTextDisplay.lineSpacing)
			{
				rTextDisplay.chosenFont = rNewFontChoice;
				rTextDisplay.fontSize = iNewFontSize;
				rTextDisplay.lineSpacing = fNewLineSpacing;
                if (rMultiLanguageText.dm_rTextRenderer != null)
				{
					rMultiLanguageText.dm_rTextRenderer.font = rNewFontChoice;
					rMultiLanguageText.dm_rTextRenderer.fontSize = iNewFontSize;
					if(fNewLineSpacing > 0.0f)
						rMultiLanguageText.dm_rTextRenderer.lineSpacing = fNewLineSpacing;
				}
			}

			// Make sure line spacing meets contraints
			if(rTextDisplay.lineSpacing < 0.05f && rMultiLanguageText.dm_rTextRenderer != null)
				rTextDisplay.lineSpacing = rMultiLanguageText.dm_rTextRenderer.lineSpacing;


			rectpos.y -= rectpos.height + 5;
			if (eChosenLanguage != GameManager.SystemLanguages.ENGLISH && eChosenLanguage != GameManager.SystemLanguages.CHINESE_SIMPLIFIED && 
				eChosenLanguage != GameManager.SystemLanguages.ARABIC && eChosenLanguage != GameManager.SystemLanguages.JAPANESE &&
				eChosenLanguage != GameManager.SystemLanguages.PERSIAN && eChosenLanguage != GameManager.SystemLanguages.RUSSIAN)
			{
				if (GUI.Button(rectpos, "Provide Nightingale Font"))
				{
					rTextDisplay.chosenFont = rMultiLanguageText.m_arLanguageText[0].chosenFont;
					rTextDisplay.fontPosition = rMultiLanguageText.m_arLanguageText[0].fontPosition;
					rTextDisplay.lineSpacing = rMultiLanguageText.m_arLanguageText[0].lineSpacing;
					rTextDisplay.fontSize = rMultiLanguageText.m_arLanguageText[0].fontSize;
					if (rMultiLanguageText.dm_rTextRenderer != null)
					{
						rMultiLanguageText.dm_rTextRenderer.font = rNewFontChoice;
						rMultiLanguageText.dm_rTextRenderer.fontSize = iNewFontSize;
						rMultiLanguageText.dm_rTextRenderer.rectTransform.localPosition = rTextDisplay.fontPosition;
						if (rTextDisplay.lineSpacing > 0.0f)
							rMultiLanguageText.dm_rTextRenderer.lineSpacing = rTextDisplay.lineSpacing;
					}
				}
			}
			rectpos.x += (rectpos.width + 15);
			if(GUI.Button(rectpos, "Remove \"Font\" Tags"))
			{
				rTextDisplay.text = rTextDisplay.text.Replace("<Font>", "").Replace("</Font>", "");
				if(rMultiLanguageText.dm_rTextRenderer != null)
					rMultiLanguageText.dm_rTextRenderer.text = rTextDisplay.text;
			}

			AddSpaces(3);
		}
	#endregion

		#region DRAW TEXT OPTIONS
		// Draw Text Options
		EditorGUI.indentLevel += 1;
		{
			Rect scale = GetScaledRect();
			int lineCount = rTextDisplay.text.Split(new char[]{'\n'}).Length;
			if(lineCount < 2)
				lineCount = 2;
			scale.height = 13.5f * lineCount;
			AddSpaces((int)(lineCount * 2f));

			// If no English Text has been assigned. Assign whatever text is in the TextRenderer. Chances are are it's in English anyway.
			if (bForceAutoEnglishComplete && eChosenLanguage == GameManager.SystemLanguages.ENGLISH && rMultiLanguageText.dm_rTextRenderer != null)
			{
				if (rTextDisplay.text == "")
				{
					CopyTextRendererValuesIntoMultiLanguageTextComponent(rTextDisplay, rMultiLanguageText.dm_rTextRenderer);
				}
			}

			// No Font is Set? Default to Arial!
			if (rTextDisplay.chosenFont == null)
				rTextDisplay.chosenFont = Resources.GetBuiltinResource<Font>("Arial.ttf");



			// Change Text, show changes in scene if possible
			string newText = EditorGUI.TextArea(scale, rTextDisplay.text);
			if (newText != rTextDisplay.text)
			{
				rTextDisplay.text = newText;
				CopyMultiLanguageTextComponentValuesIntoTextRenderer(rMultiLanguageText.dm_rTextRenderer, rTextDisplay);
			}

			// If Text in TextRenderer is different. Do you want to copy that instead?
			if (rTextDisplay != null && rMultiLanguageText.dm_rTextRenderer != null)
			{
				AddSpaces(2);
				Rect drawPos = GetScaledRect();
				float btnWidth = 215.0f;
				drawPos.x += (drawPos.width - btnWidth);
				if (rTextDisplay.text != rMultiLanguageText.dm_rTextRenderer.text)
				{
					drawPos.width = btnWidth;
					drawPos.height = 18.0f;
					if (GUI.Button(drawPos, new GUIContent("Copy Text From Text Renderer", "Text in the TextRenderer is different from the Text above. Do you want to copy the TextRenderer text into the text above?")))
					{
						//string message = "Replacing Multi-Language Text:\n\t" + rTextDisplay.text + "\n\nWith TextRenderer Text:\n\t" + rMultiLanguageText.dm_rTextRenderer.text + "\n\n\n\nAre you sure you wish to copy text from the text renderer?";
						//if (EditorUtility.DisplayDialog("Are you sure?", message, "Confirm", "Deny"))
							rTextDisplay.text = rMultiLanguageText.dm_rTextRenderer.text;
					}
				}
				if (rMultiLanguageText.dm_rTextRenderer != null)
				{
					drawPos.x -= btnWidth;
					drawPos.width = btnWidth;
					drawPos.height = 18.0f;
					BooleanState wrapState = (rMultiLanguageText.dm_rTextRenderer.horizontalOverflow == HorizontalWrapMode.Wrap ? BooleanState.TRUE : BooleanState.FALSE);
					BooleanState changedState = (BooleanState)EditorGUI.EnumPopup(drawPos, wrapState);
					if (changedState != wrapState)
					{
						rMultiLanguageText.dm_rTextRenderer.horizontalOverflow = (changedState == BooleanState.TRUE ? HorizontalWrapMode.Wrap : HorizontalWrapMode.Overflow);
					}
					drawPos.x -= 85.0f;
					EditorGUI.LabelField(drawPos, "Wrap Text:");


					if (eChosenLanguage == GameManager.SystemLanguages.ARABIC || eChosenLanguage == GameManager.SystemLanguages.PERSIAN || eChosenLanguage == GameManager.SystemLanguages.URDU)
					{
						AddSpaces(2);
						if (GUILayout.Button("Attempt to Fix Right-To-Left Text  =>  RTL"))
						{
							string text = RTLService.RTL.Convert(rTextDisplay.text);
							rMultiLanguageText.dm_rTextRenderer.text = text;
						}
						if (GUILayout.Button("Attempt to Fix Right-To-Left Text  =>  ArbSupport"))
						{
							string text = ArabicSupport.ArabicFixer.Fix(rTextDisplay.text);
							rMultiLanguageText.dm_rTextRenderer.text = text;
						}
						AddSpaces(2);
						if (GUILayout.Button("Attempt To Fix Right-To-Left Text with WordWrap  =>  RTL"))
						{
							string text = RTLService.RTL.ConvertWordWrap(rTextDisplay.text, rMultiLanguageText.dm_rTextRenderer.flexibleWidth, rTextDisplay.chosenFont);
							rMultiLanguageText.dm_rTextRenderer.text = text;
						}
						AddSpaces(3);
						string support = EditorGUILayout.TextField("Arabic Text To Convert", m_sArabicText);
						if (support != m_sArabicText)
						{
							m_sArabicText		= support;
							m_sArabicFixText	= ArabicSupport.ArabicFixer.Fix(support);
							m_sArabicRTLText	= RTLService.RTL.Convert(support, RTLService.RTL.NumberFormat.English);
						}
						EditorGUILayout.TextField("Arabic Fix Text", m_sArabicFixText);
						EditorGUILayout.TextField("Arabic RTL Text", m_sArabicRTLText);
					}
				}

			}
		}
		EditorGUI.indentLevel -= 1;
		AddSpaces(4);
	#endregion
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Set Game Language
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void SetGameLanguage(GameManager.SystemLanguages eChosenLanguage)
	{
		if(Camera.main != null && Camera.main.GetComponent<GameManager>() != null)
		{
			Camera.main.GetComponent<GameManager>().m_eSystemLanguages = eChosenLanguage;
			int iLanguageID = (int)eChosenLanguage;
			var sceneList = CustomEditor_GameManager.ScanForMultiLanguageTextComponents(Camera.main.GetComponent<GameManager>(), eChosenLanguage, CustomEditor_GameManager.MultiLanguageTextReceiveMode.ALL);
			foreach(var list in sceneList)
			{
				foreach(var instance in list)
				{
					MultiLanguageText languageInstance = instance.rInstance;
					if (languageInstance.dm_rTextRenderer != null)
					{
						if (languageInstance.m_arLanguageText.Length > iLanguageID)
						{
							languageInstance.dm_rTextRenderer.rectTransform.localPosition = languageInstance.m_arLanguageText[iLanguageID].fontPosition;
							languageInstance.dm_rTextRenderer.font = languageInstance.m_arLanguageText[iLanguageID].chosenFont;
							languageInstance.dm_rTextRenderer.fontSize = languageInstance.m_arLanguageText[iLanguageID].fontSize;
							languageInstance.dm_rTextRenderer.lineSpacing = languageInstance.m_arLanguageText[iLanguageID].lineSpacing;
							languageInstance.dm_rTextRenderer.text = languageInstance.m_arLanguageText[iLanguageID].text;
							languageInstance.dm_rTextRenderer.alignment = languageInstance.GetTextAlignment(eChosenLanguage, languageInstance.dm_rTextRenderer);

							languageInstance.dm_rTextRenderer.horizontalOverflow = ((eChosenLanguage == GameManager.SystemLanguages.ARABIC || eChosenLanguage == GameManager.SystemLanguages.PERSIAN || eChosenLanguage == GameManager.SystemLanguages.URDU) ? HorizontalWrapMode.Overflow : HorizontalWrapMode.Wrap);
						}
					}
				}
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Copy Text Renderer Value Into Multi-Language Text Component
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void CopyTextRendererValuesIntoMultiLanguageTextComponent(MultiLanguageText.TextDisplayValues rMultiLanguageTextDisplay, UnityEngine.UI.Text rTextRenderer)
	{
		if(rTextRenderer != null && rMultiLanguageTextDisplay != null)
		{
			rMultiLanguageTextDisplay.text			= rTextRenderer.text;
			rMultiLanguageTextDisplay.chosenFont	= rTextRenderer.font;
			rMultiLanguageTextDisplay.fontSize		= rTextRenderer.fontSize;
			rMultiLanguageTextDisplay.lineSpacing	= rTextRenderer.lineSpacing;
			rMultiLanguageTextDisplay.fontPosition	= rTextRenderer.rectTransform.localPosition;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Copy Into Multi-Language Text Component Values Into Text Renderer
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void CopyMultiLanguageTextComponentValuesIntoTextRenderer(UnityEngine.UI.Text rTextRenderer, MultiLanguageText.TextDisplayValues rMultiLanguageTextDisplay)
	{
		if(rTextRenderer != null && rMultiLanguageTextDisplay != null)
		{
			rTextRenderer.text							= rMultiLanguageTextDisplay.text;
			rTextRenderer.font							= rMultiLanguageTextDisplay.chosenFont;
			rTextRenderer.fontSize						= rMultiLanguageTextDisplay.fontSize;
			rTextRenderer.lineSpacing					= rMultiLanguageTextDisplay.lineSpacing;
			rTextRenderer.rectTransform.localPosition	= rMultiLanguageTextDisplay.fontPosition;
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Serialized Object Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawSerialisedObjectOptions(string name, string variableName, string tooltip = "")
	{
		serializedObject.Update();
		SerializedProperty property = serializedObject.FindProperty(variableName);
		EditorGUI.BeginChangeCheck();
		GUIContent label = (tooltip != "" ? new GUIContent(name, tooltip) : new GUIContent(name));
		EditorGUILayout.PropertyField(property, label, true);
		if(EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Array Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawArrayOptions(string name, string arrayVariableName, string tooltip = "")
	{
		serializedObject.Update();
		SerializedProperty property = serializedObject.FindProperty(arrayVariableName);
		EditorGUI.BeginChangeCheck();
		GUIContent label = (tooltip != "" ? new GUIContent(name, tooltip) : new GUIContent(name));
		EditorGUILayout.PropertyField(property, label, true);
		if(EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
			OnArrayModification(arrayVariableName);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Resize Array
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static void ResizeNonReferenceArray<T>(ref T[] arrayField, int newSize)
	{
		T[] newArray = new T[newSize];
		for(int i = 0; i < newSize; ++i)
		{
			if(arrayField.Length > i)
			{
				OnArrayVariableModification(ref newArray[i], ref arrayField[i]);
			}
		}

		arrayField = newArray;
		OnArrayModification(ref arrayField);
	}

	protected static void ResizeArray<T>(ref T[] arrayField, int newSize) where T: class
	{
		T[] newArray = new T[newSize];
		for (int i = 0; i < newSize; ++i)
		{
			if (arrayField.Length > i)
			{
				OnArrayVariableModification(ref newArray[i], ref arrayField[i]);
			}
			else
			{
				OnArrayVariableModification(ref newArray[i]); 
			}
		}

		arrayField = newArray;
		OnArrayModification(ref arrayField);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Prepend Array
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static void PrependArray<T>(ref T[] original, ref T[] prependingArray)
	{
		T[] newArray = prependingArray.Clone() as T[];
		AppendArray(ref newArray, ref original);
		original = newArray;
    }
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Append Array
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static void AppendArray<T>(ref T[] original, ref T[] appendingArray)
	{
		int originalLength = (original != null ? original.Length : 0);
		int newSize = originalLength + appendingArray.Length;
        T[] newArray = new T[newSize];

		int appendingIndex = 0;
		for(int i = 0; i < newSize; ++i)
		{
			if(originalLength > i)
			{
				OnArrayVariableModification(ref newArray[i], ref original[i]);
			}
			else
			{
				newArray[i] = appendingArray[appendingIndex++];
            }
		}

		original = newArray;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Callback Methods: On Array Modification
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected static void OnArrayVariableModification<T>(ref T destination) where T: class
	{
		// MonoBehaviour is not allow to be instantiated in Code without using the Instantiate Method. This will cause a warning... Hence the check for 'MonoBehaviour'
		//try
		//{
		//	if (typeof(T) != typeof(MonoBehaviour))
		//		destination = System.Activator.CreateInstance(typeof(T), true) as T;
		//}
		//catch (System.Exception e)
		//{
		//	return;
		//}
	}

	protected static void OnArrayVariableModification<T>(ref T destination, ref T source)
	{
		destination = source;
	}

	protected static void OnArrayModification(string whichArray)
	{
	}

	protected static void OnArrayModification<T>(ref T[] arrayField)
	{
	}





	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Draw Colour Animation Effect Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void DrawColourAnimationEffectOptions(ref ColourAnimationEffect[] rAnimationEffects, Material mTarget, SpriteRenderer sprTarget)
	{
		int iTotalSize = DrawIntField("Total Animation Frames: ", rAnimationEffects.Length);
		if (iTotalSize != rAnimationEffects.Length)
		{
			rAnimationEffects = ColourAnimationEffect.ResizeArray(rAnimationEffects, iTotalSize);
		}

		for (int i = 0; i < rAnimationEffects.Length; ++i)
		{
			// Assign new instance if not already existing
			if (rAnimationEffects[i] == null)
				rAnimationEffects[i] = new ColourAnimationEffect();

			DrawColourAnimationEffectOptions(ref rAnimationEffects[i], mTarget, sprTarget);
			if (rAnimationEffects[i].m_bDisplayAnimationOptions)
				AddSpaces(3);
		}
	}

	public void DrawColourAnimationEffectOptions(ref ColourAnimationEffect rAnimationEffect, Material mTarget, SpriteRenderer sprTarget)
	{
		rAnimationEffect.Target = mTarget;
		if (mTarget == null)
			return;

		EditorGUI.indentLevel += 1;
		{
			//~~~ Draw Name of Animation Effect Option and Target Transform Object ~~~
			{
				AddSpaces(1);
				EditorGUI.ObjectField(new Rect(320, GUILayoutUtility.GetLastRect().y, GUILayoutUtility.GetLastRect().width - 320, 16), sprTarget.transform, typeof(Transform), true);
				rAnimationEffect.m_sEffectName = EditorGUI.TextArea(new Rect(50, GUILayoutUtility.GetLastRect().y, 275, 16), rAnimationEffect.m_sEffectName);
				rAnimationEffect.m_bDisplayAnimationOptions = EditorGUI.Foldout(new Rect(0, GUILayoutUtility.GetLastRect().y, 100, 16), rAnimationEffect.m_bDisplayAnimationOptions, rAnimationEffect.m_bDisplayAnimationOptions ? "Hide" : "Show", true, EditorStyles.foldout);
				AddSpaces(2);
			}

			if (rAnimationEffect.m_bDisplayAnimationOptions)
			{
				// Draw Global Copy/Paste Options
				bool showClipboardOptions = true;
				if (showClipboardOptions)
				{
					AddSpaces(1);
					int draw_xPos = EditorGUI.indentLevel * 20;
					if (GUI.Button(new Rect(draw_xPos, GUILayoutUtility.GetLastRect().y, (GUILayoutUtility.GetLastRect().width / 2) - (10 + (draw_xPos / 2)), 16), new GUIContent("Copy Animation Effect Values", "Copies the values of this Animation Effect into a global clipboard which can then be  used to paste these values into another Animation Effect")))
					{
						ColourAnimationEffect.sm_rAnimationEffectInstance = rAnimationEffect;
					}
					if (GUI.Button(new Rect((draw_xPos / 2) + ((GUILayoutUtility.GetLastRect().width / 2) - 5), GUILayoutUtility.GetLastRect().y, (GUILayoutUtility.GetLastRect().width / 2), 16), new GUIContent("Paste Copied Animation Effect Values", "Paste the values of the Animation Effect which has been copied into the global clipboard (if any exist)")))
					{
						if (AnimationEffect.sm_rAnimationEffectInstance == null)
						{
							EditorUtility.DisplayDialog("Error!", "There is no Colour Animation Effect to copy!", "Okay");
						}
						else
						{
							string values = "  Start Colour:  \t" + ColourAnimationEffect.sm_rAnimationEffectInstance.m_cStartingColour.ToString() + "\n" +
											"  End Colour:    \t" + ColourAnimationEffect.sm_rAnimationEffectInstance.m_cEndColour.ToString();
							if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to replace the current Animation Effect (" + rAnimationEffect.m_sEffectName + ") with the copied one (" + AnimationEffect.sm_rAnimationEffectInstance.m_sEffectName + ")?  \n\nContaining Values:\n" + values, "Confirm", "Deny"))
							{
								rAnimationEffect = ColourAnimationEffect.sm_rAnimationEffectInstance.Clone();
							}
						}
					}
					AddSpaces(2);
				}


				EditorGUI.indentLevel += 1;
				{
					rAnimationEffect.m_fTotalAnimationTime = EditorGUILayout.FloatField("Total Animation Time: ", rAnimationEffect.m_fTotalAnimationTime);

					//~~~ Starting Colour ~~~
					Color newColourValue = EditorGUILayout.ColorField("Starting Colour: ", rAnimationEffect.m_cStartingColour);
					if (newColourValue != rAnimationEffect.m_cStartingColour)
					{
						rAnimationEffect.m_cStartingColour = newColourValue;
						sprTarget.color = newColourValue;
						sprTarget.sharedMaterial.SetColor("_Colour", sprTarget.color);
					}
					//~~~ Ending Colour ~~~
					newColourValue = EditorGUILayout.ColorField("Ending Colour: ", rAnimationEffect.m_cEndColour);
					if (newColourValue != rAnimationEffect.m_cEndColour)
					{
						rAnimationEffect.m_cEndColour = newColourValue;
						sprTarget.color = newColourValue;
						sprTarget.sharedMaterial.SetColor("_Colour", sprTarget.color);
					}

					//~~~ GUI Slider Movement ~~~
					float percentage = EditorGUILayout.Slider("Reveal: ", rAnimationEffect.m_fCompletionRange, 0.0f, 1.0f);
					if (rAnimationEffect.m_fCompletionRange != percentage)
					{
						rAnimationEffect.m_fCompletionRange = percentage;
						sprTarget.color = Color.Lerp(rAnimationEffect.m_cStartingColour, rAnimationEffect.m_cEndColour, percentage);
						sprTarget.sharedMaterial.SetColor("_Colour", sprTarget.color);
					}


					//~~~ Copy/Paste Button Options ~~~
					EditorGUI.indentLevel += 1;
					{
						AddSpaces(1);
						EditorGUILayout.LabelField(new GUIContent("Start Options: "));
						float x = 195;
						float w = 105;
						float ew = 180;
						Rect Pos = new Rect(x, GUILayoutUtility.GetLastRect().y, w, 20);
						if (GUI.Button(Pos, "Copy Colour"))
						{
							rAnimationEffect.m_cStartingColour = sprTarget.color;
						}
						Pos.x += Pos.width + 10;
						Pos.width = ew;
						if (GUI.Button(Pos, "Show Colour"))
						{
							sprTarget.color = rAnimationEffect.m_cStartingColour;
							sprTarget.sharedMaterial.SetColor("_Colour", sprTarget.color);
						}
						EditorGUILayout.LabelField(new GUIContent("End Options: "));
						Pos.x = x;
						Pos.width = w;
						Pos.y += Pos.height;
						AddSpaces(1);
						if (GUI.Button(Pos, "Copy Colour"))
						{
							rAnimationEffect.m_cEndColour = sprTarget.color;
						}
						Pos.x += Pos.width + 10;
						Pos.width = ew;
						if (GUI.Button(Pos, "Show Colour"))
						{
							sprTarget.color = rAnimationEffect.m_cEndColour;
							sprTarget.sharedMaterial.SetColor("_Colour", sprTarget.color);
						}
						AddSpaces(1);
					}
					EditorGUI.indentLevel -= 1;
				}
				EditorGUI.indentLevel -= 1;
			}
		}
		EditorGUI.indentLevel -= 1;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Methods: Draw Animation Effect Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public void DrawAnimationEffectOptions(ref AnimationEffect[] rAnimationEffects, Transform TrTarget = null)
	{
		int iTotalSize = DrawIntField("Total Animation Frames: ", rAnimationEffects.Length);
		if (iTotalSize != rAnimationEffects.Length)
		{
			rAnimationEffects = AnimationEffect.ResizeArray(rAnimationEffects, iTotalSize);
		}

		for (int i = 0; i < rAnimationEffects.Length; ++i)
		{
			// Assign new instance if not already existing
			if (rAnimationEffects[i] == null)
				rAnimationEffects[i] = new AnimationEffect();

			DrawAnimationEffectOptions(ref rAnimationEffects[i], TrTarget);
			if (rAnimationEffects[i].m_bDisplayAnimationOptions)
				AddSpaces(3);
		}
	}

	public void DrawAnimationEffectOptions(ref AnimationEffect rAnimationEffect, Transform TrTarget)
	{
		rAnimationEffect.Target = TrTarget;

		EditorGUI.indentLevel += 1;
		{
			//~~~ Draw Name of Animation Effect Option and Target Transform Object ~~~
			{
				AddSpaces(1);
				EditorGUI.ObjectField(new Rect(320, GUILayoutUtility.GetLastRect().y, GUILayoutUtility.GetLastRect().width - 320, 16), TrTarget, typeof(Transform), true);
				rAnimationEffect.m_sEffectName = EditorGUI.TextArea(new Rect(50, GUILayoutUtility.GetLastRect().y, 275, 16), rAnimationEffect.m_sEffectName);
				rAnimationEffect.m_bDisplayAnimationOptions = EditorGUI.Foldout(new Rect(0, GUILayoutUtility.GetLastRect().y, 100, 16), rAnimationEffect.m_bDisplayAnimationOptions, rAnimationEffect.m_bDisplayAnimationOptions ? "Hide" : "Show", true, EditorStyles.foldout);
				AddSpaces(2);
			}

			if (rAnimationEffect.m_bDisplayAnimationOptions)
			{
				// Draw Global Copy/Paste Options
				bool showClipboardOptions = true;
				if (showClipboardOptions)
				{
					AddSpaces(1);
					int draw_xPos = EditorGUI.indentLevel * 20;
					if (GUI.Button(new Rect(draw_xPos, GUILayoutUtility.GetLastRect().y, (GUILayoutUtility.GetLastRect().width / 2) - (10 + (draw_xPos / 2)), 16), new GUIContent("Copy Animation Effect Values", "Copies the values of this Animation Effect into a global clipboard which can then be  used to paste these values into another Animation Effect")))
					{
						AnimationEffect.sm_rAnimationEffectInstance = rAnimationEffect;
					}
					if (GUI.Button(new Rect((draw_xPos / 2) + ((GUILayoutUtility.GetLastRect().width / 2) - 5), GUILayoutUtility.GetLastRect().y, (GUILayoutUtility.GetLastRect().width / 2), 16), new GUIContent("Paste Copied Animation Effect Values", "Paste the values of the Animation Effect which has been copied into the global clipboard (if any exist)")))
					{
						if (AnimationEffect.sm_rAnimationEffectInstance == null)
						{
							EditorUtility.DisplayDialog("Error!", "There is no Animation Effect to copy!", "Okay");
						}
						else
						{
							string values = "  Start Position:\t" + AnimationEffect.sm_rAnimationEffectInstance.m_vStartingPosition.ToString() + "\n" +
											"  End Position:  \t" + AnimationEffect.sm_rAnimationEffectInstance.m_vEndPosition.ToString() + "\n" +
											"  Start Rotation:\t" + AnimationEffect.sm_rAnimationEffectInstance.m_vStartingRotation.ToString() + "\n" +
											"  End Rotation   \t" + AnimationEffect.sm_rAnimationEffectInstance.m_vEndRotation.ToString() + "\n" +
											"  Start Scale:   \t" + AnimationEffect.sm_rAnimationEffectInstance.m_vStartingScale.ToString() + "\n" +
											"  End Scale:     \t" + AnimationEffect.sm_rAnimationEffectInstance.m_vEndScale.ToString() + "\n" +
											"  Start Colour:  \t" + AnimationEffect.sm_rAnimationEffectInstance.m_cStartingColour.ToString() + "\n" +
											"  End Colour:    \t" + AnimationEffect.sm_rAnimationEffectInstance.m_cEndColour.ToString() + "\n" +
											(AnimationEffect.sm_rAnimationEffectInstance.m_sprNewSprite != null ? "  New Sprite:    \t" + AnimationEffect.sm_rAnimationEffectInstance.m_sprNewSprite.name : "");
							if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to replace the current Animation Effect (" + rAnimationEffect.m_sEffectName + ") with the copied one (" + AnimationEffect.sm_rAnimationEffectInstance.m_sEffectName + ")?  \n\nContaining Values:\n" + values, "Confirm", "Deny"))
							{
								rAnimationEffect = AnimationEffect.sm_rAnimationEffectInstance.Clone();
							}
						}
					}
					AddSpaces(2);
				}


				EditorGUI.indentLevel += 1;
				{
					rAnimationEffect.m_fTotalAnimationTime = EditorGUILayout.FloatField("Total Animation Time: ", rAnimationEffect.m_fTotalAnimationTime);

					//~~~ Starting Position ~~~
					Vector3 newVectorValue = EditorGUILayout.Vector3Field("Starting Position: ", rAnimationEffect.m_vStartingPosition);
					if (newVectorValue != rAnimationEffect.m_vStartingPosition)
					{
						rAnimationEffect.m_vStartingPosition = newVectorValue;
						TrTarget.localPosition = newVectorValue;
					}
					//~~~ Ending Position ~~~
					if (rAnimationEffect.m_vStartingPosition != rAnimationEffect.m_vEndPosition)
					{
						newVectorValue = EditorGUILayout.Vector3Field("Ending Position: ", rAnimationEffect.m_vEndPosition);
						if (newVectorValue != rAnimationEffect.m_vEndPosition)
						{
							rAnimationEffect.m_vEndPosition = newVectorValue;
							TrTarget.localPosition = newVectorValue;
						}
					}

					//~~~ Starting Rotation ~~~
					newVectorValue = EditorGUILayout.Vector3Field("Starting Rotation: ", rAnimationEffect.m_vStartingRotation);
					if (newVectorValue != rAnimationEffect.m_vStartingRotation)
					{
						rAnimationEffect.m_vStartingRotation = newVectorValue;
						TrTarget.localRotation = Quaternion.Euler(newVectorValue);
					}
					//~~~ Ending Rotation ~~~
					if (rAnimationEffect.m_vStartingRotation != rAnimationEffect.m_vEndRotation)
					{
						newVectorValue = EditorGUILayout.Vector3Field("Ending Rotation: ", rAnimationEffect.m_vEndRotation);
						if (newVectorValue != rAnimationEffect.m_vEndRotation)
						{
							rAnimationEffect.m_vEndRotation = newVectorValue;
							TrTarget.localRotation = Quaternion.Euler(newVectorValue);
						}
					}

					//~~~ Starting Scale ~~~
					newVectorValue = EditorGUILayout.Vector3Field("Starting Scale: ", rAnimationEffect.m_vStartingScale);
					if (newVectorValue != rAnimationEffect.m_vStartingScale)
					{
						rAnimationEffect.m_vStartingScale = newVectorValue;
						TrTarget.localScale = newVectorValue;
					}
					//~~~ Ending Scale ~~~
					if (rAnimationEffect.m_vStartingScale != rAnimationEffect.m_vEndScale)
					{
						newVectorValue = EditorGUILayout.Vector3Field("Ending Scale: ", rAnimationEffect.m_vEndScale);
						if (newVectorValue != rAnimationEffect.m_vEndScale)
						{
							rAnimationEffect.m_vEndScale = newVectorValue;
							TrTarget.localScale = newVectorValue;
						}
					}

					//~~~ Starting Colour ~~~
					Color newColourValue = EditorGUILayout.ColorField("Starting Colour: ", rAnimationEffect.m_cStartingColour);
					if (newColourValue != rAnimationEffect.m_cStartingColour)
					{
						rAnimationEffect.m_cStartingColour = newColourValue;
						if (TrTarget.GetComponent<SpriteRenderer>() != null) TrTarget.GetComponent<SpriteRenderer>().color = newColourValue;
						else if (TrTarget.GetComponent<UnityEngine.UI.Image>() != null) TrTarget.GetComponent<UnityEngine.UI.Image>().color = newColourValue;
						else if (TrTarget.GetComponent<UnityEngine.UI.Text>() != null) TrTarget.GetComponent<UnityEngine.UI.Text>().color = newColourValue;
					}
					//~~~ Ending Colour ~~~
					newColourValue = EditorGUILayout.ColorField("Ending Colour: ", rAnimationEffect.m_cEndColour);
					if (newColourValue != rAnimationEffect.m_cEndColour)
					{
						rAnimationEffect.m_cEndColour = newColourValue;
						if (TrTarget.GetComponent<SpriteRenderer>() != null) TrTarget.GetComponent<SpriteRenderer>().color = newColourValue;
						else if (TrTarget.GetComponent<UnityEngine.UI.Image>() != null) TrTarget.GetComponent<UnityEngine.UI.Image>().color = newColourValue;
						else if (TrTarget.GetComponent<UnityEngine.UI.Text>() != null) TrTarget.GetComponent<UnityEngine.UI.Text>().color = newColourValue;
					}

					//~~~ First Frame - New Sprite ~~~
					rAnimationEffect.m_sprNewSprite = (Sprite)EditorGUILayout.ObjectField("New Sprite: ", rAnimationEffect.m_sprNewSprite, typeof(Sprite), true);


					//~~~ GUI Slider Movement ~~~
					float percentage = rAnimationEffect.m_fCompletionRange;
					rAnimationEffect.m_fCompletionRange = EditorGUILayout.Slider("Reveal: ", rAnimationEffect.m_fCompletionRange, 0.0f, 1.0f);
					if (rAnimationEffect.m_fCompletionRange != percentage)
					{
						rAnimationEffect.UpdateFromSliderGUI();
					}


					//~~~ Copy/Paste Button Options ~~~
					EditorGUI.indentLevel += 1;
					{
						AddSpaces(1);
						EditorGUILayout.LabelField(new GUIContent("Start Options: "));
						float x = 195;
						float w = 105;
						float ew = 180;
						Rect Pos = new Rect(x, GUILayoutUtility.GetLastRect().y, w, 20);
						if (GUI.Button(Pos, "Copy Transform"))
						{
							rAnimationEffect.CopyTransformToBegin();
						}
						Pos.x += Pos.width + 10;
						Pos.width = ew;
						if (GUI.Button(Pos, "Show Animation Transform"))
						{
							rAnimationEffect.ShowBeginTransform();
						}
						EditorGUILayout.LabelField(new GUIContent("End Options: "));
						Pos.x = x;
						Pos.width = w;
						Pos.y += Pos.height;
						AddSpaces(1);
						if (GUI.Button(Pos, "Copy Transform"))
						{
							rAnimationEffect.CopyTransformToEnd();
						}
						Pos.x += Pos.width + 10;
						Pos.width = ew;
						if (GUI.Button(Pos, "Show Animation Transform"))
						{
							rAnimationEffect.ShowEndTransform();
						}
						AddSpaces(1);
					}
					EditorGUI.indentLevel -= 1;
				}
				EditorGUI.indentLevel -= 1;
			} // ~~~ if(rAnimationEffect.m_bDisplayAnimationOptions)
		}
		EditorGUI.indentLevel -= 1;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Show "Open File Dialogue"
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	/// <summary>
	/// Opens up the "Open File Dialog" Window on both Mac & PC. Will allow you to select what you want to have selected
	/// </summary>
	/// <param name="filters">Pass in the extensions you wish to allow the user (ie. you) to be able to select. EG: "txt". Pass in with a separating semicolon
	/// for multiple extension types (EG: "ogg;mp3;wav")</param>
	/// <returns>The absolute path to the desired file. Will return an empty string if the user chooses to exit the window without selecting a file</returns>
	protected string ShowOpenFileDialogueWindow(string filters)
	{
		string openFilename = EditorUtility.OpenFilePanel("Open File", sm_sOpenPathDirectory, filters);
		if(openFilename != "")
		{
			sm_sOpenPathDirectory = System.IO.Path.GetDirectoryName(openFilename) + '/';
		}
		return openFilename;
	}
}
