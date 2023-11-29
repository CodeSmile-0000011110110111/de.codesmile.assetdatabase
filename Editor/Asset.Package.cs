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
		///     Groups import/export functionality for
		///     <a href="https://docs.unity3d.com/Manual/AssetPackages.html">.unitypackage files</a> (Asset Packages).
		/// </summary>
		/// <remarks>
		///     Does not contain Package Manager (npm packages) functionality.
		/// </remarks>
		public static class Package
		{
			/// <summary>
			///     Silently imports a .unitypackage file at the given path.
			/// </summary>
			/// <param name="packagePath">Path to file with the .unitypackage extension.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Package.ImportInteractive" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportPackage.html">AssetDatabase.ImportPackage</a>
			/// </seealso>
			public static void Import(Path packagePath)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

				AssetDatabase.ImportPackage(packagePath, false);
			}

			/// <summary>
			///     Imports a .unitypackage file at the given path interactively.
			/// </summary>
			/// <remarks> Shows the import package dialogue to the user before importing.</remarks>
			/// <param name="packagePath">Path to file with the .unitypackage extension.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Package.Import" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportPackage.html">AssetDatabase.ImportPackage</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // not testable
			public static void ImportInteractive(Path packagePath)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

				AssetDatabase.ImportPackage(packagePath, true);
			}

			/// <summary>
			///     Exports the asset and its dependencies to a .unitypackage file.
			/// </summary>
			/// <param name="assetPath">The asset to export.</param>
			/// <param name="packagePath">Path to file with the .unitypackage extension.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ExportPackageOptions.html">ExportPackageOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Package.Export(String[],String,ExportPackageOptions)" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ExportPackage.html">AssetDatabase.ExportPackage</a>
			/// </seealso>
			public static void Export(Path assetPath, String packagePath,
				ExportPackageOptions options = ExportPackageOptions.Default)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

				AssetDatabase.ExportPackage(assetPath, packagePath, options);
			}

			/// <summary>
			///     Exports multiple assets and their dependencies to the packagePath file.
			/// </summary>
			/// <param name="assetPaths">The assets to export.</param>
			/// <param name="packagePath">Path to file with the .unitypackage extension.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ExportPackageOptions.html">ExportPackageOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Package.Export(CodeSmile.Editor.Asset.Path,String,ExportPackageOptions)" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ExportPackage.html">AssetDatabase.ExportPackage</a>
			/// </seealso>
			public static void Export(Path[] assetPaths, String packagePath,
				ExportPackageOptions options = ExportPackageOptions.Default) =>
				Export(Path.ToStrings(assetPaths), packagePath, options);

			/// <summary>
			///     Exports multiple assets and their dependencies to the packagePath file.
			/// </summary>
			/// <param name="assetPaths">The assets to export.</param>
			/// <param name="packagePath">Path to file with the .unitypackage extension.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ExportPackageOptions.html">ExportPackageOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Package.Export(CodeSmile.Editor.Asset.Path,String,ExportPackageOptions)" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ExportPackage.html">AssetDatabase.ExportPackage</a>
			/// </seealso>
			public static void Export(String[] assetPaths, String packagePath,
				ExportPackageOptions options = ExportPackageOptions.Default)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

				AssetDatabase.ExportPackage(assetPaths, packagePath, options);
			}
		}
	}
}
