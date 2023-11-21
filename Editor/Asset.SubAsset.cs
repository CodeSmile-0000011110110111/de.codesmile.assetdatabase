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
		/// <summary>
		///     Loads and returns all sub objects the asset is comprised of.
		///     NOTE: Whether the main object is included in this list depends on the type of asset.
		/// </summary>
		public Object[] SubAssets => SubAsset.LoadAll(m_AssetPath);

		/// <summary>
		///     Loads and returns only those asset objects that are shown in the project view.
		///     NOTE: Does NOT include the main asset!
		/// </summary>
		public Object[] VisibleSubAssets => SubAsset.LoadVisible(m_AssetPath);

		/// <summary>
		///     Adds an object as a sub-object to the asset. The object must not already be an asset.
		/// </summary>
		/// <param name="subObject"></param>
		public void AddObject(Object subObject) => SubAsset.Add(subObject, m_MainObject);

		/// <summary>
		///     Removes an object from the asset's sub-objects.
		/// </summary>
		/// <param name="subObject"></param>
		/// W
		public void RemoveObject(Object subObject) => SubAsset.Remove(subObject);

		/// <summary>
		///     Groups all SubAsset related functionality.
		/// </summary>
		public static class SubAsset
		{
			/// <summary>
			///     Extracts a sub-object of an asset as an asset of its own. This is the same as picking a sub-asset
			///     and dragging it outside the containing asset in the project view.
			///     Note: This only works with visible sub objects.
			/// </summary>
			/// <param name="subObject"></param>
			/// <param name="destinationPath"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Boolean Extract(Object subObject, Path destinationPath)
			{
				ThrowIf.ArgumentIsNull(subObject, nameof(subObject));
				ThrowIf.ArgumentIsNull(destinationPath, nameof(destinationPath));

				return Succeeded(AssetDatabase.ExtractAsset(subObject, destinationPath));
			}

			/// <summary>
			///     Adds an object as sub-object to the asset object.
			/// </summary>
			/// <param name="subObject"></param>
			/// <param name="assetObject"></param>
			public static void Add(Object subObject, Object assetObject)
			{
				ThrowIf.ArgumentIsNull(subObject, nameof(subObject));
				ThrowIf.SubObjectIsGameObject(subObject);
				ThrowIf.AlreadyAnAsset(subObject);
				ThrowIf.ArgumentIsNull(assetObject, nameof(assetObject));
				ThrowIf.NotAnAssetWithAssetExtension(assetObject);

				AssetDatabase.AddObjectToAsset(subObject, assetObject);
			}

			/// <summary>
			///     Removes an object as sub-object from whatever asset it is contained in.
			/// </summary>
			/// <param name="subObject"></param>
			public static void Remove(Object subObject)
			{
				ThrowIf.ArgumentIsNull(subObject, nameof(subObject));

				AssetDatabase.RemoveObjectFromAsset(subObject);
			}

			/// <summary>
			///     Sets (changes) an asset's 'main' object.
			///     Note: The subObject must already be a sub object of the targeted asset.
			/// </summary>
			/// <param name="subObject"></param>
			/// <param name="path"></param>
			public static void SetMain(Object subObject, Path path)
			{
				AssetDatabase.SetMainObject(subObject, path);
				File.Import(path);
			}

			/// <summary>
			///     Sets (changes) an asset's 'main' object.
			///     Note: The subObject must already be a sub object of the targeted asset.
			/// </summary>
			/// <param name="subObject"></param>
			/// <param name="assetObject"></param>
			public static void SetMain(Object subObject, Object assetObject) =>
				SetMain(subObject, Path.Get(assetObject));

			/// <summary>
			///     Loads all sub-asset objects of an asset.
			///     NOTE: Whether the main object is included in this list depends on the type of asset,
			///     and whether onlyVisible is true. (Details still unclear - please ask!)
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Object[] LoadAll(Path path) => AssetDatabase.LoadAllAssetsAtPath(path);

			/// <summary>
			///     Loads only the visible (representation) sub-asset objects of an asset.
			///     NOTE: Whether the main object is included in this list depends on the type of asset,
			///     and whether onlyVisible is true. (Details still unclear - please ask!)
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Object[] LoadVisible(Path path) => AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
		}
	}
}
