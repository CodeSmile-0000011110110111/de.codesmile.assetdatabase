// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public partial class Path
		{
			// all are lowercase
			private static readonly String[] s_AllowedAssetSubfolders =
				{ "assets", "library", "logs", "packages", "projectsettings", "temp", "usersettings" };

			/// <summary>
			///     Returns the path to the project's 'Assets' subfolder.
			/// </summary>
			public static String FullAssetsPath => Application.dataPath;

			/// <summary>
			///     Returns the path to the project's 'Packages' subfolder.
			/// </summary>
			public static String FullPackagesPath => $"{FullProjectPath}/Packages";

			/// <summary>
			///     Returns the path to the project's root folder.
			/// </summary>
			public static String FullProjectPath => FullAssetsPath.Substring(0, Application.dataPath.Length - 6);

			/// <summary>
			///     Gets the path of an asset file.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns>The path to the asset file, or null if the object is not an asset.</returns>
			public static Path Get(Object obj)
			{
				var path = AssetDatabase.GetAssetPath(obj);
				return String.IsNullOrEmpty(path) ? null : (Path)path;
			}

			/// <summary>
			///     Gets the path of an asset file.
			/// </summary>
			/// <param name="guid"></param>
			/// <returns>The path to the asset file, or null if the object is not an asset.</returns>
			public static Path Get(GUID guid)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				return String.IsNullOrEmpty(path) ? null : (Path)path;
			}

			/// <summary>
			///     Returns the GUID for the path.
			///     Returns an empty GUID if the asset at the path does not exist in the database.
			///     <see cref="Exists" />
			///     <see cref="ExistsInFileSystem" />
			/// </summary>
			/// <param name="path"></param>
			/// <param name="options"></param>
			/// <returns></returns>
			public static GUID GetGuid(Path path,
				AssetPathToGUIDOptions options = AssetPathToGUIDOptions.IncludeRecentlyDeletedAssets) =>
				new(AssetDatabase.AssetPathToGUID(path, options));

			/// <summary>
			///     Returns the .meta file path from a path to an asset.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Path ToMeta(Path path) => AssetDatabase.GetTextMetaFilePathFromAssetPath(path);

			/// <summary>
			///     Returns the asset's file path from its .meta file path.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Path FromMeta(Path path) => AssetDatabase.GetAssetPathFromTextMetaFilePath(path);

			/// <summary>
			///     Returns the scene's path if the object is instantiated in a scene, otherwise returns the object's path.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Path GetScene(Object obj) => AssetDatabase.GetAssetOrScenePath(obj);

			/// <summary>
			///     Returns true if the provided path is valid. This means it contains no illegal folder or file name
			///     characters and it isn't too long.
			///     If this returns false, Asset.GetLastErrorMessage() contains more detailed information.
			///     <see cref="Asset.GetLastErrorMessage()" />
			/// </summary>
			/// <param name="path"></param>
			/// <returns>
			///     True if the string is a valid path and contains no illegal characters for a path or file.
			///     False in all other cases.
			/// </returns>
			public static Boolean IsValid(String path)
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
			///     Tests if the given file exists.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean FileExists(Path path) => System.IO.File.Exists(path.m_RelativePath);

			/// <summary>
			///     Returns true if the folder exists. False otherwise, or if the path is to a file.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>true if the folder exists</returns>
			public static Boolean FolderExists(Path path) =>
				path != null ? AssetDatabase.IsValidFolder(path.m_RelativePath) : false;

			/// <summary>
			///     Creates the folders in the path recursively. Path may point to a file, but only folders
			///     will be created. If the last path chunk has an extension, it is considered a file.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>the GUID of the deepest folder in the hierarchy</returns>
			public static GUID CreateFolders(Path path)
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
			///     Returns the path either unaltered or with a numbering to make the file unique.
			///     This is only done if an asset file exists at the path. It does not alter folder paths.
			///     See also: Project Settings => Editor => Numbering Scheme
			///     Note: 'Uniquify' is a proper english verb, it means "to make unique".
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Path UniquifyFileName(Path path)
			{
				var uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
				return String.IsNullOrEmpty(uniquePath) ? path : uniquePath;
			}

			/// <summary>
			///     Converts an array of objects to their asset paths.
			///     The returned array has the same size.
			///     Note that some items may be null if the input was null or not an asset.
			/// </summary>
			/// <param name="objects"></param>
			/// <returns></returns>
			public static String[] ToAssetPaths(Object[] objects)
			{
				ThrowIf.ArgumentIsNull(objects, nameof(objects));

				var objectCount = objects.Length;
				var paths = new String[objectCount];
				for (var i = 0; i < objectCount; i++)
					paths[i] = Get(objects[i]);

				return paths;
			}

			/// <summary>
			///     Converts a collection of Path instances to a string array.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns></returns>
			public static String[] ToStrings(IEnumerable<Path> paths) => paths.Cast<String>().ToArray();

			/// <summary>
			///     Returns the names of all subfolders in the path.
			///     Returns an empty array of the path points to a file.
			/// </summary>
			/// <param name="path"></param>
			/// <returns>Paths to all subfolders, or empty array if there are no subfolders or the path points to a file.</returns>
			public static String[] GetSubFolders(Path path) => AssetDatabase.GetSubFolders(path);

			internal static Path UniquifyAsNeeded(Path path, Boolean overwriteExisting) =>
				overwriteExisting ? path : path.UniqueFilePath;

			private static GUID CreateSubFolder(Path parentFolder, String subFolderName) =>
				new(AssetDatabase.CreateFolder(parentFolder, subFolderName));

			private static String ToRelative(String fullOrRelativePath)
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

			private static Boolean IsRelative(String path)
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

			private static String MakeRelative(String fullOrRelativePath) =>
				fullOrRelativePath.Substring(FullProjectPath.Length).Trim('/');

			private static GUID GuidForExistingPath(String path) =>
				new(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));
		}
	}
}
