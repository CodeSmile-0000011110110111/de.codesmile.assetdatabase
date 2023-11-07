// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

public class AssetCtorExistingTests : AssetTestBase
{
	[Test] public void CtorPath_Null_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset((Asset.Path)null));

	[Test] public void CtorPath_NotExistingPath_Throws() =>
		Assert.Throws<FileNotFoundException>(() => new Asset("Assets/does not.exist"));

	[Test] public void CtorPath_ExistingPath_Succeeds()
	{
		var assetObject = CreateTestAssetObject(TestAssetPath);

		var asset = new Asset(TestAssetPath);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, assetObject);
		Assert.AreEqual(asset.Guid, Asset.Path.GetGuid(TestAssetPath));
	}

	[Test] public void CtorObject_Null_Throws() => Assert.Throws<ArgumentNullException>(() => new Asset((Object)null));

	[Test] public void CtorObject_NotAnAsset_Throws() =>
		Assert.Throws<ArgumentException>(() => new Asset(Instantiate.ExampleSO()));

	[Test] public void CtorObject_ExistingAsset_Succeeds()
	{
		var assetObject = CreateTestAssetObject(TestAssetPath);

		var asset = new Asset(assetObject);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, assetObject);
		Assert.AreEqual(asset.Guid, Asset.Path.GetGuid(TestAssetPath));
	}

	[Test] public void CtorGuid_EmptyGuid_Throws() => Assert.Throws<ArgumentException>(() => new Asset(new GUID()));

	[Test] public void CtorGuid_NotAnAsset_Throws() =>
		Assert.Throws<ArgumentException>(() => new Asset(GUID.Generate()));

	[Test] public void CtorGuid_ExistingAsset_Succeeds()
	{
		var assetObject = CreateTestAssetObject(TestAssetPath);
		var guid = Asset.Path.GetGuid(TestAssetPath);

		var asset = new Asset(guid);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, assetObject);
		Assert.True(asset.Guid.Equals(guid));
	}

	[Test] public void CtorGuid_ExistingFolder_Succeeds()
	{
		var guid = Asset.Path.GetGuid("Assets");

		var asset = new Asset(guid);

		Assert.True(asset.AssetPath == "Assets");
		Assert.NotNull(asset.MainObject);
		Assert.AreEqual(asset.MainObject.GetType(), typeof(DefaultAsset));
		Assert.True(asset.Guid.Equals(guid));
	}
}
