// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CodeSmileAssetDatabaseExamples
{
	private const String TestFolder = "Assets/_sub_folder_";

	/// <summary>
	///     For more usage examples please refer to the numerous Unit Tests:
	///     Packages/CodeSmile AssetDatabase/Tests/Editor/*.cs
	/// </summary>
	/// <remarks>API reference: https://codesmile-0000011110110111.github.io/de.codesmile.assetdatabase</remarks>
	[MenuItem("Window/CodeSmile/AssetDatabase Examples/Create and Delete Asset")]
	private static void CreateAndDeleteAsset()
	{
		var path = $"{TestFolder}/TestAsset.asset";
		var asset = new Asset(ScriptableObject.CreateInstance<TestAsset>(), path);

		var instance = asset.MainObject;
		var assetPath = asset.AssetPath;
		Debug.Log($"Asset '{instance}' created at '{assetPath}'");

		var message = asset.GetMain<TestAsset>().Message;
		Debug.Log($"Asset says: '{message}'");

		asset.Delete();
		Debug.Log($"Asset deleted: {asset.IsDeleted}");

		// a folder is an asset, too!
		var folder = new Asset(TestFolder);
		folder.Delete();

		Debug.Log($"Testfolder removed: {!Directory.Exists(TestFolder)}");
	}
}
