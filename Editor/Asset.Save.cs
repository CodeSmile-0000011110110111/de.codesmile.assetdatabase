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
		///     Creates (saves) a new asset file at the target path. Also creates all non-existing folders in the path.
		///     Defaults to generating a unique filename if there is an existing asset file.
		///     Can overwrite existing files, if specified.
		/// </summary>
		/// <param name="obj">The object to save as an asset file.</param>
		/// <param name="path">The relative asset path with filename and extension.</param>
		/// <param name="overwriteExisting">(Default: false) If true, any existing asset file will be overwritten.</param>
		/// <returns></returns>
		public static Asset Create(Object obj, Path path, Boolean overwriteExisting = false)
		{
			Path.CreateFolders(path);
			return new Asset(CreateAsset(obj, path, overwriteExisting));
		}

		/// <summary>
		///     Creates (saves) a new asset file at the target path. Also creates all non-existing folders in the path.
		///     Defaults to generating a unique filename if there is an existing asset file.
		///     Can overwrite existing files, if specified.
		/// </summary>
		/// <param name="obj">The object to save as an asset file.</param>
		/// <param name="path">The relative asset path with filename and extension.</param>
		/// <param name="overwriteExisting">(Default: false) If true, any existing asset file will be overwritten.</param>
		/// <returns></returns>
		public static Asset Create(Object obj, String path, Boolean overwriteExisting = false) =>
			Create(obj, (Path)path, overwriteExisting);

		private static Object CreateAsset(Object obj, Path path, Boolean overwriteExisting)
		{
			// TODO: more error handling:
			// invalid extension
			// StreamingAssets path

			var newPath = Path.GetOverwriteOrUnique(path, overwriteExisting);
			AssetDatabase.CreateAsset(obj, newPath);
			return obj;
		}

		//
		// public void Save()
		// {
		// 	// TODO: guid overload, check null
		// 	AssetDatabase.SaveAssetIfDirty(m_MainObject);
		// }
	}
}