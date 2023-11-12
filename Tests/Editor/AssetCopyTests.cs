// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;

public class AssetCopyTests : AssetTestBase
{
	[Test] public void CopyStatic_NullPath_Throws()
	{
		Assert.Throws<ArgumentNullException>(() => Asset.Copy(null, "Assets"));
		Assert.Throws<ArgumentNullException>(() => Asset.Copy("Assets", null));
	}

	[Test] public void CopyStatic_MissingSourcePath_Throws() =>
		Assert.Throws<ArgumentException>(() => Asset.Copy(TestAssetPath, TestAssetPath));

	[Test] public void CopyStatic_OntoItselfOverwrite_Throws()
	{
		var asset = CreateTestAsset(TestAssetPath);

		Assert.Throws<ArgumentException>(() => Asset.Copy(asset.AssetPath, asset.AssetPath, true));
	}

	[Test] public void CopyStatic_ToNotExistingFolder_Succeeds()
	{
		var asset = CreateTestAsset(TestAssetPath);

		var destPath = DeleteAfterTest((Asset.Path)"Assets/subfolder/copy.asset");
		var didCopy = Asset.Copy(asset.AssetPath, destPath);
		//Debug.Log(asset.LastErrorMessage);

		Assert.True(didCopy);
		Assert.True(destPath.Exists);
		Assert.AreEqual(String.Empty, asset.LastErrorMessage);
	}

	[Test] public void CopyStatic_OntoItselfNoOverwrite_CreatesCopy()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var expectedCopyPath = DeleteAfterTest(Asset.Path.UniquifyFilename(asset.AssetPath));

		var success = Asset.Copy(asset.AssetPath, (String)asset.AssetPath);

		Assert.True(success);
		Assert.True(expectedCopyPath.Exists);
	}

	[Test] public void Copy_OntoItselfNoOverwrite_CreatesCopy()
	{
		var asset = CreateTestAsset(TestAssetPath);

		var assetCopy = DeleteAfterTest(asset.Copy(asset.AssetPath));

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
