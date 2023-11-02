// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using UnityEngine;
using Object = System.Object;

public class AssetPathEqualityTests
{
	[Test] public void AssetPath_Equality_EqualToSelf()
	{
		var assetPath = new AssetPath(Application.dataPath);
		Assert.True(assetPath.Equals(assetPath));

#pragma warning disable 1718 // did you maybe not want to compare x with itself? Yes, I meant to!
		Assert.True(assetPath == assetPath);
		Assert.False(assetPath != assetPath);
#pragma warning restore 1718
	}

	[Test] public void AssetPath_Equality_EqualToOther()
	{
		Assert.True(new AssetPath("\\Assets\\folder").Equals(new AssetPath("Assets/folder/")));
		Assert.True(new AssetPath(Application.dataPath + "\\") == new AssetPath(Application.dataPath));
	}

	[Test] public void AssetPath_Equality_NotEqualToOther()
	{
		Assert.True(new AssetPath(Application.dataPath + "/file.ext") != new AssetPath(Application.dataPath));
		Assert.False(new AssetPath(Application.dataPath + "/file.ext").Equals(new AssetPath(Application.dataPath)));
	}

	[Test] public void AssetPath_Equality_EqualToString()
	{
		Assert.True("Assets".Equals(new AssetPath(Application.dataPath)));
		Assert.True("Assets" == new AssetPath(Application.dataPath));
		Assert.True("Assets/file.ext".Equals(new AssetPath(Application.dataPath + "/file.ext")));
		Assert.True("Assets/file.ext" == new AssetPath(Application.dataPath + "/file.ext"));
		Assert.True(new AssetPath(Application.dataPath + "/file.ext").Equals("Assets\\file.ext"));
		Assert.True(new AssetPath(Application.dataPath + "/file.ext") == "Assets/file.ext");
	}

	[Test] public void AssetPath_Equality_NotEqualToString()
	{
		Assert.AreNotEqual(Application.dataPath, new AssetPath(Application.dataPath + "/file.ext"));
		Assert.AreNotEqual(null, new AssetPath(Application.dataPath + "/file.ext"));
	}

	[Test] public void AssetPath_Equality_EqualToObject()
	{
		Assert.True(new AssetPath("\\Assets\\folder").Equals((Object)new AssetPath("Assets/folder/")));
		Assert.True(new AssetPath(Application.dataPath + "\\") == (Object)new AssetPath(Application.dataPath));
		Assert.True((Object)new AssetPath(Application.dataPath + "\\") == new AssetPath(Application.dataPath));
	}

	[Test] public void AssetPath_Equality_NotEqualToObject()
	{
		Assert.True(new AssetPath(Application.dataPath + "/file.ext") != (Object)new AssetPath(Application.dataPath));
		Assert.True((Object)new AssetPath(Application.dataPath + "/file.ext") != new AssetPath(Application.dataPath));
		Assert.False(new AssetPath(Application.dataPath + "/f.x").Equals((Object)new AssetPath(Application.dataPath)));
	}

	[Test] public void AssetPath_Equality_NotEqualToNull()
	{
		Assert.True(null != new AssetPath(Application.dataPath));
		Assert.True(new AssetPath(Application.dataPath) != null);
		Assert.False(null == new AssetPath(Application.dataPath));
		Assert.False(new AssetPath(Application.dataPath) == null);
		Assert.False(new AssetPath(Application.dataPath + "/file.ext").Equals(null));
	}

	[Test] public void AssetPath_GetHashCode_SameAsToStringHashCode()
	{
		var assetPath = new AssetPath(Application.dataPath);
		Assert.AreEqual(assetPath.GetHashCode(), assetPath.ToString().GetHashCode());
	}
}
