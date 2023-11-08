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
		public static Boolean Delete(Path path) => path != null && path.Exists && AssetDatabase.DeleteAsset(path);

		/// <summary>
		///     Deletes the asset file. Does nothing if there is no file at the given path.
		/// </summary>
		/// <param name="path"></param>
		public static Boolean Delete(String path) => Delete((Path)path);

		/// <summary>
		///     Deletes the asset. Does nothing if the object is not an asset.
		/// </summary>
		/// <param name="path"></param>
		public static Boolean Delete(Object obj) => Delete(Path.Get(obj));

		/// <summary>
		///     Deletes the asset.
		///     Does not Destroy the object.
		///     CAUTION: The asset instance should be discarded afterwards.
		/// </summary>
		/// <returns>
		///     If successful, returns the former MainObject - it is still valid but it is no longer an asset.
		///		Returns null if the object wasn't deleted.
		/// </returns>
		public Object Delete()
		{
			if (IsDeleted == false)
			{
				if (Delete(m_AssetPath))
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
