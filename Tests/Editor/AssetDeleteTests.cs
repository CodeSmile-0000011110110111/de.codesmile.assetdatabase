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
		var obj = CreateTestAssetObject(TestAssetPath);
		Assert.True(TestAssetPath.ExistsInFileSystem);
		Assert.True(Asset.Exists(obj));

		Asset.Delete(obj);

		Assert.False(TestAssetPath.ExistsInFileSystem);
		Assert.False(Asset.Exists(obj));
	}

	[Test] public void DeleteStatic_ExistingAssetPath_FileDeleted()
	{
		var obj = CreateTestAssetObject(TestAssetPath);
		Assert.True(TestAssetPath.ExistsInFileSystem);
		Assert.True(Asset.Exists(obj));

		Asset.Delete(TestAssetPath);

		Assert.False(TestAssetPath.ExistsInFileSystem);
		Assert.False(Asset.Exists(obj));
	}

	[Test] public void Delete_ExistingAssetObject_FileDeleted()
	{
		var asset = new Asset(CreateTestAssetObject(TestAssetPath));

		var deletedObj = asset.Delete();
		asset.Delete(); // delete again should not do anything

		Assert.NotNull(deletedObj);
		Assert.False(Asset.Exists(deletedObj));
		Assert.False(TestAssetPath.ExistsInFileSystem);
		Assert.Null(asset.Delete());
	}

	[Test] public void Delete_SaveAfterDelete_Throws()
	{
		var asset = new Asset(CreateTestAssetObject(TestAssetPath));
		asset.Delete();

		Assert.Throws<InvalidOperationException>(() => asset.Save());
	}
}
