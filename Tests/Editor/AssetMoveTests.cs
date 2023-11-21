// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;

namespace CodeSmile.Editor
{
	public class AssetMoveTests : AssetTestBase
	{
		[Test] public void CanMoveStatic_NullPaths_False()
		{
			Assert.False(Asset.File.CanMove(null, "Assets/moved.asset"));
			Assert.False(Asset.File.CanMove("Assets/not ex.ist", null));
		}

		[Test] public void CanMoveStatic_NonExistingFile_False() =>
			Assert.False(Asset.File.CanMove("Assets/file does not exist.asset", "Assets/moved.asset"));

		[TestCase("Assets", false)]
		[TestCase("Assets/other.asset", true)]
		[TestCase("Assets/other.change extension", true)]
		public void CanMoveStatic_FileToValidDestination_AsExpected(String destPath, Boolean expectedCanMove)
		{
			CreateTestAsset(TestAssetPath);

			var canMove = Asset.File.CanMove(TestAssetPath, destPath);

			Assert.AreEqual(expectedCanMove, canMove);
		}

		[TestCase("Assets", false)]
		[TestCase("Assets/other.asset", true)]
		public void CanMove_FileToValidDestination_AsExpected(String destPath, Boolean expectedCanMove)
		{
			var asset = CreateTestAsset(TestAssetPath);

			var canMove = asset.CanMove(destPath);

			Assert.AreEqual(expectedCanMove, canMove);
		}

		[Test] public void MoveStatic_NullPaths_False()
		{
			Assert.False(Asset.File.Move(null, "Assets/moved.asset"));
			Assert.False(Asset.File.Move("Assets/not ex.ist", null));
		}

		[Test] public void MoveStatic_FileToNonExistingFolderAndChangeExtension_Succeeds()
		{
			var destPath = "Assets/subfolder/now with a different.extension";
			var asset = CreateTestAsset(TestAssetPath);

			var didMove = Asset.File.Move(TestAssetPath, destPath);
			asset = new Asset(destPath); // original asset is no longer valid since we used the static method

			Assert.AreEqual(String.Empty, Asset.GetLastErrorMessage());
			Assert.True(didMove);
			Assert.AreEqual(destPath, Asset.Path.Get(asset.MainObject));
			Assert.True(((Asset.Path)destPath).Exists);
		}

		[Test] public void Move_NullPath_False()
		{
			var asset = CreateTestAsset(TestAssetPath);
			Assert.False(asset.Move(null));
		}

		[Test] public void Move_FileToNonExistingFolderAndChangeExtension_Succeeds()
		{
			var destPath = "Assets/subfolder/now with a different.extension";
			var asset = CreateTestAsset(TestAssetPath);

			var didMove = asset.Move(destPath);

			Assert.AreEqual(String.Empty, Asset.GetLastErrorMessage());
			Assert.True(didMove);
			Assert.AreEqual(destPath, asset.AssetPath);
			Assert.True(asset.AssetPath.Exists);
		}
	}
}
