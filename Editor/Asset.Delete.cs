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
		///     Deletes the asset file. Does nothing if there is no file at the given path.
		/// </summary>
		/// <param name="moveToTrash">If true, file will be moved to OS trashcan (recoverable delete).</param>
		/// <param name="path"></param>
		public static Boolean Delete(Path path, Boolean moveToTrash = false)
		{
			if (path == null || path.Exists == false)
				return false;

			return moveToTrash ? AssetDatabase.MoveAssetToTrash(path) : AssetDatabase.DeleteAsset(path);
		}

		/// <summary>
		///     Deletes the asset. Does nothing if the object is not an asset.
		/// </summary>
		/// <param name="moveToTrash">If true, file will be moved to OS trashcan (recoverable delete).</param>
		/// <param name="path"></param>
		public static Boolean Delete(Object obj, Boolean moveToTrash = false) => Delete(Path.Get(obj), moveToTrash);

		/// <summary>
		///     Moves the asset file to the OS trash (same as Delete, but recoverable).
		///     Does nothing if there is no file at the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>True if successfully trashed</returns>
		public static Boolean Trash(Path path) => path != null && path.Exists && AssetDatabase.MoveAssetToTrash(path);

		/// <summary>
		///     Moves the asset to the OS trash (same as Delete, but recoverable).
		///     Does nothing if the object is not an asset.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>True if successfully trashed</returns>
		public static Boolean Trash(Object obj) => Trash(Path.Get(obj));

		/// <summary>
		///     Deletes the asset.
		///     Does not Destroy the object.
		///     CAUTION: The asset instance should be discarded afterwards.
		/// </summary>
		/// <param name="moveToTrash">If true, file will be moved to OS trashcan (recoverable delete).</param>
		/// <returns>
		///     If successful, returns the former MainObject - it is still valid but it is no longer an asset.
		///     Returns null if the object wasn't deleted.
		/// </returns>
		public Object Delete(Boolean moveToTrash = false)
		{
			var mainObject = m_MainObject;
			if (IsDeleted == false && Delete(m_AssetPath, moveToTrash))
				InvalidateInstance();

			return mainObject;
		}

		/// <summary>
		///     Moves the asset to the OS trash (same as Delete, but recoverable).
		///     Does not Destroy the object.
		///     CAUTION: The asset instance should be discarded afterwards.
		/// </summary>
		/// <returns>
		///     If successful, returns the former MainObject - it is still valid but it is no longer an asset.
		///     Returns null if the object wasn't trashed.
		/// </returns>
		public Object Trash()
		{
			var mainObject = m_MainObject;
			if (IsDeleted == false && Trash(m_AssetPath))
				InvalidateInstance();

			return mainObject;
		}
	}
}
