// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;

public class AssetPathCreateFolderTests : AssetTestBase
{
	[Test] public void CreateFolders_NullPath_Throws() =>
		Assert.Throws<ArgumentNullException>(() => Asset.Path.CreateFolders(null));

	[TestCase("Assets")]
	[TestCase("Assets/some.file")]
	[TestCase(TestSubFolderPath + "/")]
	[TestCase(TestSubFoldersPath)]
	[TestCase(TestSubFoldersPath + "/fn.ext")]
	public void CreateFolders_ExistingFolder_ReturnsExistingFolderGuid(String dirPath)
	{
		// create the folder first
		var assetPath = (AssetPath)dirPath;
		Asset.Path.CreateFolders(assetPath);

		var folderGuid = Asset.Path.CreateFolders(assetPath);

		Assert.False(folderGuid.Empty());
		Assert.AreEqual(Asset.Guid.Get(assetPath.FolderPath), folderGuid);
		Assert.True(Asset.Path.FolderExists(assetPath));
	}

	[Test] public void CreateFolders_CreateValidPath_FolderExists()
	{
		var dirPath = TestSubFolderPath;

		var folderGuid = DeleteAfterTest(Asset.Path.CreateFolders((AssetPath)(dirPath + "/some.file")));

		Assert.False(folderGuid.Empty());
		Assert.AreEqual(Asset.Guid.Get(dirPath), folderGuid);
		Assert.True(Asset.Path.FolderExists((AssetPath)dirPath));
	}

	[Test] public void CreateFolders_CreateFoldersRecursive_FolderExists()
	{
		var dirPath = TestSubFoldersPath;

		var folderGuid = DeleteAfterTest(Asset.Path.CreateFolders((AssetPath)(dirPath + "/some.file")));

		Assert.False(folderGuid.Empty());
		Assert.AreEqual(Asset.Guid.Get(dirPath), folderGuid);
		Assert.True(Asset.Path.FolderExists((AssetPath)dirPath));
	}
}
