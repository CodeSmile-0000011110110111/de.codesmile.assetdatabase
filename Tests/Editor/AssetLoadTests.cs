﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmileEditor.Tests
{
	public class AssetLoadTests : AssetTestBase
	{
		[Test] public void LoadStatic_NotExistingPath_Null() => Assert.IsNull(Asset.File.Load<Object>("Assets/exist.not"));

		[Test] public void LoadStatic_ExistingPath_Succeeds()
		{
			var obj = CreateTestAssetObject(TestAssetPath);

			var loaded = Asset.File.Load<Object>(TestAssetPath);

			Assert.NotNull(loaded);
			Assert.AreEqual(obj, loaded);
			Assert.AreEqual(obj.GetType(), loaded.GetType());
		}

		[Test] public void LoadMainStatic_NotExistingPath_Throws() =>
			Assert.Throws<FileNotFoundException>(() => Asset.File.LoadMain<Object>("Assets/exist.not"));

		[Test] public void LoadMainStatic_ExistingPath_Succeeds()
		{
			var obj = CreateTestAssetObject(TestAssetPath);

			var loaded = Asset.File.LoadMain<Object>(TestAssetPath);

			Assert.NotNull(loaded);
			Assert.AreEqual(obj, loaded);
			Assert.AreEqual(obj.GetType(), loaded.GetType());
		}

		[Test] public void Load_ImplicitConversion_Succeeds()
		{
			var obj = CreateTestAssetObject(TestAssetPath);

			Asset loaded = TestAssetPath;

			Assert.NotNull(loaded);
			Assert.AreEqual(obj, (Object)loaded);
		}

		[Test] public void Load_ImplicitStringConversion_Succeeds()
		{
			var obj = CreateTestAssetObject(TestAssetPath);

			String stringPath = TestAssetPath;
			Asset loaded = stringPath;

			Assert.NotNull(loaded);
			Assert.AreEqual(obj, (Object)loaded);
		}

		[Test] public void LoadMainStatic_NotExistingGuid_Throws() =>
			Assert.Throws<ArgumentException>(() => Asset.File.LoadMain<Object>(new GUID()));

		[Test] public void LoadMainStatic_ExistingGuid_Succeeds()
		{
			var obj = CreateTestAssetObject(TestAssetPath);

			var loaded = Asset.File.LoadMain<Object>(Asset.Path.GetGuid((String)TestAssetPath));

			Assert.NotNull(loaded);
			Assert.AreEqual(obj, loaded);
			Assert.AreEqual(obj.GetType(), loaded.GetType());
			Assert.AreEqual(obj.GetType(), Asset.File.GetMainType((String)TestAssetPath));
		}

		[Test] public void LoadMainStatic_TypeMismatch_ReturnsNull()
		{
			CreateTestAssetObject(TestAssetPath);

			// a Material is not assignable from ExampleSO (ScriptableObject) => wrong type
			Assert.Null(Asset.File.LoadMain<Material>(TestAssetPath));
		}

		[Test] public void LoadStatic_TypeMismatch_ReturnsNull()
		{
			CreateTestAssetObject(TestAssetPath);

			// a Material is not assignable from ExampleSO (ScriptableObject) => wrong type
			Assert.Null(Asset.File.Load<Material>(TestAssetPath));
		}

		[Test] public void LoadMainStatic_AssetDatabasePaused_ReturnsNull()
		{
			CreateTestAssetObject(TestAssetPath);

			Asset.File.BatchEditing(() =>
			{
				Asset.File.Import(TestAssetPath, ImportAssetOptions.ForceUpdate);

				// import while ADB is 'paused' gets queued, thus the asset cannot be loaded here
				// internally the asset is instantly removed from the assetdatabase
				// loading the queued-for-import asset in the same batch-edit block returns null
				Assert.Null(Asset.File.LoadMain<Object>(TestAssetPath));
			});

			// after batch editing, import has run and we can load the asset
			Assert.NotNull(Asset.File.LoadMain<Object>(TestAssetPath));
		}

		[ExcludeFromCodeCoverage]
		[Test] public void LoadOrCreate_ExistingPath_DoesNotRunCallback()
		{
			var obj = CreateTestAssetObject(TestAssetPath);

			// using the CreateOrLoad alias that routes to LoadOrCreate
			var loaded = Asset.File.CreateOrLoad<ExampleSO>(TestAssetPath, () =>
			{
				// should not run
				Assert.Fail();
				return null;
			});

			Assert.NotNull(loaded);
			Assert.AreEqual(obj, loaded);
		}

		[Test] public void LoadOrCreate_NotExistingPath_RunsCallback()
		{
			var didRunCallback = false;
			Assert.False(TestAssetPath.Exists);

			// using the CreateOrLoad alias that routes to LoadOrCreate
			var loaded = Asset.File.CreateOrLoad(TestAssetPath, () =>
			{
				didRunCallback = true;
				return Instantiate.ExampleSO() as ExampleSO;
			});

			Assert.NotNull(loaded);
			Assert.True(didRunCallback);
			Assert.True(TestAssetPath.Exists);
		}
	}
}
