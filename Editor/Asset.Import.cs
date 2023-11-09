// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Imports a file at a given path that was created or modified 'externally', ie not via Asset(Database) methods.
		///     For example, any file/folder modified via System.IO.File.Write*() methods or through batch scripts.
		///     Note: If the path does not exist, this method does nothing.
		///     You will need to call Asset.Database.ImportAll() if you want to get rid of an externally deleted file
		///     that still exists in the AssetDatabase.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="options"></param>
		public static void Import(Path path, ImportAssetOptions options = ImportAssetOptions.Default)
		{
			if (path != null && path.ExistsInFileSystem)
				AssetDatabase.ImportAsset(path, options);
		}

		// NOTE: there is no Import() instance method since the Asset instance's object is guaranteed to be imported
	}
}
