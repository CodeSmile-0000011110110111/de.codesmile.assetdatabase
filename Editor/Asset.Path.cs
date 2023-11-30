// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace CodeSmileEditor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Represents a relative path to an asset file or folder, typically under 'Assets' or 'Packages'.
		/// </summary>
		/// <remarks>
		///     - Guards against inconsistencies like invalid paths (not under project root), illegal characters, etc.
		///     - Converts path separators to forward slashes for compatibility with OS X, Linux
		///     - Converts absolute paths to relative paths
		///     - Implicitly convertible to/from string eg "use it like a string path"
		///     - Provides quick access to File I/O tasks like: get file's folder, extension, full path, exists, meta file, etc.
		/// </remarks>
		public partial class Path : IEquatable<Path>, IEquatable<String>
		{
			private const String DefaultExtension = "asset";

			private String m_RelativePath = String.Empty;

			/// <summary>
			///     Returns the GUID for the path.
			/// </summary>
			/// <returns>The GUID for the asset at the path, or an empty GUID if the asset does not exist in the database.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.Exists" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.AssetPathToGUID.html">AssetDatabase.AssetPathToGUID</a>
			/// </seealso>
			public GUID Guid => GetGuid(this, AssetPathToGUIDOptions.OnlyExistingAssets);

			/// <summary>
			///     Returns true if the path exists in the AssetDatabase.
			/// </summary>
			/// <remarks>
			///     This may still return true for asset files that have been deleted externally if those changes
			///     have not been imported via CodeSmileEditor.Asset.Database.ImportAll.
			/// </remarks>
			/// <remarks>In Unity 2023.2 or newer uses the new AssetPathExists method. In earlier versions AssetPathToGUID is used.</remarks>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.ExistsInFileSystem" />
			///     - <see cref="CodeSmileEditor.Asset.Status.IsImported" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.AssetPathExists.html">AssetDatabase.AssetPathExists</a>
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.AssetPathToGUID.html">AssetDatabase.AssetPathToGUID</a>
			/// </seealso>
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
			///     Returns true if the path exists in the file system.
			/// </summary>
			/// <remarks>
			///     This checks for physical existance of a file or folder using the System.IO methods.
			///     It may return true where CodeSmileEditor.Asset.Path.Exists returns false.
			/// </remarks>
			/// <remarks>
			///     In such a case this may indicate that the asset has been created but not yet imported.
			///     The corresponding status check is: CodeSmileEditor.Asset.Status.IsImported.
			/// </remarks>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.Exists" />
			///     - <see cref="CodeSmileEditor.Asset.Status.IsImported" />
			/// </seealso>
			public Boolean ExistsInFileSystem => FileExists(this) || FolderExists(this);

			/// <summary>
			///     Returns the path to the .meta file if the path is an asset file or folder.
			/// </summary>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.AssetPath" />
			///     - <see cref="CodeSmileEditor.Asset.Path.ToMeta" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetTextMetaFilePathFromAssetPath.html">AssetDatabase.GetTextMetaFilePathFromAssetPath</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public Path MetaPath => ToMeta(this);

			/// <summary>
			///     Returns the path to the asset file if the path represents a .meta file.
			/// </summary>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.MetaPath" />
			///     - <see cref="CodeSmileEditor.Asset.Path.FromMeta" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetPathFromTextMetaFilePath.html">AssetDatabase.GetAssetPathFromTextMetaFilePath</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public Path AssetPath => FromMeta(this);

			/// <summary>
			///     Returns the extension of the path.
			/// </summary>
			/// <returns>The extension with a leading dot (eg '.txt') or an empty string if there is no extension.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.FileName" />
			///     - <see cref="CodeSmileEditor.Asset.Path.FileNameWithoutExtension" />
			/// </seealso>
			public String Extension => System.IO.Path.GetExtension(m_RelativePath);

			/// <summary>
			///     Returns the file name with extension.
			/// </summary>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.Extension" />
			///     - <see cref="CodeSmileEditor.Asset.Path.FileNameWithoutExtension" />
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public String FileName => System.IO.Path.GetFileName(m_RelativePath);

			/// <summary>
			///     Returns the file name without extension.
			/// </summary>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.Extension" />
			///     - <see cref="CodeSmileEditor.Asset.Path.FileName" />
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public String FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(m_RelativePath);

			/// <summary>
			///     Returns the full (absolute) path with forward slashes as separators.
			/// </summary>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.FolderPath" />
			/// </seealso>
			public String FullPath => System.IO.Path.GetFullPath(m_RelativePath).ToForwardSlashes();

			/// <summary>
			///     Returns the names of all folders in a path to a folder.
			/// </summary>
			/// <remarks>
			///     If the path points to a file this returns an empty array. Use
			///     CodeSmileEditor.Asset.Path.FolderPath.SubFolders in this case to first get the file's folder.
			/// </remarks>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.GetSubFolders" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetSubFolders.html">AssetDatabase.GetSubFolders</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public String[] SubFolders => GetSubFolders(this);

			/// <summary>
			///     Returns the relative path to the directory the file or folder is in.
			/// </summary>
			/// <returns>The parent folder of the file or folder. Returns null if it's the root path eg "Assets".</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.FileName" />
			///     - <see cref="CodeSmileEditor.Asset.Path.FileNameWithoutExtension" />
			///     - <see cref="CodeSmileEditor.Asset.Path.Extension" />
			/// </seealso>
			public Path FolderPath
			{
				get
				{
					var dirName = System.IO.Path.GetDirectoryName(m_RelativePath);
					return String.IsNullOrEmpty(dirName) ? null : dirName;
				}
			}

			/// <summary>
			///     Returns the path altered with a numbering if an asset already exists (and is imported) at the path.
			/// </summary>
			/// <remarks>
			///     See also: Project Settings => Editor => Numbering Scheme
			/// </remarks>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.UniquifyFileName" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GenerateUniqueAssetPath.html">AssetDatabase.GenerateUniqueAssetPath</a>
			/// </seealso>
			public Path UniqueFilePath => UniquifyFileName(this);

			[ExcludeFromCodeCoverage] private Path() {} // disallowed parameterless ctor

			/// <summary>
			///     Creates an asset path from either an absolute or relative path.
			/// </summary>
			/// <remarks>
			///     When passing an absolute path, it is required that the absolute path starts with the project's
			///     root folder. This safeguards against accidentally using an absolute path from a different project.
			/// </remarks>
			/// <remarks>
			///     A path can also point to other locations in the project root folder where assets may be stored.
			///     The allowed subfolders (case insensitive) are:
			///     - Assets
			///     - Library
			///     - Logs
			///     - Packages
			///     - ProjectSettings
			///     - Temp
			///     - UserSettings
			/// </remarks>
			/// <example>
			///     Example input path:<code>C:\Users\Urso Clever\Untiy Porjects\Firts Projcet\Assets\something\file.asset</code>
			///     Results in: <code>Assets/something/file.asset</code>
			/// </example>
			/// <param name="fullOrRelativePath">Relative or absolute path to an asset file or folder.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path(String,String,String)" />
			/// </seealso>
			public Path(String fullOrRelativePath)
			{
				ThrowIf.NullOrWhitespace(fullOrRelativePath, nameof(fullOrRelativePath));
				m_RelativePath = ToRelative(fullOrRelativePath.ToForwardSlashes());
			}

			/// <summary>
			///     Creates an asset path by combining folder path, file name and extension.
			/// </summary>
			/// <remarks>The default extension is ".asset". Note that AssetImporters rely on file extensions.</remarks>
			/// <example>
			///     Example parameters:
			///     <c>"\Assets\Some/Sub\Dir", "New File", "MyExt"</c>
			/// </example>
			/// <example>
			///     Resulting path:
			///     <c>"Assets/Some/Sub/Dir/New File.myext"</c>
			/// </example>
			/// <param name="folderPath">
			///     Absolute or relative path to a folder. Leading/trailing path separators are trimmed.
			/// </param>
			/// <param name="fileName">Name of the file without extension. Must not contain path separators.</param>
			/// <param name="extension">
			///     Extension of the file (default: 'asset'). Leading dots will be trimmed. Extension will be lowercase.
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path(String)" />
			/// </seealso>
			public Path(String folderPath, String fileName, String extension = DefaultExtension)
			{
				ThrowIf.NullOrWhitespace(folderPath, nameof(folderPath));
				ThrowIf.NullOrWhitespace(fileName, nameof(fileName));
				ThrowIf.NullOrWhitespace(extension, nameof(extension));
				ThrowIf.ContainsPathSeparators(fileName, nameof(fileName));

				var relativeDir = ToRelative(folderPath.ToForwardSlashes());
				m_RelativePath = $"{relativeDir}/{fileName}.{extension.TrimStart('.').ToLower()}";
			}

			/// <summary>
			///     Tests another path for equality.
			/// </summary>
			/// <remarks>They are equal if their internal relative path strings are equal.</remarks>
			/// <param name="other">A path instance or null.</param>
			/// <returns>True if this path equals the other path. False otherwise.</returns>
			public Boolean Equals(Path other)
			{
				if (ReferenceEquals(other, null))
					return false;
				if (ReferenceEquals(this, other))
					return true;

				return m_RelativePath.Equals(other.m_RelativePath);
			}

			/// <summary>
			///     Tests another path for equality.
			/// </summary>
			/// <remarks>They are equal if their internal relative path strings are equal.</remarks>
			/// <remarks>The string is elevated to a Path instance, and then compared.</remarks>
			/// <param name="other">A path string or null.</param>
			/// <returns>True if this path equals the other path. False otherwise.</returns>
			public Boolean Equals(String other) => m_RelativePath.Equals(new Path(other).m_RelativePath);

			/// <summary>
			///     Implicit conversion to string.
			/// </summary>
			/// <param name="path">Input path.</param>
			/// <returns>Relative path as string.</returns>
			public static implicit operator String(Path path) => path != null ? path.m_RelativePath : null;

			/// <summary>
			///     Implicit conversion to Path from a string.
			/// </summary>
			/// <param name="path">Input string path. Path may be absolute or relative.</param>
			/// <returns>Relative path as string.</returns>
			public static implicit operator Path(String path) => path != null ? new Path(path) : null;

			/// <summary>
			///     Tests two path instances for equality.
			/// </summary>
			/// <param name="path1">A Path.</param>
			/// <param name="path2">Another path.</param>
			/// <returns>True if both paths point to the same location. False otherwise.</returns>
			public static Boolean operator ==(Path path1, Path path2)
			{
				if (ReferenceEquals(path1, path2))
					return true;
				if (ReferenceEquals(path1, null))
					return false;
				if (ReferenceEquals(path2, null))
					return false;

				return path1.Equals(path2);
			}

			/// <summary>
			///     Tests two path instances for inequality.
			/// </summary>
			/// <param name="path1">A Path.</param>
			/// <param name="path2">Another path.</param>
			/// <returns></returns>
			public static Boolean operator !=(Path path1, Path path2) => !(path1 == path2);

			/// <summary>
			///     Tests for equality with an object.
			/// </summary>
			/// <param name="path1">A Path.</param>
			/// <param name="other">Any instance.</param>
			/// <returns>True if both paths point to the same location. False otherwise.</returns>
			public static Boolean operator ==(Path path1, Object other) =>
				other is String str ? path1.Equals(str) : path1.Equals(other as Path);

			/// <summary>
			///     Tests for inequality with an object.
			/// </summary>
			/// <param name="path1">A Path.</param>
			/// <param name="other">Any instance.</param>
			/// <returns></returns>
			public static Boolean operator !=(Path path1, Object other) => !(path1 == other);

			/// <summary>
			///     Tests for equality with an object.
			/// </summary>
			/// <param name="other">Any instance.</param>
			/// <param name="path">A Path.</param>
			/// <returns>True if both paths point to the same location. False otherwise.</returns>
			public static Boolean operator ==(Object other, Path path) => path == other;

			/// <summary>
			///     Tests for inequality with an object.
			/// </summary>
			/// <param name="other">Any instance.</param>
			/// <param name="path">A Path.</param>
			/// <returns></returns>
			public static Boolean operator !=(Object other, Path path) => !(path == other);

			/// <summary>
			///     Opens the folder externally, for example File Explorer (Windows) or Finder (Mac).
			/// </summary>
			/// <seealso cref="">
			///     - <a href="https://docs.unity3d.com/ScriptReference/Application.OpenURL.html">Application.OpenURL</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // cannot be tested
			public void OpenExternal() => Application.OpenURL(System.IO.Path.GetFullPath(FolderPath));

			/// <summary>
			///     Renames the last element of the CodeSmileEditor.Asset.Path instance.
			/// </summary>
			/// <remarks>NOTE: This does **not** rename a file/folder on disk! It renames the Path instance.</remarks>
			/// <remarks>When renaming a file you must specifiy the new filename with extension.</remarks>
			/// <param name="newFileOrFolderName">Name of a folder or file with extension. Does nothing if input is null or empty.</param>
			public void Rename(String newFileOrFolderName)
			{
				if (String.IsNullOrEmpty(newFileOrFolderName) == false)
					m_RelativePath = $"{FolderPath}/{System.IO.Path.GetFileName(newFileOrFolderName)}";
			}

			/// <summary>
			///     Creates the folders in the path recursively.
			/// </summary>
			/// <remarks>
			///     The CodeSmileEditor.Asset.File write operations use this internally to create any missing folders,
			///     so you do not need to call this yourself in those cases.
			/// </remarks>
			/// <remarks>
			///     Path may point to a file. It is assumed that if the last element of the path
			///     contains a dot it has an extension and is therefore a file.
			/// </remarks>
			/// <returns>The GUID of the deepest (last) folder in the path.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Path.CreateFolders(CodeSmileEditor.Asset.Path)" />
			/// </seealso>
			public GUID CreateFolders() => CreateFolders(this);

			/// <summary>
			///     Returns the relative path as string.
			/// </summary>
			/// <remarks>Same as implicit string conversion.</remarks>
			/// <returns>The relative path as a string.</returns>
			public override String ToString() => m_RelativePath;

			/// <summary>
			///     Tests path for equality with an object.
			/// </summary>
			/// <remarks>
			///     If the object is a Path or String will use the respective Equals method for these types.
			///     In all other cases returns false.
			/// </remarks>
			/// <param name="obj"></param>
			/// <returns>True if the path equals the input string or Path. False otherwise or if input is neither string nor Path.</returns>
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
			/// <returns>The hash code of the relative path string.</returns>
			public override Int32 GetHashCode() => m_RelativePath.GetHashCode();
		}
	}
}
