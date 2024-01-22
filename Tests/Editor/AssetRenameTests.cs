// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;

namespace CodeSmileEditor.Tests
{
	public class AssetRenameTests : AssetTestBase
	{
		[Test] public void RenameStatic_NullPaths_False()
		{
			Assert.False(Asset.File.Move(null, "Assets/moved.asset"));
			Assert.False(Asset.File.Move("Assets/not ex.ist", null));
		}

		[Test] public void Rename_NullPath_False()
		{
			var asset = CreateTestAsset(TestAssetPath);
			Assert.False(asset.Rename(null));
		}

		[Test] public void Rename_PathInsteadOfFileName_False()
		{
			var asset = CreateTestAsset(TestAssetPath);
			Assert.False(asset.Rename("Assets/renamed.asset")); // cannot use a path
		}

		[Test] public void RenameStatic_ExistingFile_DidRename()
		{
			var asset = CreateTestAsset(TestAssetPath);

			var newPath = DeleteAfterTest((Asset.Path)"Assets/NewName.asset");
			var success = Asset.File.Rename((String)asset.AssetPath, "NewName");
			asset = new Asset(newPath); // original asset no longer valid, we used the static Rename method

			Assert.IsEmpty(Asset.GetLastErrorMessage());
			Assert.IsTrue(success);
			Assert.AreEqual(newPath, Asset.Path.Get(asset.MainObject));
			Assert.IsTrue(newPath.Exists);
			Assert.IsFalse(TestAssetPath.Exists);
		}

		[Test] public void Rename_ExistingFile_DidRename()
		{
			var asset = CreateTestAsset(TestAssetPath);

			var newPath = DeleteAfterTest((Asset.Path)"Assets/NewName.asset");
			var success = asset.Rename("NewName");

			Assert.IsEmpty(Asset.GetLastErrorMessage());
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

#if UNITY_2021_3_OR_NEWER && !UNITY_2022_1_OR_NEWER
			UnityEngine.Debug.LogWarning($"{nameof(Rename_DifferentExtension_RetainsOriginalExtension)} expected warning in Unity 2021.3: "+
				$"\"Main Object Name '..' does not match filename '..'\"");
#endif
			var success = asset.Rename("NewName.ext");

			Assert.IsEmpty(Asset.GetLastErrorMessage());
			Assert.IsTrue(success);
			Assert.AreEqual("Assets/NewName.ext.asset", asset.AssetPath);
		}

		[Test] public void Rename_Folder_DidRename()
		{
			var assetPath = (Asset.Path)"Assets/subfolder/ping";
			assetPath.CreateFolders();

			var asset = DeleteAfterTest(new Asset(assetPath));
			var success = asset.Rename("PONG");

			Assert.IsEmpty(Asset.GetLastErrorMessage());
			Assert.IsTrue(success);
			Assert.AreEqual(asset.AssetPath, "Assets/subfolder/PONG");
		}
	}
}
