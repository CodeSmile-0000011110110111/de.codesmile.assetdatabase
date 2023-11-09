// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Object = UnityEngine.Object;

public sealed class TestAssets : IDisposable
{
	private readonly HashSet<Object> m_AssetObjects = new();

	public void Dispose() => DeleteTempAssets();

	private void DeleteTempAssets()
	{
		if (m_AssetObjects != null)
		{
			foreach (var obj in m_AssetObjects)
				Asset.Delete(obj);

			m_AssetObjects.Clear();
		}
	}

	[ExcludeFromCodeCoverage]
	public void Add(Object asset)
	{
		if (asset == null)
			throw new ArgumentNullException(nameof(asset), "cannot add null asset");

		m_AssetObjects.Add(asset);
	}
}
