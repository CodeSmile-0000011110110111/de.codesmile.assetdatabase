// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

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
			private const String DefaultExtension = "asset";

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
#if UNITY_2023_2_OR_NEWER
					return AssetDatabase.AssetPathExists(m_RelativePath);
#else
					return AssetDatabase.AssetPathToGUID(m_RelativePath, AssetPathToGUIDOptions.OnlyExistingAssets).Length > 0;
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
			///     Returns the path to the .meta file if the path represents an asset file.
			/// </summary>
			public Path MetaPath => ToMeta(this);

			/// <summary>
			///     Returns the path to the asset file if the path represents a .meta file.
			/// </summary>
			public Path AssetPath => FromMeta(this);

			/// <summary>
			///     Returns the extension of the file path.
			/// </summary>
			/// <value>The extension with a leading dot (eg '.txt') or an empty string.</value>
			public String Extension => System.IO.Path.GetExtension(m_RelativePath);

			/// <summary>
			///     Returns the file name with extension.
			/// </summary>
			public String FileName => System.IO.Path.GetFileName(m_RelativePath);

			/// <summary>
			///     Returns the file name without extension.
			/// </summary>
			public String FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(m_RelativePath);

			/// <summary>
			///     Creates and returns the full path, with forward slashes as separators.
			/// </summary>
			public String FullPath => System.IO.Path.GetFullPath(m_RelativePath).ToForwardSlashes();

			/// <summary>
			///     Returns the names of all subfolders in the current directory.
			///     If the path points to a file it returns an empty array, use "path.FolderPath.SubFolders"
			///     in this case.
			/// </summary>
			public String[] SubFolders => GetSubFolders(this);

			/// <summary>
			///     Returns the relative path to the directory the file or folder is in.
			///     This means for folders it returns the parent folder.
			/// </summary>
			/// <returns>The parent folder of the file or folder. Returns null if it's the root path.</returns>
			public Path FolderPath
			{
				get
				{
					var dirName = System.IO.Path.GetDirectoryName(m_RelativePath);
					return String.IsNullOrEmpty(dirName) ? null : dirName;
				}
			}

			/// <summary>
			///     Returns the path either unaltered or with a numbering to make the file unique if an asset file
			///     already exists at the path. Does not alter path if it does not exist or points to a folder.
			///     See also: Project Settings => Editor => Numbering Scheme
			/// </summary>
			public Path UniqueFilePath => UniquifyFileName(this);

			[ExcludeFromCodeCoverage] private Path() {} // disallowed parameterless ctor

			/// <summary>
			///     Creates an asset path from either a full or relative path.
			///     Example input path: "C:\Users\Urso Clever\Untiy Projects\First Projcet\Assets\something"
			///     Resulting AssetPath: "Assets/something"
			/// </summary>
			/// <param name="fullOrRelativePath">Absolute path to project's root folder or a subfolder.</param>
			/// <exception cref="ArgumentException">
			///     If path does not start with the project's Assets path (Application.dataPath).
			/// </exception>
			public Path(String fullOrRelativePath)
			{
				ThrowIf.NullOrWhitespace(fullOrRelativePath, nameof(fullOrRelativePath));
				m_RelativePath = ToRelative(fullOrRelativePath.ToForwardSlashes());
			}

			/// <summary>
			///     Creates an asset path from either a full or relative path to a directory, a filename,
			///     and an optional extension (default: 'asset').
			/// </summary>
			/// <example>
			///     Example parameters:
			///     <c>"\Assets\Some/Sub\Dir", "New File", "MyExt"</c>
			/// </example>
			/// <example>
			///     Resulting path:
			///     <c>"Assets/Some/Sub/Dir/New File.myext"</c>
			/// </example>
			/// <param name="directory">
			///     Path to a directory under Assets, or just 'Assets'. Can be a relative or absolute path.
			///     Leading/trailing path separators are trimmed.
			/// </param>
			/// <param name="fileName">Name of the file without extension. Must not contain path separators.</param>
			/// <param name="extension">
			///     Extension of the file (default: 'asset'). Leading dots will be trimmed. Extension will be
			///     lowercase.
			/// </param>
			/// <exception cref="ArgumentException"></exception>
			public Path(String directory, String fileName, String extension = DefaultExtension)
			{
				ThrowIf.NullOrWhitespace(directory, nameof(directory));
				ThrowIf.NullOrWhitespace(fileName, nameof(fileName));
				ThrowIf.NullOrWhitespace(extension, nameof(extension));
				ThrowIf.ContainsPathSeparators(fileName, nameof(fileName));

				var relativeDir = ToRelative(directory.ToForwardSlashes());
				m_RelativePath = $"{relativeDir}/{fileName}.{extension.TrimStart('.').ToLower()}";
			}

			/// <summary>
			///     Tests another path for equality. They are equal if their internal relative path strings are equal.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public Boolean Equals(Path other)
			{
				if (ReferenceEquals(other, null))
					return false;
				if (ReferenceEquals(this, other))
					return true;

				return m_RelativePath.Equals(other.m_RelativePath);
			}

			/// <summary>
			///     Tests paths for equality with a string. The string is elevated to a Path instance, and then
			///     their internal path strings are compared.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public Boolean Equals(String other) => m_RelativePath.Equals(new Path(other).m_RelativePath);

			/// <summary>
			///     Implicit conversion to string (relative asset path). Same as ToString().
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static implicit operator String(Path path) => path != null ? path.m_RelativePath : null;

			/// <summary>
			///     Implicit conversion of an AssetPath instance from a string path (full or relative).
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static implicit operator Path(String path) => path != null ? new Path(path) : null;

			/// <summary>
			///     Tests two path instances for equality.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="path2"></param>
			/// <returns></returns>
			public static Boolean operator ==(Path path, Path path2)
			{
				if (ReferenceEquals(path, path2))
					return true;
				if (ReferenceEquals(path, null))
					return false;
				if (ReferenceEquals(path2, null))
					return false;

				return path.Equals(path2);
			}

			/// <summary>
			///     Tests two path instances for inequality.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="path2"></param>
			/// <returns></returns>
			public static Boolean operator !=(Path path, Path path2) => !(path == path2);

			/// <summary>
			///     Tests for equality with an object.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="other"></param>
			/// <returns></returns>
			public static Boolean operator ==(Path path, Object other) =>
				other is String str ? path.Equals(str) : path.Equals(other as Path);

			/// <summary>
			///     Tests for inequality with an object.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="other"></param>
			/// <returns></returns>
			public static Boolean operator !=(Path path, Object other) => !(path == other);

			/// <summary>
			///     Tests for equality with an object.
			/// </summary>
			/// <param name="other"></param>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean operator ==(Object other, Path path) => path == other;

			/// <summary>
			///     Tests for inequality with an object.
			/// </summary>
			/// <param name="other"></param>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean operator !=(Object other, Path path) => !(path == other);

			/// <summary>
			///     Opens the folder externally, for example File Explorer (Windows) or Finder (Mac).
			/// </summary>
			[ExcludeFromCodeCoverage] // cannot be tested
			public void OpenExternal() => Application.OpenURL(System.IO.Path.GetFullPath(FolderPath));

			/// <summary>
			///     Renames the file or folder with a new name.
			/// </summary>
			/// <param name="newFileOrFolderName"></param>
			/// <returns>True if rename succeeded, false otherwise.</returns>
			public Boolean Rename(String newFileOrFolderName)
			{
				if (String.IsNullOrEmpty(newFileOrFolderName))
					return false;

				m_RelativePath = $"{FolderPath}/{System.IO.Path.GetFileName(newFileOrFolderName)}";
				return true;
			}

			/// <summary>
			///     Creates the folders in the path recursively. Path may point to a file but only folders
			///     will be created.
			/// </summary>
			/// <returns>the GUID of the deepest folder in the hierarchy</returns>
			public GUID CreateFolders() => CreateFolders(this);

			/// <summary>
			///     Returns the relative path as string. Same as implicit string conversion.
			/// </summary>
			/// <returns></returns>
			public override String ToString() => m_RelativePath;

			/// <summary>
			///     Tests path for equality with an object. If the object is a Path or String will use the respective
			///     Equals method for these types. In all other cases returns false.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public override Boolean Equals(Object obj)
			{
				if (obj is Path path)
					return Equals(path);
				if (obj is String str)
					return Equals(str);

				return false;
			}

			/// <summary>
			///     Returns the internal path string's hash code.
			/// </summary>
			/// <returns></returns>
			public override Int32 GetHashCode() => m_RelativePath.GetHashCode();
		}
	}
}
