// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace CodeSmileEditor.Tests
{
	public class AssetImporterTests : AssetTestBase
	{
		[Test] public void ActiveImporter_AssignOverride_ReturnsOverride()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.test");
			var asset = new Asset("no content", path, true);

			asset.ActiveImporter = typeof(TestImporter);

			Assert.AreEqual(typeof(TestImporter), asset.ActiveImporter);
		}

		[Test] public void ActiveImporter_AssignNull_ReturnsDefault()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.test");
			var asset = new Asset("no content", path, true);

			var defaultImporter = asset.DefaultImporter;

			asset.ActiveImporter = typeof(TestImporter);
			asset.ActiveImporter = null;

			Assert.AreEqual(defaultImporter, asset.ActiveImporter);
		}

		[ScriptedImporter(1, "test")]
		public class TestImporter : ScriptedImporter
		{
			public override void OnImportAsset(AssetImportContext ctx)
			{
				//Debug.Log($"OnImportAsset: {ctx}");
			}
		}
	}
}
