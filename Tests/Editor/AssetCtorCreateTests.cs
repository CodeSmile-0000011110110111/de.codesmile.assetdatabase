// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;
using System.Text;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CodeSmileEditor.Tests
{
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

			var asset = new Asset(obj, (String)TestAssetPath);

			Assert.True(TestAssetPath.Exists);
			Assert.NotZero(asset.FileId);
			Assert.False(asset.Guid.Empty());
			Assert.NotNull(asset.GetMain<ExampleSO>());
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

		// UI Toolkit runtime theme
		private readonly string TssContents = "@import url(\"unity-theme://default\");\nVisualElement {}";
		[Test] public void CreateCtor_StringContents_CreatesAndImportsAsset()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.tss");

			var asset = new Asset(TssContents, path, true);

			Assert.True(Asset.Status.IsImported(path));
			Assert.True(asset.MainObject is ThemeStyleSheet);
			Assert.AreEqual(typeof(ThemeStyleSheet), asset.MainObjectType);
		}

		[Test] public void CreateCtor_ByteArray_CreatesAndImportsAsset()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.tss");

			var asset = new Asset(Encoding.UTF8.GetBytes(TssContents), path, true);

			Assert.True(Asset.Status.IsImported(path));
			Assert.True(asset.MainObject is ThemeStyleSheet);
			Assert.AreEqual(typeof(ThemeStyleSheet), asset.MainObjectType);
		}

		[Test] public void CreateStatic_StringContents_CreatesAndImportsAsset()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.tss");

			var obj = Asset.File.Create(TssContents, path);

			Assert.True(Asset.Status.IsImported(path));
			Assert.True(obj is ThemeStyleSheet);
		}

		[Test] public void CreateStatic_ByteArray_CreatesAndImportsAsset()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.tss");

			var obj = Asset.File.Create(Encoding.UTF8.GetBytes(TssContents), path);

			Assert.True(Asset.Status.IsImported(path));
			Assert.True(obj is ThemeStyleSheet);
		}

		[Test] public void CreateAsNewStatic_StringContents_CreatesAndImportsAsset()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.tss");

			var obj = Asset.File.CreateAsNew(TssContents, path);

			Assert.True(Asset.Status.IsImported(path));
			Assert.True(obj is ThemeStyleSheet);
		}

		[Test] public void CreateAsNewStatic_ByteArray_CreatesAndImportsAsset()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.tss");

			var obj = Asset.File.CreateAsNew(Encoding.UTF8.GetBytes(TssContents), path);

			Assert.True(Asset.Status.IsImported(path));
			Assert.True(obj is ThemeStyleSheet);
		}

		// Verifying a theory that creating a new folder and subsequently creating an asset within
		// does not actually require to import the new folder.
		[Test] public void CreateStatic_InCreatedButNotImportedDirectory_CreatesAssetAndFolderIsImported()
		{
			Assert.False(System.IO.Directory.Exists(TestSubFolderPath));
			System.IO.Directory.CreateDirectory(TestSubFolderPath);

			var assetPath = TestSubFolderPath + "/test-so.asset";
			var so = Instantiate.ExampleSO();

			AssetDatabase.CreateAsset(so, assetPath);

			Assert.True(Asset.Path.FileExists(assetPath));
			Assert.True(Asset.Status.IsImported(TestSubFolderPath));
			Assert.True(Asset.Status.IsImported(assetPath));
		}
	}
}
