// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmileEditor
{
	/// <summary>
	///     Extensions for System.String
	/// </summary>
	internal static class StringExtensions
	{
		/// <summary>
		///     Converts any backslashes in the path to forward slashes
		/// </summary>
		/// <remarks>
		///     Forward slashes are compatible with all supported Unity Editor platforms: Windows, Mac, Linux.
		///     Backslashes only work on Windows.
		/// </remarks>
		/// <param name="path">Input string.</param>
		/// <returns>The string with any backslashes replaced by forward slashes.</returns>
		internal static String ToForwardSlashes(this String path) => path.Replace('\\', '/');
	}
}
