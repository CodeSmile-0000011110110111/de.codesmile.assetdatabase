// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

public abstract class AssetTestBase
{
	protected const String TestSubFoldersPath = "Assets/sub/fol/der";
	protected readonly AssetPath ExamplePath = new("Assets/Examples/");
	protected readonly AssetPath TestAssetPath = new("Assets/test.asset");
	protected const String TestSubFolderPath = ("Assets/subfolder");

	private readonly TestAssets m_TestAssets = new();

	public AssetTestBase() => Asset.Path.CreateFolders(ExamplePath);

	[TearDown] public void TearDown()
	{
		Assert.DoesNotThrow(m_TestAssets.Dispose);
		AssetDatabase.DeleteAsset(TestSubFolderPath);
		AssetDatabase.DeleteAsset(TestSubFoldersPath);
		AssetDatabase.DeleteAsset(Path.GetDirectoryName(TestSubFoldersPath));
		AssetDatabase.DeleteAsset(Path.GetDirectoryName(Path.GetDirectoryName(TestSubFoldersPath)));
	}

	protected Object DeleteAfterTest(Object asset)
	{
		m_TestAssets.Add(asset);
		return asset;
	}

	protected GUID DeleteAfterTest(GUID guid)
	{
		var obj = Asset.LoadMain<Object>(guid);
		m_TestAssets.Add(obj);
		return guid;
	}

	protected Object CreateTestAsset(String path) => DeleteAfterTest(AssetOld.Create(Instantiate.ExampleSO(), path));
}
