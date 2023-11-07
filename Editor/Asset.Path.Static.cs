// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public partial class Path
		{
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
			///     Tests if the given file exists.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean FileExists(Path path) => FileExists((String)path);

			/// <summary>
			///     Tests if the given file exists.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean FileExists(String path) => File.Exists(path);

			/// <summary>
			///     Returns true if the folder exists. False otherwise, or if the path is to a file.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>true if the folder exists</returns>
			public static Boolean FolderExists(Path path) => FolderExists((String)path);

			/// <summary>
			///     Returns true if the folder exists. Also works with paths pointing to a file.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>true if the folder exists</returns>
			public static Boolean FolderExists(String path) => path != null ? AssetDatabase.IsValidFolder(path) : false;

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

			/// <summary>
			///     Returns the path either unaltered or with a numbering to make the file unique.
			///     This is only done if an asset file exists at the path. It does not alter folder paths.
			///     See also: Project Settings => Editor => Numbering Scheme
			///     Note: 'Uniquify' is a proper english verb, it means "to make unique".
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Path UniquifyFilename(Path path) => UniquifyFilename((String)path);

			/// <summary>
			///     Returns the path either unaltered or with a numbering to make the file unique.
			///     This is only done if an asset file exists at the path. It does not alter folder paths.
			///     See also: Project Settings => Editor => Numbering Scheme
			///     Note: 'Uniquify' is a proper english verb, it means "to make unique".
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Path UniquifyFilename(String path)
			{
				var uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
				return (Path)(String.IsNullOrEmpty(uniquePath) ? path : uniquePath);
			}

			private static GUID GuidForExistingPath(String path) =>
				new(AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets));
		}
	}
}
