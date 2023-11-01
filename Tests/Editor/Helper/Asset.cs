// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Helper
{
	public class Asset
	{
		public static Boolean FileExists(Object asset)
		{
			var path = AssetDatabase.GetAssetPath(asset);
			return string.IsNullOrEmpty(path) == false && File.Exists(path);
		}
	}
}
