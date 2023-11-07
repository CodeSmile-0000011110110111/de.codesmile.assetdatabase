// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

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
		}
	}
}