// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;
using UnityEditor;

namespace CodeSmileEditor.Tests
{
	public class AssetPathCreateFoldersTests : AssetTestBase
	{
		[Test] public void CreateFolders_NullPath_Throws() =>
			Assert.Throws<ArgumentNullException>(() => Asset.Path.CreateFolders(null));

		[TestCase(TestSubFoldersPath)]
		[TestCase("Assets/some.file")]
		[TestCase(TestSubFoldersPath + "/fn.ext")]
		public void CreateFolders_ExistingFolder_ReturnsExistingFolderGuid(String dirPath)
		{
			// create the folder first
			var assetPath = (Asset.Path)dirPath;
			var createdGuid = assetPath.CreateFolders();
			Assert.True(Asset.Path.FolderExists(assetPath.FolderPath));

			var folderGuid = assetPath.CreateFolders();

			Assert.False(folderGuid.Empty());
			Assert.AreEqual(createdGuid, folderGuid);
			Assert.True(Asset.Path.FolderExists(assetPath.FolderPath));
		}

		[Test] public void CreateFolders_InvalidPath_Throws()
		{
			var path = "Asset/this * is ? illagel/file.asset";
			Assert.Throws<ArgumentException>(() => Asset.Path.CreateFolders(path));
		}

		[Test] public void CreateFolders_InvalidFileName_Throws()
		{
			var path = "Asset/subfolder/you < are | not : allowed * to do > this.asset";
			Assert.Throws<ArgumentException>(() => Asset.Path.CreateFolders(path));
		}

		[Test] public void CreateFolders_ValidFilePath_FolderExists()
		{
			var dirPath = TestSubFolderPath;

			var folderGuid = DeleteAfterTest(Asset.Path.CreateFolders(dirPath + "/some.file"));

			Assert.False(folderGuid.Empty());
			Assert.AreEqual(((Asset.Path)dirPath).Guid, folderGuid);
			Assert.True(Asset.Path.FolderExists(dirPath));
		}

		[Test] public void CreateFolders_CreateAllFoldersRecursive_FolderExists()
		{
			var dirPath = TestSubFoldersPath;

			var folderGuid = DeleteAfterTest(Asset.Path.CreateFolders(dirPath + "/some.file"));

			Assert.False(folderGuid.Empty());
			Assert.AreEqual(((Asset.Path)dirPath).Guid, folderGuid);
			Assert.True(Asset.Path.FolderExists(dirPath));
		}

		[Test] public void CreateFolders_CreateOnlySomeFoldersRecursive_FolderExists()
		{
			// have topmost folder already created
			var dirPath = TestSubFoldersPath;
			var splitPath = dirPath.Split('/');
			var alreadyExistingFolder = $"{splitPath[0]}/{splitPath[1]}";

			Asset.Path.CreateFolders(alreadyExistingFolder);
			Asset.File.Import(alreadyExistingFolder, ImportAssetOptions.ForceUpdate);
			Assert.True(Asset.Path.FolderExists(alreadyExistingFolder));

			var folderGuid = DeleteAfterTest(Asset.Path.CreateFolders(dirPath + "/some.file"));

			Assert.False(folderGuid.Empty());
			Assert.AreEqual(((Asset.Path)dirPath).Guid, folderGuid);
			Assert.True(Asset.Path.FolderExists(dirPath));
		}
	}
}
