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
		///     Returns true if the asset exists in the Database. Convenient shortcut for Asset.Database.Contains().
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>False if the object is null or not in the database.</returns>
		public static Boolean Exists(Object obj) => Database.Contains(obj);

		public static Type GetMainAssetType(Path path) => AssetDatabase.GetMainAssetTypeAtPath(path);
		public static Type GetMainAssetType(String path) => GetMainAssetType((Path)path);

		/// <summary>
		///     Query the status of an asset.
		/// </summary>
		public static class Status // class needed: there are non-static methods of the same name
		{
			[ExcludeFromCodeCoverage]
			public static Boolean IsForeignAsset(Object obj) => AssetDatabase.IsForeignAsset(obj);

			[ExcludeFromCodeCoverage]
			public static Boolean IsNativeAsset(Object obj) => AssetDatabase.IsNativeAsset(obj);

			[ExcludeFromCodeCoverage]
			public static Boolean IsSubAsset(Object obj) => AssetDatabase.IsSubAsset(obj);

			[ExcludeFromCodeCoverage]
			public static Boolean IsMainAsset(Object obj) => AssetDatabase.IsMainAsset(obj);

			[ExcludeFromCodeCoverage]
			public static Boolean IsLoaded(Object obj) => AssetDatabase.IsMainAssetAtPathLoaded(Path.Get(obj));

			[ExcludeFromCodeCoverage]
			public static Boolean IsLoaded(Path path) => AssetDatabase.IsMainAssetAtPathLoaded(path);
		}
	}
}