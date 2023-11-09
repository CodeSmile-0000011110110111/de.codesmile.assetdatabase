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

		[ExcludeFromCodeCoverage] public Boolean IsForeignAsset => IsForeign(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsNativeAsset => IsNative(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsSubAsset => IsSub(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsMainAsset => IsMain(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsLoaded => Loaded(m_MainObject);

		[ExcludeFromCodeCoverage]
		public static Boolean IsForeign(Object obj) => AssetDatabase.IsForeignAsset(obj);

		[ExcludeFromCodeCoverage]
		public static Boolean IsNative(Object obj) => AssetDatabase.IsNativeAsset(obj);

		[ExcludeFromCodeCoverage]
		public static Boolean IsSub(Object obj) => AssetDatabase.IsSubAsset(obj);

		[ExcludeFromCodeCoverage]
		public static Boolean IsMain(Object obj) => AssetDatabase.IsMainAsset(obj);

		[ExcludeFromCodeCoverage]
		public static Boolean Loaded(Object obj) => AssetDatabase.IsMainAssetAtPathLoaded(Path.Get(obj));

		[ExcludeFromCodeCoverage]
		public static Boolean Loaded(Path path) => AssetDatabase.IsMainAssetAtPathLoaded(path);

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
		public static Type MainType(Path path) => AssetDatabase.GetMainAssetTypeAtPath(path);
	}
}
