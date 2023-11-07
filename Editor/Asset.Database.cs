// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static class Database
		{
			/// <summary>
			///     Checks if the object is an asset in the AssetDatabase.
			///     Unlike AssetDatabase, will not throw a NullRef if you pass null.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns>Returns false if the object isn't in the database or if the object is null.</returns>
			public static Boolean Contains(Object obj) => obj ? AssetDatabase.Contains(obj) : false;

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

			// SaveAll

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