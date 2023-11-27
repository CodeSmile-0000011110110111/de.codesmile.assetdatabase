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
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.DesiredWorkerCount.html">AssetDatabase.DesiredWorkerCount</a>
			/// </seealso>
			/// <seealso cref="">
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ForceToDesiredWorkerCount.html">AssetDatabase.ForceToDesiredWorkerCount</a>
			/// </seealso>
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
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsDirectoryMonitoringEnabled.html">AssetDatabase.IsDirectoryMonitoringEnabled</a>
			/// </seealso>
			public static Boolean DirectoryMonitoring
			{
				get => AssetDatabase.IsDirectoryMonitoringEnabled();
				set => EditorPrefs.SetBool("DirectoryMonitoring", value);
			}

			/// <summary>
			///     Tests if the asset is in the database.
			/// </summary>
			/// <param name="obj">Asset to test.</param>
			/// <returns>Returns true if the asset is imported. Returns false if the asset is not in the database, or if obj is null.</returns>
			/// <seealso cref="">
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.Contains.html">AssetDatabase.Contains</a>
			/// </seealso>
			public static Boolean Contains(Object obj) => obj != null ? AssetDatabase.Contains(obj) : false;

			/// <summary>
			///     Tests if the asset is in the database.
			/// </summary>
			/// <param name="instanceId">The instance ID of an asset.</param>
			/// <returns>Returns true if the asset is imported. Returns false if the asset is not in the database.</returns>
			/// <seealso cref="">
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.Contains.html">AssetDatabase.Contains</a>
			/// </seealso>
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
			/// <seealso cref="CodeSmile.Editor.Asset.Database.DisallowAutoRefresh" />
			/// <seealso cref="">
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
			/// <seealso cref="CodeSmile.Editor.Asset.Database.AllowAutoRefresh" />
			/// <seealso cref="">
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
			/// <seealso
			///     cref="CodeSmile.Editor.Asset.Database.ForceReserialize(IEnumerable{string},UnityEditor.ForceReserializeAssetsOptions)" />
			/// <seealso cref="">
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
			/// <param name="options"></param>
			/// <seealso cref="CodeSmile.Editor.Asset.Database.ForceReserializeAll" />
			/// <seealso
			///     cref="CodeSmile.Editor.Asset.Database.ForceReserialize(IEnumerable{String},UnityEditor.ForceReserializeAssetsOptions)" />
			/// <seealso cref="">
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
			/// <param name="options"></param>
			/// <seealso cref="CodeSmile.Editor.Asset.Database.ForceReserializeAll" />
			/// <seealso
			///     cref="CodeSmile.Editor.Asset.Database.ForceReserialize(IEnumerable{String},UnityEditor.ForceReserializeAssetsOptions)" />
			/// <seealso cref="">
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
			/// <see cref="CodeSmile.Editor.Asset.File.Save" />
			/// <see cref="CodeSmile.Editor.Asset.File.ForceSave" />
			/// <see cref="CodeSmile.Editor.Asset.File.BatchEditing" />
			/// <seealso cref="">
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SaveAssets.html">AssetDatabase.SaveAssets</a>
			/// </seealso>
			public static void SaveAll() => AssetDatabase.SaveAssets();

			/// <summary>
			///     Known as 'Refresh' this scans for and imports assets that have been modified *externally*.
			///     Also unloads unused resources!
			/// </summary>
			/// <remarks>
			///     IMPORTANT: The 'Refresh' method has traditionally been largely misunderstood and over-used.
			///     Calling Refresh where it is unnecessary can lead to degraded Editor performance due to unloading 'unused'
			///     assets. Those assets will then need to be reloaded when accessed again. A simple editor script that
			///     calls 'Refresh' after common editor operations, for example on every Selection change,
			///     can severely degrade Editor performance!
			/// </remarks>
			/// <remarks>
			///     When to call ImportAll / 'Refresh'?
			///     - After System.IO.* operations modified files/folders in the project.
			///     - After running an external process that modified files/folders in the project.
			///     There is no need to call ImportAll / 'Refresh' in any other situation! REALLY!!
			/// </remarks>
			/// <remarks>
			///     This needs to be stressed: when you use only CodeSmile.Editor.Asset methods to modify the file
			///     system you do NOT need to call ImportAll. Likewise it is unnecessary to call 'Refresh' after using
			///     only AssetDatabase methods.
			/// </remarks>
			/// <remarks>
			///     Further reading for the curious:
			///     <a href="https://docs.unity3d.com/Manual/AssetDatabaseRefreshing.html">AssetDatabase Refreshing</a>
			/// </remarks>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ImportAssetOptions.html">ImportAssetOptions</a>
			/// </param>
			/// <seealso cref="">
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.Refresh.html">AssetDatabase.Refresh</a>
			/// </seealso>
			public static void ImportAll(ImportAssetOptions options = ImportAssetOptions.Default) =>
				AssetDatabase.Refresh(options);
		}
	}
}
