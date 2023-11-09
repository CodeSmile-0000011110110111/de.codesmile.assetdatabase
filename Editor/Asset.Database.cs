// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static class Database
		{
			private static List<String> s_DeleteFailedPaths = new();

			/// <summary>
			///     Gets or sets the desired worker count.
			///     Setting the worked count calls ForceToDesiredWorkerCount() to ensure the worker count is updated.
			/// </summary>
			[ExcludeFromCodeCoverage] public static Int32 DesiredWorkerCount
			{
				get => AssetDatabase.DesiredWorkerCount;
				set
				{
					AssetDatabase.DesiredWorkerCount = value;
					AssetDatabase.ForceToDesiredWorkerCount();
				}
			}

			/// <summary>
			/// Returns whether directory monitoring is enabled in Preferences, or disabled due to symlinks.
			/// </summary>
			[ExcludeFromCodeCoverage] public static Boolean DirectoryMonitoringEnabled =>
				AssetDatabase.IsDirectoryMonitoringEnabled();

			/// <summary>
			///     The paths that failed to be deleted or trashed. Is an empty list if no failure occured on the
			///     last call to DeleteMany or TrashMany.
			/// </summary>
			/// <returns></returns>
			/// <see cref="TrashMany(System.Collections.Generic.IEnumerable{CodeSmile.Editor.Asset.Path})" />
			/// <see cref="DeleteMany(System.Collections.Generic.IEnumerable{CodeSmile.Editor.Asset.Path})" />
			public static IList<String> DeleteFailedPaths => s_DeleteFailedPaths;

			/// <summary>
			///     Checks if the object is an asset in the AssetDatabase.
			///     Unlike AssetDatabase, will not throw a NullRef if you pass null.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns>Returns false if the object isn't in the database or if the object is null.</returns>
			public static Boolean Contains(Object obj) => obj ? AssetDatabase.Contains(obj) : false;

			/// <summary>
			///     Finds the assets by the given filter criteria.
			///     Returns an array of string GUIDs for compatibility reasons.
			/// </summary>
			/// <param name="filter"></param>
			/// <param name="searchInFolders"></param>
			/// <returns></returns>
			/// <see cref="FindGuids" />
			/// <see cref="FindPaths" />
			public static String[] Find(String filter, String[] searchInFolders = null) => searchInFolders == null
				? AssetDatabase.FindAssets(filter)
				: AssetDatabase.FindAssets(filter, searchInFolders);

			/// <summary>
			///     Finds the assets by the given filter criteria. Returns an array of asset paths.
			/// </summary>
			/// <param name="filter"></param>
			/// <param name="searchInFolders"></param>
			/// <returns></returns>
			/// <see cref="Find" />
			/// <see cref="FindGuids" />
			public static Path[] FindPaths(String filter, String[] searchInFolders = null) =>
				Find(filter, searchInFolders).Select(guid => Path.Get(new GUID(guid))).ToArray();

			/// <summary>
			///     Finds the assets by the given filter criteria. Returns an array of GUID instances.
			/// </summary>
			/// <param name="filter"></param>
			/// <param name="searchInFolders"></param>
			/// <returns></returns>
			/// <see cref="Find" />
			/// <see cref="FindPaths" />
			public static GUID[] FindGuids(String filter, String[] searchInFolders = null) =>
				Find(filter, searchInFolders).Select(guid => new GUID(guid)).ToArray();

			/// <summary>
			///     Will stop Unity from automatically importing assets. Must be called in pair with DisallowAutoRefresh.
			///     Multiple calls must be matched with an equal number of calls to DisallowAutoRefresh since internally
			///     this is using a counter that needs to return to 0 before auto refresh is going to be enabled again.
			///     Note: Has no effect if Preferences => Asset Pipeline => Auto Refresh is disabled to begin with.
			///     Same as AssetDatabase.AllowAutoRefresh().
			///     <see cref="DisallowAutoRefresh" />
			/// </summary>
			[ExcludeFromCodeCoverage] // untestable
			public static void AllowAutoRefresh() => AssetDatabase.AllowAutoRefresh();

			/// <summary>
			///     Same as AssetDatabase.DisallowAutoRefresh().
			///     <see cref="AllowAutoRefresh" />
			/// </summary>
			[ExcludeFromCodeCoverage] // untestable
			public static void DisallowAutoRefresh() => AssetDatabase.DisallowAutoRefresh();

			/// <summary>
			///     Releases any cached file handles Unity holds. May be required to allow manual asset or meta file
			///     modifications that bypass the AssetDatabase.
			/// </summary>
			[ExcludeFromCodeCoverage]
			public static void ReleaseFileHandles() => AssetDatabase.ReleaseCachedFileHandles();

			/// <summary>
			///     Forces reserializing all assets. This loads all assets, upgrades them to their current serialization
			///     version, and then writes them back to disk.
			///     Note: this can potentially change a lot of files in version control. It should be done cautiously.
			/// </summary>
			/// <see
			///     cref="ForceReserialize(System.Collections.Generic.IEnumerable{CodeSmile.Editor.Asset.Path},UnityEditor.ForceReserializeAssetsOptions)" />
			[ExcludeFromCodeCoverage]
			public static void ForceReserializeAll() => AssetDatabase.ForceReserializeAssets();

			/// <summary>
			///     Forces reserializing the assets as the given paths.
			///     This loads the assets, upgrades them to their current serialization version, and then writes them
			///     back to disk.
			///     Note: this can potentially change a lot of files in version control. It should be done cautiously.
			/// </summary>
			/// <see cref="ForceReserialize()" />
			[ExcludeFromCodeCoverage]
			public static void ForceReserialize(IEnumerable<Path> paths,
				ForceReserializeAssetsOptions options = ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata)
			{
				ThrowIf.ArgumentIsNull(paths, nameof(paths));
				ForceReserialize(paths.Cast<String>().ToArray(), options);
			}

			[ExcludeFromCodeCoverage]
			public static void ForceReserialize(IEnumerable<String> paths,
				ForceReserializeAssetsOptions options = ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata)
			{
				ThrowIf.ArgumentIsNull(paths, nameof(paths));
				AssetDatabase.ForceReserializeAssets(paths, options);
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

			/// <summary>
			///     <p>
			///         Saves all unsaved (dirty) objects.
			///     </p>
			///     <p>
			///         CAUTION: Consider that the user may NOT want to have unsaved assets 'randomly' saved!
			///         If you work with specific object(s) already it is in the user's best interest if you use
			///         Asset.Save(obj) instead. Also consider using Asset.BatchEditing(Action) in that case.
			///     </p>
			///     <see cref="Asset.Save(Object)" />
			///     <see cref="Asset.BatchEditing(System.Action)" />
			/// </summary>
			public static void SaveAll() => AssetDatabase.SaveAssets();

			/// <summary>
			///     Tries to delete all the given assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns>
			///     True if all assets where deleted, false if one or more failed to delete whose paths
			///     you can access via the Asset.Database.DeleteFailedPaths property.
			/// </returns>
			/// <see cref="DeleteFailedPaths" />
			public static Boolean DeleteMany(IEnumerable<Path> paths) => DeleteMany(paths.Cast<String>());

			/// <summary>
			///     Tries to delete all the given assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns>
			///     True if all assets where deleted, false if one or more failed to delete whose paths
			///     you can access via the Asset.Database.DeleteFailedPaths property.
			/// </returns>
			/// <see cref="DeleteFailedPaths" />
			public static Boolean DeleteMany(IEnumerable<String> paths) =>
				AssetDatabase.DeleteAssets(paths.ToArray(), s_DeleteFailedPaths = new List<String>());

			/// <summary>
			///     Tries to move all the given assets to the OS trash.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns>
			///     True if all assets where trashed, false if one or more failed to trash whose paths
			///     you can access via the Asset.Database.DeleteFailedPaths property.
			/// </returns>
			/// <see cref="DeleteFailedPaths" />
			public static Boolean TrashMany(IEnumerable<Path> paths) => TrashMany(paths.Cast<String>());

			/// <summary>
			///     Tries to move all the given assets to the OS trash.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns>
			///     True if all assets where trashed, false if one or more failed to trash whose paths
			///     you can access via the Asset.Database.DeleteFailedPaths property.
			/// </returns>
			/// <see cref="DeleteFailedPaths" />
			public static Boolean TrashMany(IEnumerable<String> paths) =>
				AssetDatabase.MoveAssetsToTrash(paths.ToArray(), s_DeleteFailedPaths = new List<String>());

			/// <summary>
			///     Internal on purpose: use Asset.BatchEditing(Action) instead
			///     <see cref="Asset.BatchEditing" />
			/// </summary>
			[ExcludeFromCodeCoverage]
			internal static void StartAssetEditing() => AssetDatabase.StartAssetEditing();

			/// <summary>
			///     Internal on purpose: use Asset.BatchEditing(Action) instead
			///     <see cref="Asset.BatchEditing" />
			/// </summary>
			[ExcludeFromCodeCoverage]
			internal static void StopAssetEditing() => AssetDatabase.StartAssetEditing();

			public static class CacheServer
			{
				/*
	RefreshSettings
	CanConnectToCacheServer
	CloseCacheServerConnection
	GetCacheServerAddress
	GetCacheServerEnableDownload
	GetCacheServerEnableUpload
	GetCacheServerNamespacePrefix
	GetCacheServerPort
	GetCurrentCacheServerIp
	IsCacheServerEnabled
	IsConnectedToCacheServer
	ResetCacheServerReconnectTimer
	WriteImportSettingsIfDirty
				 */
			}

			public static class Labels {}
			public static class SubAsset {}
			public static class Meta {}
			public static class Load {}

			public static class VersionControl
			{
				/*
	CanOpenAssetInEditor
	CanOpenForEdit
	IsMetaFileOpenForEdit
	IsOpenForEdit
	MakeEditable
				 */
			}

			// separate class
			public static class Package
			{
				//Import
				//Export
			}

			public static class Importer
			{
				/*
	ClearImporterOverride
	GetAvailableImporters
	GetDefaultImporter
	GetImporterOverride
	GetImporterType
	GetImporterTypes
	SetImporterOverride
				 */
			}

			public static class Bundle
			{
				/*
	GetAllAssetBundleNames
	GetAssetBundleDependencies
	GetAssetPathsFromAssetBundle
	GetAssetPathsFromAssetBundleAndAssetName
	GetImplicitAssetBundleName
	GetImplicitAssetBundleVariantName
	GetUnusedAssetBundleNames
	RemoveAssetBundleName
	RemoveUnusedAssetBundleNames
				 */
			}
		}
	}
}
