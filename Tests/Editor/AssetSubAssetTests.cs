// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor;
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

			Assert.AreEqual(2, asset.SubObjects.Length);
			Assert.Contains(subObject, asset.SubObjects);

			asset = null;
		}

		yield return null;

		GC.Collect(0, GCCollectionMode.Forced);
		Asset.ImportAll(ImportAssetOptions.ForceUpdate);

		yield return null;

		{
			var asset = (Asset)Asset.Load<Object>(TestAssetPath);
			Assert.AreEqual(2, asset.SubObjects.Length);
			Assert.AreEqual(0, asset.VisibleSubObjects.Length);
			Assert.Contains(subObject, asset.SubObjects);
		}
	}

	[Test] public void RemoveObject_SubObject_Succeeds()
	{
		var subObject = Instantiate.ExampleSO();
		var asset = CreateTestAsset(TestAssetPath);
		asset.AddObject(subObject);
		Assert.AreEqual(2, asset.SubObjects.Length);

		asset.RemoveObject(subObject);

		Assert.AreEqual(1, asset.SubObjects.Length);
	}

	[Test] public void AllObjects_SingleAsset_ReturnsOne()
	{
		var asset = CreateTestAsset(TestAssetPath);

		Assert.AreEqual(1, asset.SubObjects.Length);
	}

	[Test] public void VisibleObjects_SingleAsset_ReturnsZero()
	{
		var asset = CreateTestAsset(TestAssetPath);

		Assert.AreEqual(0, asset.VisibleSubObjects.Length);
	}
}
