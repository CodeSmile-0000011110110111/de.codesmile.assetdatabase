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

	[Test] public void CopyStatic_OntoItselfNoOverwrite_CreatesCopy()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var expectedCopyPath = Asset.Path.UniquifyFilename(asset.AssetPath);
		DeleteAfterTest(expectedCopyPath);

		var success = Asset.Copy(asset.AssetPath, (String)asset.AssetPath);

		Assert.True(success);
		Assert.True(expectedCopyPath.Exists);
	}

	[Test] public void Copy_OntoItselfNoOverwrite_CreatesCopy()
	{
		var asset = CreateTestAsset(TestAssetPath);

		var assetCopy = asset.Copy(asset.AssetPath);
		DeleteAfterTest(assetCopy);

		Assert.NotNull(assetCopy);
		Assert.AreNotEqual(asset, assetCopy);
	}
}