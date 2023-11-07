// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

public abstract class AssetTestBase
{
	protected const String TestSubFolderPath = "Assets/subfolder";
	protected const String TestSubFoldersPath = "Assets/sub/fol/der";
	protected const String TestAssetFileName = "°CodeSmile-UnitTestAsset°.asset";
	protected readonly Asset.Path TestAssetPath = new($"Assets/{TestAssetFileName}");
	protected readonly Asset.Path TestSubFoldersAssetPath = new($"{TestSubFoldersPath}/{TestAssetFileName}");

	private readonly TestAssets m_TestAssets = new();
	private readonly List<Asset.Path> m_TestFilePaths = new();

	[TearDown] public void TearDown()
	{
		Assert.DoesNotThrow(m_TestAssets.Dispose);
		DeleteTestFiles();

		Asset.Delete(TestSubFolderPath);
		Asset.Delete(TestSubFoldersPath);
		Asset.Delete(Path.GetDirectoryName(TestSubFoldersPath));
		Asset.Delete(Path.GetDirectoryName(Path.GetDirectoryName(TestSubFoldersPath)));
	}

	[ExcludeFromCodeCoverage]
	private void DeleteTestFiles()
	{
		var didDelete = false;
		foreach (var filePath in m_TestFilePaths)
		{
			if (filePath != null && File.Exists(filePath))
			{
				didDelete = true;
				File.Delete(filePath);

				var metaFilePath = filePath + ".meta";
				if (File.Exists(metaFilePath))
					File.Delete(metaFilePath);
			}
		}
		m_TestFilePaths.Clear();

		if (didDelete)
			Asset.ImportAll(ImportAssetOptions.ForceUpdate);
	}

	[ExcludeFromCodeCoverage]
	protected Asset.Path DeleteFileAfterTest(Asset.Path filePath)
	{
		m_TestFilePaths.Add(filePath);
		return filePath;
	}

	protected String DeleteFileAfterTest(String filePath)
	{
		m_TestFilePaths.Add((Asset.Path)filePath);
		return filePath;
	}

	protected Asset DeleteAfterTest(Asset asset)
	{
		m_TestAssets.Add(asset.MainObject);
		return asset;
	}

	protected Object DeleteAfterTest(Object assetObject)
	{
		m_TestAssets.Add(assetObject);
		return assetObject;
	}

	protected GUID DeleteAfterTest(GUID assetGuid)
	{
		var obj = Asset.LoadMain<Object>(assetGuid);
		m_TestAssets.Add(obj);
		return assetGuid;
	}

	protected Object CreateTestAsset() => CreateTestAsset(TestAssetPath);

	protected Object CreateTestAsset(String path) =>
		DeleteAfterTest(Asset.Create(Instantiate.ExampleSO(), path).MainObject);
}
