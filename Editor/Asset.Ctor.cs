// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		internal static Object CreateFoldersAndAsset(Object obj, Path path, Boolean overwriteExisting)
		{
			// TODO: more error handling, eg invalid extension, StreamingAssets path
			var newPath = GetNewAssetPath(path, overwriteExisting);
			Path.CreateFolders(newPath);
			AssetDatabase.CreateAsset(obj, newPath);

			var value1 = Random.Range(1f, 22f);
			var value2 = Random.Range(1, 22);

			return obj;
		}

		[ExcludeFromCodeCoverage] private Asset() {} // Hidden parameterless ctor

		/// <summary>
		///     Returns an instance by creating (saving) the object as an asset file at the given path.
		///     The object must not already be an asset. If it is an asset you must use the ctor without a path.
		/// </summary>
		/// <param name="obj">The object to create/save as asset file.</param>
		/// <param name="pathtPath">The path to the asset file. It must have a valid asset extension.</param>
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
			ThrowIf.IsExistingAsset(obj);

			CreateFoldersAndAsset(obj, path, overwriteExisting);
			SetMainObjectAndPath(obj);
		}

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
		public Asset(Object obj, String path, Boolean overwriteExisting = false) :
			this(obj, (Path)path, overwriteExisting) {}

		/// <summary>
		///     Returns an instance from a path to an existing asset.
		/// </summary>
		/// <param name="pathtPath"></param>
		/// <exception cref="ArgumentNullException">If the assetPath is null.</exception>
		/// <exception cref="FileNotFoundException">If the assetPath does not point to an existing asset file.</exception>
		public Asset(Path path) => SetMainObjectAndPath(path);

		/// <summary>
		///     Returns an instance from a path to an existing asset.
		/// </summary>
		/// <param name="path"></param>
		/// <exception cref="ArgumentNullException">If the assetPath is null.</exception>
		/// <exception cref="FileNotFoundException">If the assetPath does not point to an existing asset file.</exception>
		public Asset(String path) : this(new Path(path)) {}

		/// <summary>
		///     Returns an instance from an existing asset reference.
		/// </summary>
		/// <param name="obj"></param>
		/// <exception cref="ArgumentNullException">If the object is null.</exception>
		/// <exception cref="ArgumentException">If the object is not an asset reference.</exception>
		public Asset(Object obj) => SetMainObjectAndPath(obj);

		/// <summary>
		///     Returns an instance from an existing asset's GUID.
		/// </summary>
		/// <param name="assetGuid"></param>
		/// <exception cref="ArgumentException">If the GUID is not in the AssetDatabase (not an asset).</exception>
		public Asset(GUID assetGuid) => SetMainObjectAndPath(assetGuid);

		private void SetMainObjectAndPath(Path path)
		{
			ThrowIf.ArgumentIsNull(path, nameof(path));
			ThrowIf.DoesNotExist(path);

			m_AssetPath = path;
			m_MainObject = LoadMain<Object>();
			ThrowIf.NotAnAssetAtPath(m_MainObject, m_AssetPath);
		}

		private void SetMainObjectAndPath(Object obj)
		{
			ThrowIf.ArgumentIsNull(obj, nameof(obj));
			ThrowIf.NotAnAsset(obj);

			m_MainObject = obj;
			m_AssetPath = (Path)AssetDatabase.GetAssetPath(obj);
		}

		private void SetMainObjectAndPath(GUID guid)
		{
			ThrowIf.NotAnAsset(guid);

			SetMainObjectAndPath((Path)AssetDatabase.GUIDToAssetPath(guid));
		}
	}
}
