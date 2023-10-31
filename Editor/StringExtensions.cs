using System;

namespace CodeSmile.Editor
{
	public static class StringExtensions
	{
		/// <summary>
		///     Converts any backslashes in the path to forward slashes to ensure the path works correctly
		///     on all supported editor platforms (Windows, Mac, Linux).
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static String NormalizePathSeparators(this String path) => path.Replace('\\', '/');
	}
}