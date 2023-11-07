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
		[ExcludeFromCodeCoverage]
		internal static class ThrowIf
		{
			public static void ArgumentIsNull(Object arg, String argName)
			{
				if (arg == null)
					throw new ArgumentNullException(argName);
			}

			public static void DoesNotExistInFileSystem(Path path)
			{
				if (path.ExistsInFileSystem == false)
					throw new FileNotFoundException($"file/folder does not exist: '{path}'");
			}

			public static void ExistingAsset(UnityEngine.Object obj)
			{
				if (Database.Contains(obj))
					throw new ArgumentException($"object already is an asset file: {obj}");
			}

			public static void AssetPathNotInDatabase(Path path)
			{
				if (path.Exists == false)
					throw new ArgumentException($"path does not exist or not imported: {path}");
			}

			public static void NotInDatabase(UnityEngine.Object obj, Path path)
			{
				if (Database.Contains(obj) == false)
					throw new ArgumentException($"path does not exist or not imported: {path}");
			}

			public static void NotInDatabase(UnityEngine.Object obj)
			{
				if (Database.Contains(obj) == false)
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

			public static void GenericTypeNotAssignableFromAssetType<T>(Path path) where T : UnityEngine.Object
			{
				var assetType = GetMainType(path);
				if (typeof(T).IsAssignableFrom(assetType) == false)
					throw new ArgumentException($"'{typeof(T)}' not assignable from asset type '{assetType}'");
			}
		}
	}
}
