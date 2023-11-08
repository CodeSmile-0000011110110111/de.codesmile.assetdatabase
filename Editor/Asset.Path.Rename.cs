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
		public partial class Path
		{
			/// <summary>
			///     Renames the file or folder with a new name.
			/// </summary>
			/// <param name="newFileOrFolderName"></param>
			/// <returns>True if rename succeeded, false otherwise.</returns>
			public Boolean Rename(String newFileOrFolderName)
			{
				if (String.IsNullOrEmpty(newFileOrFolderName))
					return false;

				m_RelativePath = $"{DirectoryName}/{System.IO.Path.GetFileName(newFileOrFolderName)}";
				return true;
			}
		}
	}
}
