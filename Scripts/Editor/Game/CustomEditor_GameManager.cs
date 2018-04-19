//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor - Game Manager
//             Author: Christopher Diamond
//             Date: November 26, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
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
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public partial class CustomEditor_GameManager : CustomEditor_Base<GameManager>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*- Private Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private MultiLanguageTextList[] m_alMultiLanguageTextItems;
	private GameManager.SystemLanguages m_eSelectedLanguage = GameManager.SystemLanguages.TOTAL_LANGUAGES;
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return  "This script maintains important values to the system. Including whether or\n" +
					"not the game is the lite/full version and the chosen language of the system.";
		}
	}

	protected override InspectorRegion[] AdditionalRegions
	{
		get
		{
			return new InspectorRegion[]
			{
				new InspectorRegion()
				{
					label = "~Multi-Language Text Options~ (Translate to other languages)",
					representingDrawMethod = DrawMultiLanguageTextSystemOptions
				}
			};
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*{} Class Declarations
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public enum MultiLanguageTextReceiveMode
	{
		ALL,
		BLANK_ONLY,
		FILLED_ONLY,
	}

	public class MultiLanguageTextInstanceInfo
	{
		public MultiLanguageText rInstance;
		public Component rOwningParent;
		public string sDisplayLabel = "";
		public bool bShowDisplay = false;
	}

	public class MultiLanguageTextList : LinkedList<MultiLanguageTextInstanceInfo>
	{
		public new bool Contains(MultiLanguageTextInstanceInfo instance)
		{
			foreach(var val in this)
				if(val.rInstance == instance.rInstance)
					return true;
			return false;
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Editable Values Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawEditableValuesOptions()
	{
		if(GUILayout.Button("Clear PLAYER PREFS"))
		{
			if(EditorUtility.DisplayDialog("Are you sure?", "Clear the Player Prefs?", "Confirm", "Deny"))
				PlayerPrefs.DeleteAll();
		}

		AddSpaces(4);

		if(Target.dm_asReleasedAppName.Length != (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT * 2)
			ResizeNonReferenceArray(ref Target.dm_asReleasedAppName, (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT * 2);
		if (Target.dm_asLiteVersionBundleID.Length != (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT)
			ResizeNonReferenceArray(ref Target.dm_asLiteVersionBundleID, (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT);
		if (Target.dm_asFullVersionBundleID.Length != (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT)
			ResizeNonReferenceArray(ref Target.dm_asFullVersionBundleID, (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT);

		GameManager.BuildVersion eBuildType = (GameManager.BuildVersion)EditorGUILayout.EnumPopup(new GUIContent("Selected Build Version: ", "Some features will be locked in the lite version whilst accessible in the full version. Please make sure this option is set to full version when intended for release as full version"), Target.m_eBuildVersion);
		GameManager.DisplayableLanguagesTypes eDisplayableLanguage = (GameManager.DisplayableLanguagesTypes)EditorGUILayout.EnumPopup(new GUIContent("Displayable Language: ", "Determines Which Language WIll Be Visible Once Released To Public"), Target.dm_eDisplayableLanguagesTypes);
		for(int i = 0; i < (int)GameManager.DisplayableLanguagesTypes.TOTAL_OPTIONS_COUNT; ++i)
		{
			DrawLabel("~" + ((GameManager.DisplayableLanguagesTypes)i).ToString() + " Options~", EditorStyles.boldLabel);
			EditorGUI.indentLevel += 1;
			{
				Target.dm_asReleasedAppName[i * 2] = EditorGUILayout.TextField(new GUIContent("Lite Release Name:", "Name Given to this App Upon Release in this state"), Target.dm_asReleasedAppName[i * 2]);
				Target.dm_asLiteVersionBundleID[i] = EditorGUILayout.TextField(new GUIContent("Lite Version Bundle Identity:", "Bundle Identity Given to this app when released as the Lite Version.\n\nMust be Unique and must not be the same as the Full Version Bundle Identity"), Target.dm_asLiteVersionBundleID[i]);
				AddSpaces(1);
				Target.dm_asReleasedAppName[(i * 2) + 1] = EditorGUILayout.TextField(new GUIContent("Full Release Name:", "Name Given to this App Upon Release in this state"), Target.dm_asReleasedAppName[(i * 2) + 1]);
				Target.dm_asFullVersionBundleID[i] = EditorGUILayout.TextField(new GUIContent("Full Version Bundle Identity:", "Bundle Identity Given to this app when released as the Full Version.\n\nMust be Unique and must not be the same as the Lite Version Bundle Identity"), Target.dm_asFullVersionBundleID[i]);
			}
			EditorGUI.indentLevel -= 1;
			AddSpaces(2);
		}


		Target.dm_sprLiteVersionAppIcon = DrawObjectOption("Lite Version App Icon:", Target.dm_sprLiteVersionAppIcon, "Icon that will be used with the Lite Version on the Executable/App File when released on a platform (Android/iOS/Windows/Mac/Etc)");
		Target.dm_sprFullVersionAppIcon = DrawObjectOption("Full Version App Icon:", Target.dm_sprFullVersionAppIcon, "Icon that will be used with the Full Version on the Executable/App File when released on a platform (Android/iOS/Windows/Mac/Etc)");


		
		if (eBuildType != Target.m_eBuildVersion || eDisplayableLanguage != Target.dm_eDisplayableLanguagesTypes)
		{
			Target.m_eBuildVersion = eBuildType;
			Target.dm_eDisplayableLanguagesTypes = eDisplayableLanguage;

			// Apply New Bundle Indetifiers
			PlayerSettings.productName				= Target.dm_asReleasedAppName[((int)Target.dm_eDisplayableLanguagesTypes * 2) + Mathf.Min((int)Target.m_eBuildVersion, 1)];
			PlayerSettings.bundleIdentifier			= (Target.m_eBuildVersion == GameManager.BuildVersion.LITE_VERSION ? Target.dm_asLiteVersionBundleID[(int)Target.dm_eDisplayableLanguagesTypes] : Target.dm_asFullVersionBundleID[(int)Target.dm_eDisplayableLanguagesTypes]);
			PlayerSettings.iPhoneBundleIdentifier	= (Target.m_eBuildVersion == GameManager.BuildVersion.LITE_VERSION ? Target.dm_asLiteVersionBundleID[(int)Target.dm_eDisplayableLanguagesTypes] : Target.dm_asFullVersionBundleID[(int)Target.dm_eDisplayableLanguagesTypes]);

			string scriptingDefines = "";
			switch (Target.dm_eDisplayableLanguagesTypes)
			{
				case GameManager.DisplayableLanguagesTypes.URDU_ONLY:	scriptingDefines += "URDU_LANGUAGE_ONLY;"; break;
				default:												scriptingDefines += "INCLUDE_ALL_LANGUAGES;"; break;
			}

			BuildTargetGroup[] targetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.Standalone };
			foreach (BuildTargetGroup targetGroup in targetGroups)
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, scriptingDefines);

				Texture2D[] icons = PlayerSettings.GetIconsForTargetGroup(targetGroup);
				for (int i = 0; i < icons.Length; ++i)
				{
					icons[i] = (Target.m_eBuildVersion == GameManager.BuildVersion.LITE_VERSION ? Target.dm_sprLiteVersionAppIcon : Target.dm_sprFullVersionAppIcon).texture;
				}
				PlayerSettings.SetIconsForTargetGroup(targetGroup, icons);
			}
		}
		AddSpaces(2);
		Target.m_eSystemLanguages = (GameManager.SystemLanguages)EditorGUILayout.EnumPopup(new GUIContent("Selected Default Language: ", "The language which will be shown by default. This is only affected in the Unity Editor. Nothing will change on the mobile device"), Target.m_eSystemLanguages);

		AddSpaces(3);
		DrawArrayOptions("Root Parents Of Multi-Language Text Components", "m_agoRootParentsOfMultiLanguageTextComponents", "GameManager will scan through the selected objects and all of their children (iteratively) to find any related Multi-Language Text Components to show off");
        AddSpaces(6);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Multi-Language Text System Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawMultiLanguageTextSystemOptions()
	{
		m_eSelectedLanguage = (GameManager.SystemLanguages)Mathf.Clamp((int)((GameManager.SystemLanguages)EditorGUILayout.EnumPopup(new GUIContent("Language To Edit: ", "The language which will be shown when a following button is pressed. If TOTAL_LANGUAGES is selected then each individual MultiLanguageText component will be able to select which language to display respectively"), m_eSelectedLanguage)), (int)GameManager.SystemLanguages.ENGLISH, (int)GameManager.SystemLanguages.TOTAL_LANGUAGES);
		AddSpaces(1);
		Rect rectPos = GetScaledRect();
		AddSpaces(3);
		if (m_eSelectedLanguage == GameManager.SystemLanguages.TOTAL_LANGUAGES)
		{
			rectPos.width /= 2;
			rectPos.x += rectPos.width;
			if (GUI.Button(rectPos, new GUIContent("Show All Entries", "Show All Multi-Language Text components which may or may not contain data for the Selected Langauge")))
				m_alMultiLanguageTextItems = ScanForMultiLanguageTextComponents(Target, m_eSelectedLanguage, MultiLanguageTextReceiveMode.ALL);
		}
		else
		{
			rectPos.width /= 3;
			if (GUI.Button(rectPos, new GUIContent("Show Blank Entries", "Show remaining Multi-Language Text components which are missing data for the Selected Langauge")))
				m_alMultiLanguageTextItems = ScanForMultiLanguageTextComponents(Target, m_eSelectedLanguage, MultiLanguageTextReceiveMode.BLANK_ONLY);
            rectPos.x += rectPos.width;
			if (GUI.Button(rectPos, new GUIContent("Show All Entries", "Show All Multi-Language Text components which may or may not contain data for the Selected Langauge")))
				m_alMultiLanguageTextItems = ScanForMultiLanguageTextComponents(Target, m_eSelectedLanguage, MultiLanguageTextReceiveMode.ALL);
            rectPos.x += rectPos.width;
			if (GUI.Button(rectPos, new GUIContent("Show Completed Entries", "Show All Multi-Language Text components which currently contain data for the Selected Langauge")))
				m_alMultiLanguageTextItems = ScanForMultiLanguageTextComponents(Target, m_eSelectedLanguage, MultiLanguageTextReceiveMode.FILLED_ONLY);
        }
		

		if (m_alMultiLanguageTextItems != null)
		{
			bool valid = false;
			for(int i = 0; i < m_alMultiLanguageTextItems.Length; ++i)
			{
				if(m_alMultiLanguageTextItems[i].Count > 0)
				{
					valid = true;
					break;
				}
			}

			if(valid)
			{
				// Generate/Load Translation Files
				if(m_eSelectedLanguage != GameManager.SystemLanguages.ENGLISH)
				{
					AddSpaces(2);
					rectPos = GetScaledRect();
					rectPos.x += rectPos.width;
					rectPos.width = 300.0f;
					rectPos.height = 20.0f;
					rectPos.x -= rectPos.width;
					if(GUI.Button(rectPos, new GUIContent("Generate Language Translation File for " + GameManager.GetLanguageAsEnglishString(m_eSelectedLanguage), "Generates a file that can be handed over to translators to perform language translation")))
						GenerateLanguageTranslationFile();

					AddSpaces(2);
					rectPos.y += rectPos.height + 3;
					if(GUI.Button(rectPos, new GUIContent("Read Language Translation File for " + GameManager.GetLanguageAsEnglishString(m_eSelectedLanguage), "Loads data from a specified xml document which contains data for this language")))
						ReadDataFromLanguageTranslationFile();
				}
				


				GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
				foldoutStyle.normal.textColor = new Color32(72, 164, 26, 255);
				foldoutStyle.onActive.textColor = new Color32(0, 0, 255, 255);
				int itemsCount = 1;
				for(int i = 0; i < m_alMultiLanguageTextItems.Length; ++i)
				{
					AddSpaces(3);
					foreach(MultiLanguageTextInstanceInfo mlti in m_alMultiLanguageTextItems[i])
					{
						string prefix = ((itemsCount < 1000 ? "0" : "") + (itemsCount < 100 ? "0" : "") + (itemsCount < 10 ? "0" : "") + itemsCount) + " ~ ";
						mlti.bShowDisplay = EditorGUI.Foldout(GetScaledRect(), mlti.bShowDisplay, prefix + mlti.sDisplayLabel, true, foldoutStyle);
						AddSpaces(3);
						if(mlti.bShowDisplay)
						{
							DrawSplitter();
							AddSpaces(1);
							DrawMultiLanguageText(mlti.rInstance, m_eSelectedLanguage);
							AddSpaces(5);
						}

						itemsCount += 1;
					}
				}
			}
			else
			{
				GUIStyle fontstyle = new GUIStyle(EditorStyles.boldLabel);
				fontstyle.normal.textColor = new Color32(43, 82, 174, 255);
				fontstyle.alignment = TextAnchor.UpperCenter;
				AddSpaces(3);
				EditorGUI.LabelField(GetScaledRect(), "No \"Multi-Language Text Components\" Were Found Under The Selected Setting(s)!", fontstyle);
				AddSpaces(3);
			}
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Scan for Multi-Language Text Components
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public static MultiLanguageTextList[] ScanForMultiLanguageTextComponents(GameManager rGameManager, GameManager.SystemLanguages eChosenLanguage, MultiLanguageTextReceiveMode eMultiLanguageTextReceiveMode)
	{
		// Declare Holders of Text Items - This is to sort them without having to use exspensive algorthms/RegularExpressions (Even though it doesn't matter so much in the CustomEditor classes).
		MultiLanguageTextList[] alMultiLanguageTextItems = new MultiLanguageTextList[rGameManager.m_agoRootParentsOfMultiLanguageTextComponents.Length];
		for (int i = 0; i < alMultiLanguageTextItems.Length; ++i)
			alMultiLanguageTextItems[i] = new MultiLanguageTextList();

		// Use #Reflection to identify Multi-Language Components found within script fields (i.e Variables) then capture those instances and use them here. Essentially compiling a database
		for(int i = 0; i < rGameManager.m_agoRootParentsOfMultiLanguageTextComponents.Length; ++i)
		{
			LinkedList<Transform> transformList = new LinkedList<Transform>();
			GetAllChildren(transformList, rGameManager.m_agoRootParentsOfMultiLanguageTextComponents[i].transform);
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
			foreach (Transform obj in transformList)
			{
				foreach (Component component in obj.GetComponents<Component>())
				{
					FieldInfo[] fields = component.GetType().GetFields(flags);
					foreach(FieldInfo variable in fields)
					{
						CheckVariable(rGameManager, eChosenLanguage, ref alMultiLanguageTextItems, variable, i, component, component, eMultiLanguageTextReceiveMode, flags);
                    }
				}
			}
		}

		return alMultiLanguageTextItems;
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check Variables INSIDE of a Class/Struct/Interface
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static void CheckInnerDeclarations(GameManager rGameManager, GameManager.SystemLanguages eChosenLanguage, ref MultiLanguageTextList[] alMultiLanguageTextItems, FieldInfo a_declaration, int a_iCurrentSceneID, Component a_parent, object a_ContainingType, MultiLanguageTextReceiveMode eMultiLanguageTextReceiveMode, BindingFlags a_flags)
	{
		if(a_declaration.FieldType.IsArray)
		{
			object[] data = a_ContainingType.GetType().GetField(a_declaration.Name).GetValue(a_ContainingType) as object[];
            if(data != null)
			{
				for(int j = 0; j < data.Length; ++j)
				{
					if(data[j] != null)
					{
						FieldInfo[] arrayFields = data[j].GetType().GetFields(a_flags);
						foreach(FieldInfo info in arrayFields)
							CheckVariable(rGameManager, eChosenLanguage, ref alMultiLanguageTextItems, info, a_iCurrentSceneID, a_parent, data[j], eMultiLanguageTextReceiveMode, a_flags, ((j + 1) < 100 ? "0" : "") + ((j + 1) < 10 ? "0" : "") + (j + 1).ToString());
					}
				}
				return;
			}
		}

		// Check Fields
		FieldInfo[] fields = a_declaration.GetType().GetFields(a_flags);
		foreach(FieldInfo variable in fields)
		{
			CheckVariable(rGameManager, eChosenLanguage, ref alMultiLanguageTextItems, variable, a_iCurrentSceneID, a_parent, a_ContainingType, eMultiLanguageTextReceiveMode, a_flags);
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Check Variable for Valid MultiLanguageText Reference
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static void CheckVariable(GameManager rGameManager, GameManager.SystemLanguages eChosenLanguage, ref MultiLanguageTextList[] alMultiLanguageTextItems, FieldInfo a_variable, int a_iCurrentSceneID, Component a_parent, object a_ContainingType, MultiLanguageTextReceiveMode eMultiLanguageTextReceiveMode, BindingFlags a_flags, string appendedName = "")
	{
		FieldInfo currentField = a_ContainingType.GetType().GetField(a_variable.Name);
		if(currentField == null)
		{
			return;
		}
		object currentObject = currentField.GetValue(a_ContainingType);


		if(a_variable.FieldType == typeof(MultiLanguageText))
		{
			CaptureMultiTextObject(rGameManager, eChosenLanguage, ref alMultiLanguageTextItems, currentObject as MultiLanguageText, a_iCurrentSceneID, a_parent, eMultiLanguageTextReceiveMode, a_variable.Name, appendedName);
        }
		else if(a_variable.FieldType == typeof(MultiLanguageText[]))
		{
			MultiLanguageText[] aTextComponents = currentObject as MultiLanguageText[];
			for(int j = 0; j < aTextComponents.Length; ++j)
			{
				CaptureMultiTextObject(rGameManager, eChosenLanguage, ref alMultiLanguageTextItems, aTextComponents[j] as MultiLanguageText, a_iCurrentSceneID, a_parent, eMultiLanguageTextReceiveMode, a_variable.Name, appendedName);
			}
		}
		else
		{
			// Keep Searching through all declared objects... I want EVERY Multi-Laguage Component
			CheckInnerDeclarations(rGameManager, eChosenLanguage, ref alMultiLanguageTextItems, a_variable, a_iCurrentSceneID, a_parent, a_ContainingType, eMultiLanguageTextReceiveMode, a_flags);
		}
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Capture the MultiLanugageText Reference we found earlier
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static void CaptureMultiTextObject(GameManager rGameManager, GameManager.SystemLanguages eChosenLanguage, ref MultiLanguageTextList[] alMultiLanguageTextItems, MultiLanguageText a_rMultiTextObject, int a_iCurrentSceneID, Component a_parent, MultiLanguageTextReceiveMode eMultiLanguageTextReceiveMode, string a_variableName, string appendedName = "")
	{
		if(eMultiLanguageTextReceiveMode == MultiLanguageTextReceiveMode.BLANK_ONLY)
		{
			if(a_rMultiTextObject.m_arLanguageText[(int)eChosenLanguage].text != "")
				return;
		}
		else if(eMultiLanguageTextReceiveMode == MultiLanguageTextReceiveMode.FILLED_ONLY)
		{
			if(a_rMultiTextObject.m_arLanguageText[(int)eChosenLanguage].text == "")
				return;
		}

		MultiLanguageTextInstanceInfo mlti = new MultiLanguageTextInstanceInfo();
		mlti.rInstance = a_rMultiTextObject;
		mlti.rOwningParent = a_parent;

		Transform trans = a_parent.transform;
		mlti.sDisplayLabel = a_parent.name + "\\" + a_variableName + appendedName;
		while(trans != rGameManager.m_agoRootParentsOfMultiLanguageTextComponents[a_iCurrentSceneID].transform)
		{
			mlti.sDisplayLabel = trans.name + "\\" + mlti.sDisplayLabel;
			trans = trans.parent;
		}

		if(!alMultiLanguageTextItems[a_iCurrentSceneID].Contains(mlti))
			alMultiLanguageTextItems[a_iCurrentSceneID].AddLast(mlti);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Get ALL Children in Assigned Transforms
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private static void GetAllChildren(LinkedList<Transform> transformList, Transform root)
	{
		transformList.AddLast(root);
		foreach (Transform child in root)
		{
			GetAllChildren(transformList, child);
        }
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Generate Language Translation File
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void GenerateLanguageTranslationFile()
	{
		LinkedList<MultiLanguageTextInstanceInfo> acquiredMultiLanguageTextInstance = new LinkedList<MultiLanguageTextInstanceInfo>();
		for(int i = 0; i < m_alMultiLanguageTextItems.Length; ++i)
		{
			foreach(MultiLanguageTextInstanceInfo mlti in m_alMultiLanguageTextItems[i])
			{
				acquiredMultiLanguageTextInstance.AddLast(mlti);
			}
		}
		Utility_MultiLanguageTextFileHandler.GenerateFile(acquiredMultiLanguageTextInstance, m_eSelectedLanguage);
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Read Data From Language Translation File
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	private void ReadDataFromLanguageTranslationFile()
	{
		LinkedList<MultiLanguageTextInstanceInfo> acquiredMultiLanguageTextInstance = new LinkedList<MultiLanguageTextInstanceInfo>();
		for(int i = 0; i < m_alMultiLanguageTextItems.Length; ++i)
		{
			foreach(MultiLanguageTextInstanceInfo mlti in m_alMultiLanguageTextItems[i])
			{
				acquiredMultiLanguageTextInstance.AddLast(mlti);
			}
		}
		Utility_MultiLanguageTextFileHandler.ReadFromFile(acquiredMultiLanguageTextInstance, m_eSelectedLanguage);
	}
}
