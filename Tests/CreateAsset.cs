// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using DefaultNamespace;
using Helper;
using NUnit.Framework;
using System;
using UnityEditor;

public class CreateAsset : AssetTestBase
{
	[Test] public void CreateNewAsset_NullPath_Throws() =>
		Assert.Throws<ArgumentNullException>(() => AssetDB.CreateNewAsset(Instantiate.ExampleSO(), null));

	[Test] public void CreateNewAsset_WhitespacePath_Throws() =>
		Assert.Throws<ArgumentException>(() => AssetDB.CreateNewAsset(Instantiate.ExampleSO(), " "));

	[Test] public void CreateNewAsset_FileExists()
	{
		var obj = Instantiate.ExampleSO();
		obj.name = $"New {nameof(ExampleSO)}";
		var path = AssetDB.CombineAssetPath(ExamplePath, obj.name);

		var asset = DeleteAfterTest(AssetDB.CreateNewAsset(obj, path));

		Assert.That(Asset.FileExists(asset));
	}

	[Test] public void CreateNewAssets_SameName_NoOverwrite()
	{
		var obj1 = Instantiate.ExampleSO();
		var obj2 = Instantiate.ExampleSO();
		var path = AssetDB.CombineAssetPath(ExamplePath, nameof(ExampleSO));

		var asset1 = DeleteAfterTest(AssetDB.CreateNewAsset(obj1, path));
		var asset2 = DeleteAfterTest(AssetDB.CreateNewAsset(obj2, path));

		Assert.That(Asset.FileExists(asset1));
		Assert.That(Asset.FileExists(asset2));
	}

	[Test] public void CreateOrReplaceAsset_FileExists()
	{
		var obj = Instantiate.ExampleSO();
		obj.name = $"New {nameof(ExampleSO)}";
		var path = AssetDB.CombineAssetPath(ExamplePath, obj.name);

		var asset = DeleteAfterTest(AssetDB.CreateOrReplaceAsset(obj, path));

		Assert.That(Asset.FileExists(asset));
	}

	[Test] public void CreateOrReplaceAssets_SameName_Overwrites()
	{
		var obj1 = Instantiate.ExampleSO();
		var obj2 = Instantiate.ExampleSO();
		var path = AssetDB.CombineAssetPath(ExamplePath, nameof(ExampleSO));

		var asset1 = DeleteAfterTest(AssetDB.CreateOrReplaceAsset(obj1, path));
		var asset2 = DeleteAfterTest(AssetDB.CreateOrReplaceAsset(obj2, path));

		Assert.That(String.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj1)));
		Assert.That(path == AssetDatabase.GetAssetPath(obj2));
		Assert.That(Asset.FileExists(asset1) == false);
		Assert.That(Asset.FileExists(asset2));
	}
}
