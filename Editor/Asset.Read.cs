// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static T LoadMain<T>(GUID guid) where T : Object => LoadMain<T>(Path.Get(guid));

		public static T LoadMain<T>(AssetPath assetPath) where T : Object =>
			(T)AssetDatabase.LoadMainAssetAtPath(assetPath);

		public T LoadMain<T>() where T : Object => (T)(m_MainObject = LoadMain<T>(m_AssetPath));

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
	}
}
