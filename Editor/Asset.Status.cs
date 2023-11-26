// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Groups asset status related functions.
		/// </summary>
		public static class Status
		{
			/// <summary>
			///     Checks if the object is an asset in the AssetDatabase. If it isn't but you know
			///     the asset file exists then you need to Import() the asset.
			///     Unlike AssetDatabase, will not throw a NullRef if you pass null.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns>Returns false if the object isn't in the database or if the object is null.</returns>
			public static Boolean IsImported(Object obj) => obj != null ? AssetDatabase.Contains(obj) : false;

			/// <summary>
			///     Checks if the object is an asset in the AssetDatabase. If it isn't but you know
			///     the asset file exists then you need to Import() the asset.
			///     Unlike AssetDatabase, will not throw a NullRef if you pass null.
			/// </summary>
			/// <param name="path"></param>
			/// <returns>Returns false if the object isn't in the database or if the object is null.</returns>
			public static Boolean IsImported(Path path)
			{
#if UNITY_2023_2_OR_NEWER
				return path != null ? AssetDatabase.AssetPathExists(path) : false;
#else
				return path != null ? AssetDatabase.GetMainAssetTypeAtPath(path) != null : false;
#endif
			}

			/// <summary>
			///     Returns whether this object is the asset's 'main' object.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Boolean IsMain(Object obj) => AssetDatabase.IsMainAsset(obj);

			/// <summary>
			///     Returns whether this object is a sub-asset of a composite asset. For example an Animation inside an FBX file.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Boolean IsSub(Object obj) => AssetDatabase.IsSubAsset(obj);

			/// <summary>
			///     Returns whether this is a foreign asset.
			///     <p>
			///         A foreign asset is any type of file that Unity doesn't use
			///         directly but rather maintains cached versions of it in the Library folder. For example, a .png image
			///         is a foreign asset, there is no editor inside Unity for it, and the representation of the .png depends
			///         on the asset's settings and build platform (eg compression, max size, etc).
			///         Other foreign assets: scenes (.unity), prefabs, assembly definitions.
			///     </p>
			/// </summary>
			/// <see cref="IsNative" />
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Boolean IsForeign(Object obj) => AssetDatabase.IsForeignAsset(obj);

			/// <summary>
			///     Returns whether this is a native asset. Native assets are serialized directly by Unity, such as materials.
			///     Note that scenes, prefabs and assembly definitions are considered foreign assets.
			/// </summary>
			/// <see cref="IsForeign" />
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Boolean IsNative(Object obj) => AssetDatabase.IsNativeAsset(obj);

			/// <summary>
			///     Returns whether this object's main asset is loaded.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Boolean IsLoaded(Object obj) => AssetDatabase.IsMainAssetAtPathLoaded(Path.Get(obj));

			/// <summary>
			///     Returns true if the given object is of type SceneAsset.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Boolean IsScene(Object obj) => obj.GetType().Equals(typeof(SceneAsset));

			/// <summary>
			///     Returns whether this path's main asset is loaded.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean IsLoaded(Path path) => AssetDatabase.IsMainAssetAtPathLoaded(path);
		}
	}
}
