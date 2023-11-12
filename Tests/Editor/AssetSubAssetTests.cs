// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine.TestTools;

public class AssetSubAssetTests : AssetTestBase
{

	[UnityTest] public IEnumerator AddObject_AddWithoutSave_Succeeds()
	{
		var subObject = Instantiate.ExampleSO();

		{
			var asset = CreateTestAsset(TestAssetPath);
			asset.AddObject(subObject);

			Assert.AreEqual(2, asset.AllObjects.Length);
			Assert.Contains(subObject, asset.AllObjects);

			asset = null;
		}

		yield return null;

		GC.Collect(0, GCCollectionMode.Forced);
		Asset.ImportAll(ImportAssetOptions.ForceUpdate);

		yield return null;

		{
			var asset = (Asset)Asset.Load<UnityEngine.Object>(TestAssetPath);
			Assert.AreEqual(2, asset.AllObjects.Length);
			Assert.Contains(subObject, asset.AllObjects);
		}
	}

	[Test] public void RemoveObject_SubObject_Succeeds()
	{
		var subObject = Instantiate.ExampleSO();
		var asset = CreateTestAsset(TestAssetPath);
		asset.AddObject(subObject);
		Assert.AreEqual(2, asset.AllObjects.Length);

		asset.RemoveObject(subObject);

		Assert.AreEqual(1, asset.AllObjects.Length);
	}
}
