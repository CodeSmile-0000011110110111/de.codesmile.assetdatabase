// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetCtorCreateTests : AssetTestBase
{
	[Test] public void CreateCtor_NullObject_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset((Object)null, (String)TestAssetPath));

	[Test] public void CreateCtor_NullPath_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset(Instantiate.ExampleSO(), (String)null));

	[Test] public void CreateCtor_ObjectAlreadyAnAsset_Throws()
	{
		var existing = CreateTestAssetObject(TestAssetPath);
		Assert.Throws<ArgumentException>(() => new Asset(existing, (String)TestAssetPath));
	}

	[Test] public void CreateCtor_AssetExistsNoOverwrite_CreatesAssetWithUniqueName()
	{
		var testPath = TestAssetPath;
		var existing = CreateTestAssetObject(testPath);
		var expectedPath = Asset.Path.UniquifyFileName(testPath);
		var newObject = Instantiate.ExampleSO();

		var newAsset = DeleteAfterTest(new Asset(newObject, (String)TestAssetPath));

		Assert.True(Asset.Path.FileExists(testPath));
		Assert.True(Asset.Path.FileExists(expectedPath));
		Assert.AreEqual(expectedPath, newAsset.AssetPath);
		Assert.AreNotEqual(existing, newAsset.MainObject);
	}

	[Test] public void CreateCtor_AssetExistsShouldOverwrite_ReplacesExistingAsset()
	{
		var testPath = TestAssetPath;
		var existing = CreateTestAssetObject(testPath);
		var expectedPath = Asset.Path.UniquifyFileName(testPath);
		var newObject = Instantiate.ExampleSO();

		var newAsset = DeleteAfterTest(new Asset(newObject, (String)TestAssetPath, true));

		Assert.True(Asset.Path.FileExists(testPath));
		Assert.False(Asset.Path.FileExists(expectedPath));
		Assert.AreEqual(testPath, newAsset.AssetPath);
		Assert.AreNotEqual(existing, newAsset.MainObject);
	}

	[Test] public void CreateCtor_ObjectNotAnAssetAndValidPath_CreatesAsset()
	{
		var obj = DeleteAfterTest((Object)Instantiate.ExampleSO());

		new Asset(obj, (String)TestAssetPath);

		Assert.True(TestAssetPath.Exists);
	}

	[Test] public void CreateCtor_NotExistingSubFoldersPath_CreatesFoldersAndAsset()
	{
		var obj = DeleteAfterTest((Object)Instantiate.ExampleSO());

		new Asset(obj, (String)TestSubFoldersAssetPath);

		Assert.True(TestSubFoldersAssetPath.Exists);
	}

	[Test] public void CreateStatic_ObjectNotAnAssetAndValidPath_CreatesAsset()
	{
		var obj = DeleteAfterTest((Object)Instantiate.ExampleSO());

		Asset.File.Create(obj, (String)TestAssetPath);

		Assert.True(TestAssetPath.Exists);
	}

	[Test] public void CreateCtor_StringContents_CreatesAndImportsAsset()
	{
		var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.tss");
		var tss = "@import url(\"unity-theme://default\");\nVisualElement {}"; // UI Toolkit runtime theme

		var asset = new Asset(tss, path, true);

		Assert.True(Asset.Status.IsImported(path));
		Assert.True(asset.MainObject is UnityEngine.UIElements.ThemeStyleSheet);
		Assert.AreEqual(typeof(UnityEngine.UIElements.ThemeStyleSheet), asset.MainObjectType);
	}
}
