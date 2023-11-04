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
			public static Boolean IsLoaded(Object obj) => AssetDatabase.IsMainAssetAtPathLoaded(AssetPath.Get(obj));

			[ExcludeFromCodeCoverage]
			public static Boolean IsLoaded(AssetPath assetPath) => AssetDatabase.IsMainAssetAtPathLoaded(assetPath);
		}
	}
}
