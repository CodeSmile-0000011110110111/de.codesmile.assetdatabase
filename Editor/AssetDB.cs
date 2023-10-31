// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public static class AssetDB
	{
		public const String DefaultAssetExtension = "asset";

		public static String CombineAssetPath(String path, String fileName, String extension = DefaultAssetExtension) =>
			$"{CheckAndNormalizePath(path)}/{fileName}.{extension.TrimStart('.')}";

		/// <summary>
		///     Creates the non-existing folder(s) at the given path and returns the path's GUID.
		///     NOTE: Unlike AssetDatabase.CreateFolder() this version creates folders recursively where necessary,
		///     and it returns a GUID instance, not a GUID string representation.
		/// </summary>
		/// <param name="path">The path that should be created.</param>
		/// <returns>The GUID instance.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static GUID CreateFolder(String path)
		{
			// first check if path already exists
			if (AssetDatabase.IsValidFolder(path))
				return AssetDatabase.GUIDFromAssetPath(path);

			// only if the path doesn't exist the input path gets normalized
			path = CheckAndNormalizePath(path);

			var folderNames = path.Split(new[] { '/' });
			if (folderNames.Length < 2)
				throw new ArgumentException($"path too short: '{path}'");

			var pathGuid = AssetDatabase.GUIDFromAssetPath(folderNames[0]);
			for (var i = 1; i < folderNames.Length; i++)
			{
				var folderPath = AssetDatabase.GUIDToAssetPath(pathGuid);
				var guidString = AssetDatabase.CreateFolder(folderPath, folderNames[i]);
				pathGuid = new GUID(guidString);
			}
			return pathGuid;
		}

		/// <summary>
		///     Creates a new asset file. Generates a unique asset filename to avoid overwriting existing assets.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Object CreateNewAsset(Object obj, String path)
		{
			path = CheckAndNormalizePath(path);

			var uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
			AssetDatabase.CreateAsset(obj, uniquePath);
			return obj;
		}

		/// <summary>
		///     Creates a new asset file. Any existing asset at the given path will be overwritten (replaced).
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="folderPath"></param>
		/// <param name="fileName"></param>
		/// <param name="extension"></param>
		/// <returns></returns>
		public static Object CreateOrReplaceAsset(Object obj, String path)
		{
			path = CheckAndNormalizePath(path);

			AssetDatabase.CreateAsset(obj, path);
			return obj;
		}

		private static String CheckAndNormalizePath(String path)
		{
			CheckPathNotNullOrWhitespace(path);
			path = NormalizePathSeparators(path);
			CheckPathStartsWithAssets(path);
			return path;
		}

		private static void CheckPathStartsWithAssets(String path)
		{
#if DEBUG
			// path must start with "Assets" and it's either just "Assets" (valid) or if the path is longer
			// it must be followed by a path separator eg "Assets/.."
			var startsWithAssets = path.ToLower().StartsWith("assets");
			if (startsWithAssets == false || path.Length > 6 && IsPathSeparator(path[6]) == false)
				throw new ArgumentException("path must start with 'Assets'", nameof(path));

#endif
		}

		private static Boolean IsPathSeparator(Char c) => c.Equals('/') || c.Equals('\\');

		private static void CheckPathNotNullOrWhitespace(String path)
		{
#if DEBUG
			if (path == null)
				throw new ArgumentNullException(nameof(path));
			if (String.IsNullOrWhiteSpace(path))
				throw new ArgumentException(nameof(path), "path is empty or whitespace");
#endif
		}

		private static String NormalizePathSeparators(String folderPath) =>
			folderPath.Replace('\\', '/').TrimStart('/').TrimEnd('/');
	}
}
