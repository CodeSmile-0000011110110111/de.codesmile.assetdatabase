// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System.IO;

public class AssetImportTests : AssetTestBase
{
	[Test] public void ImportStatic_SystemIOCreatedFile_ExistsInDatabase()
	{
		var testPath = DeleteAfterTest("Assets/file.txt");
		File.WriteAllText(testPath, "<for no eyes only>");
		Assert.Throws<AssetLoadException>(() => new Asset(testPath));

		Asset.Import(testPath);

		Assert.DoesNotThrow(() => new Asset(testPath));
		DeleteAfterTest(new Asset(testPath));
	}

	[Test] public void ImportAllStatic_SystemIODeletedFile_AssetObjectLifetimeAsExpected()
	{
		var testPath = DeleteAfterTest("Assets/file.txt");
		File.WriteAllText(testPath, "<for no eyes only>");
		Asset.Import(testPath);
		Assert.DoesNotThrow(() => new Asset(testPath));
		var asset = new Asset(testPath);

		// 'externally' delete the file and it's meta
		File.Delete(testPath);
		File.Delete(testPath + ".meta");
		Assert.True(Asset.IsImported(asset.MainObject)); // deleted, but still in the database
		Asset.Import(testPath); // Import() does nothing if the path doesn't exist
		Assert.True(Asset.IsImported(asset.MainObject)); // it's still in the database

		Asset.ImportAll();

		Assert.False(Asset.IsImported(asset.MainObject)); // now it's gone
	}

	[Test] public void ImportAllStatic_SystemIOCreatedFile_ExistsInDatabase()
	{
		var testPath = DeleteAfterTest("Assets/file.txt");
		Assert.Throws<FileNotFoundException>(() => new Asset(testPath)); // throws, file does not exist

		File.WriteAllText(testPath, "test file contents are irrelevant");
		Assert.Throws<AssetLoadException>(() => new Asset(testPath)); // throws, asset not in database

		Asset.ImportAll();

		Assert.DoesNotThrow(() => new Asset(testPath));
		DeleteAfterTest(new Asset(testPath));
	}
}
