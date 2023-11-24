// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Exports this asset and its dependencies as a .unitypackage.
		/// </summary>
		/// <param name="packagePath"></param>
		/// <param name="options"></param>
		public void ExportPackage(String packagePath, ExportPackageOptions options = ExportPackageOptions.Default)
		{
			ThrowIf.AssetDeleted(this);

			Package.Export(m_AssetPath, packagePath, options);
		}

		/// <summary>
		///     Groups asset package import/export functionality, eg files with '.unitypackage' extension.
		///     Does not contain PackageManager functionality.
		/// </summary>
		public static class Package
		{
			/// <summary>
			///     Silently imports a .unitypackage file at the given path.
			///     Note: This is not used for importing Package Manager packages!
			/// </summary>
			/// <param name="packagePath">path to file with the .unitypackage extension</param>
			/// <see cref="ImportInteractive" />
			public static void Import(Path packagePath)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

				AssetDatabase.ImportPackage(packagePath, false);
			}

			/// <summary>
			///     Imports a .unitypackage file at the given path. Shows the import package dialogue to the
			///     user before import.
			///     Note: This is not used for importing Package Manager packages!
			/// </summary>
			/// <param name="packagePath">path to file with the .unitypackage extension</param>
			/// <see cref="Import" />
			[ExcludeFromCodeCoverage] // not testable
			public static void ImportInteractive(Path packagePath)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

				AssetDatabase.ImportPackage(packagePath, true);
			}

			/// <summary>
			///     Exports the asset and its dependencies to the packagePath file.
			/// </summary>
			/// <param name="assetPath"></param>
			/// <param name="packagePath">path to file with the .unitypackage extension</param>
			/// <param name="options"></param>
			public static void Export(Path assetPath, String packagePath,
				ExportPackageOptions options = ExportPackageOptions.Default)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

				AssetDatabase.ExportPackage(assetPath, packagePath, options);
			}

			/// <summary>
			///     Exports multiple assets and their dependencies to the packagePath file.
			/// </summary>
			/// <param name="assetPaths"></param>
			/// <param name="packagePath"></param>
			/// <param name="options"></param>
			public static void Export(Path[] assetPaths, String packagePath,
				ExportPackageOptions options = ExportPackageOptions.Default) =>
				Export(Path.ToStrings(assetPaths), packagePath, options);

			/// <summary>
			///     Exports multiple assets and their dependencies to the packagePath file.
			/// </summary>
			/// <param name="assetPaths"></param>
			/// <param name="packagePath"></param>
			/// <param name="options"></param>
			public static void Export(String[] assetPaths, String packagePath,
				ExportPackageOptions options = ExportPackageOptions.Default)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

				AssetDatabase.ExportPackage(assetPaths, packagePath, options);
			}
		}
	}
}
