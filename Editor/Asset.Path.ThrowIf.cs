// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public partial class Path
		{
			public static class ThrowIf
			{
				public static void ArgumentIsNull(Object arg, [NotNull] String argName)
				{
					if (arg == null)
						throw new ArgumentNullException(argName);
				}

				public static void NotAProjectPath(String normalizedPath, String fullPath)
				{
					var rootPath = FullProjectPath;
					if (normalizedPath.StartsWith(rootPath) == false)
						throw new ArgumentException($"not a project path: '{fullPath}' - must start with: {rootPath}");
				}

				public static void NullOrWhitespace(String param, String paramName)
				{
					if (param == null)
						throw new ArgumentNullException($"{paramName} is null");
					if (String.IsNullOrWhiteSpace(param))
						throw new ArgumentException($"{paramName} is empty or whitespace");
				}

				public static void ContainsPathSeparators(String fileName, String paramName)
				{
					var normalized = fileName.ToForwardSlashes();
					if (normalized.Contains('/'))
						throw new ArgumentException($"filename contains path separators: '{fileName}'", paramName);
				}
			}
		}
	}
}