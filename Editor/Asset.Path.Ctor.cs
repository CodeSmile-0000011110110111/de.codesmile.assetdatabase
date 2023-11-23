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
			[ExcludeFromCodeCoverage] private Path() {} // disallowed parameterless ctor

			/// <summary>
			///     Creates an asset path from either a full or relative path.
			///     Example input path: "C:\Users\Urso Clever\Untiy Projects\First Projcet\Assets\something"
			///     Resulting AssetPath: "Assets/something"
			/// </summary>
			/// <param name="fullOrRelativePath">Absolute path to project's root folder or a subfolder.</param>
			/// <exception cref="ArgumentException">
			///     If path does not start with the project's Assets path (Application.dataPath).
			/// </exception>
			public Path(String fullOrRelativePath)
			{
				ThrowIf.NullOrWhitespace(fullOrRelativePath, nameof(fullOrRelativePath));
				m_RelativePath = ToRelative(fullOrRelativePath.ToForwardSlashes());
			}

			/// <summary>
			///     Creates an asset path from either a full or relative path to a directory, a filename,
			///     and an optional extension (default: 'asset').
			/// </summary>
			/// <example>
			///     Example parameters:
			///     <c>"\Assets\Some/Sub\Dir", "New File", "MyExt"</c>
			/// </example>
			/// <example>
			///     Resulting path:
			///     <c>"Assets/Some/Sub/Dir/New File.myext"</c>
			/// </example>
			/// <param name="directory">
			///     Path to a directory under Assets, or just 'Assets'. Can be a relative or absolute path.
			///     Leading/trailing path separators are trimmed.
			/// </param>
			/// <param name="fileName">Name of the file without extension. Must not contain path separators.</param>
			/// <param name="extension">
			///     Extension of the file (default: 'asset'). Leading dots will be trimmed. Extension will be
			///     lowercase.
			/// </param>
			/// <exception cref="ArgumentException"></exception>
			public Path(String directory, String fileName, String extension = DefaultExtension)
			{
				ThrowIf.NullOrWhitespace(directory, nameof(directory));
				ThrowIf.NullOrWhitespace(fileName, nameof(fileName));
				ThrowIf.NullOrWhitespace(extension, nameof(extension));
				ThrowIf.ContainsPathSeparators(fileName, nameof(fileName));

				var relativeDir = ToRelative(directory.ToForwardSlashes());
				m_RelativePath = $"{relativeDir}/{fileName}.{extension.TrimStart('.').ToLower()}";
			}
		}
	}
}
