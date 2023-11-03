// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public Boolean IsForeignAsset => Status.IsForeignAsset(m_MainObject);
		public Boolean IsNativeAsset => Status.IsNativeAsset(m_MainObject);
		public Boolean IsSubAsset => Status.IsSubAsset(m_MainObject);
		public Boolean IsMainAsset => Status.IsMainAsset(m_MainObject);
		public Boolean IsLoaded => Status.IsLoaded(m_MainObject);

		//public static Boolean Exists(Object obj) => AssetDatabase.Contains(obj);

		public static Boolean FileExists(AssetPath assetPath) => FileExists((string)assetPath);
		public static Boolean FileExists(string path) => File.Exists(path);

		public static class Status
		{
			public static Boolean IsAsset(Object obj) => AssetDatabase.Contains(obj);
			public static Boolean IsForeignAsset(Object obj) => AssetDatabase.IsForeignAsset(obj);
			public static Boolean IsNativeAsset(Object obj) => AssetDatabase.IsNativeAsset(obj);
			public static Boolean IsSubAsset(Object obj) => AssetDatabase.IsSubAsset(obj);
			public static Boolean IsMainAsset(Object obj) => AssetDatabase.IsMainAsset(obj);
			public static Boolean IsLoaded(Object obj) => AssetDatabase.IsMainAssetAtPathLoaded(Path.Get(obj));

			public static Boolean IsLoaded(AssetPath assetPath) => AssetDatabase.IsMainAssetAtPathLoaded(assetPath);
		}
	}
}
