// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor.old
{
	public static partial class AssetOld
	{
		/// <summary>
		///     Creates a new asset file. Defaults to generating a unique filename in case there is an
		///     existing asset file with the same path.	Can overwrite existing files if specified.
		/// </summary>
		/// <param name="obj">The object to save as an asset file.</param>
		/// <param name="path">The relative asset path with filename and extension.</param>
		/// <param name="overwriteExisting">(Default: false) If true, any existing asset file will be overwritten.</param>
		/// <returns></returns>
		public static Object Create(Object obj, AssetPath assetPath, Boolean overwriteExisting = false)
		{
			AssetDatabase.CreateAsset(obj, overwriteExisting ? assetPath : assetPath.UniqueFilePath);
			return obj;
		}

		public static Object Create(Object obj, String path, Boolean overwriteExisting = false) =>
			Create(obj, (AssetPath)path, overwriteExisting);

		// import, (can) move, trash, rename, copy, delete
		// load, save, open

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
				Database.DisallowAutoRefresh();
				Database.StartAssetEditing();

				assetEditingAction?.Invoke();
			}
			finally
			{
				Database.StopAssetEditing();
				Database.AllowAutoRefresh();
			}
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

		public static class Database
		{
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
	}
}