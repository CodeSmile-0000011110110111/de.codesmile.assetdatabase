// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Represents a relative path to an asset file in the project.
		///     Instances are implicitly and explicitly convertible to/from string.
		///     All path separators are forward slashes for compatibility with Windows, Mac, Linux.
		///     Leading and trailing path separators are trimmed.
		///     Instances can be initialized with a relative or full (absolute) path, or a relative/full path
		///     to a directory plus a filename and optional extension (default: 'asset').
		/// </summary>
		public partial class Path : IEquatable<Path>, IEquatable<String>
		{
			public const String DefaultExtension = "asset";

			private readonly String m_RelativePath = String.Empty;

			/// <summary>
			///     Returns the path either unaltered or with a numbering to make the file unique if an asset file
			///     already exists at the path. Does not alter path if it does not exist or points to a folder.
			///     See also: Project Settings => Editor => Numbering Scheme
			/// </summary>
			public Path UniqueFilePath => UniquifyFilename(this);

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
			public Path FolderPathAssumptive
			{
				get
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

					return this;
				}
			}

			/// <summary>
			///     Returns true if the path exists, be it file or folder. Returns false if the path does not exist.
			/// </summary>
			public Boolean Exists => FileExists(this) || FolderExists(this);

			public GUID Guid =>
				new(AssetDatabase.AssetPathToGUID(m_RelativePath, AssetPathToGUIDOptions.OnlyExistingAssets));

			private static String ToRelative(String fullOrRelativePath)
			{
				if (IsRelative(fullOrRelativePath))
					return fullOrRelativePath.Trim('/');

				ThrowIf.NotAProjectPath(fullOrRelativePath, fullOrRelativePath);
				return MakeRelative(fullOrRelativePath);
			}

			private static Boolean IsRelative(String path)
			{
				// path must start with "Assets" or "Packages/"
				// it may also be just "Assets" (length == 6), otherwise a path separator must follow: "Assets/.."
				path = path.TrimStart('/').ToLower();
				var startsWithAssets = path.StartsWith("assets");
				var startsWithPackages = path.StartsWith("packages/");
				return startsWithAssets && (path.Length <= 6 || path[6].Equals('/')) || startsWithPackages;
			}

			private static String MakeRelative(String fullOrRelativePath) =>
				fullOrRelativePath.Substring(FullProjectPath.Length).Trim('/');

			private static GUID GuidForStringPath(String path) =>
				new(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));

			private Path ToFolderPath() => new(System.IO.Path.GetDirectoryName(m_RelativePath));

			/// <summary>
			///     Returns the relative path as string. Same as implicit string conversion.
			/// </summary>
			/// <returns></returns>
			public override String ToString() => m_RelativePath;
		}
	}
}
