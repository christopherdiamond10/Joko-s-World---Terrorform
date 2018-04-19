//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             Text Colour Helper
//             Author: Christopher Diamond
//             Date: January 16, 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script interacts with the UnityInspector in order to help the 
//		developer chose a highlighting colour for text.
//
//	  It holds a reference to a text renderer so that whilst modifying colour, 
//		that text changes highlighting colour.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class TextColourHelper : MonoBehaviour
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	*+ Public Instance Variables
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#if UNITY_EDITOR
	public UnityEngine.UI.Text dm_rAssociatedTextRenderer;
	public Color dm_cChosenColour = Color.white;
	public string dm_sHighlightedText = "";
#endif
}
