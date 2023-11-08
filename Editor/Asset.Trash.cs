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
		///     Moves the asset file to the OS trash (same as Delete, but recoverable).
		///     Does nothing if there is no file at the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>True if successfully trashed</returns>
		public static Boolean Trash(Path path) => path != null && path.Exists && AssetDatabase.MoveAssetToTrash(path);

		/// <summary>
		///     Moves the asset file to the OS trash (same as Delete, but recoverable).
		///     Does nothing if there is no file at the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>True if successfully trashed</returns>
		public static Boolean Trash(String path) => Trash((Path)path);

		/// <summary>
		///     Moves the asset to the OS trash (same as Delete, but recoverable).
		///     Does nothing if the object is not an asset.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>True if successfully trashed</returns>
		public static Boolean Trash(Object obj) => Trash(Path.Get(obj));

		/// <summary>
		///     Moves the asset to the OS trash (same as Delete, but recoverable).
		///     Does not Destroy the object.
		///     CAUTION: The asset instance should be discarded afterwards.
		/// </summary>
		/// <returns>
		///     If successful, returns the former MainObject - it is still valid but it is no longer an asset.
		///		Returns null if the object wasn't trashed.
		/// </returns>
		public Object Trash()
		{
			if (IsDeleted == false)
			{
				if (Trash(m_AssetPath))
				{
					var mainObject = m_MainObject;
					InvalidateInstance();
					return mainObject;
				}
			}

			return null;
		}
	}
}
