// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static class Database
		{
			public static Boolean Contains(Object obj) => AssetDatabase.Contains(obj);

			/// <summary>
			///     Speed up mass asset editing (create, modify, delete, import, etc).
			///     Within the callback action, the AssetDatabase does neither import nor auto-refresh assets.
			///     This can significantly speed up mass asset editing tasks where you work with individual assets
			///     in a loop.
			///     Internally calls Start/StopAssetEditing in a try/finally block so that exceptions will
			///     not cause the AssetDatabase to remain stopped indefinitely.
			/// </summary>
			/// <param name="assetEditingAction"></param>
			public static void BatchEditing(Action assetEditingAction)
			{
				try
				{
					DisallowAutoRefresh();
					StartAssetEditing();

					assetEditingAction?.Invoke();
				}
				finally
				{
					StopAssetEditing();
					AllowAutoRefresh();
				}
			}

			public static void AllowAutoRefresh() => AssetDatabase.AllowAutoRefresh();
			public static void DisallowAutoRefresh() => AssetDatabase.DisallowAutoRefresh();

			/// <summary>
			///     Internal on purpose: use Asset.BatchEditing(Action) instead
			/// </summary>
			internal static void StartAssetEditing() => AssetDatabase.StartAssetEditing();

			/// <summary>
			///     Internal on purpose: use Asset.BatchEditing(Action) instead
			/// </summary>
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
