// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

public sealed class TestAssets : IDisposable
{
	private readonly HashSet<Object> m_TestAssets = new();

	public void Dispose() => DeleteTempAssets();

	private void DeleteTempAssets()
	{
		if (m_TestAssets != null)
		{
			foreach (var asset in m_TestAssets)
			{
				var path = AssetDatabase.GetAssetPath(asset);
				if (String.IsNullOrEmpty(path))
					continue;

				AssetDatabase.DeleteAsset(path);
			}

			m_TestAssets.Clear();
		}
	}

	public void Add(Object asset)
	{
		if (asset == null)
			throw new ArgumentNullException(nameof(asset), "cannot add null asset");

		m_TestAssets.Add(asset);
	}
}
