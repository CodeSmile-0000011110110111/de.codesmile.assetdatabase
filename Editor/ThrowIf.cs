// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	internal static class ThrowIf
	{
		public static void ArgumentIsNull(object arg, String argName)
		{
			if (arg == null)
				throw new ArgumentNullException(argName);
		}

		public static void FileOrFolderDoesNotExist(AssetPath assetPath)
		{
			if ((AssetPath.FileExists(assetPath) || AssetPath.FolderExists(assetPath)) == false)
				throw new FileNotFoundException($"file/folder does not exist: '{assetPath}'");
		}

		public static void IsExistingAsset(Object obj)
		{
			if (AssetDatabase.Contains(obj))
				throw new ArgumentException($"object already is an asset file: {obj}");
		}

		public static void NotAnAsset(Object obj)
		{
			if (AssetDatabase.Contains(obj) == false)
				throw new ArgumentException($"object is not an asset file: {obj}");
		}

		public static void NotAnAsset(GUID guid)
		{
			if (AssetDatabase.GUIDToAssetPath(guid).Length == 0)
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
