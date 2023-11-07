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
		public static void Trash(Path path)
		{
			if (path != null && path.Exists)
				AssetDatabase.MoveAssetToTrash(path);
		}

		/// <summary>
		///     Moves the asset file to the OS trash (same as Delete, but recoverable).
		///     Does nothing if there is no file at the given path.
		/// </summary>
		/// <param name="path"></param>
		public static void Trash(String path) => Trash((Path)path);

		/// <summary>
		///     Moves the asset to the OS trash (same as Delete, but recoverable).
		///     Does nothing if the object is not an asset.
		/// </summary>
		/// <param name="obj"></param>
		public static void Trash(Object obj) => Trash(Path.Get(obj));

		/// <summary>
		///     Moves the asset to the OS trash (same as Delete, but recoverable).
		///     Does not Destroy the object.
		///     CAUTION: The asset instance should be discarded afterwards.
		/// </summary>
		/// <returns>
		///     the former MainObject - it is still valid but it is no longer an asset.
		///     To destroy the object, you can simply write: Destroy(asset.Trash()).
		/// </returns>
		public Object Trash()
		{
			var mainObject = m_MainObject;
			if (IsDeleted == false)
			{
				Trash(m_AssetPath);
				InvalidateInstance();
			}
			return mainObject;
		}
	}
}
