// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using System;
using System.IO;
using Object = UnityEngine.Object;

public static class AssetHelper
{
	public static Boolean FileExists(Object asset) => FileExists(Asset.Path.Get(asset));
	public static Boolean FileExists(AssetPath assetPath) => FileExists((String)assetPath);
	public static Boolean FileExists(String path) => File.Exists(path);
}
