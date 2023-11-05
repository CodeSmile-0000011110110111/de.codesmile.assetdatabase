// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static T LoadMain<T>(GUID guid) where T : Object => LoadMain<T>(Path.Get(guid));

		public static T LoadMain<T>(Path path) where T : Object => (T)AssetDatabase.LoadMainAssetAtPath(path);

		// public static T LoadFirst<T>(AssetPath assetPath) where T : Object =>
		// 	AssetDatabase.LoadAssetAtPath<T>(assetPath);
		//
		// public static Object[] LoadAll(AssetPath assetPath) => AssetDatabase.LoadAllAssetsAtPath(assetPath);
		//
		// public static Object[] LoadOnlyVisible(AssetPath assetPath) =>
		// 	AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

		// public Object[] LoadAll() => SelectAndAssignMainObject(LoadAll(m_AssetPath));
		// public Object[] LoadOnlyVisible() => SelectAndAssignMainObject(LoadOnlyVisible(m_AssetPath));
		//
		// public Object LoadMainAsync(Action<Object> onLoadComplete)
		// {
		// 	// TODO: use coroutine to load async
		// 	Object obj = null;
		// 	onLoadComplete?.Invoke(obj);
		// 	return null;
		// }
		//
		// public Object LoadAsync(Int32 fileId, Action<Object> onLoadComplete)
		// {
		// 	// TODO: use coroutine to load async
		// 	Object obj = null;
		// 	onLoadComplete?.Invoke(obj);
		// 	return null;
		// }

		//
		// public Boolean OpenExternal(Int32 lineNumber = -1, Int32 columnNumber = -1) =>
		// 	// TODO: overload for object and instanceId
		// 	// TODO: check null
		// 	AssetDatabase.OpenAsset(MainObject, lineNumber, columnNumber);
		//
		// public void Import()
		// {
		// 	// TODO: check that path is valid
		// }

		/// <summary>
		///     Imports a file at a given path that was created or modified 'externally', ie not via Asset(Database) methods.
		///     For example, any file/folder modified via System.IO.File.Write*() methods or through batch scripts.
		///     Note: If the path does not exist, this method does nothing.
		///     You will need to call Asset.Database.ImportAll() if you want to get rid of an externally deleted file
		///     that still exists in the AssetDatabase.
		/// </summary>
		/// <param name="pathtPath"></param>
		/// <param name="options"></param>
		public static void Import(Path path, ImportAssetOptions options = ImportAssetOptions.Default)
		{
			if (path == null || path.Exists)
				AssetDatabase.ImportAsset(path, options);
		}

		public static void Import(String path, ImportAssetOptions options = ImportAssetOptions.Default) =>
			Import((Path)path, options);

		public T LoadMain<T>() where T : Object => (T)(m_MainObject = LoadMain<T>(m_AssetPath));
	}
}
