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
			///     Returns the path either unaltered or with a numbering to make the file unique if an asset file
			///     already exists at the path. Does not alter path if it does not exist or points to a folder.
			///     See also: Project Settings => Editor => Numbering Scheme
			/// </summary>
			public Path UniqueFilePath => UniquifyFilename(this);

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

			internal static Path GetOverwriteOrUnique(Path destPath, Boolean overwriteExisting) =>
				overwriteExisting ? destPath : destPath.UniqueFilePath;
		}
	}
}