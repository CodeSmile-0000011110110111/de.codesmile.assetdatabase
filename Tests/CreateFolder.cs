// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using DefaultNamespace;
using NUnit.Framework;
using System;
using UnityEditor;
using UnityEngine.Windows;

public class CreateFolder : AssetTestBase
{
	[Test] public void CreateFolder_NullPath_Throws() =>
		Assert.Throws<ArgumentNullException>(() => AssetDB.CreateFolder(null));

	[Test] public void CreateFolder_EmptyPath_Throws() =>
		Assert.Throws<ArgumentException>(() => AssetDB.CreateFolder(String.Empty));

	[Test] public void CreateFolder_WhitespacePath_Throws() =>
		Assert.Throws<ArgumentException>(() => AssetDB.CreateFolder("  "));

	[Test] public void CreateFolder_AssetsPath_Exists()
	{
		var folderGuid = AssetDB.CreateFolder("Assets");

		Assert.AreEqual(AssetDatabase.GUIDToAssetPath(folderGuid), "Assets");
	}

	[Test] public void CreateFolder_PathWithTrailingSeparator_ReturnsEmptyGuid() =>
		Assert.True(AssetDB.CreateFolder("Assets/").Empty());

	[Test] public void CreateFolderRecursive_NotExistingSubFolder_GetsCreated()
	{
		var folder = "-create_subfolder_test-";
		var path = $"Assets/{folder}";
		var folderGuid = AssetDB.CreateFolder(path);

		Assert.AreEqual(AssetDatabase.GUIDToAssetPath(folderGuid), path);
		Assert.True(Directory.Exists(path));

		AssetDatabase.DeleteAsset(path);
	}

	[Test] public void CreateFolder_RecursiveSubFolders_GetCreated()
	{
		var folder = "-create_subfolder_test-";
		var path = $"Assets/{folder}/{folder}/{folder}";
		var folderGuid = AssetDB.CreateFolder(path);

		Assert.AreEqual(AssetDatabase.GUIDToAssetPath(folderGuid), path);
		Assert.True(Directory.Exists(path));

		AssetDatabase.DeleteAsset($"Assets/{folder}/{folder}/{folder}");
		AssetDatabase.DeleteAsset($"Assets/{folder}/{folder}");
		AssetDatabase.DeleteAsset($"Assets/{folder}");
	}
}
