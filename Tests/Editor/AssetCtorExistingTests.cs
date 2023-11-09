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
	[Test] public void PathCtor_Null_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset((Asset.Path)null));

	[Test] public void PathCtor_NotExistingPath_Throws() =>
		Assert.Throws<FileNotFoundException>(() => new Asset("Assets/does not.exist"));

	[Test] public void PathCtor_ExistingPath_Succeeds()
	{
		var obj = CreateTestAssetObject(TestAssetPath);

		var asset = new Asset(TestAssetPath);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, obj);
		Assert.AreEqual(asset.Guid, Asset.Path.GetGuid(TestAssetPath));
	}

	[Test] public void ObjectCtor_Null_Throws() => Assert.Throws<ArgumentNullException>(() => new Asset((Object)null));

	[Test] public void ObjectCtor_NotAnAsset_Throws() =>
		Assert.Throws<ArgumentException>(() => new Asset(Instantiate.ExampleSO()));

	[Test] public void ObjectCtor_ExistingAsset_Succeeds()
	{
		var obj = CreateTestAssetObject(TestAssetPath);

		var asset = new Asset(obj);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, obj);
		Assert.AreEqual(asset.Guid, Asset.Path.GetGuid(TestAssetPath));
	}

	[Test] public void GuidCtor_EmptyGuid_Throws() => Assert.Throws<ArgumentException>(() => new Asset(new GUID()));

	[Test] public void GuidCtor_NotAnAsset_Throws() =>
		Assert.Throws<ArgumentException>(() => new Asset(GUID.Generate()));

	[Test] public void GuidCtor_ExistingAsset_Succeeds()
	{
		var obj = CreateTestAssetObject(TestAssetPath);
		var guid = Asset.Path.GetGuid(TestAssetPath);

		var asset = new Asset(guid);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, obj);
		Assert.True(asset.Guid.Equals(guid));
	}

	[Test] public void GuidCtor_ExistingFolder_Succeeds()
	{
		var guid = Asset.Path.GetGuid("Assets");

		var asset = new Asset(guid);

		Assert.True(asset.AssetPath == "Assets");
		Assert.NotNull(asset.MainObject);
		Assert.AreEqual(asset.MainObject.GetType(), typeof(DefaultAsset));
		Assert.True(asset.Guid.Equals(guid));
	}

	[Test] public void ObjectImplicit_NullAsset_IsNull()
	{
		Object obj = (Asset)null;
		Assert.Null(obj);
	}

	[Test] public void AssetImplicit_NullObject_IsNull()
	{
		Asset asset = (Object)null;
		Assert.Null(asset);
	}

	[Test] public void AssetImplicit_NullPath_IsNull()
	{
		Asset asset = (Asset.Path)null;
		Assert.Null(asset);
	}

	[Test] public void AssetImplicit_EmptyGuid_IsNull()
	{
		Asset asset = new GUID();
		Assert.Null(asset);
	}

	[Test] public void ObjectImplicit_ExistingAssetInstance_Succeeds()
	{
		var asset = CreateTestAsset(TestAssetPath);

		Object obj = asset;

		Assert.NotNull(obj);
		Assert.AreEqual(asset.MainObject, obj);
	}

	[Test] public void AssetImplicit_ExistingAssetObject_Succeeds()
	{
		var obj = CreateTestAssetObject(TestAssetPath);

		Asset asset = obj;

		Assert.NotNull(asset);
		Assert.AreEqual(obj, asset.MainObject);
	}

	[Test] public void AssetImplicit_ExistingAssetPath_Succeeds()
	{
		var obj = CreateTestAssetObject(TestAssetPath);

		Asset asset = TestAssetPath;

		Assert.NotNull(asset);
		Assert.AreEqual(obj, asset.MainObject);
		Assert.AreEqual(TestAssetPath, asset.AssetPath);
	}

	[Test] public void AssetImplicit_ExistingAssetGuid_Succeeds()
	{
		var obj = CreateTestAssetObject(TestAssetPath);
		var guid = Asset.Path.GetGuid(TestAssetPath);

		Asset asset = guid;

		Assert.NotNull(asset);
		Assert.AreEqual(obj, asset.MainObject);
		Assert.AreEqual(TestAssetPath, asset.AssetPath);
	}
}
