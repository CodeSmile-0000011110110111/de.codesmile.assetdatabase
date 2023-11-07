// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

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
			///     Creates and returns the full path, with forward slashes as separators.
			/// </summary>
			public String FullPath => System.IO.Path.GetFullPath(m_RelativePath).ToForwardSlashes();
		}
	}
}
