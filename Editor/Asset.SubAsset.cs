// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static class SubAsset
		{
			public static Boolean Extract(Object subAsset, AssetPath extractedAssetPath, out String errorMessage)
			{
				errorMessage = AssetDatabase.ExtractAsset(subAsset, extractedAssetPath);
				return errorMessage.Equals(String.Empty);
			}

			//
			// // TODO: this could operate on the main or any of the sub assets
			// public void AddObject(Object obj)
			// {
			// 	// TODO: check obj not null, not same as main
			// 	if (m_MainObject != null)
			// 		AssetDatabase.AddObjectToAsset(obj, m_MainObject);
			// 	else
			// 		AssetDatabase.AddObjectToAsset(obj, m_AssetPath);
			// }
			//
			// public void RemoveObject(Object obj) {}
			//
			// public void SetMainObject(Object obj)
			// {
			// 	m_MainObject = obj;
			// 	AssetDatabase.SetMainObject(m_MainObject, m_AssetPath);
			// 	Import();
			// }
		}
	}
}
