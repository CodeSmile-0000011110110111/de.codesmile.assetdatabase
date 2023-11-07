// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

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

			/// <summary>
			///     Returns the relative path as string. Same as implicit string conversion.
			/// </summary>
			/// <returns></returns>
			public override String ToString() => m_RelativePath;
		}
	}
}