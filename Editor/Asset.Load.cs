// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Loads the main asset object at the path.
		///     Commonly this is the only object of the asset, but there are assets that consist of multiple
		///     objects. For example Mesh assets often contain sub objects like animations and materials.
		/// </summary>
		/// <see cref="Load{T}(CodeSmile.Editor.Asset.Path)" />
		/// <param name="path"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>The asset object or null if the path does not exist or the asset is not imported.</returns>
		public static T LoadMain<T>(Path path) where T : Object
		{
			ThrowIf.ArgumentIsNull(path, nameof(path));
			ThrowIf.DoesNotExistInFileSystem(path);

			return AssetDatabase.LoadMainAssetAtPath(path) as T;
		}

		/// <summary>
		///     Loads the main asset object for the guid.
		///     Commonly this is the only object of the asset, but there are assets that
		///     consist of multiple objects such as Mesh assets that may contain for example animations and materials.
		/// </summary>
		/// <param name="guid"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>The asset object or null if the guid is not an asset guid.</returns>
		public static T LoadMain<T>(GUID guid) where T : Object
		{
			ThrowIf.NotAnAssetGuid(guid);

			return LoadMain<T>(Path.Get(guid));
		}

		/// <summary>
		///     Tries to load the object at path. If it cannot be loaded, it will be created using the Object instance
		///     returned by the getObjectInstance Func callback.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="getInstance"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T LoadOrCreate<T>(Path path, System.Func<Object> getInstance) where T : Object
		{
			if (path.Exists)
				return LoadMain<T>(path);

			var obj = getInstance.Invoke() as T;
			Create(obj, path);

			return obj;
		}

		/// <summary>
		///     Loads the first visible object of the given type from the asset at path. This usually is the main
		///     object but it could be any other visible sub-object, depending on the type.
		/// </summary>
		/// <see cref="LoadMain{T}(CodeSmile.Editor.Asset.Path)" />
		/// <param name="path"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Load<T>(Path path) where T : Object => AssetDatabase.LoadAssetAtPath<T>(path);

		/// <summary>
		///     Loads all objects of an asset.
		///     NOTE: Whether the main object is included in this list depends on the type of asset,
		///     and whether onlyVisible is true. (Details still unclear - please ask!)
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Object[] LoadAllSubAssets(Path path) => AssetDatabase.LoadAllAssetsAtPath(path);

		/// <summary>
		///     Loads only the visible (representation) objects of an asset.
		///     NOTE: Whether the main object is included in this list depends on the type of asset,
		///     and whether onlyVisible is true. (Details still unclear - please ask!)
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Object[] LoadVisibleSubAssets(Path path) => AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

		// Private on purpose: the main object is automatically loaded when instantiating an Asset class.
		private T LoadMain<T>() where T : Object =>
			m_AssetPath != null ? (T)(m_MainObject = Load<T>(m_AssetPath)) : null;

		/// <summary>
		///     Loads the first object of the given type from the asset.
		/// </summary>
		/// <param name="path"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Load<T>() where T : Object => Load<T>(m_AssetPath);
	}
}
