//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Instrument Manager
//             Author: Christopher Diamond
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
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(InstrumentManager))]
public class CustomEditor_InstrumentManager : CustomEditor_Base<InstrumentManager>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script manages the different intruments in the app. Applying the\n" +
					"different interpretations of sprites and special abilities as\n" +
					"needed. All scripts that need to interact with an instrument should\n" +
					"go through this script.";
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
					label = "~Instrument Information~ (Apply values to instrument components)",
					representingDrawMethod = DrawInstrumentOptions
				}
			};
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Inspector Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawInspectorOptions()
	{
		Target.m_rFullGamePopup = DrawObjectOption("FullFame Plug Popup Ref:", Target.m_rFullGamePopup, "The FullGame Popup Window Will tell the user to buy the full game to experience the rest of the game");
	}
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* New Method: Draw Instrument Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected void DrawInstrumentOptions()
	{
		Target.dm_eSelectedInspectorInstrument = (InstrumentManager.InstrumentMode)EditorGUILayout.EnumPopup(new GUIContent("Selected Instrument: ", "Selected instrument to apply values to"), Target.dm_eSelectedInspectorInstrument);
		AddSpaces(3);

		if(Target.m_aInstrumentDetails.Length <= (int)Target.dm_eSelectedInspectorInstrument)
			ResizeArray(ref Target.m_aInstrumentDetails, (int)Target.dm_eSelectedInspectorInstrument + 1);

		if(Target.m_aInstrumentDetails[(int)Target.dm_eSelectedInspectorInstrument] == null)
			Target.m_aInstrumentDetails[(int)Target.dm_eSelectedInspectorInstrument] = new InstrumentManager.InstrumentDetails();

		// ~~~~~ REFERENCES ~~~~~
		InstrumentManager.InstrumentDetails rSelectedInstrumentDetails = Target.m_aInstrumentDetails[(int)Target.dm_eSelectedInspectorInstrument];
		rSelectedInstrumentDetails.holder	= DrawObjectOption("Instrument Root GameObject: ", rSelectedInstrumentDetails.holder, "Main GameObject that contains all of the components of the selected instrument. This may be the entire instrument in and of itself");
		SpriteRenderer rNewRenderer			= DrawObjectOption("Instrument Renderer: ", rSelectedInstrumentDetails.renderer, "Main Renderer for selected instrument. If the instrument has special abilities that change the image of the instrument, this renderer is used to reflect those changes");
		Target.m_agoTambourineCymbals[(int)Target.dm_eSelectedInspectorInstrument] = DrawObjectOption("Assigned Tambourine Cymbals:", Target.m_agoTambourineCymbals[(int)Target.dm_eSelectedInspectorInstrument], "The GameObject holding all of the Cymbals assigned to this specific incarnation of the Tambourine Instrument");
		Target.m_arTambourineTargets[(int)Target.dm_eSelectedInspectorInstrument] = DrawObjectOption("Assigned Tambourine Targets Manager:", Target.m_arTambourineTargets[(int)Target.dm_eSelectedInspectorInstrument], "The TambourineTargetsManager which is assigned to this spcific incarnation of the Tambourine Instrument");
		Target.m_arTambourineAutoPlayCymbals[(int)Target.dm_eSelectedInspectorInstrument] = DrawObjectOption("Assigned Auto-Play Tambourine Cymbal:", Target.m_arTambourineAutoPlayCymbals[(int)Target.dm_eSelectedInspectorInstrument], "The Tambourine Cymbal which will used during \"Auto-Play\" Mode (used during the Challenge Example). This cymbal will animate when it is time to be played");
        if(rNewRenderer != rSelectedInstrumentDetails.renderer)
		{
			rSelectedInstrumentDetails.renderer = rNewRenderer;
			if(rSelectedInstrumentDetails.holder == null)
				rSelectedInstrumentDetails.holder = rNewRenderer.gameObject;
        }

		// ~~~~~ SPRITE OPTIONS ~~~~~
		if(rSelectedInstrumentDetails.holder != null && rSelectedInstrumentDetails.renderer != null)
		{
			Sprite rNewNormalSprite = DrawObjectOption("Normal Sprite for Selected Instrument: ", rSelectedInstrumentDetails.normalSprite, "Sprite used for the Selected Instrument when in the normal state");
			Sprite rNewSpecialSprite = DrawObjectOption("Special Sprite for Selected Instrument: ", rSelectedInstrumentDetails.specialSprite, "Sprite used for the Selected Instrument when in the special state");
			if(rNewNormalSprite != rSelectedInstrumentDetails.normalSprite)
			{
				if(rSelectedInstrumentDetails.normalSprite == null)
				{
					rSelectedInstrumentDetails.normalPosition	= rSelectedInstrumentDetails.holder.transform.localPosition;
					rSelectedInstrumentDetails.normalRotation	= rSelectedInstrumentDetails.holder.transform.localRotation.eulerAngles;
					rSelectedInstrumentDetails.normalScale		= rSelectedInstrumentDetails.holder.transform.localScale;
				}
				rSelectedInstrumentDetails.normalSprite = rNewNormalSprite;
				rSelectedInstrumentDetails.renderer.sprite = rNewNormalSprite;
			}

			if(rNewSpecialSprite != rSelectedInstrumentDetails.specialSprite)
			{
				if(rSelectedInstrumentDetails.specialSprite == null)
				{
					rSelectedInstrumentDetails.specialPosition	= rSelectedInstrumentDetails.holder.transform.localPosition;
					rSelectedInstrumentDetails.specialRotation	= rSelectedInstrumentDetails.holder.transform.localRotation.eulerAngles;
					rSelectedInstrumentDetails.specialScale		= rSelectedInstrumentDetails.holder.transform.localScale;
				}
				rSelectedInstrumentDetails.specialSprite = rNewSpecialSprite;
				rSelectedInstrumentDetails.renderer.sprite = rNewSpecialSprite;
			}


			AddSpaces(3);

			// ~~~~~ TRANSFORM OPTIONS ~~~~~
#region Normal_Instrument_Transform_Options
			if(rSelectedInstrumentDetails.renderer.sprite == rSelectedInstrumentDetails.normalSprite && rSelectedInstrumentDetails.normalSprite != null)
            {
				Vector3 vNewPosition	= EditorGUILayout.Vector3Field(new GUIContent((rSelectedInstrumentDetails.specialSprite != null ? "'Normal Instrument'" : "Instrument") + " Position: ", "Local Position of the instrument during normal state"), rSelectedInstrumentDetails.normalPosition);
				Vector3 vNewRotation	= EditorGUILayout.Vector3Field(new GUIContent((rSelectedInstrumentDetails.specialSprite != null ? "'Normal Instrument'" : "Instrument") + " Rotation: ", "Local Rotation of the instrument during normal state"), rSelectedInstrumentDetails.normalRotation);
				Vector3 vNewScale		= EditorGUILayout.Vector3Field(new GUIContent((rSelectedInstrumentDetails.specialSprite != null ? "'Normal Instrument'" : "Instrument") + " Scale: ", "Local Scale of the instrument during normal state"), rSelectedInstrumentDetails.normalScale);
				if(vNewPosition != rSelectedInstrumentDetails.normalPosition)
				{
					rSelectedInstrumentDetails.normalPosition = vNewPosition;
					rSelectedInstrumentDetails.holder.transform.localPosition = vNewPosition;
				}
				if(vNewRotation != rSelectedInstrumentDetails.normalRotation)
				{
					rSelectedInstrumentDetails.normalRotation = vNewRotation;
					rSelectedInstrumentDetails.holder.transform.localRotation = Quaternion.Euler(vNewRotation);
				}
				if(vNewScale != rSelectedInstrumentDetails.normalScale)
				{
					rSelectedInstrumentDetails.normalScale = vNewScale;
					rSelectedInstrumentDetails.holder.transform.localScale = vNewScale;
				}

				AddSpaces(1);
				Rect drawPos = GetScaledRect();
				float buttonWidth = 220.0f;
				drawPos.x += drawPos.width;
				drawPos.width = buttonWidth;
				drawPos.height = 18.0f;
				// Has different values to the actual Instrument?
				if(rSelectedInstrumentDetails.normalPosition != rSelectedInstrumentDetails.holder.transform.localPosition || rSelectedInstrumentDetails.normalRotation != rSelectedInstrumentDetails.holder.transform.localRotation.eulerAngles || rSelectedInstrumentDetails.normalScale != rSelectedInstrumentDetails.holder.transform.localScale)
				{
					// ~~~ Copy from Editable values to Selected Instrument Transform ~~~
					drawPos.x -= buttonWidth;
					if(GUI.Button(drawPos, new GUIContent("Show Instrument Transform Values", "Will change visuals in the Scene:\n\nCopies values from the editable options above and paste them into the transform for the object holding the instrument")))
					{
						string oldValues =  "  Position:\t" + rSelectedInstrumentDetails.holder.transform.localPosition.ToString() + "\n" +
											"  Rotation:\t" + rSelectedInstrumentDetails.holder.transform.localRotation.eulerAngles.ToString() + "\n" +
											"  Scale:\t\t" + rSelectedInstrumentDetails.holder.transform.localScale.ToString();
						string newValues =	"  Position:\t" + rSelectedInstrumentDetails.normalPosition.ToString() + "\n" +
											"  Rotation:\t" + rSelectedInstrumentDetails.normalRotation.ToString() + "\n" +
											"  Scale:\t\t" + rSelectedInstrumentDetails.normalScale.ToString();
						if(EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to modify the transform of the Selected Instrument containing:\n" + oldValues + "\n\nto these new values:\n" + newValues + "\n\nThis will change the Transform of the Selected Instrument", "Confirm", "Deny"))
						{
							rSelectedInstrumentDetails.holder.transform.localPosition = rSelectedInstrumentDetails.normalPosition;
							rSelectedInstrumentDetails.holder.transform.localRotation = Quaternion.Euler(rSelectedInstrumentDetails.normalRotation);
							rSelectedInstrumentDetails.holder.transform.localScale	  = rSelectedInstrumentDetails.normalScale;
						}
					}

					// ~~~ Copy from Selected Instrument Transform into Editable values ~~~
					drawPos.x -= (buttonWidth + 2.0f);
					if(GUI.Button(drawPos, new GUIContent("Copy Instrument Transform Values", "Will change values in the inspector:\n\nCopies values from the object holding the selected instrument into the editable options above")))
					{
						string oldValues =  "  Position:\t" + rSelectedInstrumentDetails.normalPosition.ToString() + "\n" +
											"  Rotation:\t" + rSelectedInstrumentDetails.normalRotation.ToString() + "\n" +
											"  Scale:\t\t" + rSelectedInstrumentDetails.normalScale.ToString();
						string newValues =	"  Position:\t" + rSelectedInstrumentDetails.holder.transform.localPosition.ToString() + "\n" +
											"  Rotation:\t" + rSelectedInstrumentDetails.holder.transform.localRotation.eulerAngles.ToString() + "\n" +
											"  Scale:\t\t" + rSelectedInstrumentDetails.holder.transform.localScale.ToString();
						if(EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to replace the current values:\n" + oldValues + "\n\nWith these new values:\n" + newValues, "Confirm", "Deny"))
						{
							rSelectedInstrumentDetails.normalPosition	= rSelectedInstrumentDetails.holder.transform.localPosition;
							rSelectedInstrumentDetails.normalRotation	= rSelectedInstrumentDetails.holder.transform.localRotation.eulerAngles;
							rSelectedInstrumentDetails.normalScale		= rSelectedInstrumentDetails.holder.transform.localScale;
						}
					}
				}
				AddSpaces(4);
				drawPos = GetScaledRect();
				buttonWidth = 200.0f;
				drawPos.x += drawPos.width;
				drawPos.width = buttonWidth;
				drawPos.height = 18.0f;
				if(rSelectedInstrumentDetails.specialSprite != null)
				{
					drawPos.x -= buttonWidth;
					if(GUI.Button(drawPos, new GUIContent("Show Special Instrument Options", "Change the selected sprite to the Special Instrument Sprite, which allows you to edit options for the instrument during it's special state")))
					{
						Target.ShowSpecialInstrumentState(Target.dm_eSelectedInspectorInstrument);
					}
                }
				if(Target.m_arTambourineTargets[(int)Target.dm_eSelectedInspectorInstrument] != null)
				{
					drawPos.x -= (buttonWidth + 3.0f);
					if(Target.m_arTambourineTargets[(int)Target.dm_eSelectedInspectorInstrument].Visible)
					{
						if(GUI.Button(drawPos, new GUIContent("Hide Tambourine Targets", "Hides the Tambourine Targets from view")))
							Target.m_arTambourineTargets[(int)Target.dm_eSelectedInspectorInstrument].HideTargets();
					}
					else
					{
						if(GUI.Button(drawPos, new GUIContent("Show Tambourine Targets", "Show the Tambourine Targets in the Scene")))
							Target.m_arTambourineTargets[(int)Target.dm_eSelectedInspectorInstrument].ShowTargets();
					}
                }
				AddSpaces(3);
            }
#endregion
#region Special_Instrument_Transform_Options
			else if(rSelectedInstrumentDetails.renderer.sprite == rSelectedInstrumentDetails.specialSprite && rSelectedInstrumentDetails.specialSprite != null)
			{
				Vector3 vNewPosition	= EditorGUILayout.Vector3Field(new GUIContent("'Special Instrument' Position: ", "Local Position of the instrument during normal state"), rSelectedInstrumentDetails.specialPosition);
				Vector3 vNewRotation	= EditorGUILayout.Vector3Field(new GUIContent("'Special Instrument' Rotation: ", "Local Rotation of the instrument during normal state"), rSelectedInstrumentDetails.specialRotation);
				Vector3 vNewScale		= EditorGUILayout.Vector3Field(new GUIContent("'Special Instrument' Scale: ", "Local Scale of the instrument during normal state"), rSelectedInstrumentDetails.specialScale);
				if(vNewPosition != rSelectedInstrumentDetails.specialPosition)
				{
					rSelectedInstrumentDetails.specialPosition = vNewPosition;
					rSelectedInstrumentDetails.holder.transform.localPosition = vNewPosition;
				}
				if(vNewRotation != rSelectedInstrumentDetails.specialRotation)
				{
					rSelectedInstrumentDetails.specialRotation = vNewRotation;
					rSelectedInstrumentDetails.holder.transform.localRotation = Quaternion.Euler(vNewRotation);
				}
				if(vNewScale != rSelectedInstrumentDetails.specialScale)
				{
					rSelectedInstrumentDetails.specialScale = vNewScale;
					rSelectedInstrumentDetails.holder.transform.localScale = vNewScale;
				}

				AddSpaces(1);
				Rect drawPos = GetScaledRect();
				float buttonWidth = 220.0f;
				drawPos.x += drawPos.width;
				drawPos.width = buttonWidth;
				drawPos.height = 18.0f;
				// Has different values to the actual Instrument?
				if(rSelectedInstrumentDetails.specialPosition != rSelectedInstrumentDetails.holder.transform.localPosition || rSelectedInstrumentDetails.specialRotation != rSelectedInstrumentDetails.holder.transform.localRotation.eulerAngles || rSelectedInstrumentDetails.specialScale != rSelectedInstrumentDetails.holder.transform.localScale)
				{
					// ~~~ Copy from Editable values to Selected Instrument Transform ~~~
					drawPos.x -= buttonWidth;
					if(GUI.Button(drawPos, new GUIContent("Show Instrument Transform Values", "Will change visuals in the Scene:\n\nCopies values from the editable options above and paste them into the transform for the object holding the instrument")))
					{
						string oldValues =  "  Position:\t" + rSelectedInstrumentDetails.holder.transform.localPosition.ToString() + "\n" +
											"  Rotation:\t" + rSelectedInstrumentDetails.holder.transform.localRotation.eulerAngles.ToString() + "\n" +
											"  Scale:\t\t" + rSelectedInstrumentDetails.holder.transform.localScale.ToString();
						string newValues =	"  Position:\t" + rSelectedInstrumentDetails.specialPosition.ToString() + "\n" +
											"  Rotation:\t" + rSelectedInstrumentDetails.specialRotation.ToString() + "\n" +
											"  Scale:\t\t" + rSelectedInstrumentDetails.specialScale.ToString();
						if(EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to modify the transform of the Selected Instrument containing:\n" + oldValues + "\n\nto these new values:\n" + newValues + "\n\nThis will change the Transform of the Selected Instrument", "Confirm", "Deny"))
						{
							rSelectedInstrumentDetails.holder.transform.localPosition = rSelectedInstrumentDetails.specialPosition;
							rSelectedInstrumentDetails.holder.transform.localRotation = Quaternion.Euler(rSelectedInstrumentDetails.specialRotation);
							rSelectedInstrumentDetails.holder.transform.localScale	  = rSelectedInstrumentDetails.specialScale;
						}
					}

					// ~~~ Copy from Selected Instrument Transform into Editable values ~~~
					drawPos.x -= (buttonWidth + 2.0f);
					if(GUI.Button(drawPos, new GUIContent("Copy Instrument Transform Values", "Will change values in the inspector:\n\nCopies values from the object holding the selected instrument into the editable options above")))
					{
						string oldValues =  "  Position:\t" + rSelectedInstrumentDetails.specialPosition.ToString() + "\n" +
											"  Rotation:\t" + rSelectedInstrumentDetails.specialRotation.ToString() + "\n" +
											"  Scale:\t\t" + rSelectedInstrumentDetails.specialScale.ToString();
						string newValues =	"  Position:\t" + rSelectedInstrumentDetails.holder.transform.localPosition.ToString() + "\n" +
											"  Rotation:\t" + rSelectedInstrumentDetails.holder.transform.localRotation.eulerAngles.ToString() + "\n" +
											"  Scale:\t\t" + rSelectedInstrumentDetails.holder.transform.localScale.ToString();
						if(EditorUtility.DisplayDialog("Are you sure?", "Are you sure you wish to replace the current values:\n" + oldValues + "\n\nWith these new values:\n" + newValues, "Confirm", "Deny"))
						{
							rSelectedInstrumentDetails.specialPosition	= rSelectedInstrumentDetails.holder.transform.localPosition;
							rSelectedInstrumentDetails.specialRotation	= rSelectedInstrumentDetails.holder.transform.localRotation.eulerAngles;
							rSelectedInstrumentDetails.specialScale		= rSelectedInstrumentDetails.holder.transform.localScale;
						}
					}
				}
				AddSpaces(4);
				drawPos = GetScaledRect();
				buttonWidth = 200.0f;
				drawPos.x += drawPos.width;
				drawPos.width = buttonWidth;
				drawPos.height = 18.0f;
				if(rSelectedInstrumentDetails.normalSprite != null)
				{
					drawPos.x -= buttonWidth;
					if(GUI.Button(drawPos, new GUIContent("Show Normal Instrument Options", "Change the selected sprite to the Normal Instrument Sprite, which allows you to edit options for the instrument during it's normal state")))
					{
						Target.ShowNormalInstrumentState(Target.dm_eSelectedInspectorInstrument);
					}
					AddSpaces(3);
                }
				if(Target.m_arTambourineTargets[(int)Target.dm_eSelectedInspectorInstrument] != null)
				{
					drawPos.x -= (buttonWidth + 3.0f);
					if(Target.m_arTambourineTargets[(int)Target.dm_eSelectedInspectorInstrument].Visible)
					{
						if(GUI.Button(drawPos, new GUIContent("Hide Tambourine Targets", "Hides the Tambourine Targets from view")))
							Target.m_arTambourineTargets[(int)Target.dm_eSelectedInspectorInstrument].HideTargets();
					}
					else
					{
						if(GUI.Button(drawPos, new GUIContent("Show Tambourine Targets", "Show the Tambourine Targets in the Scene")))
							Target.m_arTambourineTargets[(int)Target.dm_eSelectedInspectorInstrument].ShowTargets();
					}
				}
			}
			#endregion
#region Unselected_Instrument_Options
			else if(rSelectedInstrumentDetails.normalSprite != null || rSelectedInstrumentDetails.specialSprite != null)
            {
				AddSpaces(1);
				Rect drawPos = GetScaledRect();
				float buttonWidth = 220.0f;
				drawPos.x += (drawPos.width - buttonWidth);
				drawPos.width = buttonWidth;
				drawPos.height = 18.0f;
				if(GUI.Button(drawPos, new GUIContent("Show Instrument Options", "Change the selected sprite renderer to the Selected Instrument Sprite, which allows you to edit options for the instrument")))
					rSelectedInstrumentDetails.renderer.sprite = (rSelectedInstrumentDetails.normalSprite != null ? rSelectedInstrumentDetails.normalSprite : rSelectedInstrumentDetails.specialSprite);
				AddSpaces(3);
			}
#endregion
		}

	}
}
