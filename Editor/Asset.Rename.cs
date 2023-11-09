// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Renames an asset's file or folder name.
		///     NOTE: Cannot be used to change a file's extension. Use Move instead.
		///     <see cref="Move" />
		/// </summary>
		/// <param name="assetPath">The path to the file or folder to rename.</param>
		/// <param name="newFileName">
		///     The new name of the file or folder, without extension. You may pass in a
		///     full path, in this case only the file or folder name of that path is used.
		/// </param>
		/// <returns>
		///     True if the rename succeeded, false otherwise.
		///     If false, Asset.LastErrorMessage provides a human-readable failure reason.
		/// </returns>
		public static Boolean Rename(Path assetPath, String newFileName)
		{
			if (assetPath == null || newFileName == null)
				return false;

			// also accept full paths .. because, why not?
			try
			{
				newFileName = System.IO.Path.GetFileName(newFileName); // throws on illegal chars
			}
			catch (Exception e)
			{
				SetLastErrorMessage(e.Message);
				return false;
			}

			return Succeeded(AssetDatabase.RenameAsset(assetPath, newFileName));
		}

		/// <summary>
		///     Renames an asset's file or folder name.
		///     NOTE: Cannot be used to change a file's extension. Use Move instead.
		///     <see cref="Move" />
		/// </summary>
		/// <param name="newFileName">
		///     The new name of the file or folder, without extension. You may pass in a
		///     full path, in this case only the file or folder name of that path is used.
		/// </param>
		/// <returns>
		///     True if the rename succeeded. The AssetPath property will be updated accordingly.
		///     If false, Asset.LastErrorMessage provides a human-readable failure reason and the AssetPath
		///     property remains unchanged.
		/// </returns>
		public Boolean Rename(String newFileName)
		{
			if (Rename(m_AssetPath, newFileName))
			{
				SetAssetPathFromObject();
				return true;
			}

			return false;
		}
	}
}
