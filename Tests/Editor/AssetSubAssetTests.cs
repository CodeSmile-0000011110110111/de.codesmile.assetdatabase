// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class AssetSubAssetTests : AssetTestBase
{
	// It is my understanding that 'saving' according to the docs for AddObjectToAsset
	// is only an issue when adding the object to an asset path rather than an asset object
	// this test confirms this hypothesis
	[UnityTest] public IEnumerator AddObject_AddWithoutSave_Succeeds()
	{
		var subObject = Instantiate.ExampleSO();

		{
			var asset = CreateTestAsset(TestAssetPath);
			asset.AddObject(subObject);

			Assert.AreEqual(2, asset.SubAssets.Length);
			Assert.Contains(subObject, asset.SubAssets);

			asset = null;
		}

		yield return null;

		GC.Collect(0, GCCollectionMode.Forced);
		Asset.Database.ImportAll(ImportAssetOptions.ForceUpdate);

		yield return null;

		{
			var asset = (Asset)Asset.File.Load<Object>(TestAssetPath);
			Assert.AreEqual(2, asset.SubAssets.Length);
			Assert.AreEqual(0, asset.VisibleSubAssets.Length);
			Assert.Contains(subObject, asset.SubAssets);
		}
	}

	[Test] public void RemoveObject_SubObject_Succeeds()
	{
		var subObject = Instantiate.ExampleSO();
		var asset = CreateTestAsset(TestAssetPath);
		asset.AddObject(subObject);
		Assert.AreEqual(2, asset.SubAssets.Length);

		asset.RemoveObject(subObject);

		Assert.AreEqual(1, asset.SubAssets.Length);
	}

	[Test] public void AllObjects_SingleAsset_ReturnsOne()
	{
		var asset = CreateTestAsset(TestAssetPath);

		Assert.AreEqual(1, asset.SubAssets.Length);
	}

	[Test] public void VisibleObjects_SingleAsset_ReturnsZero()
	{
		var asset = CreateTestAsset(TestAssetPath);

		Assert.AreEqual(0, asset.VisibleSubAssets.Length);
	}

	[Test] public void AddObject_SetSubObjectAsMain_LoadSucceeds()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var subObject = Instantiate.DifferentExampleSO();
		asset.AddObject(subObject);

		// check if the main object gets loaded after changing it
		asset.MainObject = subObject;
		var differentExampleSo = asset.Load<DifferentExampleSO>();

		Assert.AreEqual(subObject, differentExampleSo);
		Assert.AreEqual(subObject, asset.MainObject);
		Assert.AreEqual(subObject, Asset.File.LoadMain<DifferentExampleSO>(asset.AssetPath));
	}

	[Test] public void SetMainObjectStatic_SetSubObjectAsMain_LoadSucceeds()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var subObject = Instantiate.DifferentExampleSO();
		asset.AddObject(subObject);

		// using the static should also reflect the change on instances
		Asset.SubAsset.SetMain(subObject, asset);
		var differentExampleSo = Asset.File.LoadMain<Object>(asset.AssetPath);

		Assert.AreEqual(subObject, differentExampleSo);
		Assert.AreEqual(subObject, asset.MainObject);
		Assert.AreEqual(subObject, Asset.File.LoadMain<DifferentExampleSO>(asset.AssetPath));
	}

	[Test] public void AddObject_SetNonAssetObjectAsMain_Throws()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var subObject = Instantiate.DifferentExampleSO();

		Assert.Throws<UnityException>(() => asset.MainObject = subObject);
	}
}
