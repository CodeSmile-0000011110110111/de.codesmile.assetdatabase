// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using Object = UnityEngine.Object;

public class AssetDeleteTests : AssetTestBase
{
	[Test] public void DeleteStatic_Null_DoesNotThrow()
	{
		Asset.Delete((String)null);
		Asset.Delete((Asset.Path)null);
		Asset.Delete((Object)null);
	}

	[Test] public void DeleteStatic_ExistingAssetObject_FileDeleted()
	{
		var asset = CreateTestAsset(TestAssetPath);
		Assert.True(AssetHelper.FileExists(asset));

		Asset.Delete(asset);

		Assert.False(AssetHelper.FileExists(asset));
	}

	[Test] public void DeleteStatic_ExistingAssetPath_FileDeleted()
	{
		var asset = CreateTestAsset(TestAssetPath);
		Assert.True(AssetHelper.FileExists(asset));

		Asset.Delete(TestAssetPath);

		Assert.False(AssetHelper.FileExists(asset));
	}

	[Test] public void Delete_ExistingAssetObject_FileDeleted()
	{
		var asset = new Asset(CreateTestAsset(TestAssetPath));

		var deletedObj = asset.Delete();

		Assert.NotNull(deletedObj);
		Assert.False(Asset.Exists(deletedObj));
		Assert.False(AssetHelper.FileExists(TestAssetPath));
	}

	[Test] public void TrashStatic_Null_DoesNotThrow()
	{
		Asset.Trash((String)null);
		Asset.Trash((Asset.Path)null);
		Asset.Trash((Object)null);
	}

	[Test] public void TrashStatic_ExistingAssetObject_FileDeleted()
	{
		var asset = CreateTestAsset(TestAssetPath);
		Assert.True(AssetHelper.FileExists(asset));

		Asset.Trash(asset);

		Assert.False(AssetHelper.FileExists(asset));
	}

	[Test] public void TrashStatic_ExistingAssetPath_FileDeleted()
	{
		var asset = CreateTestAsset(TestAssetPath);
		Assert.True(AssetHelper.FileExists(asset));

		Asset.Trash(TestAssetPath);

		Assert.False(AssetHelper.FileExists(asset));
	}

	[Test] public void Trash_ExistingAssetObject_FileDeleted()
	{
		var asset = new Asset(CreateTestAsset(TestAssetPath));

		var deletedObj = asset.Trash();

		Assert.NotNull(deletedObj);
		Assert.False(Asset.Exists(deletedObj));
		Assert.False(AssetHelper.FileExists(TestAssetPath));
	}
}
