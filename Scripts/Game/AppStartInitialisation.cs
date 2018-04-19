//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//             On Application Start ~ Initialisation
//             Version: 1.0
//             Author: Christopher Diamond
//             Date: June 25, 2015
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//  Description:
//
//    This script is simply called when the application begins. It's useful for
//		calling upon 'once only' functions that initialise other parts of the app
//		that are costly to use during gameplay.
//
//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;

public class AppStartInitialisation : MonoBehaviour 
{
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	//	* Derived Method: Start
	//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	void Start() 
	{
#if !UNITY_EDITOR
		if(!FB.IsInitialized)
			FacebookHandler.InitialiseFacebook();
#else
		//const bool bFactoryReset = false;
		//if(bFactoryReset)
		//{
		//	PlayerPrefs.DeleteAll();
		//}
#endif
	}
}
