// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

public static class AssetHelper
{
	public static Boolean FileExists(Object asset)
	{
		var path = AssetDatabase.GetAssetPath(asset);
		return String.IsNullOrEmpty(path) == false && File.Exists(path);
	}
}
