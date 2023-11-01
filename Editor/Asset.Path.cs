// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public static partial class Asset
	{
		public static class Path
		{
			public const String DefaultAssetExtension = "asset";

			/// <summary>
			///     Creates all folder(s) of the supplied path recursively, as needed.
			///     Differences to AssetDatabase.CreateFolder():
			///     - checks for well-formed path
			///     - creates folders recursively, not just a single subfolder
			///     - returns a GUID instance, not just a GUID string
			///     - checks if the path/folder already exists, and if so, returns the existing folder's GUID
			/// </summary>
			/// <param name="path">The path that should be created.</param>
			/// <returns>The path's GUID instance.</returns>
			/// <exception cref="ArgumentException"></exception>
			public static GUID Create(String path)
			{
				// first check if path already exists
				if (AssetDatabase.IsValidFolder(path))
					return AssetDatabase.GUIDFromAssetPath(path);

				// only if the path doesn't exist the input path gets normalized
				path = ToLogical(path);

				var folderNames = path.Split(new[] { '/' });
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
			///     Combines a path to a folder in the project with a filename and extension to form a valid path string
			///     to an asset file.
			///     Note: Path separators get normalized and trimmed, for example these are valid input paths:
			///     'Assets\\folder'
			///     '\\Assets/folder'
			///     '/Assets/folder\\'
			/// </summary>
			/// <param name="path">Path to a folder for the file. Can be a relative or absolute path.</param>
			/// <param name="fileName">Name of the file without extension.</param>
			/// <param name="extension">Extension of the file (default: 'asset'). Leading dots will be trimmed.</param>
			/// <returns>
			///     The relative path to the asset file. For example if input is 'Assets/folder', 'file'
			///     and 'ext' the returned path will be 'Assets/folder/file.ext'
			/// </returns>
			/// <exception cref="ArgumentException"></exception>
			public static String Combine(String path, String fileName, String extension = DefaultAssetExtension)
			{
				ThrowIfNullOrWhitespace(path, nameof(path));
				ThrowIfNullOrWhitespace(fileName, nameof(fileName));
				ThrowIfNullOrWhitespace(extension, nameof(extension));

				var logicalPath = ToRelative(path).Trim('/');
				return $"{logicalPath}/{fileName}.{extension.TrimStart('.')}";
			}

			/// <summary>
			///     Creates a logical (shortened) path from an absolute path for certain special folders within
			///     a Unity project.
			///     See: <a href="https://docs.unity3d.com/ScriptReference/FileUtil.GetLogicalPath.html">FileUtil.GetLogicalPath</a>
			///     Notes: Any remaining leading path separator gets trimmed.
			///     Path separators are normalized to forward slashes.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static String ToLogical(String path)
			{
				ThrowIfNullOrWhitespace(path, nameof(path));
				return FileUtil.GetLogicalPath(path).TrimStart('/');
			}

			/// <summary>
			///     Strips an absolute path of their "full" part so we end up with "Assets/something" when the input,
			///     for example, is: 'C:\Users\Urso Clever\Untiy Projects\First Porject\Assets\something'
			///     Notes: Input path is required to point to the project's root or below.
			///     Path separators are normalized to forward slashes.
			/// </summary>
			/// <param name="fullPath">Absolute path to project's root folder or a subfolder.</param>
			/// <exception cref="ArgumentException">
			///     If path does not point to the current project's root folder or one of its subfolders.
			/// </exception>
			/// <returns></returns>
			public static String ToRelative(String fullPath)
			{
				ThrowIfNullOrWhitespace(fullPath, nameof(fullPath));
				var normalizedPath = fullPath.NormalizePathSeparators();

				if (IsRelative(normalizedPath))
					return normalizedPath.TrimStart('/');

				ThrowIfNotAProjectPath(normalizedPath, fullPath);
				return normalizedPath.Substring(Application.dataPath.Length - 6);
			}

			/// <summary>
			///     Returns true if the path is a relative path.
			///     A relative path starts with either 'Assets/' or '/Assets/' (any path separator), or is just 'Assets'.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean IsRelative(String path)
			{
				if (String.IsNullOrWhiteSpace(path))
					return false;

				// path must start with "Assets" and it's either just "Assets" (length == 6) or if the path is longer
				// it must be followed by a path separator, eg "Assets/"
				var normalizedPath = path.NormalizePathSeparators().TrimStart('/');
				var startsWithAssets = normalizedPath.ToLower().StartsWith("assets");
				return startsWithAssets && (normalizedPath.Length <= 6 || IsPathSeparator(normalizedPath[6]));
			}

			// GetMainAssetType (path, guid)
			// getsubfolders
			// makeunique
			public static String Get(Object obj) => AssetDatabase.GetAssetPath(obj);
			public static String Get(GUID guid) => AssetDatabase.GUIDToAssetPath(guid);
			public static String Get(String guid) => AssetDatabase.GUIDToAssetPath(guid);
			public static String Get(Int32 instanceId) => AssetDatabase.GetAssetPath(instanceId);

			private static void ThrowIfNotAProjectPath(String normalizedPath, String fullPath)
			{
				var dataPath = Application.dataPath;
				if (normalizedPath.StartsWith(dataPath) == false)
				{
					throw new ArgumentException(
						$"not a project path: '{fullPath}' - should start with: {dataPath.Substring(0, dataPath.Length - 6)}");
				}
			}

			private static void ThrowIfNullOrWhitespace(String param, String paramName)
			{
#if DEBUG
				if (param == null)
					throw new ArgumentNullException($"{paramName} is null");
				if (String.IsNullOrWhiteSpace(param))
					throw new ArgumentException($"{paramName} is empty or whitespace");
#endif
			}

			private static Boolean IsPathSeparator(Char c) => c.Equals('/') || c.Equals('\\');
		}
	}
}
