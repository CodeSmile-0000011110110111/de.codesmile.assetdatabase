// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
					if (File.Exists(m_RelativePath))
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
						if (File.Exists(m_RelativePath))
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
			public static Boolean FileExists(Path path) => File.Exists(path.m_RelativePath);

			/// <summary>
			///     Returns true if the folder exists. False otherwise, or if the path is to a file.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>true if the folder exists</returns>
			public static Boolean FolderExists(Path path) =>
				path != null ? AssetDatabase.IsValidFolder(path.m_RelativePath) : false;

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

			/// <summary>
			///     Opens the folder externally, for example File Explorer (Windows) or Finder (Mac).
			/// </summary>
			[ExcludeFromCodeCoverage]
			public void OpenFolder() => Application.OpenURL(System.IO.Path.GetFullPath(FolderPathAssumptive));

			/// <summary>
			///     Returns the relative path as string. Same as implicit string conversion.
			/// </summary>
			/// <returns></returns>
			public override String ToString() => m_RelativePath;
		}
	}
}
