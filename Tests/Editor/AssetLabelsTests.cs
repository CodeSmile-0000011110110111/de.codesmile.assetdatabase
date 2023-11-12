// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class AssetLabelsTests : AssetTestBase
{
	[Test] public void Labels_NewAsset_EmptyArray()
	{
		var asset = CreateTestAsset(TestAssetPath);

		var labels = asset.Labels;

		Assert.NotNull(labels);
		Assert.AreEqual(0, labels.Length);
	}

	[Test] public void Labels_SetGet_ReturnsSetLabels()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var labels = new[] { "one", "two", "three" };

		asset.Labels = labels;

		Assert.AreEqual(3, asset.Labels.Length);
		Assert.Contains("one", asset.Labels);
		Assert.Contains("two", asset.Labels);
		Assert.Contains("three", asset.Labels);
	}

	[UnityTest] public IEnumerator Labels_SetAndReload_ReturnsSetLabels()
	{
		var labels = new[] { "one", "two", "three" };

		{
			var asset = CreateTestAsset(TestAssetPath);
			asset.Labels = labels;
			asset = null;
		}

		yield return null;

		GC.Collect(0, GCCollectionMode.Forced);
		Asset.ImportAll(ImportAssetOptions.ForceUpdate);

		yield return null;

		{
			var asset = (Asset)Asset.Load<Object>(TestAssetPath);
			Assert.AreEqual(3, asset.Labels.Length);
			Assert.Contains("one", asset.Labels);
			Assert.Contains("two", asset.Labels);
			Assert.Contains("three", asset.Labels);
		}
	}

	[Test] public void GetLabels_ByGuid_ReturnsSetLabels()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var labels = new[] { "one", "two", "three" };
		asset.Labels = labels;

		var returnedLabels = Asset.GetLabels(asset.Guid);

		Assert.AreEqual(3, returnedLabels.Length);
		Assert.Contains("one", returnedLabels);
		Assert.Contains("two", returnedLabels);
		Assert.Contains("three", returnedLabels);
	}

	[Test] public void ClearLabels_ReturnsEmpty()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var labels = new[] { "one", "two", "three" };
		asset.Labels = labels;

		asset.ClearLabels();

		Assert.NotNull(asset.Labels);
		Assert.AreEqual(0, asset.Labels.Length);
	}

	[Test] public void AddLabel_ToEmpty_ReturnsLabel()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var label = "label";

		asset.AddLabel(label);

		Assert.AreEqual(1, asset.Labels.Length);
		Assert.Contains("label", asset.Labels);
	}

	[Test] public void AddLabels_ToEmpty_ReturnsLabels()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var labels = new[] { "one", "two", "three" };

		asset.AddLabels(labels);

		Assert.Contains("one", asset.Labels);
		Assert.Contains("two", asset.Labels);
		Assert.Contains("three", asset.Labels);
	}

	[Test] public void AddLabel_ToExisting_ReturnsCombinedLabels()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var labels = new[] { "one", "two" };
		asset.Labels = labels;

		var label = "three";
		asset.AddLabel(label);

		Assert.Contains("one", asset.Labels);
		Assert.Contains("two", asset.Labels);
		Assert.Contains(label, asset.Labels);
	}

	[Test] public void AddLabels_ToExisting_ReturnsCombinedLabels()
	{
		var asset = CreateTestAsset(TestAssetPath);
		var labels = new[] { "one", "two" };
		asset.Labels = labels;

		asset.AddLabels(new[] { "three", "four" });

		Assert.Contains("one", asset.Labels);
		Assert.Contains("two", asset.Labels);
		Assert.Contains("three", asset.Labels);
		Assert.Contains("four", asset.Labels);
	}
}
