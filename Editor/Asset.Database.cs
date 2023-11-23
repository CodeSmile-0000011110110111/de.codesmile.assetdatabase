// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Groups all asset database functionality. Turns out there isn't actually that much that is
		///     'database' related after all.
		/// </summary>
		public static partial class Database
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
			/// <param name="paths"></param>
			/// <param name="options"></param>
			[ExcludeFromCodeCoverage]
			public static void ForceReserialize(IEnumerable<Path> paths,
				ForceReserializeAssetsOptions options = ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata)
			{
				ThrowIf.ArgumentIsNull(paths, nameof(paths));
				ForceReserialize(Path.ToStrings(paths), options);
			}

			/// <summary>
			///     Forces reserializing the assets as the given paths.
			///     This loads the assets, upgrades them to their current serialization version, and then writes them
			///     back to disk.
			///     Note: this can potentially change a lot of files in version control. It should be done cautiously.
			/// </summary>
			/// <see cref="ForceReserialize()" />
			/// <param name="paths"></param>
			/// <param name="options"></param>
			[ExcludeFromCodeCoverage]
			public static void ForceReserialize(IEnumerable<String> paths,
				ForceReserializeAssetsOptions options = ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata)
			{
				ThrowIf.ArgumentIsNull(paths, nameof(paths));
				AssetDatabase.ForceReserializeAssets(paths, options);
			}

			/// <summary>
			///     Saves all unsaved (dirty) objects.
			///     CAUTION: Consider that the user may NOT want to have unsaved assets 'randomly' saved!
			///     If you work with specific object(s) (which you do most of the time, right?) it is in your (user's)
			///     best interest if you use Asset.Save(obj) instead.
			///     Also consider using BatchEditing(Action) in that case.
			/// </summary>
			/// <see cref="Asset.Save" />
			/// <see cref="Asset.ForceSave" />
			/// <see cref="Asset.File.BatchEditing" />
			public static void SaveAll() => AssetDatabase.SaveAssets();

			/// <summary>
			///     Formerly known as 'Refresh()', this scans for and imports assets that have been modified externally.
			///     External is defined as 'any file modification operation not done through the AssetDatabase', for
			///     example by using System.IO methods or by running scripts and other external tools.
			///     <p>
			///         Note: the 100% accurate name for this method would have to be:
			///         ImportAllExternallyModifiedAssetsAndAlsoUnloadUnusedAssets()
			///     </p>
			///     <p>
			///         CAUTION: Calling this may have an adverse effect on editor performance. In addition to scanning
			///         for changed files it also calls Resources.UnloadUnusedAssets internally and it also discards any
			///         unsaved objects not marked as 'dirty' that are only referenced by scripts, leading to potential
			///         loss of unsaved data.
			///         <see cref="https://docs.unity3d.com/Manual/AssetDatabaseRefreshing.html" />
			///     </p>
			///     <see cref="File.Import" />
			/// </summary>
			/// <param name="options"></param>
			public static void ImportAll(ImportAssetOptions options = ImportAssetOptions.Default) =>
				AssetDatabase.Refresh(options);

			/// <summary>
			///     Internal on purpose: use Asset.BatchEditing(Action) instead
			///     <see cref="File.BatchEditing" />
			/// </summary>
			[ExcludeFromCodeCoverage]
			internal static void StartAssetEditing() => AssetDatabase.StartAssetEditing();

			/// <summary>
			///     Internal on purpose: use Asset.BatchEditing(Action) instead
			///     <see cref="File.BatchEditing" />
			/// </summary>
			[ExcludeFromCodeCoverage]
			internal static void StopAssetEditing() => AssetDatabase.StartAssetEditing();
		}
	}
}
