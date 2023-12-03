// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;
using System.IO;

namespace CodeSmileEditor.Tests
{
	public class AssetImportTests : AssetTestBase
	{
		[Test] public void ImportStatic_SystemIOCreatedFile_ExistsInDatabase()
		{
			var testPath = DeleteAfterTest("Assets/file.txt");
			File.WriteAllText(testPath, "<for no eyes only>");
			Assert.DoesNotThrow(() => new Asset(testPath));
		}

		[Test] public void ImportAllStatic_SystemIODeletedFile_AssetObjectLifetimeAsExpected()
		{
			var testPath = DeleteAfterTest("Assets/file.txt");
			File.WriteAllText(testPath, "<for no eyes only>");
			Asset.File.Import(testPath);
			Assert.DoesNotThrow(() => new Asset(testPath));
			var asset = new Asset(testPath);

			// 'externally' delete the file and it's meta
			File.Delete(testPath);
			File.Delete(testPath + ".meta");
			Assert.True(Asset.Status.IsImported(asset.MainObject)); // deleted, but still in the database
			Assert.Throws<FileNotFoundException>(() => Asset.File.Import(testPath)); // import fail: file does not exist
			Assert.True(Asset.Status.IsImported(asset.MainObject)); // it's still in the database

			Asset.Database.ImportAll();

			Assert.False(Asset.Status.IsImported(asset.MainObject)); // now it's gone
		}

		[Test] public void ImportAllStatic_SystemIOCreatedFile_ExistsInDatabase()
		{
			var testPath = DeleteAfterTest("Assets/file.txt");
			Assert.Throws<FileNotFoundException>(() => new Asset(testPath)); // throws, file does not exist

			File.WriteAllText(testPath, "test file contents are irrelevant");
			Assert.DoesNotThrow(() => new Asset(testPath)); // does not throw, asset auto-imported

			Asset.Database.ImportAll();

			Assert.DoesNotThrow(() => new Asset(testPath));
			DeleteAfterTest(new Asset(testPath));
		}
	}
}
