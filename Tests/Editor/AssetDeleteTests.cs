// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;
using Object = UnityEngine.Object;

namespace CodeSmileEditor.Tests
{
	public class AssetDeleteTests : AssetTestBase
	{
		[Test] public void DeleteStatic_Null_DoesNotThrow()
		{
			Asset.File.Delete((String)null);
			Asset.File.Delete((Asset.Path)null);
			Asset.File.Delete((Object)null);
		}

		[Test] public void DeleteStatic_ExistingAssetObject_FileDeleted()
		{
			var obj = CreateTestAssetObject(TestAssetPath);
			Assert.True(TestAssetPath.ExistsInFileSystem);
			Assert.True(Asset.Status.IsImported(obj));

			Asset.File.Delete(obj);

			Assert.False(TestAssetPath.ExistsInFileSystem);
			Assert.False(Asset.Status.IsImported(obj));
		}

		[Test] public void DeleteStatic_ExistingAssetPath_FileDeleted()
		{
			var obj = CreateTestAssetObject(TestAssetPath);
			Assert.True(TestAssetPath.ExistsInFileSystem);
			Assert.True(Asset.Status.IsImported(obj));

			Asset.File.Delete(TestAssetPath);

			Assert.False(TestAssetPath.ExistsInFileSystem);
			Assert.False(Asset.Status.IsImported(obj));
		}

		[Test] public void Delete_ExistingAssetObject_FileDeleted()
		{
			var asset = new Asset(CreateTestAssetObject(TestAssetPath));

			var deletedObj = asset.Delete();

			Assert.NotNull(deletedObj);
			Assert.Null(asset.AssetPath);
			Assert.False(Asset.Status.IsImported(deletedObj));
			Assert.False(TestAssetPath.ExistsInFileSystem);
		}
	}
}
