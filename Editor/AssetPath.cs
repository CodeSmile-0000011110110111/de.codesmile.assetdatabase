// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace CodeSmile.Editor
{
	/// <summary>
	///     Represents a relative path to an asset file in the project.
	///     Instances are implicitly and explicitly convertible to/from string.
	///     All path separators are forward slashes for compatibility with Windows, Mac, Linux.
	///     Leading and trailing path separators are trimmed.
	///     Instances can be initialized with a relative or full (absolute) path, or a relative/full path
	///     to a directory plus a filename and optional extension (default: 'asset').
	/// </summary>
	public class AssetPath : IEquatable<AssetPath>, IEquatable<String>
	{
		public const String DefaultExtension = "asset";

		private readonly String m_RelativePath = String.Empty;

		/// <summary>
		///     Creates and returns the relative asset path. The file name may be appeneded with a number to ensure
		///     the path does not point to an existing asset file.
		///     See also: Project Settings => Editor => Numbering Scheme
		/// </summary>
		public AssetPath UniquePath => (AssetPath)AssetDatabase.GenerateUniqueAssetPath(m_RelativePath);

		/// <summary>
		///     Creates and returns the full path, with forward slashes as separators.
		/// </summary>
		public String FullPath => Path.GetFullPath(m_RelativePath).ToForwardSlashes();
		public String FolderPath
		{
			get
			{
				if (Directory.Exists(m_RelativePath))
					return m_RelativePath;

				return Path.GetDirectoryName(m_RelativePath).ToForwardSlashes();
			}
		}

		public Boolean FileExists => File.Exists(Path.GetFullPath(m_RelativePath));
		public Boolean FolderExists => Asset.Path.FolderExists(this);

		public static String FullProjectPath => FullAssetsPath.Substring(0, FullAssetsPath.Length - 6);
		public static String FullAssetsPath => Application.dataPath;
		public static String FullPackagesPath => $"{FullProjectPath}/Packages";

		/// <summary>
		///     Implicit conversion to string (relative asset path). Same as ToString().
		/// </summary>
		public static implicit operator String(AssetPath assetPath) => assetPath.m_RelativePath;

		/// <summary>
		///     Explicit creation of an AssetPath instance initialized with a string path (full or relative).
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

		public static Boolean operator ==(Object other, AssetPath ap2) => ap2 == other;

		public static Boolean operator !=(Object other, AssetPath ap2) => !(ap2 == other);

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
		///     Creates an asset path pointing to the 'Assets' root folder.
		/// </summary>
		public AssetPath() : this("Assets") {}

		/// <summary>
		///     Creates an asset path from either a full or relative path.
		///     Example input path: 'C:\Users\Urso Clever\Untiy Projects\First Projcet\Assets\something'
		///     Resulting AssetPath: 'Assets/something'
		/// </summary>
		/// <param name="fullOrRelativePath">Absolute path to project's root folder or a subfolder.</param>
		/// <exception cref="ArgumentException">
		///     If path does not start with the project's Assets path (Application.dataPath).
		/// </exception>
		/// <returns></returns>
		public AssetPath(String fullOrRelativePath)
		{
			ThrowIf.NullOrWhitespace(fullOrRelativePath, nameof(fullOrRelativePath));
			m_RelativePath = ToRelative(fullOrRelativePath.ToForwardSlashes());
		}

		/// <summary>
		///     Creates an asset path from either a full or relative path to a directory, a filename,
		///     and an optional extension (default: 'asset').
		///     Example input: '\Assets\Some/Sub\Dir' - 'New File' - 'MyExt'
		///     Resulting AssetPath: 'Assets/Some/Sub/Dir/New File.myext'
		/// </summary>
		/// <param name="directory">
		///     Path to a directory under Assets, or just 'Assets'. Can be a relative or absolute path.
		///     Leading/trailing path separators are trimmed.
		/// </param>
		/// <param name="fileName">Name of the file without extension. Must not contain path separators.</param>
		/// <param name="extension">
		///     Extension of the file (default: 'asset'). Leading dots will be trimmed. Extension will be
		///     lowercase.
		/// </param>
		/// <returns>The relative path to the asset file. </returns>
		/// <exception cref="ArgumentException"></exception>
		public AssetPath(String directory, String fileName, String extension = DefaultExtension)
		{
			ThrowIf.NullOrWhitespace(directory, nameof(directory));
			ThrowIf.NullOrWhitespace(fileName, nameof(fileName));
			ThrowIf.NullOrWhitespace(extension, nameof(extension));
			ThrowIf.ContainsPathSeparators(fileName, nameof(fileName));

			var relativeDir = ToRelative(directory.ToForwardSlashes());
			m_RelativePath = $"{relativeDir}/{fileName}.{extension.TrimStart('.').ToLower()}";
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

		/// <summary>
		///     Creates the path's folders recursively.
		/// </summary>
		public void CreateDirectories() => Asset.Path.CreateFolders(this);

		public override Boolean Equals(Object obj) => Equals(obj as AssetPath);

		public override Int32 GetHashCode() => m_RelativePath.GetHashCode();

		/// <summary>
		///     Returns the relative path as string. Same as implicit string conversion.
		/// </summary>
		/// <returns></returns>
		public override String ToString() => m_RelativePath;
	}
}
