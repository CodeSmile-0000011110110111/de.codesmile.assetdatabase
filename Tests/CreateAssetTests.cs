// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using DefaultNamespace;
using Helper;
using NUnit.Framework;
using System;
using UnityEditor;
using Asset = CodeSmile.Editor.Asset;
using Object = UnityEngine.Object;

public class CreateAssetTests : AssetTestBase
{
	[Test] public void Create_NullPath_Throws() =>
		Assert.Throws<ArgumentNullException>(() => Asset.Create(Instantiate.ExampleSO(), null));

	[Test] public void Create_WhitespacePath_Throws() =>
		Assert.Throws<ArgumentException>(() => Asset.Create(Instantiate.ExampleSO(), " "));

	[Test] public void Create_NewAsset_DidCreate()
	{
		var obj = Instantiate.ExampleSO();
		obj.name = $"New {nameof(ExampleSO)}";
		var path = Asset.Path.Combine(ExamplePath, obj.name);

		var asset = DeleteAfterTest(Asset.Create(obj, path));

		Assert.That(Helper.Asset.FileExists(asset));
	}

	[Test] public void Create_TwiceWithSameName_CreatesTwoAssets()
	{
		var obj1 = Instantiate.ExampleSO();
		var obj2 = Instantiate.ExampleSO();
		var path = Asset.Path.Combine(ExamplePath, nameof(ExampleSO));

		var asset1 = DeleteAfterTest(Asset.Create(obj1, path));
		var asset2 = DeleteAfterTest(Asset.Create(obj2, path));

		Assert.That(Helper.Asset.FileExists(asset1));
		Assert.That(Helper.Asset.FileExists(asset2));
	}

	[Test] public void Create_ReplaceExisting_DidReplace()
	{
		var obj = Instantiate.ExampleSO();
		obj.name = $"New {nameof(ExampleSO)}";
		var path = Asset.Path.Combine(ExamplePath, obj.name);

		var asset = DeleteAfterTest(Asset.Create(obj, path, true));

		Assert.That(Helper.Asset.FileExists(asset));
	}

	[Test] public void Create_TwiceWithSameName_DidReplaceExisting()
	{
		var obj1 = Instantiate.ExampleSO();
		var obj2 = Instantiate.ExampleSO();
		var path = Asset.Path.Combine(ExamplePath, nameof(ExampleSO));

		var asset1 = DeleteAfterTest(Asset.Create(obj1, path, true));
		var asset2 = DeleteAfterTest(Asset.Create(obj2, path, true));

		Assert.That(String.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj1)));
		Assert.That(path == AssetDatabase.GetAssetPath(obj2));
		Assert.That(Helper.Asset.FileExists(asset1) == false);
		Assert.That(Helper.Asset.FileExists(asset2));
	}
}
