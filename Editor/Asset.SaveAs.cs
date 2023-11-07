// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static Boolean SaveAs(Path sourcePath, Path destPath, Boolean overwriteExisting = false)
		{
			//ThrowIf.AssetDeleted(this);
			ThrowIf.ArgumentIsNull(sourcePath, nameof(sourcePath));
			ThrowIf.ArgumentIsNull(destPath, nameof(destPath));

			var newPath = Path.GetOverwriteOrUnique(destPath, overwriteExisting);
			Path.CreateFolders(newPath);
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