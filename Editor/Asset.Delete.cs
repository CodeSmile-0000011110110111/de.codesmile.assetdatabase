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
		/// <param name="path"></param>
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
		///     Deletes the asset.
		///     Does not Destroy the object.
		///     CAUTION: The asset instance should be discarded afterwards.
		/// </summary>
		/// <returns>
		///     the former MainObject - it is still valid but it is no longer an asset.
		///     To destroy the object, you can simply write: Destroy(asset.Delete()).
		/// </returns>
		public Object Delete()
		{
			var mainObject = m_MainObject;
			if (IsDeleted == false)
			{
				Delete(m_AssetPath);
				InvalidateInstance();
			}
			return mainObject;
		}
	}
}
