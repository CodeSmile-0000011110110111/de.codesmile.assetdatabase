// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using UnityEditor;
using Object = UnityEngine.Object;

public class AssetSaveTests : AssetTestBase
{
	[Test] public void StaticSave_NotExistingPath_IsNull() => Assert.Null(Asset.LoadMain<Object>("Assets/exist.not"));

	[Test] public void LoadMain_ExistingPath_Succeeds()
	{
		var obj = CreateTestAsset(TestAssetPath);
		var loaded = Asset.LoadMain<Object>(TestAssetPath);
		Assert.NotNull(loaded);
		Assert.AreEqual(obj, loaded);
		Assert.AreEqual(obj.GetType(), loaded.GetType());
	}

	[Test] public void LoadMain_NotExistingGuid_IsNull() => Assert.Null(Asset.LoadMain<Object>(new GUID()));

	[Test] public void LoadMain_ExistingGuid_Succeeds()
	{
		var obj = CreateTestAsset(TestAssetPath);

		var loaded = Asset.LoadMain<Object>(Asset.Path.GetGuid((String)TestAssetPath));

		Assert.NotNull(loaded);
		Assert.AreEqual(obj, loaded);
		Assert.AreEqual(obj.GetType(), loaded.GetType());
		Assert.AreEqual(obj.GetType(), Asset.GetMainType((String)TestAssetPath));
	}
}
