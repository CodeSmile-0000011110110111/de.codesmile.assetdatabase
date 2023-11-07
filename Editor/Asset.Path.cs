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
		///     <p>
		///         Represents a relative path to an asset file or folder under either 'Assets' or 'Packages'.
		///         Instances can be initialized with a relative or full (absolute) path, internally it will be converted
		///         to a relative path. Use the FullPath property to get the full (absolute) path.
		///     </p>
		///     <p>
		///         All path separators are converted to forward slashes for compatibility with Windows, Mac, Linux.
		///         Leading and trailing path separators are trimmed: "\Assets\folder\" => "Assets/folder"
		///     </p>
		///     <p>
		///         Instances are implicitly and explicitly convertible to/from string:
		///         <example>string strPath = (string)new Asset.Path("Assets/MyFolder/My.file");</example>
		///         <example>Asset.Path assetPath = (Asset.Path)"Assets/MyFolder/My.file";</example>
		///     </p>
		///     <p>
		///         Ideally you should pass in a string and henceforth work with the Asset.Path instance,
		///         since path sanitation occurs every time an Asset.Path instance is created.
		///     </p>
		/// </summary>
		public partial class Path : IEquatable<Path>, IEquatable<String>
		{
			public const String DefaultExtension = "asset";

			private readonly String m_RelativePath = String.Empty;

			/// <summary>
			///     Returns the GUID for the path.
			///     Returns an empty GUID if the asset at the path does not exist in the database.
			///     <see cref="Exists" />
			///     <see cref="ExistsInFileSystem" />
			/// </summary>
			/// <returns></returns>
			public GUID Guid => GetGuid(this, AssetPathToGUIDOptions.OnlyExistingAssets);

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

			internal static Path GetOverwriteOrUnique(Path destPath, Boolean overwriteExisting) =>
				overwriteExisting ? destPath : destPath.UniqueFilePath;

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

			private Path ToFolderPath() => new(System.IO.Path.GetDirectoryName(m_RelativePath));

			/// <summary>
			///     Returns the relative path as string. Same as implicit string conversion.
			/// </summary>
			/// <returns></returns>
			public override String ToString() => m_RelativePath;
		}
	}
}
