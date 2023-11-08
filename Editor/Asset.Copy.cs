// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Makes a copy of the source asset and saves it in the destination.
		///     Will create destination path folders if necessary.
		/// </summary>
		/// <param name="sourcePath">the source asset to copy from</param>
		/// <param name="destinationPath">the destination path the copy is saved</param>
		/// <param name="overwriteExisting">
		///     True if an existing destination asset should be replaced. False if a unique filename
		///     should be generated.
		/// </param>
		/// <returns>True if copying succeeded, false if it failed.</returns>
		public static Boolean Copy(Path sourcePath, Path destinationPath, Boolean overwriteExisting = false)
		{
			ThrowIf.ArgumentIsNull(sourcePath, nameof(sourcePath));
			ThrowIf.ArgumentIsNull(destinationPath, nameof(destinationPath));
			ThrowIf.AssetPathNotInDatabase(sourcePath);
			ThrowIf.OverwritingSamePath(sourcePath, destinationPath, overwriteExisting);

			var newPath = Path.GetOverwriteOrUnique(destinationPath, overwriteExisting);
			newPath.CreateFolders();
			return AssetDatabase.CopyAsset(sourcePath, newPath);
		}

		/// <summary>
		///     Makes a copy of the asset.
		///     Will create destination path folders if necessary.
		/// </summary>
		/// <param name="destinationPath"></param>
		/// <param name="overwriteExisting"></param>
		/// <returns>The Asset instance of the copy, or null if copying failed.</returns>
		public Asset Copy(Path destinationPath, Boolean overwriteExisting = false)
		{
			ThrowIf.AssetDeleted(this);

			var newPath = Path.GetOverwriteOrUnique(destinationPath, overwriteExisting);
			var success = Copy(m_AssetPath, destinationPath);
			return success ? new Asset(newPath) : null;
		}
	}
}
