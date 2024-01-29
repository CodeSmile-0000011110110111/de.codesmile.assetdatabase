// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace CodeSmileEditor.Tests
{
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
			Asset.Database.ImportAll(ImportAssetOptions.ForceUpdate);

			yield return null;

			{
				var asset = (Asset)Asset.File.Load<Object>(TestAssetPath);
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

			var returnedLabels = Asset.Label.GetAll(asset.Guid);

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

		[Test] public void RemoveLabel_NoLabels_DoesNothing()
		{
			var asset = CreateTestAsset(TestAssetPath);

			asset.RemoveLabel("wut");

			Assert.IsEmpty(asset.Labels);
		}

		[Test] public void RemoveLabel_OnlyLabel_LabelsAreEmpty()
		{
			var asset = CreateTestAsset(TestAssetPath);
			asset.Labels = new[] { "the one and only" };

			asset.RemoveLabel("the one and only");

			Assert.IsEmpty(asset.Labels);
		}

		[Test] public void RemoveLabel_ThatDoesNotExist_LeavesLabelsUnchanged()
		{
			var asset = CreateTestAsset(TestAssetPath);
			var labels = new[] { "one", "two", "three" };
			asset.Labels = labels;

			asset.RemoveLabel("????");

			Assert.True(asset.Labels.Length == 3);
			Assert.Contains("one", asset.Labels);
			Assert.Contains("two", asset.Labels);
			Assert.Contains("three", asset.Labels);
		}

		[Test] public void RemoveLabel_ThatExists_RemovesTheLabel()
		{
			var asset = CreateTestAsset(TestAssetPath);
			var labels = new[] { "one", "two", "three" };
			asset.Labels = labels;

			asset.RemoveLabel("two");

			Assert.True(asset.Labels.Length == 2);
			Assert.False(asset.Labels.Contains("two"));
			Assert.Contains("one", asset.Labels);
			Assert.Contains("three", asset.Labels);
		}
	}
}
