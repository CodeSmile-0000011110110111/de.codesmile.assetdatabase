// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		// Hidden parameterless ctor
		private Asset() {}

		/// <summary>
		///     Create an asset instance from a path. The path must point to an existing asset file.
		/// </summary>
		/// <param name="assetPath"></param>
		/// <exception cref="ArgumentNullException">If the assetPath is null.</exception>
		/// <exception cref="FileNotFoundException">If the assetPath does not point to an existing asset file.</exception>
		public Asset(AssetPath assetPath)
		{
			ThrowIf.ArgumentIsNull(assetPath, nameof(assetPath));
			ThrowIf.FileDoesNotExist(assetPath);

			SetMainObjectAndPath(assetPath);
		}

		/// <summary>
		///     Create an asset instance from a path. The path must point to an existing asset file.
		/// </summary>
		/// <param name="assetPath"></param>
		/// <exception cref="ArgumentNullException">If the assetPath is null.</exception>
		/// <exception cref="FileNotFoundException">If the assetPath does not point to an existing asset file.</exception>
		public Asset(String path) : this(new AssetPath(path)) {}

		/// <summary>
		///     Create an asset instance from a UnityEngine.Object.
		///     The Object must be an asset reference. Use the static Create() method to create an asset.
		/// </summary>
		/// <param name="obj"></param>
		/// <exception cref="ArgumentNullException">If the object is null.</exception>
		/// <exception cref="ArgumentException">If the object is not an asset reference.</exception>
		public Asset(Object obj)
		{
			ThrowIf.ArgumentIsNull(obj, nameof(obj));
			ThrowIf.NotAnAsset(obj);

			SetMainObjectAndPath(obj);
		}

		/// <summary>
		///     Create an asset instance from a GUID.
		///     The GUID must be for an existing asset.
		/// </summary>
		/// <param name="assetGuid"></param>
		public Asset(GUID assetGuid)
		{
			ThrowIf.NotAnAsset(assetGuid);

			SetMainObjectAndPath(assetGuid);
			m_AssetGuid = assetGuid;
		}

		private void SetMainObjectAndPath(AssetPath assetPath)
		{
			m_AssetPath = assetPath;
			m_MainObject = Load<Object>();
		}

		private void SetMainObjectAndPath(Object obj)
		{
			m_MainObject = obj;
			m_AssetPath = (AssetPath)AssetDatabase.GetAssetPath(obj);
		}

		private void SetMainObjectAndPath(GUID guid) =>
			SetMainObjectAndPath((AssetPath)AssetDatabase.GUIDToAssetPath(guid));
	}
}
