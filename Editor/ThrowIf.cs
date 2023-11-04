// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;

namespace CodeSmile.Editor
{
	internal static class ThrowIf
	{
		public static void ArgumentIsNull(Object arg, String argName)
		{
			if (arg == null)
				throw new ArgumentNullException(argName);
		}

		public static void DoesNotExist(AssetPath assetPath)
		{
			if (assetPath.Exists == false)
				throw new FileNotFoundException($"file/folder does not exist: '{assetPath}'");
		}

		public static void IsExistingAsset(UnityEngine.Object obj)
		{
			if (Asset.Database.Contains(obj))
				throw new ArgumentException($"object already is an asset file: {obj}");
		}

		public static void NotAnAssetAtPath(UnityEngine.Object obj, AssetPath path)
		{
			if (Asset.Database.Contains(obj) == false)
				throw new ArgumentException("path does not exist or not imported", path);
		}

		public static void NotAnAsset(UnityEngine.Object obj)
		{
			if (Asset.Database.Contains(obj) == false)
				throw new ArgumentException($"object is not an asset file: {obj}");
		}

		public static void NotAnAsset(GUID guid)
		{
			if (AssetPath.Get(guid) == null)
				throw new ArgumentException($"guid {guid} is not an asset file");
		}

		public static void NotAProjectPath(String normalizedPath, String fullPath)
		{
			var rootPath = AssetPath.FullProjectPath;
			if (normalizedPath.StartsWith(rootPath) == false)
				throw new ArgumentException($"not a project path: '{fullPath}' - must start with: {rootPath}");
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
