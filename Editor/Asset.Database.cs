// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		/// Groups all asset database functionality. Turns out there isn't actually that much that is
		/// 'database' related after all. Most of the tasks are in fact operations on an asset itself.
		/// </summary>
		public static class Database
		{
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
			///     Returns whether directory monitoring is enabled in Preferences, or disabled due to symlinks.
			/// </summary>
			[ExcludeFromCodeCoverage] public static Boolean DirectoryMonitoringEnabled =>
				AssetDatabase.IsDirectoryMonitoringEnabled();

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
		}

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
	}
}
