﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;

namespace CodeSmileEditor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Groups all asset bundle related functionality.
		/// </summary>
		/// <remarks>
		///     Note: Unity recommends to use the Addressables package to manage Bundles rather than through code.
		/// </remarks>
		public static class Bundle
		{
			/// <summary>
			///     Returns all asset bundle names.
			/// </summary>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.Unused" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAllAssetBundleNames.html">AssetDatabase.GetAllAssetBundleNames</a>
			/// </seealso>
			public static String[] All => AssetDatabase.GetAllAssetBundleNames();

			/// <summary>
			///     Returns all unused asset bundle names.
			/// </summary>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.All" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetUnusedAssetBundleNames.html">AssetDatabase.GetUnusedAssetBundleNames</a>
			/// </seealso>
			public static String[] Unused => AssetDatabase.GetUnusedAssetBundleNames();

			/// <summary>
			///     Removes all unused asset bundles.
			/// </summary>
			/// <summary>
			///     Returns all unused asset bundle names.
			/// </summary>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.Unused" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.RemoveUnusedAssetBundleNames.html">AssetDatabase.RemoveUnusedAssetBundleNames</a>
			/// </seealso>
			public static void RemoveUnused() => AssetDatabase.RemoveUnusedAssetBundleNames();

			/// <summary>
			///     Removes a specific asset bundle by name.
			/// </summary>
			/// <remarks>If the bundle is currently in use, it will NOT be removed.</remarks>
			/// <param name="bundleName">Name of the asset bundle to remove.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.ForceRemove" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.RemoveAssetBundleName.html">AssetDatabase.RemoveAssetBundleName</a>
			/// </seealso>
			public static void Remove([NotNull] String bundleName) => AssetDatabase.RemoveAssetBundleName(bundleName, false);

			/// <summary>
			///     Removes a specific asset bundle by name.
			/// </summary>
			/// <remarks>CAUTION: The bundle is removed even if it is currently in use.</remarks>
			/// <param name="bundleName">Name of the asset bundle to remove.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.Remove" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.RemoveAssetBundleName.html">AssetDatabase.RemoveAssetBundleName</a>
			/// </seealso>
			public static void ForceRemove([NotNull] String bundleName) => AssetDatabase.RemoveAssetBundleName(bundleName, true);

			/// <summary>
			///     Returns the bundle names that the given asset bundle directly depends on.
			/// </summary>
			/// <param name="bundleName">Name of the asset bundle.</param>
			/// <returns>Directly dependent asset bundle names or an empty array if there are no direct dependencies.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.GetAllDependencies" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetBundleDependencies.html">AssetDatabase.GetAssetBundleDependencies</a>
			/// </seealso>
			public static String[] GetDirectDependencies([NotNull] String bundleName) =>
				AssetDatabase.GetAssetBundleDependencies(bundleName, false);

			/// <summary>
			///     Returns all bundle names that the given asset bundle depends on, directly or indirectly (recursive).
			/// </summary>
			/// <param name="bundleName">Name of the asset bundle.</param>
			/// <returns>Directly dependent asset bundle names or an empty array if there are no direct dependencies.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.GetAllDependencies" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetBundleDependencies.html">AssetDatabase.GetAssetBundleDependencies</a>
			/// </seealso>
			public static String[] GetAllDependencies([NotNull] String bundleName) =>
				AssetDatabase.GetAssetBundleDependencies(bundleName, true);

			/// <summary>
			///     Returns all asset paths that are part of a given asset bundle.
			/// </summary>
			/// <param name="bundleName">Name of the asset bundle.</param>
			/// <returns>The paths to assets belonging to this bundle. Is empty if no assets belong to the bundle.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.GetPaths" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetPathsFromAssetBundle.html">AssetDatabase.GetAssetPathsFromAssetBundle</a>
			/// </seealso>
			public static String[] GetAllPaths([NotNull] String bundleName) => AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

			/// <summary>
			///     Returns the asset paths in a given asset bundle that matches an asset name.
			/// </summary>
			/// <param name="bundleName">Name of the asset bundle.</param>
			/// <param name="assetName">Filter string that asset name needs to match.</param>
			/// <returns>The paths to assets whose name matches the filter string. Empty string if there are no matches.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.GetAllPaths" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName.html">AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName</a>
			/// </seealso>
			public static String[] GetPaths([NotNull] String bundleName, [NotNull] String assetName) =>
				AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);

			/// <summary>
			///     Returns the bundle name that contains the asset path.
			/// </summary>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>
			///     The name of the bundle this asset belongs to. Empty string if the asset path does not belong to an asset
			///     bundle.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.GetOwningBundleVariant" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImplicitAssetBundleName.html">AssetDatabase.GetImplicitAssetBundleName</a>
			/// </seealso>
			public static String GetOwningBundle([NotNull] Path path) => AssetDatabase.GetImplicitAssetBundleName(path);

			/// <summary>
			///     Returns the bundle variant name that contains the asset path.
			/// </summary>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>
			///     The name of the bundle variant this asset belongs to. Empty string if the asset path does not belong to an
			///     asset bundle.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Bundle.GetOwningBundle" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImplicitAssetBundleVariantName.html">AssetDatabase.GetImplicitAssetBundleVariantName</a>
			/// </seealso>
			public static String GetOwningBundleVariant([NotNull] Path path) => AssetDatabase.GetImplicitAssetBundleVariantName(path);
		}
	}
}
