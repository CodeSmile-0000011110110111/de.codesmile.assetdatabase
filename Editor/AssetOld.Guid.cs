// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public static partial class AssetOld
	{
		/*public static class Guid
		{
			public static GUID Get(Object obj) => AssetDatabase.GUIDFromAssetPath(Path.Get(obj));
			public static GUID Get(String path) => AssetDatabase.GUIDFromAssetPath(Path.ToRelative(path));

			public static GUID Get(String path, Boolean includeRecentlyDeleted) => new(
				AssetDatabase.AssetPathToGUID(Path.ToRelative(path), includeRecentlyDeleted
					? AssetPathToGUIDOptions.IncludeRecentlyDeletedAssets
					: AssetPathToGUIDOptions.OnlyExistingAssets));
		}*/


	}
}
