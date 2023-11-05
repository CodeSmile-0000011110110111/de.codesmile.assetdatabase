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
		/// <param name="pathtPath"></param>
		public static void Delete(Path path)
		{
			if (path != null && path.Exists)
				AssetDatabase.DeleteAsset(path);
		}

		/// <summary>
		///     Deletes the asset file. Does nothing if there is no file at the given path.
		/// </summary>
		/// <param name="path"></param>
		public static void Delete(String path) => Delete((Path)path);

		/// <summary>
		///     Deletes the asset. Does nothing if the object is not an asset.
		/// </summary>
		/// <param name="path"></param>
		public static void Delete(Object obj) => Delete(Path.Get(obj));

		/// <summary>
		///     Moves the asset file to the OS trash (same as Delete, but recoverable).
		///     Does nothing if there is no file at the given path.
		/// </summary>
		/// <param name="pathtPath"></param>
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
		///     Deletes the asset.
		///     CAUTION: The asset instance should be discarded afterwards.
		/// </summary>
		/// <returns>the former MainObject - it is still valid but it is no longer an asset</returns>
		public Object Delete()
		{
			var mainObject = m_MainObject;
			Delete(m_AssetPath);
			InvalidateInstance();

			return mainObject;
		}

		/// <summary>
		///     Moves the asset to the OS trash (same as Delete, but recoverable).
		///     CAUTION: The asset instance should be discarded afterwards.
		/// </summary>
		/// <returns>the former MainObject - it is still valid but it is no longer an asset</returns>
		public Object Trash()
		{
			var mainObject = m_MainObject;
			Trash(m_AssetPath);
			InvalidateInstance();

			return mainObject;
		}
	}
}
