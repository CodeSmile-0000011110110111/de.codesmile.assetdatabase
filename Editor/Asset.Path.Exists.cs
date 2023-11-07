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
		}
	}
}
