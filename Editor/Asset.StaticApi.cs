// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Creates a new asset file. Defaults to generating a unique filename in case there is an
		///     existing asset file with the same path.	Can overwrite existing files if specified.
		/// </summary>
		/// <param name="obj">The object to save as an asset file.</param>
		/// <param name="path">The relative asset path with filename and extension.</param>
		/// <param name="overwriteExisting">(Default: false) If true, any existing asset file will be overwritten.</param>
		/// <returns></returns>
		public static Asset Create(Object obj, AssetPath assetPath, Boolean overwriteExisting = false)
		{
			AssetDatabase.CreateAsset(obj, overwriteExisting ? assetPath : assetPath.UniquePath);
			return new Asset(obj);
		}

		public static Asset Create(Object obj, String path, Boolean overwriteExisting = false) =>
			Create(obj, (AssetPath)path, overwriteExisting);

		public static T LoadFirst<T>(AssetPath assetPath) where T : Object =>
			AssetDatabase.LoadAssetAtPath<T>(assetPath);

		public static T LoadMain<T>(AssetPath assetPath) where T : Object =>
			(T)AssetDatabase.LoadMainAssetAtPath(assetPath);

		public static T LoadMain<T>(GUID guid) where T : Object => LoadMain<T>(Path.Get(guid));

		public static Object[] LoadAll(AssetPath assetPath) => AssetDatabase.LoadAllAssetsAtPath(assetPath);

		public static Object[] LoadOnlyVisible(AssetPath assetPath) =>
			AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

		public static Boolean SaveAs(AssetPath sourcePath, AssetPath destPath, Boolean overwriteExisting = false) =>
			AssetDatabase.CopyAsset(sourcePath, GetTargetPath(destPath, overwriteExisting));

		private static AssetPath GetTargetPath(AssetPath destPath, Boolean overwriteExisting) =>
			overwriteExisting ? destPath : destPath.UniquePath;

		public static class SubAsset
		{
			public static Boolean Extract(Object subAsset, AssetPath extractedAssetPath, out String errorMessage)
			{
				errorMessage = AssetDatabase.ExtractAsset(subAsset, extractedAssetPath);
				return errorMessage.Equals(String.Empty);
			}
		}

		public static class Status
		{
			public static Boolean IsForeignAsset(Object obj) => AssetDatabase.IsForeignAsset(obj);
			public static Boolean IsNativeAsset(Object obj) => AssetDatabase.IsNativeAsset(obj);
			public static Boolean IsSubAsset(Object obj) => AssetDatabase.IsSubAsset(obj);
			public static Boolean IsMainAsset(Object obj) => AssetDatabase.IsMainAsset(obj);
			public static Boolean IsLoaded(Object obj) => AssetDatabase.IsMainAssetAtPathLoaded(Path.Get(obj));

			public static Boolean IsLoaded(AssetPath assetPath) => AssetDatabase.IsMainAssetAtPathLoaded(assetPath);
		}

		public static class Path
		{
			/// <summary>
			/// Returns true if the folder exists. Also works with paths pointing to a file.
			/// </summary>
			/// <param name="assetPath">path to a file or folder</param>
			/// <returns>true if the folder exists</returns>
			public static Boolean FolderExists(AssetPath assetPath)
			{
				ThrowIf.ArgumentIsNull(assetPath, nameof(assetPath));
				return AssetDatabase.IsValidFolder(assetPath.FolderPath);
			}

			/// <summary>
			/// Creates the folders in the path recursively. Path may point to a file, but only folders
			/// will be created.
			/// </summary>
			/// <param name="assetPath">path to a file or folder</param>
			/// <returns>the GUID of the deepest folder in the hierarchy</returns>
			public static GUID CreateFolders(AssetPath assetPath)
			{
				ThrowIf.ArgumentIsNull(assetPath, nameof(assetPath));
				if (FolderExists(assetPath))
					return Guid.Get(assetPath.FolderPath);

				var folderNames = assetPath.FolderPath.Split(new[] { '/' });
				var folderGuid = Guid.Get(folderNames[0]); // first is "Assets"
				var partialPath = folderNames[0];
				for (var i = 1; i < folderNames.Length; i++)
				{
					partialPath += $"/{folderNames[i]}";
					if (AssetDatabase.IsValidFolder(partialPath))
						continue;

					var guidString = AssetDatabase.CreateFolder(Get(folderGuid), folderNames[i]);
					folderGuid = new GUID(guidString);
				}

				return folderGuid;
			}

			public static AssetPath Get(Object obj) => (AssetPath)AssetDatabase.GetAssetPath(obj);

			public static AssetPath Get(GUID guid) => (AssetPath)AssetDatabase.GUIDToAssetPath(guid);
		}

		public static class Guid
		{
			public static GUID Get(String path) =>
				new(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));

			public static GUID Get(AssetPath assetPath) =>
				new(AssetDatabase.AssetPathToGUID(assetPath, AssetPathToGUIDOptions.OnlyExistingAssets));
		}

		// import, (can) move, trash, rename, copy, delete
		// load, save, open

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
