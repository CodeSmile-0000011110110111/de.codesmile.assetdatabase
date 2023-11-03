// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static class Path
		{
			/// <summary>
			///     Gets the path of an asset file.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns>The path to the asset file, or null if the object is not an asset.</returns>
			public static AssetPath Get(Object obj)
			{
				var path = AssetDatabase.GetAssetPath(obj);
				return String.IsNullOrEmpty(path) ? null : (AssetPath)path;
			}

			/// <summary>
			///     Gets the path of an asset file.
			/// </summary>
			/// <param name="guid"></param>
			/// <returns>The path to the asset file, or null if the object is not an asset.</returns>
			public static AssetPath Get(GUID guid)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				return String.IsNullOrEmpty(path) ? null : (AssetPath)path;
			}

			/// <summary>
			///     Returns the path either unaltered or with a numbering to make the file unique.
			///     This is only done if an asset file exists at the path. It does not alter folder paths.
			///     See also: Project Settings => Editor => Numbering Scheme
			///     Note: 'Uniquify' is a proper english verb, it means "to make unique".
			/// </summary>
			/// <param name="assetPath"></param>
			/// <returns></returns>
			public static AssetPath UniquifyFileNameIfAssetExists(AssetPath assetPath) =>
				UniquifyFileNameIfAssetExists((String)assetPath);

			/// <summary>
			///     Returns the path either unaltered or with a numbering to make the file unique.
			///     This is only done if an asset file exists at the path. It does not alter folder paths.
			///     See also: Project Settings => Editor => Numbering Scheme
			///     Note: 'Uniquify' is a proper english verb, it means "to make unique".
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static AssetPath UniquifyFileNameIfAssetExists(String path)
			{
				var uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
				return (AssetPath)(String.IsNullOrEmpty(uniquePath) ? path : uniquePath);
			}

			/// <summary>
			///     Tests if the given file exists.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean FileExists(String path) => File.Exists(path);

			/// <summary>
			///     Tests if the given file exists.
			/// </summary>
			/// <param name="assetPath"></param>
			/// <returns></returns>
			public static Boolean FileExists(AssetPath assetPath) => FileExists((String)assetPath);

			/// <summary>
			///     Returns true if the folder exists. Also works with paths pointing to a file.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>true if the folder exists</returns>
			public static Boolean FolderExists(String path) => AssetDatabase.IsValidFolder(path);

			/// <summary>
			///     Returns true if the folder exists. False otherwise, or if the path is to a file.
			/// </summary>
			/// <param name="assetPath">path to a file or folder</param>
			/// <returns>true if the folder exists</returns>
			public static Boolean FolderExists(AssetPath assetPath)
			{
				ThrowIf.ArgumentIsNull(assetPath, nameof(assetPath));
				return FolderExists((String)assetPath);
			}

			/// <summary>
			///     Creates the folders in the path recursively. Path may point to a file, but only folders
			///     will be created.
			/// </summary>
			/// <param name="path">path to a file or folder</param>
			/// <returns>the GUID of the deepest folder in the hierarchy</returns>
			public static GUID CreateFolders(String path) => CreateFolders((AssetPath)path);

			/// <summary>
			///     Creates the folders in the path recursively. Path may point to a file, but only folders
			///     will be created.
			/// </summary>
			/// <param name="assetPath">path to a file or folder</param>
			/// <returns>the GUID of the deepest folder in the hierarchy</returns>
			public static GUID CreateFolders(AssetPath assetPath)
			{
				ThrowIf.ArgumentIsNull(assetPath, nameof(assetPath));

				var folderPath = assetPath.FolderPathAssumptive;
				if (FileExists(assetPath) || FolderExists(folderPath))
					return Guid.Get(folderPath);

				var folderNames = folderPath.Split(new[] { '/' });
				var folderGuid = Guid.Get(folderNames[0]); // first is "Assets"
				var partialPath = folderNames[0];
				for (var i = 1; i < folderNames.Length; i++)
				{
					partialPath += $"/{folderNames[i]}";
					if (FolderExists(partialPath))
					{
						folderGuid = Guid.Get(partialPath);
						continue;
					}

					var guidString = AssetDatabase.CreateFolder(Get(folderGuid), folderNames[i]);
					folderGuid = new GUID(guidString);
				}

				return folderGuid;
			}


		}
	}
}
