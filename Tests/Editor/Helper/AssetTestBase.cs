// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmileEditor.Tests.Helper
{
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

			DeleteAfterTest(TestAssetPath); // always try to delete the test asset
			DeleteTestFiles();

			Asset.File.Delete(TestSubFolderPath);
			Asset.File.Delete(TestSubFoldersPath);
			Asset.File.Delete(Path.GetDirectoryName(TestSubFoldersPath));
			Asset.File.Delete(Path.GetDirectoryName(Path.GetDirectoryName(TestSubFoldersPath)));
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
				Asset.Database.ImportAll(ImportAssetOptions.ForceUpdate);
		}

		[ExcludeFromCodeCoverage]
		protected Asset.Path DeleteAfterTest(Asset.Path filePath)
		{
			m_TestFilePaths.Add(filePath);
			return filePath;
		}

		protected String DeleteAfterTest(String filePath)
		{
			m_TestFilePaths.Add(filePath);
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
			var obj = Asset.File.LoadMain<Object>(assetGuid);
			m_TestAssets.Add(obj);
			return assetGuid;
		}

		protected Object CreateTestAssetObject() => CreateTestAssetObject(TestAssetPath);

		protected Object CreateTestAssetObject(String path) =>
			DeleteAfterTest(Asset.File.Create(Instantiate.ExampleSO(), path));

		protected Asset CreateTestAsset(String path) =>
			new(DeleteAfterTest(Asset.File.Create(Instantiate.ExampleSO(), path)));
	}
}
