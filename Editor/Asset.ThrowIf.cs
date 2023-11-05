// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		internal static class ThrowIf
		{
			public static void ArgumentIsNull(Object arg, String argName)
			{
				if (arg == null)
					throw new ArgumentNullException(argName);
			}

			public static void DoesNotExist(Path path)
			{
				if (path.Exists == false)
					throw new FileNotFoundException($"file/folder does not exist: '{path}'");
			}

			public static void IsExistingAsset(UnityEngine.Object obj)
			{
				if (Database.Contains(obj))
					throw new ArgumentException($"object already is an asset file: {obj}");
			}

			public static void NotAnAssetAtPath(UnityEngine.Object obj, Path path)
			{
				if (Database.Contains(obj) == false)
					throw new ArgumentException("path does not exist or not imported", path);
			}

			public static void NotAnAsset(UnityEngine.Object obj)
			{
				if (Database.Contains(obj) == false)
					throw new ArgumentException($"object is not an asset file: {obj}");
			}

			public static void NotAnAsset(GUID guid)
			{
				if (Path.Get(guid) == null)
					throw new ArgumentException($"guid {guid} is not an asset file");
			}
		}
	}
}
