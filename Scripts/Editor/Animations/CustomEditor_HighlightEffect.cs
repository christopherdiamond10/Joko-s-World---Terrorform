//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Custom Editor: Highlight Effect
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
using UnityEditor;

[CustomEditor(typeof(HighlightEffect))]
public class CustomEditor_HighlightEffect : CustomEditor_Base<HighlightEffect>
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Attr_Readers
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override string ScriptDescription
	{
		get
		{
			return "This script interacts with the 'SolidColour' shader changing the displayed\n" +
					"material colour. Creating a HighlightEffect.";
		}
	}



	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Overwritten Method: Draw Animation Options
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	protected override void DrawAnimationOptions()
	{
		SpriteRenderer sprRend = Target.GetComponent<SpriteRenderer>();
		DrawColourAnimationEffectOptions(ref Target.m_arColourAnimation, sprRend.sharedMaterial, sprRend);
	}
}
