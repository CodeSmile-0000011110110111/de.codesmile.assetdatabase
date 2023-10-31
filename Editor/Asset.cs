// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public static partial class Asset
	{
		/// <summary>
		///     Creates a new asset file. Defaults to generating a unique filename in case there is an
		///     existing asset file with the same path.	Can overwrite existing files if specified.
		/// </summary>
		/// <param name="obj">The object to save as an asset file.</param>
		/// <param name="path">The relative asset path with filename and extension.</param>
		/// <param name="overwriteExisting">(Default: false) If true, any existing asset file will be overwritten.</param>
		/// <returns></returns>
		public static Object Create(Object obj, String path, Boolean overwriteExisting = false)
		{
			var assetPath = Path.ToLogical(path);

			if (overwriteExisting == false)
				assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

			AssetDatabase.CreateAsset(obj, assetPath);
			return obj;
		}

		// import, (can) move, trash, rename, copy, delete
		// load, save, open

		public static class Labels {}
		public static class SubAsset {}
		public static class Meta {}
		public static class Load {}

		public static class DB
		{
			/// <summary>
			///     Speed up mass asset editing by using the supplied callback action.
			///     Within the callback, the AssetDatabase does neither	import nor auto refresh assets.
			///     This can significantly speed up mass asset editing.
			///     Internally calls Start/StopAssetEditing and Disallow/AllowAutoRefresh in a try/finally
			///     block so that exceptions will not cause the AssetDatabase to remain stopped indefinitely.
			/// </summary>
			/// <param name="batchAssetEditing"></param>
			public static void BatchEditing(Action batchAssetEditing)
			{
				try
				{
					AssetDatabase.DisallowAutoRefresh();
					AssetDatabase.StartAssetEditing();

					batchAssetEditing?.Invoke();
				}
				finally
				{
					AssetDatabase.StopAssetEditing();
					AssetDatabase.AllowAutoRefresh();
				}
			}

			public static class Package
			{
				//Import
				//Export
			}

			public static class VCS // VersionControl
			{
				/*
	CanOpenAssetInEditor
	CanOpenForEdit
	IsMetaFileOpenForEdit
	IsOpenForEdit
	MakeEditable
				 */
			}

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
