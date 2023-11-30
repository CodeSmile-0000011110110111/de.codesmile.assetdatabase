// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Groups all asset database functionality.
		/// </summary>
		/// <remarks>Turns out there isn't actually that much that is 'database' related after all.</remarks>
		public static partial class Database
		{
			/// <summary>
			///     Gets or sets the desired worker count.
			/// </summary>
			/// <remarks>
			///     Setting the worked count calls ForceToDesiredWorkerCount() to ensure the worker count is updated.
			/// </remarks>
			/// <seealso cref="">
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.DesiredWorkerCount.html">AssetDatabase.DesiredWorkerCount</a>
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ForceToDesiredWorkerCount.html">AssetDatabase.ForceToDesiredWorkerCount</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Int32 DesiredWorkerCount
			{
				get => AssetDatabase.DesiredWorkerCount;
				set
				{
					AssetDatabase.DesiredWorkerCount = value;
					AssetDatabase.ForceToDesiredWorkerCount();
				}
			}

			/// <summary>
			///     Returns whether directory monitoring is enabled in Preferences.
			/// </summary>
			/// <remarks>
			///     DirectoryMonitoring is automatically disabled when symlinks are used in the project.
			/// </remarks>
			/// <seealso cref="">
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsDirectoryMonitoringEnabled.html">AssetDatabase.IsDirectoryMonitoringEnabled</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Boolean DirectoryMonitoring
			{
				get => AssetDatabase.IsDirectoryMonitoringEnabled();
				set => EditorPrefs.SetBool("DirectoryMonitoring", value);
			}

			/// <summary>
			///     Tests if the asset is in the database.
			/// </summary>
			/// <param name="instance">Instance to test.</param>
			/// <returns>Returns true if the asset is imported. Returns false if the asset is not in the database, or if obj is null.</returns>
			/// <seealso cref="">
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.Contains.html">AssetDatabase.Contains</a>
			/// </seealso>
			public static Boolean Contains(Object instance) =>
				instance != null ? AssetDatabase.Contains(instance) : false;

			/// <summary>
			///     Tests if the asset is in the database.
			/// </summary>
			/// <param name="instanceId">The instance ID of an asset.</param>
			/// <returns>Returns true if the asset is imported. Returns false if the asset is not in the database.</returns>
			/// <seealso cref="">
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.Contains.html">AssetDatabase.Contains</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Boolean Contains(Int32 instanceId) => AssetDatabase.Contains(instanceId);

			/// <summary>
			///     Will allow Unity to automatically import assets.
			/// </summary>
			/// <remarks>
			///     This has no effect if Preferences => Asset Pipeline => Auto Refresh is disabled.
			/// </remarks>
			/// <remarks>
			///     Must be called in pair with CodeSmile.Editor.Asset.Database.DisallowAutoRefresh.
			/// </remarks>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Database.DisallowAutoRefresh" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.AllowAutoRefresh.html">AssetDatabase.AllowAutoRefresh</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // untestable
			public static void AllowAutoRefresh() => AssetDatabase.AllowAutoRefresh();

			/// <summary>
			///     Will stop Unity from automatically importing assets.
			/// </summary>
			/// <remarks>
			///     Must be called in pair with CodeSmile.Editor.Asset.Database.AllowAutoRefresh.
			/// </remarks>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Database.AllowAutoRefresh" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.DisallowAutoRefresh.html">AssetDatabase.DisallowAutoRefresh</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // untestable
			public static void DisallowAutoRefresh() => AssetDatabase.DisallowAutoRefresh();

			/// <summary>
			///     Releases any cached file handles that Unity holds.
			/// </summary>
			/// <remarks>
			///     May be required to allow external asset or meta file modifications to operate without causing
			///     access exceptions.
			/// </remarks>
			/// <seealso cref="">
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ReleaseCachedFileHandles.html">AssetDatabase.ReleaseCachedFileHandles</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // untestable
			public static void ReleaseFileHandles() => AssetDatabase.ReleaseCachedFileHandles();

			/// <summary>
			///     Updates all native asset files to reflect any changes in serialization in the current editor version.
			/// </summary>
			/// <remarks>
			///     Use this after upgrading Unity Editor versions and you want to make sure all native assets are
			///     serialized to the current serialization format. Depending on whether Unity made changes to the serialization
			///     format this may change between none to all native assets, including .meta files. Use with caution when
			///     working with source control: discuss implications with the team / tech lead.
			/// </remarks>
			/// <seealso cref="">
			///     -
			///     <see
			///         cref="CodeSmile.Editor.Asset.Database.ForceReserialize(IEnumerable{string},ForceReserializeAssetsOptions)" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ForceReserializeAssets.html">AssetDatabase.ForceReserializeAssets</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // untestable
			public static void ForceReserializeAll() => AssetDatabase.ForceReserializeAssets();

			/// <summary>
			///     Updates all native asset files to reflect any changes in serialization in the current editor version.
			/// </summary>
			/// <remarks>
			///     Use this after upgrading Unity Editor versions and you want to make sure all native assets are
			///     serialized to the current serialization format. Depending on whether Unity made changes to the serialization
			///     format this may change between none to all native assets. Use with caution when working with source control:
			///     discuss implications with the team / tech lead.
			/// </remarks>
			/// <param name="paths">Paths to assets to reserialize to the current serialization version.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ForceReserializeAssetsOptions.html">ForceReserializeAssetsOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Database.ForceReserializeAll" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ForceReserializeAssets.html">AssetDatabase.ForceReserializeAssets</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // untestable
			public static void ForceReserialize(IEnumerable<Path> paths,
				ForceReserializeAssetsOptions options = ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata) =>
				ForceReserialize(Path.ToStrings(paths), options);

			/// <summary>
			///     Updates all native asset files to reflect any changes in serialization in the current editor version.
			/// </summary>
			/// <remarks>
			///     Use this after upgrading Unity Editor versions and you want to make sure all native assets are
			///     serialized to the current serialization format. Depending on whether Unity made changes to the serialization
			///     format this may change between none to all native assets. Use with caution when working with source control:
			///     discuss implications with the team / tech lead.
			/// </remarks>
			/// <param name="paths">Paths to assets to reserialize to the current serialization version.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ForceReserializeAssetsOptions.html">ForceReserializeAssetsOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Database.ForceReserializeAll" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ForceReserializeAssets.html">AssetDatabase.ForceReserializeAssets</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // untestable
			public static void ForceReserialize(IEnumerable<String> paths,
				ForceReserializeAssetsOptions options = ForceReserializeAssetsOptions.ReserializeAssetsAndMetadata) =>
				AssetDatabase.ForceReserializeAssets(paths, options);

			/// <summary>
			///     Saves all unsaved (dirty) assets.
			/// </summary>
			/// <remarks>
			///     CAUTION: Consider that a user generally does NOT want an editor script to 'randomly' save all
			///     unsaved assets! Use only where absolutely appropriate, otherwise prefer to use
			///     CodeSmile.Editor.Asset.File.Save to explicitly save each modified asset. Preferably do so within
			///     a CodeSmile.Editor.Asset.File.BatchEditing operation. Just be nice to fellow Editor users. ;)
			/// </remarks>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.File.Save" />
			///     - <see cref="CodeSmile.Editor.Asset.File.ForceSave" />
			///     - <see cref="CodeSmile.Editor.Asset.File.BatchEditing" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SaveAssets.html">AssetDatabase.SaveAssets</a>
			/// </seealso>
			public static void SaveAll() => AssetDatabase.SaveAssets();

			/// <summary>
			///     Scans for **external** file system modifications and updates the Database accordingly. Prefer to use
			///     CodeSmile.Editor.Asset.File.Import
			///     within CodeSmile.Editor.Asset.File.BatchEditing. **CAUTION**: ImportAll ('Refresh') unloads unused resources.
			///     This can degrade editor performance!
			/// </summary>
			/// <remarks>
			///     For best performance, prefer to use CodeSmile.Editor.Asset.File.Import(String[],ImportAssetOptions) to import
			///     multiple assets in a batch operation.
			/// </remarks>
			/// <remarks>
			///     When to call ImportAll (same as AssetDatabase.Refresh):
			///     - After System.IO.File/Directory or similar methods modified files/folders in the project.
			///     - After running an external process that possibly modified files/folders in the project.
			///     There is no need to call ImportAll / 'Refresh' in any other situation!
			/// </remarks>
			/// <remarks>
			///     Modified means the following:
			///     - Create a file / folder
			///     - Delete or trash a file / folder
			///     - Move or rename a file / folder
			///     - Change a file's contents
			///     - Change attributes of a file/folder
			/// </remarks>
			/// <remarks>
			///     History: I believe there was a time around Unity 3.x-ish where the Editor did not have an "auto refresh"
			///     feature, so calling 'AssetDatabase.Refresh' after any AssetDatabase operation was common.
			///     This seems to have stuck, even though it is no longer required. 'Refresh' remains over-used with no thought given
			///     to its necessity or performance implications.
			/// </remarks>
			/// <remarks>
			///     Any file operation done VIA the AssetDatabase these days does **not** require an AssetDatabase refresh!
			///     Only file operations that bypass the AssetDatabase require importing affected files. In most cases you know
			///     which files are affects, so import them individually. Everyone will thank you for the effort!
			/// </remarks>
			/// <remarks>
			///     **CAUTION:** Since this method unloads 'unused' assets, any unloaded asset will have to be reloaded when accessed
			///     again.
			///     Worst case scenario: An editor script that indiscriminately calls this method after common editor operations,
			///     such as selection change events, can severely degrade Editor performance!
			/// </remarks>
			/// <remarks>
			///     Further reading for the curious:
			///     <a href="https://docs.unity3d.com/Manual/AssetDatabaseRefreshing.html">AssetDatabase Refreshing</a>
			/// </remarks>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ImportAssetOptions.html">ImportAssetOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.File.Import(String[],ImportAssetOptions)" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.Refresh.html">AssetDatabase.Refresh</a>
			/// </seealso>
			public static void ImportAll(ImportAssetOptions options = ImportAssetOptions.Default) =>
				AssetDatabase.Refresh(options);
		}
	}
}
