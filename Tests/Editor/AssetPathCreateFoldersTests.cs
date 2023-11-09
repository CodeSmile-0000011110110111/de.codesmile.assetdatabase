// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using UnityEditor;

public class AssetPathCreateFoldersTests : AssetTestBase
{
	[Test] public void CreateFolders_NullPath_Throws() =>
		Assert.Throws<ArgumentNullException>(() => Asset.Path.CreateFolders(null));

	[TestCase("Assets")]
	[TestCase(TestSubFoldersPath)]
	[TestCase("Assets/some.file")]
	[TestCase(TestSubFoldersPath + "/fn.ext")]
	public void CreateFolders_ExistingFolder_ReturnsExistingFolderGuid(String dirPath)
	{
		// create the folder first
		var assetPath = (Asset.Path)dirPath;
		assetPath.CreateFolders();
		Assert.True(Asset.Path.FolderExists(assetPath.FolderPathAssumptive));

		var folderGuid = assetPath.CreateFolders();

		Assert.False(folderGuid.Empty());
		// we test for "assumptive" folder because some test cases include paths to a non-existing file
		Assert.AreEqual(assetPath.FolderPathAssumptive.Guid, folderGuid);
		Assert.True(Asset.Path.FolderExists(assetPath.FolderPathAssumptive));
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
		Asset.Import(alreadyExistingFolder, ImportAssetOptions.ForceUpdate);
		Assert.True(Asset.Path.FolderExists(alreadyExistingFolder));

		var folderGuid = DeleteAfterTest(Asset.Path.CreateFolders(dirPath + "/some.file"));

		Assert.False(folderGuid.Empty());
		Assert.AreEqual(((Asset.Path)dirPath).Guid, folderGuid);
		Assert.True(Asset.Path.FolderExists(dirPath));
	}
}
