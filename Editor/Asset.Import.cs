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

		/// <summary>
		///     <p>
		///         Formerly known as 'Refresh()', this scans for and imports assets that have been modified externally.
		///         External is defined as 'any file modification operation not done through the AssetDatabase', for
		///         example by using System.IO methods or by running scripts and other external tools.
		///     </p>
		///     <p>
		///         Since Refresh() 'traditionally' gets called way too many times needlessly a more descriptive name
		///         was chosen to reflect what this method does. I even considered naming it 100% accurately as:
		///         ImportExternallyModifiedAndUnloadUnusedAssets()
		///     </p>
		///     <p>
		///         CAUTION: Calling this may have an adverse effect on editor performance, since it calls
		///         Resources.UnloadUnusedAssets internally and it also discards any unsaved objects not marked as
		///         'dirty' that are only referenced by scripts, leading to potential loss of unsaved data.
		///         <see cref="https://docs.unity3d.com/Manual/AssetDatabaseRefreshing.html" />
		///     </p>
		///     <see cref="Asset.Import(Path, ImportAssetOptions)" />
		/// </summary>
		/// <param name="options"></param>
		public static void ImportAll(ImportAssetOptions options = ImportAssetOptions.Default) =>
			AssetDatabase.Refresh(options);

		// NOTE: there is no Import() instance method since the Asset instance's object is guaranteed to be imported
	}
}