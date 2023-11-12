// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		// private Object[] SelectAndAssignMainObject(Object[] objects)
		// {
		// 	m_AssetObjects = objects;
		//
		// 	if (m_MainObject == null)
		// 	{
		// 		foreach (var obj in objects)
		// 		{
		// 			if (AssetDatabase.IsMainAsset(obj))
		// 			{
		// 				m_MainObject = obj;
		// 				break;
		// 			}
		// 		}
		// 	}
		//
		// 	return objects;
		// }

		public static Boolean ExtractObject(Object subObject, Path destinationPath)
		{
			ThrowIf.ArgumentIsNull(subObject, nameof(subObject));
			ThrowIf.ArgumentIsNull(destinationPath, nameof(destinationPath));

			return Succeeded(AssetDatabase.ExtractAsset(subObject, destinationPath));
		}

		public static void AddObjectToAsset(Object subObject, Object assetObject)
		{
			ThrowIf.ArgumentIsNull(subObject, nameof(subObject));
			ThrowIf.SubObjectIsGameObject(subObject);
			ThrowIf.AlreadyAnAsset(subObject);
			ThrowIf.ArgumentIsNull(assetObject, nameof(assetObject));
			ThrowIf.NotAnAssetWithAssetExtension(assetObject);

			AssetDatabase.AddObjectToAsset(subObject, assetObject);
		}

		public static void RemoveObjectFromAsset(Object subObject)
		{
			ThrowIf.ArgumentIsNull(subObject, nameof(subObject));

			AssetDatabase.RemoveObjectFromAsset(subObject);
		}

		public static void SetMainObject(Object obj, Path path)
		{
			AssetDatabase.SetMainObject(obj, path);
			Import(path);
		}

		public void AddObject(Object subObject) => AddObjectToAsset(subObject, m_MainObject);

		public void RemoveObject(Object subObject) => RemoveObjectFromAsset(subObject);
	}
}
