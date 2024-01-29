// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmileEditor
{
	public sealed partial class Asset
	{
		public partial class Path
		{
			// all lowercase
			private static readonly String[] s_AllowedAssetSubfolders =
				{ "assets", "library", "logs", "packages", "projectsettings", "temp", "usersettings" };

			/// <summary>
			///     Returns the absolute path to the project's <c>Assets</c> subfolder.
			/// </summary>
			/// <seealso cref="">
			///     - <a href="https://docs.unity3d.com/ScriptReference/Application-dataPath.html">Application.dataPath</a>
			/// </seealso>
			public static String FullAssetsPath => Application.dataPath;

			/// <summary>
			///     Returns the absolute path to the project's <c>Packages</c> subfolder.
			/// </summary>
			public static String FullPackagesPath => $"{FullProjectPath}/Packages";

			/// <summary>
			///     Returns the absolute path to the project's <c>Library</c> subfolder.
			/// </summary>
			[ExcludeFromCodeCoverage] // returns string interpolation
			public static String FullLibraryPath => $"{FullProjectPath}/Library";

			/// <summary>
			///     Returns the absolute path to the project's <c>Logs</c> subfolder.
			/// </summary>
			[ExcludeFromCodeCoverage] // returns string interpolation
			public static String FullLogsPath => $"{FullProjectPath}/Logs";

			/// <summary>
			///     Returns the absolute path to the project's <c>ProjectSettings</c> subfolder.
			/// </summary>
			[ExcludeFromCodeCoverage] // returns string interpolation
			public static String FullProjectSettingsPath => $"{FullProjectPath}/ProjectSettings";

			/// <summary>
			///     Returns the absolute path to the project's <c>UserSettings</c> subfolder.
			/// </summary>
			[ExcludeFromCodeCoverage] // returns string interpolation
			public static String FullUserSettingsPath => $"{FullProjectPath}/UserSettings";

			/// <summary>
			///     Returns the absolute path to the project's <c>Temp</c> subfolder.
			/// </summary>
			[ExcludeFromCodeCoverage] // returns string interpolation
			public static String FullProjectTempPath => $"{FullProjectPath}/Temp";

			/// <summary>
			///     Returns the absolute path to the project's root folder.
			/// </summary>
			public static String FullProjectPath => FullAssetsPath.Substring(0, Application.dataPath.Length - 6);

			/// <summary>
			///     Gets the relative path of an asset.
			/// </summary>
			/// <param name="asset">Instance of an asset.</param>
			/// <returns>The relative path to the asset file, or null if the object is not an asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.Get(GUID)" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetPath.html">AssetDatabase.GetAssetPath</a>
			/// </seealso>
			public static Path Get([NotNull] Object asset)
			{
				var path = AssetDatabase.GetAssetPath(asset);
				return String.IsNullOrEmpty(path) ? null : (Path)path;
			}

			/// <summary>
			///     Gets the relative path of an asset.
			/// </summary>
			/// <param name="guid">GUID of an asset.</param>
			/// <returns>The relative path to the asset file, or null if the object is not an asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.Get(Object)" />
			///     - <see cref="CodeSmileEditor.Asset.Path.Get(Object[])" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetPath.html">AssetDatabase.GetAssetPath</a>
			/// </seealso>
			public static Path Get(GUID guid)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				return String.IsNullOrEmpty(path) ? null : (Path)path;
			}

			/// <summary>
			///     Converts an array of asset instances to their asset paths.
			/// </summary>
			/// <param name="assets">Asset instances.</param>
			/// <returns>
			///     An array of paths for each input object. The returned array has the same size. Items can be null if the input
			///     object was either null or not an asset.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.Get(Object)" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetPath.html">AssetDatabase.GetAssetPath</a>
			/// </seealso>
			public static String[] Get([NotNull] Object[] assets)
			{
				ThrowIf.ArgumentIsNull(assets, nameof(assets));

				var objectCount = assets.Length;
				var paths = new String[objectCount];
				for (var i = 0; i < objectCount; i++)
					paths[i] = Get(assets[i]);

				return paths;
			}

			/// <summary>
			///     Returns the GUID for an asset path.
			/// </summary>
			/// <param name="path">Path to an asset.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetPathToGUIDOptions.html">AssetPathToGUIDOptions</a>
			/// </param>
			/// <returns>GUID of the asset or an empty GUID if the path does not exist in the database.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.Exists" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.AssetPathToGUID.html">AssetDatabase.AssetPathToGUID</a>
			/// </seealso>
			public static GUID GetGuid([NotNull] Path path,
				AssetPathToGUIDOptions options = AssetPathToGUIDOptions.IncludeRecentlyDeletedAssets) =>
				new(AssetDatabase.AssetPathToGUID(path, options));

			/// <summary>
			///     Returns the .meta file path for an asset path.
			/// </summary>
			/// <param name="path">Path to an asset.</param>
			/// <returns>The corresponding .meta file path.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.FromMeta" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetTextMetaFilePathFromAssetPath.html">AssetDatabase.GetTextMetaFilePathFromAssetPath</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Path ToMeta([NotNull] Path path) => AssetDatabase.GetTextMetaFilePathFromAssetPath(path);

			/// <summary>
			///     Returns the asset's file path from a .meta file path.
			/// </summary>
			/// <param name="path">Path to a .meta file.</param>
			/// <returns>The corresponding path to an asset file or folder.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.ToMeta" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetPathFromTextMetaFilePath.html">AssetDatabase.GetAssetPathFromTextMetaFilePath</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Path FromMeta([NotNull] Path path) => AssetDatabase.GetAssetPathFromTextMetaFilePath(path);

			/// <summary>
			///     Returns the scene's path if the object is instantiated in a scene, otherwise returns the object's path.
			/// </summary>
			/// <param name="instanceOrAsset">An object instance or asset.</param>
			/// <returns>The scene path if the object is an instance in the scene. Otherwise the asset's path.</returns>
			/// <seealso cref="">
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetOrScenePath.html">AssetDatabase.GetAssetOrScenePath</a>
			/// </seealso>
			public static Path GetScene([NotNull] Object instanceOrAsset) => AssetDatabase.GetAssetOrScenePath(instanceOrAsset);

			/// <summary>
			///     Returns true if the provided path is valid.
			/// </summary>
			/// <remarks>
			///     If this returns false CodeSmileEditor.Asset.GetLastErrorMessage contains the error message.
			/// </remarks>
			/// <param name="path">String representation of an absolute or relative path.</param>
			/// <returns>
			///     True if the string is a valid path and contains no illegal characters for a path or file, and isn't too long.
			///     False in all other cases.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			/// </seealso>
			public static Boolean IsValid([NotNull] String path)
			{
				var isValid = true;

				try
				{
					// System.IO will throw for most illegal chars, plus some extra checks
					var fileName = System.IO.Path.GetFileName(path);
					var folderName = System.IO.Path.GetDirectoryName(path);

					// check folder name for some chars that System.IO allows in GetDirectoryName
					var testIllegalChars = new Func<Char, Boolean>(c => c == '*' || c == '?' || c == ':');
					isValid = folderName.Any(testIllegalChars) == false;

					if (isValid)
					{
						// check filename for some chars that System.IO allows in GetFileName
						fileName = path.Substring(folderName.Length, path.Length - folderName.Length);
						isValid = fileName.Any(testIllegalChars) == false;
					}
				}
				catch (Exception ex)
				{
					SetLastErrorMessage($"{ex.Message} => \"{path}\"");
					isValid = false;
				}

				return isValid;
			}

			/// <summary>
			///     Tests if the given file exists in the file system.
			/// </summary>
			/// <param name="path">Path to a file.</param>
			/// <returns>True if the file exists in the file system. False otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.FolderExists" />
			/// </seealso>
			public static Boolean FileExists([NotNull] Path path) => System.IO.File.Exists(path.m_RelativePath);

			/// <summary>
			///     Tests if the given folder exists in the file system.
			/// </summary>
			/// <param name="path">Path to a folder.</param>
			/// <returns>True if the folder exists in the file system. False otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.FileExists" />
			/// </seealso>
			public static Boolean FolderExists([NotNull] Path path) => Directory.Exists(path.m_RelativePath);

			/// <summary>
			///     Creates any missing folders in the path.
			/// </summary>
			/// <remarks>Unlike AssetDatabase.CreateFolder this creates the entire path in one go rather than each folder one by one.</remarks>
			/// <remarks> Path may point to either a file or folder. If the last path element has an extension it is considered a file. </remarks>
			/// <param name="path">Path to a file or folder.</param>
			/// <returns>GUID of the deepest (last) folder in the path.</returns>
			/// <seealso cref="">
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CreateFolder.html">AssetDatabase.CreateFolder</a>
			/// </seealso>
			public static GUID CreateFolders([NotNull] Path path)
			{
				ThrowIf.ArgumentIsNull(path, nameof(path));
				ThrowIf.PathIsNotValid(path);

				if (FileExists(path))
					return path.FolderPath.Guid;

				// if the last part has an extension we assume the path points to a file
				var isPresumablyFilePath = String.IsNullOrEmpty(path.Extension) == false;
				var folderPath = isPresumablyFilePath ? path.FolderPath : path;
				if (FolderExists(folderPath))
					return folderPath.Guid;

				var folderNames = ((String)folderPath).Split(new[] { '/' });
				var folderGuid = GuidForExistingPath(folderNames[0]); // first is "Assets"
				var partialPath = folderNames[0];
				for (var i = 1; i < folderNames.Length; i++)
				{
					partialPath += $"/{folderNames[i]}";
					if (FolderExists(partialPath))
					{
						folderGuid = GuidForExistingPath(partialPath);
						continue;
					}

					folderGuid = CreateSubFolder(Get(folderGuid), folderNames[i]);
				}

				return folderGuid;
			}

			/// <summary>
			///     Returns the names of all subfolders in the path.
			/// </summary>
			/// <param name="path">Path to a folder.</param>
			/// <returns>Names of each subfolder in the path. Empty array if there are no subfolders or the path points to a file.</returns>
			/// <seealso cref="">
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetSubFolders.html">AssetDatabase.GetSubFolders</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static String[] GetSubFolders([NotNull] Path path) => AssetDatabase.GetSubFolders(path);

			/// <summary>
			///     Returns the path altered with a numbering if an asset already exists (and is imported) at the path.
			/// </summary>
			/// <remarks>
			///     See also: Project Settings => Editor => Numbering Scheme
			/// </remarks>
			/// <remarks>
			///     PS: "Uniquify" is a proper english verb. It means "to make unique". Methods carrying this verb
			///     are commonly found in SQL database APIs.
			/// </remarks>
			/// <param name="path">The input path.</param>
			/// <returns>The path possibly altered with a number in the last path element.</returns>
			/// <seealso cref="">
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GenerateUniqueAssetPath.html">AssetDatabase.GenerateUniqueAssetPath</a>
			/// </seealso>
			public static Path UniquifyFileName([NotNull] Path path)
			{
				var uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
				return String.IsNullOrEmpty(uniquePath) ? path : uniquePath;
			}

			/// <summary>
			///     Converts an IEnumerable collection of Path instances to a string array.
			/// </summary>
			/// <param name="paths">Input paths.</param>
			/// <returns>Relative paths as strings.</returns>
			public static String[] ToStrings([NotNull] IEnumerable<Path> paths) => paths.Select(path => (String)path).ToArray();

			internal static Path UniquifyAsNeeded([NotNull] Path path, Boolean overwriteExisting) =>
				overwriteExisting ? path : path.UniqueFilePath;

			private static GUID CreateSubFolder([NotNull] Path parentFolder, [NotNull] String subFolderName) =>
				new(AssetDatabase.CreateFolder(parentFolder, subFolderName));

			private static String ToRelative([NotNull] String fullOrRelativePath)
			{
				var relativePath = fullOrRelativePath;
				if (IsRelative(relativePath) == false)
				{
					ThrowIf.NotAProjectPath(fullOrRelativePath);
					relativePath = MakeRelative(fullOrRelativePath);
				}

				relativePath = relativePath.Trim('/');

				ThrowIf.PathIsNotValid(relativePath);
				return relativePath;
			}

			private static Boolean IsRelative([NotNull] String path)
			{
				path = path.TrimStart('/').ToLower();

				// path must start with given project root subfolder names (eg 'Assets', 'Packages', 'Library' ..)
				// and bei either just the subfolder (length equals) or be followed by a path separator
				foreach (var allowedSubfolder in s_AllowedAssetSubfolders)
				{
					var doesStartsWith = path.StartsWith(allowedSubfolder);
					var subfolderLength = allowedSubfolder.Length;
					var lengthMatches = path.Length == subfolderLength;
					if (doesStartsWith && (lengthMatches || path[subfolderLength].Equals('/')))
						return true;
				}

				return false;
			}

			private static String MakeRelative([NotNull] String fullOrRelativePath) =>
				fullOrRelativePath.Substring(FullProjectPath.Length).Trim('/');

			private static GUID GuidForExistingPath([NotNull] String path) =>
				new(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));
		}
	}
}
