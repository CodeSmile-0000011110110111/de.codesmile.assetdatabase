// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetLoadTests : AssetTestBase
{
	[Test] public void LoadMainStatic_NotExistingPath_Throws() =>
		Assert.Throws<FileNotFoundException>(() => Asset.LoadMain<Object>("Assets/exist.not"));

	[Test] public void LoadMainStatic_ExistingPath_Succeeds()
	{
		var obj = CreateTestAssetObject(TestAssetPath);
		var loaded = Asset.LoadMain<Object>(TestAssetPath);
		Assert.NotNull(loaded);
		Assert.AreEqual(obj, loaded);
		Assert.AreEqual(obj.GetType(), loaded.GetType());
	}

	[Test] public void LoadMainStatic_NotExistingGuid_Throws() =>
		Assert.Throws<ArgumentException>(() => Asset.LoadMain<Object>(new GUID()));

	[Test] public void LoadMainStatic_ExistingGuid_Succeeds()
	{
		var obj = CreateTestAssetObject(TestAssetPath);

		var loaded = Asset.LoadMain<Object>(Asset.Path.GetGuid((String)TestAssetPath));

		Assert.NotNull(loaded);
		Assert.AreEqual(obj, loaded);
		Assert.AreEqual(obj.GetType(), loaded.GetType());
		Assert.AreEqual(obj.GetType(), Asset.MainType((String)TestAssetPath));
	}

	[Test] public void LoadMainStatic_TypeMismatch_Throws()
	{
		CreateTestAssetObject(TestAssetPath);

		// a Material is not assignable from ExampleSO (ScriptableObject) => wrong type
		Assert.Throws<AssetLoadException>(() => Asset.LoadMain<Material>(TestAssetPath));
	}

	[Test] public void LoadMainStatic_AssetDatabasePaused_Throws()
	{
		CreateTestAssetObject(TestAssetPath);

		// NOTE: I could not use Asset.BatchEditing here due to throwing an exception in an Action callback
		// causing the TestRunner progress bar to be stuck at 'Perform Undo' stage.
		try
		{
			AssetDatabase.StartAssetEditing();

			// import while ADB is 'paused' is not possible!
			// this will make the asset 'unloadable' => Load returns null
			Asset.Import(TestAssetPath, ImportAssetOptions.ForceUpdate);
			Assert.Throws<AssetLoadException>(() => Asset.LoadMain<Object>(TestAssetPath));
		}
		finally
		{
			AssetDatabase.StopAssetEditing();
		}
	}
}
