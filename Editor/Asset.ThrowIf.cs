// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
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

			public static void AlreadyInDatabase(UnityEngine.Object obj)
			{
				if (IsImported(obj))
					throw new ArgumentException($"object already is an asset file: {obj}");
			}

			public static void AssetPathNotInDatabase(Path path)
			{
				if (path.Exists == false)
					throw new ArgumentException($"path does not exist or not imported: {path}");
			}

			public static void NotInDatabase(UnityEngine.Object obj)
			{
				if (IsImported(obj) == false)
					throw new ArgumentException($"object is not an asset: {obj}");
			}

			public static void NotAnAssetGuid(GUID guid)
			{
				if (Path.Get(guid) == null)
					throw new ArgumentException($"guid is not an asset: {guid}");
			}

			public static void AssetDeleted(Asset asset)
			{
				if (asset.IsDeleted)
					throw new InvalidOperationException("asset has been deleted");
			}

			public static void OverwritingSamePath(Path sourcePath, Path destinationPath, Boolean overwriteExisting)
			{
				if (overwriteExisting && sourcePath.Equals(destinationPath))
				{
					throw new ArgumentException(
						$"destination path must not equal source if overwrite is specified: {destinationPath}");
				}
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

			[ExcludeFromCodeCoverage]
			public static void AssetLoadReturnedNull(Object obj, Path path)
			{
				if (obj == null)
				{
					// path exists + type is known, yet load throws null?
					// Probably ADB or asset in invalid state ...
					throw new AssetLoadException("asset load returned null - this can occur if the AssetDatabase " +
					                             "is currently initializing (eg static ctor) or when importing an asset " +
					                             "async or while ADB is 'paused', or some other reason (please report); " +
					                             $"path: {path}");
				}
			}

			public static void PathIsNotValid(Path destinationPath)
			{
				if (destinationPath == null)
					throw new ArgumentNullException(nameof(destinationPath));
				if (destinationPath.IsValid == false)
					throw new ArgumentException($"destination path is invalid: {GetLastErrorMessage()}");
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
		}
	}
}
