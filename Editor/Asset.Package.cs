// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
#if UNITY_6000_6_OR_NEWER
// Unity 6000.6 made AssetDatabase.ImportPackage and AssetDatabase.ExportPackage obsolete and
// replaced them with UnityEditor.AssetPackage.Package.
using UnityEditor.AssetPackage;
#endif

namespace CodeSmileEditor
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
			///     - <see cref="CodeSmileEditor.Asset.Package.ImportInteractive" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportPackage.html">AssetDatabase.ImportPackage</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static void Import([NotNull] Path packagePath)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

#if UNITY_6000_6_OR_NEWER
				UnityEditor.AssetPackage.Package.Import(packagePath, interactive: false);
#else
				AssetDatabase.ImportPackage(packagePath, false);
#endif
			}

			/// <summary>
			///     Imports a .unitypackage file at the given path interactively.
			/// </summary>
			/// <remarks> Shows the import package dialogue to the user before importing.</remarks>
			/// <param name="packagePath">Path to file with the .unitypackage extension.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Package.Import" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportPackage.html">AssetDatabase.ImportPackage</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // not testable
			public static void ImportInteractive([NotNull] Path packagePath)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

#if UNITY_6000_6_OR_NEWER
				UnityEditor.AssetPackage.Package.Import(packagePath, interactive: true);
#else
				AssetDatabase.ImportPackage(packagePath, true);
#endif
			}

			/// <summary>
			///     Exports the asset and its dependencies to a .unitypackage file.
			/// </summary>
			/// <param name="assetPath">The asset to export.</param>
			/// <param name="packagePath">Path to file with the .unitypackage extension.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ExportPackageOptions.html">ExportPackageOptions</a>
			/// </param>
#if UNITY_6000_6_OR_NEWER
			/// <param name="ownerOrgId">
			///     The organization ID Unity associates with the exported package as its signing organization.
			///     The Organization ID is in the Unity Cloud dashboard under Administration => Settings.
			///     Available in Unity 6000.6 and newer. Leaving it null exports exactly as before.
			/// </param>
#endif
			/// <seealso cref="">
#if UNITY_6000_6_OR_NEWER
			///     - <see cref="CodeSmileEditor.Asset.Package.Export(String[],String,ExportPackageOptions,String)" />
#else
			///     - <see cref="CodeSmileEditor.Asset.Package.Export(String[],String,ExportPackageOptions)" />
#endif
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ExportPackage.html">AssetDatabase.ExportPackage</a>
			/// </seealso>
			public static void Export([NotNull] Path assetPath, [NotNull] String packagePath,
				ExportPackageOptions options = ExportPackageOptions.Default
#if UNITY_6000_6_OR_NEWER
				, String ownerOrgId = null
#endif
			)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

#if UNITY_6000_6_OR_NEWER
				UnityEditor.AssetPackage.Package.Export(new ExportPackageParameters(
					assetPathName: assetPath, fileName: packagePath, ownerOrgId: ownerOrgId, flags: options));
#else
				AssetDatabase.ExportPackage(assetPath, packagePath, options);
#endif
			}

			/// <summary>
			///     Exports multiple assets and their dependencies to the packagePath file.
			/// </summary>
			/// <param name="assetPaths">The assets to export.</param>
			/// <param name="packagePath">Path to file with the .unitypackage extension.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ExportPackageOptions.html">ExportPackageOptions</a>
			/// </param>
#if UNITY_6000_6_OR_NEWER
			/// <param name="ownerOrgId">
			///     The organization ID Unity associates with the exported package as its signing organization.
			///     The Organization ID is in the Unity Cloud dashboard under Administration => Settings.
			///     Available in Unity 6000.6 and newer. Leaving it null exports exactly as before.
			/// </param>
#endif
			/// <seealso cref="">
#if UNITY_6000_6_OR_NEWER
			///     -
			///     <see
			///         cref="CodeSmileEditor.Asset.Package.Export(CodeSmileEditor.Asset.Path,String,ExportPackageOptions,String)" />
#else
			///     - <see cref="CodeSmileEditor.Asset.Package.Export(CodeSmileEditor.Asset.Path,String,ExportPackageOptions)" />
#endif
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ExportPackage.html">AssetDatabase.ExportPackage</a>
			/// </seealso>
			public static void Export([NotNull] Path[] assetPaths, [NotNull] String packagePath,
				ExportPackageOptions options = ExportPackageOptions.Default
#if UNITY_6000_6_OR_NEWER
				, String ownerOrgId = null
#endif
			) =>
#if UNITY_6000_6_OR_NEWER
				Export(Path.ToStrings(assetPaths), packagePath, options, ownerOrgId);
#else
				Export(Path.ToStrings(assetPaths), packagePath, options);
#endif

			/// <summary>
			///     Exports multiple assets and their dependencies to the packagePath file.
			/// </summary>
			/// <param name="assetPaths">The assets to export.</param>
			/// <param name="packagePath">Path to file with the .unitypackage extension.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ExportPackageOptions.html">ExportPackageOptions</a>
			/// </param>
#if UNITY_6000_6_OR_NEWER
			/// <param name="ownerOrgId">
			///     The organization ID Unity associates with the exported package as its signing organization.
			///     The Organization ID is in the Unity Cloud dashboard under Administration => Settings.
			///     Available in Unity 6000.6 and newer. Leaving it null exports exactly as before.
			/// </param>
#endif
			/// <seealso cref="">
#if UNITY_6000_6_OR_NEWER
			///     -
			///     <see
			///         cref="CodeSmileEditor.Asset.Package.Export(CodeSmileEditor.Asset.Path,String,ExportPackageOptions,String)" />
#else
			///     - <see cref="CodeSmileEditor.Asset.Package.Export(CodeSmileEditor.Asset.Path,String,ExportPackageOptions)" />
#endif
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ExportPackage.html">AssetDatabase.ExportPackage</a>
			/// </seealso>
			public static void Export([NotNull] String[] assetPaths, [NotNull] String packagePath,
				ExportPackageOptions options = ExportPackageOptions.Default
#if UNITY_6000_6_OR_NEWER
				, String ownerOrgId = null
#endif
			)
			{
				ThrowIf.ExtensionIsNotUnityPackage(packagePath);

#if UNITY_6000_6_OR_NEWER
				UnityEditor.AssetPackage.Package.Export(new ExportPackageParameters(
					assetPathNames: assetPaths, fileName: packagePath, ownerOrgId: ownerOrgId, flags: options));
#else
				AssetDatabase.ExportPackage(assetPaths, packagePath, options);
#endif
			}
		}
	}
}
