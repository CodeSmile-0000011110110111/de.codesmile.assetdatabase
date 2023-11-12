// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		[ExcludeFromCodeCoverage] private Asset() {} // disallowed parameterless ctor

		/// <summary>
		///     Returns an instance by creating (saving) the object as an asset file at the given path.
		///     The object must not already be an asset. If it is an asset you must use the ctor without a path.
		/// </summary>
		/// <param name="obj">The object to create/save as asset file.</param>
		/// <param name="path">The path to the asset file. It must have a valid asset extension.</param>
		/// <param name="overwriteExisting">
		///     If true, will overwrite any existing asset file at the location. Otherwise may generate
		///     a uniquely numbered file name.
		/// </param>
		/// <exception cref="ArgumentNullException">If the object is null.</exception>
		/// <exception cref="ArgumentNullException">If the assetPath is null.</exception>
		/// <exception cref="ArgumentException">If the object already is an asset.</exception>
		public Asset(Object obj, Path path, Boolean overwriteExisting = false)
		{
			ThrowIf.ArgumentIsNull(obj, nameof(obj));
			ThrowIf.ArgumentIsNull(path, nameof(path));
			ThrowIf.AlreadyAnAsset(obj);

			Create(obj, path, overwriteExisting);
			SetMainObjectAndAssetPath(obj);
		}

		/// <summary>
		///     Returns an instance from a path to an existing asset.
		/// </summary>
		/// <param name="path"></param>
		/// <exception cref="ArgumentNullException">If the assetPath is null.</exception>
		/// <exception cref="FileNotFoundException">If the assetPath does not point to an existing asset file.</exception>
		public Asset(Path path) => SetMainObjectAndAssetPath(path);

		/// <summary>
		///     Returns an instance from an existing asset reference.
		/// </summary>
		/// <param name="obj"></param>
		/// <exception cref="ArgumentNullException">If the object is null.</exception>
		/// <exception cref="ArgumentException">If the object is not an asset reference.</exception>
		public Asset(Object obj) => SetMainObjectAndAssetPath(obj);

		/// <summary>
		///     Returns an instance from an existing asset's GUID.
		/// </summary>
		/// <param name="assetGuid"></param>
		/// <exception cref="ArgumentException">If the GUID is not in the AssetDatabase (not an asset).</exception>
		public Asset(GUID assetGuid) => SetMainObjectAndAssetPath(assetGuid);

		private void SetMainObjectAndAssetPath(Path path)
		{
			ThrowIf.ArgumentIsNull(path, nameof(path));
			ThrowIf.DoesNotExistInFileSystem(path);

			m_AssetPath = path;
			m_MainObject = LoadMain<Object>();

			ThrowIf.AssetLoadReturnedNull(m_MainObject, m_AssetPath);
		}

		private void SetMainObjectAndAssetPath(Object obj)
		{
			ThrowIf.ArgumentIsNull(obj, nameof(obj));
			ThrowIf.NotInDatabase(obj);

			m_MainObject = obj;
			m_AssetPath = AssetDatabase.GetAssetPath(obj);
		}

		private void SetMainObjectAndAssetPath(GUID guid)
		{
			ThrowIf.NotAnAssetGuid(guid);

			SetMainObjectAndAssetPath(AssetDatabase.GUIDToAssetPath(guid));
		}

		private void SetAssetPathFromObject() => m_AssetPath = Path.Get(m_MainObject);
	}
}
