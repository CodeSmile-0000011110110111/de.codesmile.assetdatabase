// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Contains error- and sanity-checking methods for the entire Asset group of classes.
		/// </summary>
		internal static class ThrowIf
		{
			public static void ArgumentIsNull(Object obj, String argName)
			{
				if (obj == null)
					throw new ArgumentNullException(argName);
			}

			public static void DoesNotExistInFileSystem(Path path)
			{
				if (path.ExistsInFileSystem == false)
					throw new FileNotFoundException($"path does not exist: '{path}'");
			}

			public static void AlreadyAnAsset(UnityEngine.Object obj)
			{
				if (Status.IsImported(obj))
					throw new ArgumentException($"object already is an asset file: {obj}");
			}

			public static void AssetPathNotInDatabase(Path path)
			{
				if (path.Exists == false)
					throw new ArgumentException($"path does not exist or not imported: {path}");
			}

			public static void NotInDatabase(UnityEngine.Object obj)
			{
				if (Status.IsImported(obj) == false)
					throw new ArgumentException($"object is not an asset: {obj}");
			}

			public static void NotAnAssetGuid(GUID guid)
			{
				if (Path.Get(guid) == null)
					throw new ArgumentException($"guid is not an asset: {guid}");
			}

			/*
			public static void AssetDeleted(Asset asset)
			{
				if (asset.IsDeleted)
					throw new InvalidOperationException("asset has been deleted");
			}

			public static void AssetNotImported(Path path, Type assetType)
			{
				if (assetType == null)
				{
					// path exists in file system yet ADB does not know its type - missing Import() ?
					throw new AssetLoadException("cannot load asset - file exists but asset type is null, " +
					                             $"most likely the asset has not been imported; path: {path}");
				}
			}

			public static void AssetTypeMismatch<T>(Path path, Type assetType) where T : UnityEngine.Object
			{
				if (typeof(T).IsAssignableFrom(assetType) == false)
				{
					// types just don't match
					throw new AssetLoadException($"cannot load asset, type mismatch: {typeof(T)} " +
					                             $"not assignable from asset type: {assetType.FullName}; path: {path}");
				}
			}
			*/

			public static void AssetLoadReturnedNull(UnityEngine.Object obj, Path path)
			{
				if (obj == null)
				{
					// path exists + type is known, yet load throws null?
					// Probably ADB or asset in invalid state ...
					throw new AssetLoadException("asset load returned null - this can occur if the AssetDatabase " +
					                             "is currently initializing (eg static ctor) or when importing an asset " +
					                             "async or while ADB is 'paused', or if the type does not math, or " +
					                             $"some other reason (please report); path: {path}");
				}
			}

			public static void PathIsNotValid(String path)
			{
				if (Path.IsValid(path) == false)
					throw new ArgumentException($"invalid path: {GetLastErrorMessage()}");
			}

			public static void NotAProjectPath(String fullPath)
			{
				var rootPath = Path.FullProjectPath;
				if (fullPath.StartsWith(rootPath) == false)
				{
					throw new ArgumentException(
						$"invalid relative or project path: '{fullPath}' - relative paths must start with 'Assets', full paths must include the project's root directory");
				}
			}

			public static void NullOrWhitespace(String param, String paramName)
			{
				if (param == null)
					throw new ArgumentNullException($"{paramName} is null");
				if (String.IsNullOrWhiteSpace(param))
					throw new ArgumentException($"{paramName} is empty or whitespace");
			}

			public static void ContainsPathSeparators(String fileName, String paramName)
			{
				var normalized = fileName.ToForwardSlashes();
				if (normalized.Contains('/'))
					throw new ArgumentException($"filename contains path separators: '{fileName}'", paramName);
			}

			public static void SubObjectIsGameObject(UnityEngine.Object subObject)
			{
				if (subObject is GameObject go)
					throw new ArgumentException($"sub assets must not be of type GameObject: {subObject}");
			}

			public static void NotAnAssetWithAssetExtension(UnityEngine.Object assetObject)
			{
				var path = Path.Get(assetObject);
				if (path.Extension.Equals(".asset") == false)
					throw new ArgumentException("sub assets only supported with '.asset' extension: {path}");
			}

			public static void ExtensionIsNotUnityPackage(Path path)
			{
				if (path.Extension.ToLower().Equals(".unitypackage") == false)
					throw new ArgumentException($"file does not have .unitypackage extension: {path}");
			}

			public static void SourceAndDestPathAreEqual(Path sourcePath, Path destinationPath)
			{
				if (sourcePath.Equals(destinationPath))
					throw new ArgumentException($"source and destination path are equal: {sourcePath}");
			}

			/*public static void NotAnAsset(UnityEngine.Object obj)
			{
				if (Database.Contains(obj) == false)
					throw new ArgumentException($"{obj} is not an asset file");
			}

			public static void NotAnAsset(Int32 instanceId)
			{
				if (Database.Contains(instanceId) == false)
					throw new ArgumentException($"{instanceId} is not an asset instance ID");
			}*/
		}
	}
}
