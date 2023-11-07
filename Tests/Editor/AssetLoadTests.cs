// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

public class AssetLoadTests : AssetTestBase
{
	[Test] public void Import_SystemIOCreatedFile_ExistsInDatabase()
	{
		var testPath = DeleteFileAfterTest("Assets/file.txt");
		File.WriteAllText(testPath, "<for no eyes only>");
		Assert.Throws<ArgumentException>(() => new Asset(testPath));

		Asset.Import(testPath);

		Assert.DoesNotThrow(() => new Asset(testPath));
		DeleteAfterTest(new Asset(testPath));
	}

	[Test] public void ImportAll_SystemIODeletedFile_AssetObjectLifetimeAsExpected()
	{
		var testPath = DeleteFileAfterTest("Assets/file.txt");
		File.WriteAllText(testPath, "<for no eyes only>");
		Asset.Import(testPath);
		Assert.DoesNotThrow(() => new Asset(testPath));
		var asset = new Asset(testPath);

		// 'externally' delete the file and it's meta
		File.Delete(testPath);
		File.Delete(testPath + ".meta");
		Assert.True(Asset.Database.Contains(asset.MainObject)); // deleted, but still in the database
		Asset.Import(testPath); // Import() does nothing if the path doesn't exist
		Assert.True(Asset.Database.Contains(asset.MainObject)); // it's still in the database

		Asset.Database.ImportAll();

		Assert.False(Asset.Database.Contains(asset.MainObject)); // now it's gone
	}

	[Test] public void ImportAll_SystemIOCreatedFile_ExistsInDatabase()
	{
		var testPath = DeleteFileAfterTest("Assets/file.txt");
		File.WriteAllText(testPath,
			"This used to be called Refresh(), ya know? You should prefer Import() of a given path though.");
		Assert.Throws<ArgumentException>(() => new Asset(testPath));

		Asset.Database.ImportAll();

		Assert.DoesNotThrow(() => new Asset(testPath));
		DeleteAfterTest(new Asset(testPath));
	}

	[Test] public void LoadMain_NotExistingPath_IsNull() => Assert.Null(Asset.LoadMain<Object>("Assets/exist.not"));

	[Test] public void LoadMain_ExistingPath_Succeeds()
	{
		var obj = CreateTestAsset(TestAssetPath);
		var loaded = Asset.LoadMain<Object>(TestAssetPath);
		Assert.NotNull(loaded);
		Assert.AreEqual(obj, loaded);
		Assert.AreEqual(obj.GetType(), loaded.GetType());
	}

	[Test] public void LoadMain_NotExistingGuid_IsNull() => Assert.Null(Asset.LoadMain<Object>(new GUID()));

	[Test] public void LoadMain_ExistingGuid_Succeeds()
	{
		var obj = CreateTestAsset(TestAssetPath);

		var loaded = Asset.LoadMain<Object>(Asset.Path.GetGuid((String)TestAssetPath));

		Assert.NotNull(loaded);
		Assert.AreEqual(obj, loaded);
		Assert.AreEqual(obj.GetType(), loaded.GetType());
		Assert.AreEqual(obj.GetType(), Asset.GetMainType((String)TestAssetPath));
	}
}
