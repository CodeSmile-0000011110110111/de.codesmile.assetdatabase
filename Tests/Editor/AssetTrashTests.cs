// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using Object = UnityEngine.Object;

public class AssetTrashTests : AssetTestBase
{
	[Test] public void TrashStatic_Null_DoesNotThrow()
	{
		Asset.Trash((String)null);
		Asset.Trash((Asset.Path)null);
		Asset.Trash((Object)null);
	}

	[Test] public void TrashStatic_ExistingAssetObject_FileDeleted()
	{
		var obj = CreateTestAsset(TestAssetPath);
		Assert.True(TestAssetPath.ExistsInFileSystem);
		Assert.True(Asset.Exists(obj));

		Asset.Trash(obj);

		Assert.False(TestAssetPath.ExistsInFileSystem);
		Assert.False(Asset.Exists(obj));
	}

	[Test] public void TrashStatic_ExistingAssetPath_FileDeleted()
	{
		var obj = CreateTestAsset(TestAssetPath);
		Assert.True(TestAssetPath.ExistsInFileSystem);
		Assert.True(Asset.Exists(obj));

		Asset.Trash(TestAssetPath);

		Assert.False(TestAssetPath.ExistsInFileSystem);
		Assert.False(Asset.Exists(obj));
	}

	[Test] public void Trash_ExistingAssetObject_FileDeleted()
	{
		var asset = new Asset(CreateTestAsset(TestAssetPath));

		var deletedObj = asset.Trash();

		Assert.NotNull(deletedObj);
		Assert.False(Asset.Exists(deletedObj));
		Assert.False(TestAssetPath.ExistsInFileSystem);
		Assert.Null(asset.Trash());
	}
}