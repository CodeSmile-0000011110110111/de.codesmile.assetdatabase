// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;

public class AssetCreateTests : AssetTestBase
{
	[Test] public void CtorCreate_NullObject_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset(null, (String)TestAssetPath));

	[Test] public void CtorCreate_NullPath_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset(Instantiate.ExampleSO(), (String)null));

	[Test] public void CtorCreate_ObjectAlreadyAnAsset_Throws()
	{
		var existing = CreateTestAsset(TestAssetPath);
		Assert.Throws<ArgumentException>(() => new Asset(existing, (String)TestAssetPath));
	}

	[Test] public void CtorCreate_AssetExistsNoOverwrite_CreatesAssetWithUniqueName()
	{
		var testPath = TestAssetPath;
		var existing = CreateTestAsset(testPath);
		var expectedPath = Asset.Path.UniquifyFilename(testPath);
		var newObject = Instantiate.ExampleSO();

		var newAsset = DeleteAfterTest(new Asset(newObject, (String)TestAssetPath));

		Assert.True(Asset.Path.FileExists(testPath));
		Assert.True(Asset.Path.FileExists(expectedPath));
		Assert.AreEqual(expectedPath, newAsset.AssetPath);
		Assert.AreNotEqual(existing, newAsset.MainObject);
	}

	[Test] public void CtorCreate_AssetExistsShouldOverwrite_ReplacesExistingAsset()
	{
		var testPath = TestAssetPath;
		var existing = CreateTestAsset(testPath);
		var expectedPath = Asset.Path.UniquifyFilename(testPath);
		var newObject = Instantiate.ExampleSO();

		var newAsset = DeleteAfterTest(new Asset(newObject, (String)TestAssetPath, true));

		Assert.True(Asset.Path.FileExists(testPath));
		Assert.False(Asset.Path.FileExists(expectedPath));
		Assert.AreEqual(testPath, newAsset.AssetPath);
		Assert.AreNotEqual(existing, newAsset.MainObject);
	}

	[Test] public void CtorCreate_ObjectNotAnAssetAndValidPath_CreatesAsset()
	{
		var obj = DeleteAfterTest(Instantiate.ExampleSO());

		new Asset(obj, (String)TestAssetPath);

		Assert.True(TestAssetPath.ExistsInFileSystem);
	}

	[Test] public void CtorCreate_NotExistingSubFoldersPath_CreatesFoldersAndAsset()
	{
		var obj = DeleteAfterTest(Instantiate.ExampleSO());

		new Asset(obj, (String)TestSubFoldersAssetPath);

		Assert.True(TestSubFoldersAssetPath.ExistsInFileSystem);
	}
}
