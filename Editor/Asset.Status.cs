// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		[ExcludeFromCodeCoverage] public Boolean IsForeignAsset => Status.IsForeignAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsNativeAsset => Status.IsNativeAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsSubAsset => Status.IsSubAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsMainAsset => Status.IsMainAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsLoaded => Status.IsLoaded(m_MainObject);

		public static Boolean FileExists(AssetPath assetPath) => FileExists((String)assetPath);
		public static Boolean FileExists(String path) => File.Exists(path);

		public static class Status
		{
			[ExcludeFromCodeCoverage]
			public static Boolean IsAsset(Object obj) => AssetDatabase.Contains(obj);

			[ExcludeFromCodeCoverage]
			public static Boolean IsForeignAsset(Object obj) => AssetDatabase.IsForeignAsset(obj);

			[ExcludeFromCodeCoverage]
			public static Boolean IsNativeAsset(Object obj) => AssetDatabase.IsNativeAsset(obj);

			[ExcludeFromCodeCoverage]
			public static Boolean IsSubAsset(Object obj) => AssetDatabase.IsSubAsset(obj);

			[ExcludeFromCodeCoverage]
			public static Boolean IsMainAsset(Object obj) => AssetDatabase.IsMainAsset(obj);

			[ExcludeFromCodeCoverage]
			public static Boolean IsLoaded(Object obj) => AssetDatabase.IsMainAssetAtPathLoaded(AssetPath.Get(obj));

			[ExcludeFromCodeCoverage]
			public static Boolean IsLoaded(AssetPath assetPath) => AssetDatabase.IsMainAssetAtPathLoaded(assetPath);
		}
	}
}
