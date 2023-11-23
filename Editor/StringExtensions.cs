// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.Editor
{
	/// <summary>
	/// Extensions for System.String
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		///     Converts any backslashes in the path to forward slashes to ensure the path works correctly
		///     on all supported editor platforms (Windows, Mac, Linux).
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static String ToForwardSlashes(this String path) => path.Replace('\\', '/');
	}
}
