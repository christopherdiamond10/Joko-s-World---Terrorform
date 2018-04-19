#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

public class Utility_XCodeBuildPostProcessor 
{
	internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
	{
		if (Directory.Exists(dstPath))
			Directory.Delete(dstPath);
		if (File.Exists(dstPath))
			File.Delete(dstPath);

		Directory.CreateDirectory(dstPath);

		foreach (var file in Directory.GetFiles(srcPath))
			File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

		foreach (var dir in Directory.GetDirectories(srcPath))
			CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
	}

	[PostProcessBuild(700)]
	public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
	{
		if (target != BuildTarget.iOS)
		{
			return;
		}

		//------------------------------------------------------------------
		// Getting Objective C Code To Work with XCode
		//------------------------------------------------------------------
		string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
		PBXProject proj = new PBXProject();
		proj.ReadFromString(File.ReadAllText(projPath));
		string targetGUID = proj.TargetGuidByName("Unity-iPhone");
		proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");   
		proj.SetBuildProperty(targetGUID, "ENABLE_BITCODE", "NO");
		File.WriteAllText(projPath, proj.WriteToString());
		//------------------------------------------------------------------
		// Getting Apple To Stop Bitching About Peripheral Usage Desciptions
		//------------------------------------------------------------------
		string plistPath = pathToBuiltProject + "/Info.plist";
		PlistDocument plist = new PlistDocument();
		plist.ReadFromString(File.ReadAllText(plistPath));

		// Create Values To Appease Apple... 
		PlistElementDict rootDict = plist.root;
		rootDict.SetString("NSCalendarsUsageDescription","Advertisement would like to create a calendar event.");
		rootDict.SetString("NSPhotoLibraryUsageDescription","Advertisement would like to store a photo.");
		rootDict.SetString("NSBluetoothPeripheralUsageDescription","Advertisement would like to use bluetooth.");
		rootDict.SetString("NSCameraUsageDescription","App would like to use the Camera.");

		// Write to file
		File.WriteAllText(plistPath, plist.WriteToString());
	}
}
#endif
