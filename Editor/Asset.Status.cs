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
		///     Returns true after the asset has been deleted.
		///     <p>
		///         <see cref="Delete(CodeSmile.Editor.Asset.Path)" /> - <see cref="Trash(CodeSmile.Editor.Asset.Path)" />
		///     </p>
		/// </summary>
		public Boolean IsDeleted => m_AssetPath == null && m_MainObject == null;

		[ExcludeFromCodeCoverage] public Boolean IsForeignAsset => Status.IsForeignAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsNativeAsset => Status.IsNativeAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsSubAsset => Status.IsSubAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsMainAsset => Status.IsMainAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsLoaded => Status.IsLoaded(m_MainObject);

		/// <summary>
		///     Returns true if the asset exists in the Database. Convenient shortcut for Asset.Database.Contains().
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>False if the object is null or not in the database.</returns>
		public static Boolean Exists(Object obj) => Database.Contains(obj);

		/// <summary>
		///     Returns the type of the main asset at the path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>the type of the asset or null if the path does not exist</returns>
		public static Type GetMainType(Path path) => AssetDatabase.GetMainAssetTypeAtPath(path);

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
