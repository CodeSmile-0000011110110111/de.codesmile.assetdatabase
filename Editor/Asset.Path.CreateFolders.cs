// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public partial class Path
		{
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

			private static GUID GuidForExistingPath(String path) =>
				new(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));

			/// <summary>
			///     Creates the folders in the path recursively. Path may point to a file but only folders
			///     will be created.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>the GUID of the deepest folder in the hierarchy</returns>
			public GUID CreateFolders() => CreateFolders(this);

			private Path ToFolderPath() => new(System.IO.Path.GetDirectoryName(m_RelativePath));
		}
	}
}
