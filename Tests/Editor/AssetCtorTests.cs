// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

public class AssetCtorTests : AssetTestBase
{
	[Test] public void AssetCtor_NullAssetPath_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset((AssetPath)null));

	[Test] public void AssetCtor_NotExistingFilePath_Throws() =>
		Assert.Throws<FileNotFoundException>(() => new Asset("Assets/does not.exist"));

	[Test] public void AssetCtor_ExistingFilePath_Succeeds()
	{
		var assetObject = CreateTestAsset(TestAssetPath);

		var asset = new Asset(TestAssetPath);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, assetObject);
		Assert.AreEqual(asset.AssetGuid, AssetDatabase.GUIDFromAssetPath(TestAssetPath));
	}

	[Test] public void AssetCtor_NullObject_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset((Object)null));

	[Test] public void AssetCtor_NonAssetObject_Throws()
	{
		Assert.Throws<ArgumentException>(() => new Asset(new Object()));
	}
	[Test] public void AssetCtor_AssetObject_Succeeds()
	{
		var assetObject = CreateTestAsset(TestAssetPath);

		var asset = new Asset(assetObject);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, assetObject);
		Assert.AreEqual(asset.AssetGuid, AssetDatabase.GUIDFromAssetPath(TestAssetPath));
	}

	[Test] public void AssetCtor_EmptyGuid_Throws()
	{
		Assert.Throws<ArgumentException>(() => new Asset(new GUID()));
	}
	[Test] public void AssetCtor_NonAssetGuid_Throws()
	{
		Assert.Throws<ArgumentException>(() => new Asset(GUID.Generate()));
	}
	[Test] public void AssetCtor_GuidOfExistingAsset_Succeeds()
	{
		var assetObject = CreateTestAsset(TestAssetPath);
		var guid = AssetDatabase.GUIDFromAssetPath(TestAssetPath);

		var asset = new Asset(guid);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, assetObject);
		Assert.True(asset.AssetGuid.Equals(guid));
	}
}
