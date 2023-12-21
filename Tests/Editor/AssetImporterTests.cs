// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;
using UnityEditor;
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

			// ".test" has no default importer, so the first importer becomes the default
			Assert.False(asset.IsImporterOverridden);
			Assert.NotZero(asset.AvailableImporters.Length);
			Assert.AreEqual(typeof(TestImporter), asset.ActiveImporter);
		}

		[Test] public void ActiveImporter_OBJAssignOverride_ReturnsOverride()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.overrideobj"); // note the extension
			var asset = new Asset("irrelevant", path, true);

			asset.ActiveImporter = typeof(ObjTestOverrideImporter);

			Assert.AreEqual(typeof(ObjTestOverrideImporter), asset.ActiveImporter);
		}

		[Test] public void Importer_GetAvailable_NotEmpty()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.test");
			var asset = new Asset("no content", path, true);

			Assert.NotZero(asset.AvailableImporters.Length);
			Assert.NotNull(asset.DefaultImporter);
		}

		[Test] public void ActiveImporter_AssignNull_ReturnsDefault()
		{
			var path = DeleteAfterTest((Asset.Path)$"Assets/{TestAssetFileName}.test");
			var asset = new Asset("no content", path, true);

			var defaultImporter = asset.DefaultImporter;

			asset.ActiveImporter = typeof(TestImporter);
			asset.ActiveImporter = null;

			Assert.NotNull(defaultImporter);
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

		[ScriptedImporter(1, new[] { "testfbx" }, new[] { "fbx" })]
		public class FbxTestImporter : ScriptedImporter
		{
			public override void OnImportAsset(AssetImportContext ctx) {}
		}

		[ScriptedImporter(1, new[] { "testobj" }, new[] { "obj" })]
		public class ObjTestImporter : ScriptedImporter
		{
			public override void OnImportAsset(AssetImportContext ctx) {}
		}

		[ScriptedImporter(1, new[] { "overrideobj" }, new[] { "obj", "testobj" })]
		public class ObjTestOverrideImporter : ScriptedImporter
		{
			public override void OnImportAsset(AssetImportContext ctx) {}
		}
	}
}
