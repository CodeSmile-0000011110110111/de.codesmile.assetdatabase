// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEngine;
using Object = System.Object;

namespace CodeSmile.Editor
{
	public class AssetPath : IEquatable<AssetPath>, IEquatable<String>
	{
		public const String DefaultExtension = "asset";

		private readonly String m_RelativePath = String.Empty;

		/// <summary>
		///     Creates and returns the full path with forward slashes as separators.
		/// </summary>
		public String FullPath => Path.GetFullPath(m_RelativePath).NormalizePathSeparators();

		/// <summary>
		///     Returns the relative path. Same as ToString().
		/// </summary>
		public static implicit operator String(AssetPath assetPath) => assetPath.m_RelativePath;

		/// <summary>
		///     Creates and returns an AssetPath instance from a string.
		/// </summary>
		/// <param name="fullOrRelativePath"></param>
		/// <returns></returns>
		public static explicit operator AssetPath(String fullOrRelativePath) => new(fullOrRelativePath);

		public static Boolean operator ==(AssetPath ap1, AssetPath ap2)
		{
			if (ReferenceEquals(ap1, ap2))
				return true;
			if (ReferenceEquals(ap1, null))
				return false;
			if (ReferenceEquals(ap2, null))
				return false;

			return ap1.Equals(ap2);
		}

		public static Boolean operator !=(AssetPath ap1, AssetPath ap2) => !(ap1 == ap2);

		public static Boolean operator ==(AssetPath ap1, Object other) =>
			other is String str ? ap1.Equals(str) : ap1.Equals(other as AssetPath);

		public static Boolean operator !=(AssetPath ap1, Object other) => !(ap1 == other);

		public static Boolean operator ==(Object other, AssetPath ap2) => (ap2 == other);

		public static Boolean operator !=(Object other, AssetPath ap2) => !(ap2 == other);

		private static String ToRelative(String fullOrRelativePath)
		{
			var relativePath = fullOrRelativePath;
			if (IsRelative(fullOrRelativePath) == false)
			{
				ThrowIfNotAProjectPath(fullOrRelativePath, fullOrRelativePath);
				relativePath = fullOrRelativePath.Substring(Application.dataPath.Length - 6);
			}

			return relativePath.Trim('/');
		}

		private static Boolean IsRelative(String normalizedPath)
		{
			// path must start with "Assets" and it's either just "Assets" (length == 6) or if the path is longer
			// it must be followed by a path separator, eg "Assets/.."
			normalizedPath = normalizedPath.TrimStart('/');
			var startsWithAssets = normalizedPath.ToLower().StartsWith("assets");
			return startsWithAssets && (normalizedPath.Length <= 6 || IsPathSeparator(normalizedPath[6]));
		}

		private static void ThrowIfNotAProjectPath(String normalizedPath, String fullPath)
		{
			var dataPath = Application.dataPath;
			if (normalizedPath.StartsWith(dataPath) == false)
			{
				throw new ArgumentException(
					$"not a project path: '{fullPath}' - should start with: {dataPath.Substring(0, dataPath.Length - 6)}");
			}
		}

		private static void ThrowIfNullOrWhitespace(String param, String paramName)
		{
#if DEBUG
			if (param == null)
				throw new ArgumentNullException($"{paramName} is null");
			if (String.IsNullOrWhiteSpace(param))
				throw new ArgumentException($"{paramName} is empty or whitespace");
#endif
		}

		private static Boolean IsPathSeparator(Char c) => c.Equals('/') || c.Equals('\\');

		public AssetPath() : this(Application.dataPath) {}

		/// <summary>
		///     Normalizes a path to forward slashes and makes the path relative so it starts with 'Assets'.
		///     Also trims any leading and trailing slashes.
		///     Example input path: 'C:\Users\Urso Clever\Untiy Projects\First Porject\Assets\something'
		///     Asset path: 'Assets/something'
		///     Notes: Input path is required to point to the project's root or below.
		///     Path separators are normalized to forward slashes.
		/// </summary>
		/// <param name="fullOrRelativePath">Absolute path to project's root folder or a subfolder.</param>
		/// <exception cref="ArgumentException">
		///     If path does not point to the current project's root folder or one of its subfolders.
		/// </exception>
		/// <returns></returns>
		public AssetPath(String fullOrRelativePath)
		{
			ThrowIfNullOrWhitespace(fullOrRelativePath, nameof(fullOrRelativePath));
			m_RelativePath = ToRelative(fullOrRelativePath.NormalizePathSeparators());
		}

		/// <summary>
		///     Combines a path to a folder in the project with a filename and extension to form a valid path string
		///     to an asset file.
		///     Note: Path separators get normalized and trimmed, for example these are valid input paths:
		///     'Assets\\folder'
		///     '\\Assets/folder'
		///     '/Assets/folder\\'
		/// </summary>
		/// <param name="directory">Path to a folder for the file. Can be a relative or absolute path.</param>
		/// <param name="fileName">Name of the file without extension.</param>
		/// <param name="extension">Extension of the file (default: 'asset'). Leading dots will be trimmed.</param>
		/// <returns>
		///     The relative path to the asset file. For example if input is 'Assets/folder', 'file'
		///     and 'ext' the returned path will be 'Assets/folder/file.ext'
		/// </returns>
		/// <exception cref="ArgumentException"></exception>
		public AssetPath(String directory, String fileName, String extension = DefaultExtension)
		{
			ThrowIfNullOrWhitespace(directory, nameof(directory));
			ThrowIfNullOrWhitespace(fileName, nameof(fileName));
			ThrowIfNullOrWhitespace(extension, nameof(extension));
			ThrowIfContainsPathSeparators(fileName, nameof(fileName));

			var relativeDir = ToRelative(directory.NormalizePathSeparators());
			m_RelativePath = $"{relativeDir}/{fileName}.{extension.TrimStart('.')}";
		}

		public Boolean Equals(AssetPath other)
		{
			if (ReferenceEquals(other, null))
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return m_RelativePath.Equals(other.m_RelativePath);
		}

		public Boolean Equals(String other) => m_RelativePath.Equals(new AssetPath(other).m_RelativePath);
		public override Boolean Equals(Object obj) => Equals(obj as AssetPath);

		public override Int32 GetHashCode() => m_RelativePath.GetHashCode();

		private void ThrowIfContainsPathSeparators(String fileName, String paramName)
		{
			var normalized = fileName.NormalizePathSeparators();
			if (normalized.Contains('/'))
				throw new ArgumentException("filename contains path separators", paramName);
		}

		/// <summary>
		///     Returns the relative path as string. Same as implicit string conversion.
		/// </summary>
		/// <returns></returns>
		public override String ToString() => m_RelativePath;
	}
}
