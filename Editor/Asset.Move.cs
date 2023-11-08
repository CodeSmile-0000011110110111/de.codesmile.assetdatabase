// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Tests if a Move operation will be successful without actually moving the asset.
		/// </summary>
		/// <param name="sourcePath">The path to an existing asset.</param>
		/// <param name="destinationPath">The path where to move the asset to. Can have a different extension.</param>
		/// <returns>True if moving the asset will be successful, false otherwise.</returns>
		public static Boolean CanMove(Path sourcePath, Path destinationPath) => sourcePath != null &&
		                                                                        destinationPath != null &&
		                                                                        Succeeded(AssetDatabase.ValidateMoveAsset(sourcePath, destinationPath));

		/// <summary>
		///     Moves an asset from source to destination path. Any non-existing folders in destination path
		///     will be created.
		/// </summary>
		/// <param name="sourcePath"></param>
		/// <param name="destinationPath"></param>
		/// <returns>True if the move was successful.</returns>
		public static Boolean Move(Path sourcePath, Path destinationPath)
		{
			if (sourcePath == null || destinationPath == null)
				return false;

			destinationPath.CreateFolders();
			return Succeeded(AssetDatabase.MoveAsset(sourcePath, destinationPath));
		}

		/// <summary>
		///     Tests if a Move operation will be successful without actually moving the asset.
		/// </summary>
		/// <param name="destinationPath">The path where to move the asset to. Can have a different extension.</param>
		/// <returns>True if moving the asset will be successful, false otherwise.</returns>
		public Boolean CanMove(Path destinationPath) => CanMove(m_AssetPath, destinationPath);

		/// <summary>
		///     Moves asset to destination path. Any non-existing folders in destination path
		///     will be created. AssetPath property is updated accordingly.
		/// </summary>
		/// <param name="sourcePath"></param>
		/// <param name="destinationPath"></param>
		/// <returns>True if the move was successful.</returns>
		public Boolean Move(Path destinationPath)
		{
			if (Move(m_AssetPath, destinationPath))
			{
				SetAssetPathFromObject();
				return true;
			}

			return false;
		}
	}
}
