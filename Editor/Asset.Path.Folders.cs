// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public partial class Path
		{
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
			///     Creates the folders in the path recursively. Path may point to a file, but only folders
			///     will be created.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>the GUID of the deepest folder in the hierarchy</returns>
			public static GUID CreateFolders(Path path)
			{
				ThrowIf.ArgumentIsNull(path, nameof(path));

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
			///     Creates the folders in the path recursively. Path may point to a file, but only folders
			///     will be created.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>the GUID of the deepest folder in the hierarchy</returns>
			public static GUID CreateFolders(String path) => CreateFolders((Path)path);

			private static GUID GuidForExistingPath(String path) =>
				new(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));

			private Path ToFolderPath() => new(System.IO.Path.GetDirectoryName(m_RelativePath));
		}
	}
}
