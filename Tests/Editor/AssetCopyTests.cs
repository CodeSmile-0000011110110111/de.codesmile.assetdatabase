// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;

namespace CodeSmileEditor.Tests
{
	public class AssetCopyTests : AssetTestBase
	{
		[Test] public void CopyStatic_NullPath_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => Asset.File.Copy(null, "Assets"));
			Assert.Throws<ArgumentNullException>(() => Asset.File.Copy("Assets", null));
		}

		[Test] public void CopyStatic_MissingSourcePath_Throws() =>
			Assert.Throws<ArgumentException>(() => Asset.File.Copy(TestAssetPath, TestAssetPath));

		[Test] public void CopyStatic_OntoItself_Throws()
		{
			var asset = CreateTestAsset(TestAssetPath);

			Assert.Throws<ArgumentException>(() => Asset.File.Copy(asset.AssetPath, asset.AssetPath));
		}

		[Test] public void CopyStatic_ToNotExistingFolder_Succeeds()
		{
			var asset = CreateTestAsset(TestAssetPath);

			var destPath = DeleteAfterTest((Asset.Path)"Assets/subfolder/copy.asset");
			var didCopy = Asset.File.Copy(asset.AssetPath, destPath);
			//Debug.Log(asset.LastErrorMessage);

			Assert.True(didCopy);
			Assert.True(destPath.Exists);
			Assert.AreEqual(String.Empty, Asset.GetLastErrorMessage());
		}

		[Test] public void CopyNoOverwriteStatic_OntoItself_CreatesCopy()
		{
			var asset = CreateTestAsset(TestAssetPath);
			var expectedCopyPath = DeleteAfterTest(Asset.Path.UniquifyFileName(asset.AssetPath));

			var success = Asset.File.CopyAsNew(asset.AssetPath, (String)asset.AssetPath);

			Assert.True(success);
			Assert.True(expectedCopyPath.Exists);
		}

		[Test] public void CopyNoOverwrite_OntoItself_CreatesCopy()
		{
			var asset = CreateTestAsset(TestAssetPath);

			var assetCopy = DeleteAfterTest(asset.SaveAsNew(asset.AssetPath));

			Assert.NotNull(assetCopy);
			Assert.AreNotEqual(asset, assetCopy);
		}

		[Test] public void Duplicate_CreatesDuplicate()
		{
			var asset = CreateTestAsset(TestAssetPath);

			var assetDupe = DeleteAfterTest(asset.Duplicate());

			Assert.NotNull(assetDupe);
			Assert.AreNotEqual(asset, assetDupe);
		}
	}
}
