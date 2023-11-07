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
			///     Returns the GUID for the path.
			///     Returns an empty GUID if the asset at the path does not exist in the database.
			///     <see cref="Exists" />
			///     <see cref="ExistsInFileSystem" />
			/// </summary>
			/// <param name="path"></param>
			/// <param name="options"></param>
			/// <returns></returns>
			public static GUID GetGuid(Path path,
				AssetPathToGUIDOptions options = AssetPathToGUIDOptions.IncludeRecentlyDeletedAssets) =>
				new(AssetDatabase.AssetPathToGUID(path, options));

			/// <summary>
			///     Returns the GUID for the path.
			///     Returns an empty GUID if the asset at the path does not exist in the database.
			///     <see cref="Exists" />
			///     <see cref="ExistsInFileSystem" />
			/// </summary>
			/// <param name="path"></param>
			/// <param name="options"></param>
			/// <returns></returns>
			public static GUID GetGuid(String path,
				AssetPathToGUIDOptions options = AssetPathToGUIDOptions.IncludeRecentlyDeletedAssets) =>
				GetGuid((Path)path);
		}
	}
}
