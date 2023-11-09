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
		var assetPath = new Asset.Path(Application.dataPath);
		Assert.True(assetPath.Equals(assetPath));

#pragma warning disable 1718 // did you maybe not want to compare x with itself? Yes, I meant to!
		Assert.True(assetPath == assetPath);
		Assert.False(assetPath != assetPath);
#pragma warning restore 1718
	}

	[Test] public void AssetPath_Equality_EqualToOther()
	{
		Assert.True(new Asset.Path("\\Assets\\folder").Equals(new Asset.Path("Assets/folder/")));
		Assert.True(new Asset.Path(Application.dataPath + "\\") == new Asset.Path(Application.dataPath));
	}

	[Test] public void AssetPath_Equality_NotEqualToOther()
	{
		Assert.True(new Asset.Path(Application.dataPath + "/file.ext") != new Asset.Path(Application.dataPath));
		Assert.False(new Asset.Path(Application.dataPath + "/file.ext").Equals(new Asset.Path(Application.dataPath)));
	}

	[Test] public void AssetPath_Equality_EqualToString()
	{
		Assert.True("Assets".Equals(new Asset.Path(Application.dataPath)));
		Assert.True("Assets" == new Asset.Path(Application.dataPath));
		Assert.True("Assets/file.ext".Equals(new Asset.Path(Application.dataPath + "/file.ext")));
		Assert.True("Assets/file.ext" == new Asset.Path(Application.dataPath + "/file.ext"));
		Assert.True(new Asset.Path(Application.dataPath + "/file.ext").Equals("Assets\\file.ext"));
		Assert.True(new Asset.Path(Application.dataPath + "/file.ext") == "Assets/file.ext");
	}

	[Test] public void AssetPath_Equality_NotEqualToString()
	{
		Assert.AreNotEqual(Application.dataPath, new Asset.Path(Application.dataPath + "/file.ext"));
		Assert.AreNotEqual(null, new Asset.Path(Application.dataPath + "/file.ext"));
	}

	[Test] public void AssetPath_Equality_EqualToObject()
	{
		Assert.True(new Asset.Path("\\Assets\\folder").Equals((Object)new Asset.Path("Assets/folder/")));
		Assert.True(new Asset.Path(Application.dataPath + "\\") == (Object)new Asset.Path(Application.dataPath));
		Assert.True((Object)new Asset.Path(Application.dataPath + "\\") == new Asset.Path(Application.dataPath));
	}

	[Test] public void AssetPath_Equality_NotEqualToObject()
	{
		Assert.True(new Asset.Path(Application.dataPath + "/file.ext") != (Object)new Asset.Path(Application.dataPath));
		Assert.True((Object)new Asset.Path(Application.dataPath + "/file.ext") != new Asset.Path(Application.dataPath));
		Assert.False(
			new Asset.Path(Application.dataPath + "/f.x").Equals((Object)new Asset.Path(Application.dataPath)));
	}

	[Test] public void AssetPath_Equality_NotEqualToNull()
	{
		Assert.True(null != new Asset.Path(Application.dataPath));
		Assert.True(new Asset.Path(Application.dataPath) != null);
		Assert.False(null == new Asset.Path(Application.dataPath));
		Assert.False(new Asset.Path(Application.dataPath) == null);
		Assert.False(new Asset.Path(Application.dataPath + "/file.ext").Equals((Asset.Path)null));
	}

	[Test] public void AssetPath_GetHashCode_SameAsToStringHashCode()
	{
		var assetPath = new Asset.Path(Application.dataPath);
		Assert.AreEqual(assetPath.GetHashCode(), assetPath.ToString().GetHashCode());
	}
}
