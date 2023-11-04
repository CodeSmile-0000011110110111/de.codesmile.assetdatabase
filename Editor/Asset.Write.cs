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
		/// <param name="assetPath"></param>
		public static void Delete(AssetPath assetPath)
		{
			if (assetPath != null && assetPath.Exists)
				AssetDatabase.DeleteAsset(assetPath);
		}
		//
		// public Boolean Trash()
		// {
		// 	var didTrash = AssetDatabase.MoveAssetToTrash(m_AssetPath);
		// 	// TODO: update state
		// 	return didTrash;
		// }
		//

		/// <summary>
		///     Deletes the asset file. Does nothing if there is no file at the given path.
		/// </summary>
		/// <param name="path"></param>
		public static void Delete(String path) => Delete((AssetPath)path);

		/// <summary>
		///     Deletes the asset. Does nothing if the object is not an asset.
		/// </summary>
		/// <param name="path"></param>
		public static void Delete(Object obj) => Delete(AssetPath.Get(obj));

		public static Boolean SaveAs(AssetPath sourcePath, AssetPath destPath, Boolean overwriteExisting = false)
		{
			var newPath = GetNewAssetPath(destPath, overwriteExisting);
			AssetPath.CreateFolders(newPath);
			return AssetDatabase.CopyAsset(sourcePath, newPath);
		}

		// ----------------
		// Copy => SaveAs
		// public Boolean SaveAs(AssetPath destinationPath, Boolean overwriteExisting = false)
		// {
		// 	// TODO: check that asset is created/exists
		//
		// 	var newPath = GetTargetPath(destinationPath, overwriteExisting);
		// 	var success = SaveAs(m_AssetPath, newPath);
		// 	if (success)
		// 		SetMainObjectAndPath(newPath);
		//
		// 	return success;
		// }
		//
		// public Asset Duplicate(AssetPath destinationPath, Boolean overwriteExisting = false)
		// {
		// 	// TODO: check that asset is created/exists
		//
		// 	var newPath = GetTargetPath(destinationPath, overwriteExisting);
		// 	return SaveAs(m_AssetPath, newPath) ? new Asset(newPath) : null;
		// }
		//
		// public void Save()
		// {
		// 	// TODO: guid overload, check null
		// 	AssetDatabase.SaveAssetIfDirty(m_MainObject);
		// }
		//
		// public void Delete()
		// {
		// 	// TODO: check that asset is created/exists
		// 	// TODO: what to do with the deleted object? object remains, i assume. need to null path
		// }

		//
		// public Boolean Rename(AssetPath destinationPath, out String errorMessage)
		// {
		// 	// TODO: if return string empty, move was successful
		// 	// what to do with error message?
		// 	errorMessage = "";
		// 	return errorMessage.Equals(String.Empty);
		// }
		//
		// public Boolean Move(AssetPath destinationPath, out String errorMessage)
		// {
		// 	// TODO: if return string empty, move was successful
		// 	// what to do with error message?
		// 	errorMessage = "";
		// 	return errorMessage.Equals(String.Empty);
		// }
		//
		// public Boolean CanMove(AssetPath destinationPath, out String errorMessage)
		// {
		// 	errorMessage = "";
		// 	return errorMessage.Equals(String.Empty);
		// }
	}
}
