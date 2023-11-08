// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;

namespace CodeSmile.Editor
{
	public class AssetRenameTests : AssetTestBase
	{
		[Test] public void RenameStatic_ExistingFile_DidRename()
		{
			var asset = CreateTestAsset(TestAssetPath);

			var newPath = DeleteAfterTest((Asset.Path)"Assets/NewName.asset");
			var success = Asset.Rename((String)asset.AssetPath, newPath);

			Assert.IsEmpty(Asset.LastErrorMessage);
			Assert.IsTrue(success);
			Assert.AreEqual(newPath, Asset.Path.Get(asset.MainObject));
			Assert.IsTrue(newPath.Exists);
			Assert.IsFalse(TestAssetPath.Exists);
		}

		[Test] public void Rename_ExistingFile_DidRename()
		{
			var asset = CreateTestAsset(TestAssetPath);

			var newPath = DeleteAfterTest((Asset.Path)"Assets/NewName.asset");
			var success = asset.Rename(newPath);

			Assert.IsEmpty(Asset.LastErrorMessage);
			Assert.IsTrue(success);
			Assert.AreEqual(newPath, asset.AssetPath);
			Assert.AreEqual(newPath, Asset.Path.Get(asset.MainObject));
			Assert.IsTrue(newPath.Exists);
			Assert.IsFalse(TestAssetPath.Exists);
		}

		[Test] public void Rename_DifferentExtension_RetainsOriginalExtension()
		{
			var asset = CreateTestAsset(TestAssetPath);

			var newPath = DeleteAfterTest((Asset.Path)"Assets/NewName.ext");
			var success = asset.Rename(newPath);

			Assert.IsEmpty(Asset.LastErrorMessage);
			Assert.IsTrue(success);
			Assert.AreEqual("Assets/NewName.ext.asset", asset.AssetPath);
		}

		[Test] public void Rename_Folder_DidRename()
		{
			var assetPath = (Asset.Path)"Assets/subfolder/ping";
			assetPath.CreateFolders();

			var asset = DeleteAfterTest(new Asset(assetPath));
			var success = asset.Rename("PONG");

			Assert.IsEmpty(Asset.LastErrorMessage);
			Assert.IsTrue(success);
			Assert.AreEqual(asset.AssetPath, "Assets/subfolder/PONG");
		}
	}
}
