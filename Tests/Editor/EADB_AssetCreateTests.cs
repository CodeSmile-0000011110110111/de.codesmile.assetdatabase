// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using UnityEditor;

public class EADB_AssetCreateTests : AssetTestBase
{
	[Test] public void Create_NewAsset_DidCreate()
	{
		var obj = Instantiate.ExampleSO();
		obj.name = $"New {nameof(ExampleSO)}";

		var asset = DeleteAfterTest(AssetOld.Create(obj, (AssetPath)$"{ExamplePath}/{obj.name}.asset"));

		Assert.That(AssetHelper.FileExists(asset));
	}

	[Test] public void Create_NewAssetWithStringPath_DidCreate()
	{
		var obj = Instantiate.ExampleSO();
		obj.name = $"New {nameof(ExampleSO)}";

		var asset = DeleteAfterTest(AssetOld.Create(obj, $"{ExamplePath}/{obj.name}.asset"));

		Assert.That(AssetHelper.FileExists(asset));
	}

	[Test] public void Create_TwiceWithSameName_CreatesTwoAssets()
	{
		var obj1 = Instantiate.ExampleSO();
		var obj2 = Instantiate.ExampleSO();

		var path = new AssetPath(ExamplePath, nameof(ExampleSO));
		var asset1 = DeleteAfterTest(AssetOld.Create(obj1, path));
		var asset2 = DeleteAfterTest(AssetOld.Create(obj2, path));

		Assert.That(AssetHelper.FileExists(asset1));
		Assert.That(AssetHelper.FileExists(asset2));
	}

	[Test] public void Create_ReplaceExisting_DidReplace()
	{
		var obj = Instantiate.ExampleSO();
		obj.name = $"New {nameof(ExampleSO)}";

		var path = new AssetPath(ExamplePath, obj.name);
		var asset = DeleteAfterTest(AssetOld.Create(obj, path, true));

		Assert.That(AssetHelper.FileExists(asset));
	}

	[Test] public void Create_TwiceWithSameName_DidReplaceExisting()
	{
		var obj1 = Instantiate.ExampleSO();
		var obj2 = Instantiate.ExampleSO();

		var path = new AssetPath(ExamplePath, nameof(ExampleSO));
		var asset1 = DeleteAfterTest(AssetOld.Create(obj1, path, true));
		var asset2 = DeleteAfterTest(AssetOld.Create(obj2, path, true));

		Assert.That(String.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj1)));
		Assert.That(path == AssetDatabase.GetAssetPath(obj2));
		Assert.That(AssetHelper.FileExists(asset1) == false);
		Assert.That(AssetHelper.FileExists(asset2));
	}
}
