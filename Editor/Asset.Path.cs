// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Represents a relative path to an asset file or folder, typically under 'Assets' or 'Packages'.
		///     Implicitly converts to/from string. Guards against inconsistencies, eg invalid paths, illegal characters, etc.
		///     Provides quick access to File I/O tasks such as getting a file's folder, extension, full path, existance, etc.
		/// </summary>
		public partial class Path : IEquatable<Path>, IEquatable<String>
		{
			public const String DefaultExtension = "asset";

			// all are lowercase
			private static readonly String[] s_AllowedAssetSubfolders =
				{ "assets", "library", "logs", "packages", "projectsettings", "temp", "usersettings" };

			private String m_RelativePath = String.Empty;

			/// <summary>
			///     Returns the GUID for the path.
			///     Returns an empty GUID if the asset at the path does not exist in the database.
			///     <see cref="Exists" />
			///     <see cref="ExistsInFileSystem" />
			/// </summary>
			/// <returns></returns>
			public GUID Guid => GetGuid(this, AssetPathToGUIDOptions.OnlyExistingAssets);

			/// <summary>
			///     Returns true if the path exists in the AssetDatabase.
			///     NOTE: This may still return true for asset files that have been deleted externally.
			///     <see cref="ExistsInFileSystem" />
			/// </summary>
			public Boolean Exists
			{
				get
				{
#if UNITY_2023_1_OR_NEWER
					return AssetDatabase.AssetPathExists(m_RelativePath);
#else
					return Guid.Empty() == false;
#endif
				}
			}

			/// <summary>
			///     Returns true if the path exists in the file system, be it file or folder.
			///     Returns false if the path does not exist.
			///     NOTE: This solely checks for physical existance, a new asset at that path may still not 'exist'
			///     in the database until it has been imported.
			///     <see cref="Exists" />
			/// </summary>
			public Boolean ExistsInFileSystem => FileExists(this) || FolderExists(this);

			/// <summary>
			///     Returns the extension of the file path.
			/// </summary>
			/// <value>The extension with a leading dot (eg '.txt') or an empty string.</value>
			[ExcludeFromCodeCoverage] public String Extension => System.IO.Path.GetExtension(m_RelativePath);
			/// <summary>
			///     Returns the file name with extension.
			/// </summary>
			[ExcludeFromCodeCoverage] public String FileName => System.IO.Path.GetFileName(m_RelativePath);
			/// <summary>
			///     Returns the file name without extension.
			/// </summary>
			[ExcludeFromCodeCoverage] public String FileNameWithoutExtension =>
				System.IO.Path.GetFileNameWithoutExtension(m_RelativePath);
			/// <summary>
			///     Returns the directory name.
			/// </summary>
			public String DirectoryName => System.IO.Path.GetDirectoryName(m_RelativePath).ToForwardSlashes();

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
			///     Creates and returns the full path, with forward slashes as separators.
			/// </summary>
			public String FullPath => System.IO.Path.GetFullPath(m_RelativePath).ToForwardSlashes();

			/// <summary>
			/// Returns the names of all folders in the path.
			/// </summary>
			public String[] Folders => GetFolders(m_RelativePath);

			/// <summary>
			///     Returns the path to the file's parent folder, or the path itself if the path points to a folder.
			///     CAUTION: The path must exist! If not, throws an exception.
			/// </summary>
			/// <exception cref="InvalidOperationException">if the path does not exist</exception>
			public Path FolderPath
			{
				get
				{
					// existing directory? return that
					if (Directory.Exists(m_RelativePath))
						return this;

					// existing file? return folder path
					if (System.IO.File.Exists(m_RelativePath))
						return ToFolderPath();

					throw new InvalidOperationException("unable to determine if file or folder because path" +
					                                    $" '{m_RelativePath}' does not exist");
				}
			}

			/// <summary>
			///     Returns the path to the file's parent folder, or the path itself if the path points to a folder.
			///     If the path does not exist and it ends with an extension (has a dot) then it is assumed a file path,
			///     otherwise a folder path is assumed (Unity does not allow assets without extensions).
			///     CAUTION: This may incorrectly assume a file if the path's last folder contains a dot. In this case
			///     it returns the second to last folder in the path.
			/// </summary>
			[ExcludeFromCodeCoverage]
			public Path FolderPathAssumptive
			{
				get
				{
					try
					{
						// existing directory? return that
						if (Directory.Exists(m_RelativePath))
							return this;

						// existing file? return folder path
						if (System.IO.File.Exists(m_RelativePath))
							return ToFolderPath();

						// if it has an extension, assume it's a file (could also be a folder but alas ...)
						if (System.IO.Path.HasExtension(m_RelativePath))
							return ToFolderPath();
					}
					catch (Exception) {}

					return this;
				}
			}

			/// <summary>
			///     Returns the path either unaltered or with a numbering to make the file unique if an asset file
			///     already exists at the path. Does not alter path if it does not exist or points to a folder.
			///     See also: Project Settings => Editor => Numbering Scheme
			/// </summary>
			public Path UniqueFilePath => UniquifyFilename(this);

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
			/// Returns the .meta file path from a path to an asset.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Path GetMeta(Path path) => AssetDatabase.GetTextMetaFilePathFromAssetPath(path); // seriously, that name??

			/// <summary>
			/// Returns the scene's path if the object is instantiated in a scene, otherwise returns the object's path.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Path GetScene(Object obj) => AssetDatabase.GetAssetOrScenePath(obj);

			/// <summary>
			/// Returns the names of all folders in the path.
			/// </summary>
			public String[] GetFolders(Path path) => AssetDatabase.GetSubFolders(path);

			/// <summary>
			///     Returns true if the provided path is valid. This means it contains no illegal folder or file name
			///     characters and it isn't too long.
			///     If this returns false, Asset.GetLastErrorMessage() contains more detailed information.
			///     <see cref="Asset.GetLastErrorMessage()" />
			/// </summary>
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
			///     will be created.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>the GUID of the deepest folder in the hierarchy</returns>
			public static GUID CreateFolders(Path path)
			{
				ThrowIf.ArgumentIsNull(path, nameof(path));
				ThrowIf.PathIsNotValid(path);

				var folderPath = path.FolderPathAssumptive;
				if (FileExists(path) || FolderExists(folderPath))
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

					var guidString = AssetDatabase.CreateFolder(Get(folderGuid), folderNames[i]);
					folderGuid = new GUID(guidString);
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
			public static Path UniquifyFilename(Path path)
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

			internal static Path GetOverwriteOrUnique(Path destPath, Boolean overwriteExisting) =>
				overwriteExisting ? destPath : destPath.UniqueFilePath;

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

			/// <summary>
			///     Opens the folder externally, for example File Explorer (Windows) or Finder (Mac).
			/// </summary>
			[ExcludeFromCodeCoverage]
			public void OpenFolder() => Application.OpenURL(System.IO.Path.GetFullPath(FolderPathAssumptive));

			/// <summary>
			///     Renames the file or folder with a new name.
			/// </summary>
			/// <param name="newFileOrFolderName"></param>
			/// <returns>True if rename succeeded, false otherwise.</returns>
			public Boolean Rename(String newFileOrFolderName)
			{
				if (String.IsNullOrEmpty(newFileOrFolderName))
					return false;

				m_RelativePath = $"{DirectoryName}/{System.IO.Path.GetFileName(newFileOrFolderName)}";
				return true;
			}

			/// <summary>
			///     Creates the folders in the path recursively. Path may point to a file but only folders
			///     will be created.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>the GUID of the deepest folder in the hierarchy</returns>
			public GUID CreateFolders() => CreateFolders(this);

			private Path ToFolderPath() => new(System.IO.Path.GetDirectoryName(m_RelativePath));

			/// <summary>
			///     Returns the relative path as string. Same as implicit string conversion.
			/// </summary>
			/// <returns></returns>
			public override String ToString() => m_RelativePath;
		}
	}
}
