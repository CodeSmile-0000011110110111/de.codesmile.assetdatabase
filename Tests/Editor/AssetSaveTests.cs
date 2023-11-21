// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using UnityEditor;

public class AssetSaveTests : AssetTestBase
{
	[Test] public void SaveObjectStatic_Null_Throws() => Assert.Throws<ArgumentNullException>(() => Asset.File.Save(null));

	[Test] public void SaveObjectStatic_NotAnAsset_Throws()
	{
		var obj = Instantiate.ExampleSO();
		Assert.Throws<ArgumentException>(() => Asset.File.Save(obj));
	}

	[Test] public void Save_ModifiedAssetWithoutDirty_FileSizeUnchanged()
	{
		var soAsset = CreateTestAsset(TestAssetPath);
		var fileSize = AssetHelper.GetFileSize(TestAssetPath);

		// changing the field does not 'dirty' the object, thus won't save it
		(soAsset.MainObject as ExampleSO).Text = "Not so dirty!";
		soAsset.Save();

		// change hasn't been 'saved'
		Assert.AreEqual(fileSize, AssetHelper.GetFileSize(TestAssetPath));
	}

	[Test] public void ForceSave_ModifiedAsset_FileSizeChanged()
	{
		var soAsset = CreateTestAsset(TestAssetPath);
		var fileSize = AssetHelper.GetFileSize(TestAssetPath);

		(soAsset.MainObject as ExampleSO).Text = "Soooo dirty!";
		soAsset.ForceSave(); // ForceSave dirties the object before saving

		Assert.AreNotEqual(fileSize, AssetHelper.GetFileSize(TestAssetPath));
	}

	[Test] public void SaveAllStatic_ModifiedAsset_FileSizeChanged()
	{
		var soAsset = CreateTestAsset(TestAssetPath);
		var fileSize = AssetHelper.GetFileSize(TestAssetPath);

		(soAsset.MainObject as ExampleSO).Text = "Soooo dirty!";
		soAsset.SetDirty(); // dirty it manually because SaveAll has no 'force' variant
		Asset.File.SaveAll();

		Assert.AreNotEqual(fileSize, AssetHelper.GetFileSize(TestAssetPath));
	}

	[Test] public void SaveGuidStatic_Empty_Throws() => Assert.Throws<ArgumentException>(() => Asset.File.Save(new GUID()));

	[Test] public void SaveGuidStatic_NotAnAsset_Throws() =>
		Assert.Throws<ArgumentException>(() => Asset.File.Save(GUID.Generate()));

	[Test] public void SaveGuidStatic_ModifiedAsset_FileSizeChanged()
	{
		var soAsset = CreateTestAsset(TestAssetPath);
		var fileSize = AssetHelper.GetFileSize(TestAssetPath);

		// changing the field does not 'dirty' the object, thus won't save it
		(soAsset.MainObject as ExampleSO).Text = "Not so dirty!";
		soAsset.SetDirty();
		Asset.File.Save(soAsset.Guid);

		Assert.AreNotEqual(fileSize, AssetHelper.GetFileSize(TestAssetPath));
	}
}
