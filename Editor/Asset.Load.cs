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
		///     Commonly this is the only object of the asset, but there are assets that
		///     consist of multiple objects such as Mesh assets that may contain for example animations and materials.
		/// </summary>
		/// <param name="path"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>The asset object or null if the path does not exist or the asset is not imported.</returns>
		public static T LoadMain<T>(Path path) where T : Object
		{
			ThrowIf.ArgumentIsNull(path, nameof(path));
			ThrowIf.DoesNotExistInFileSystem(path);

			var assetType = GetMainType(path);
			ThrowIf.AssetNotImported(path, assetType);
			ThrowIf.AssetTypeMismatch<T>(path, assetType);

			var obj = AssetDatabase.LoadMainAssetAtPath(path) as T;

			// just to be sure we catch early some possible edge cases where ADB cannot load objects
			ThrowIf.AssetLoadReturnedNull(obj, path);

			return obj;
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
			return Load<T>(Path.Get(guid));
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
				return Load<T>(path);

			var obj = getInstance.Invoke() as T;
			Create(obj, path);

			return obj;
		}

		public static T Load<T>(Path path) where T : Object
		{
			var obj = AssetDatabase.LoadAssetAtPath<T>(path);

			// just to be sure we catch early some possible edge cases where ADB cannot load objects
			ThrowIf.AssetLoadReturnedNull(obj, path);

			return obj;
		}

		public static Object[] LoadAll(Path path) => AssetDatabase.LoadAllAssetsAtPath(path);

		public static Object[] LoadAllVisible(Path path) => AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

		/// <summary>
		///     Private on purpose: the main object is automatically loaded when instantiating an Asset class.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private T LoadMain<T>() where T : Object => m_AssetPath != null ? (T)(m_MainObject = Load<T>(m_AssetPath)) : null;

		public T Load<T>() where T : Object => Load<T>(m_AssetPath);

		private Object[] LoadAll() => LoadAll(m_AssetPath);

		private Object[] LoadAllVisible() => LoadAllVisible(m_AssetPath);
	}
}
