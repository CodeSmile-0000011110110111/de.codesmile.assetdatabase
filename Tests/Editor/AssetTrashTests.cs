// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;
using Object = UnityEngine.Object;

namespace CodeSmileEditor.Tests
{
	public class AssetTrashTests : AssetTestBase
	{
		[Test] public void TrashStatic_Null_DoesNotThrow()
		{
			Asset.File.Trash((String)null);
			Asset.File.Trash((Asset.Path)null);
			Asset.File.Trash((Object)null);
		}

		[Test] public void TrashStatic_ExistingAssetObject_FileDeleted()
		{
			var obj = CreateTestAssetObject(TestAssetPath);
			Assert.True(TestAssetPath.ExistsInFileSystem);
			Assert.True(Asset.Status.IsImported(obj));

			Asset.File.Trash(obj);

			Assert.False(TestAssetPath.ExistsInFileSystem);
			Assert.False(Asset.Status.IsImported(obj));
		}

		[Test] public void TrashStatic_ExistingAssetPath_FileDeleted()
		{
			var obj = CreateTestAssetObject(TestAssetPath);
			Assert.True(TestAssetPath.ExistsInFileSystem);
			Assert.True(Asset.Status.IsImported(obj));

			Asset.File.Trash(TestAssetPath);

			Assert.False(TestAssetPath.ExistsInFileSystem);
			Assert.False(Asset.Status.IsImported(obj));
		}

		[Test] public void Trash_ExistingAssetObject_FileDeleted()
		{
			var asset = new Asset(CreateTestAssetObject(TestAssetPath));

			var deletedObj = asset.Trash();

			Assert.NotNull(deletedObj);
			Assert.Null(asset.AssetPath);
			Assert.False(Asset.Status.IsImported(deletedObj));
			Assert.False(TestAssetPath.ExistsInFileSystem);
		}
	}
}
