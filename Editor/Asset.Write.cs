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
		///     Creates a new asset file at the target path. Also creates all non-existing folders in the path.
		///     Defaults to generating a unique filename if there is an existing asset file.
		///     Can overwrite existing files, if specified.
		/// </summary>
		/// <param name="obj">The object to save as an asset file.</param>
		/// <param name="assetPath">The relative asset path with filename and extension.</param>
		/// <param name="overwriteExisting">(Default: false) If true, any existing asset file will be overwritten.</param>
		/// <returns></returns>
		public static Asset Create(Object obj, AssetPath assetPath, Boolean overwriteExisting = false) =>
			new(CreateFoldersAndAsset(obj, assetPath, overwriteExisting));

		/// <summary>
		///     Creates a new asset file at the target path. Also creates all non-existing folders in the path.
		///     Defaults to generating a unique filename if there is an existing asset file.
		///     Can overwrite existing files, if specified.
		/// </summary>
		/// <param name="obj">The object to save as an asset file.</param>
		/// <param name="path">The relative asset path with filename and extension.</param>
		/// <param name="overwriteExisting">(Default: false) If true, any existing asset file will be overwritten.</param>
		/// <returns></returns>
		public static Asset Create(Object obj, String path, Boolean overwriteExisting = false) =>
			Create(obj, (AssetPath)path, overwriteExisting);

		/// <summary>
		///     Deletes the asset file. Does nothing if there is no file at the given path.
		/// </summary>
		/// <param name="path"></param>
		public static void Delete(String path)
		{
			if (FileExists(path) || Path.FolderExists(path))
				AssetDatabase.DeleteAsset(path);
		}

		public static void Delete(AssetPath assetPath) => Delete((String)assetPath);
		public static void Delete(Object obj) => Delete(Path.Get(obj));

		public static Boolean SaveAs(AssetPath sourcePath, AssetPath destPath, Boolean overwriteExisting = false)
		{
			var newPath = GetNewAssetPath(destPath, overwriteExisting);
			Path.CreateFolders(newPath);
			return AssetDatabase.CopyAsset(sourcePath, newPath);
		}

		internal static Object CreateFoldersAndAsset(Object obj, AssetPath assetPath, Boolean overwriteExisting)
		{
			// TODO: more error handling, eg invalid extension, StreamingAssets path
			var newPath = GetNewAssetPath(assetPath, overwriteExisting);
			Path.CreateFolders(newPath);
			AssetDatabase.CreateAsset(obj, newPath);
			return obj;
		}

		private static AssetPath GetNewAssetPath(AssetPath destPath, Boolean overwriteExisting) =>
			overwriteExisting ? destPath : destPath.UniqueFilePath;
	}
}
