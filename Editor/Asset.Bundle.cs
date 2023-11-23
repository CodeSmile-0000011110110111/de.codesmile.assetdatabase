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
		///     Returns the bundle name the asset belongs to.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>The bundle name or empty string.</returns>
		[ExcludeFromCodeCoverage]
		public String OwningBundle => Bundle.GetOwningBundle(m_AssetPath);

		/// <summary>
		///     Returns the bundle variant name the asset belongs to.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>The bundle variant name or empty string.</returns>
		[ExcludeFromCodeCoverage]
		public String OwningBundleVariant => Bundle.GetOwningBundleVariant(m_AssetPath);

		/// <summary>
		///     Groups all asset bundle related functionality.
		///     Note: Unity recommends Addressables over Bundles!
		/// </summary>
		public static class Bundle
		{
			/// <summary>
			///     Returns all asset bundle names.
			/// </summary>
			[ExcludeFromCodeCoverage]
			public static String[] All => AssetDatabase.GetAllAssetBundleNames();

			/// <summary>
			///     Returns all unused asset bundle names.
			/// </summary>
			[ExcludeFromCodeCoverage]
			public static String[] Unused => AssetDatabase.GetUnusedAssetBundleNames();

			/// <summary>
			///     Removes all unused asset bundles.
			/// </summary>
			[ExcludeFromCodeCoverage]
			public static void RemoveUnused() => AssetDatabase.RemoveUnusedAssetBundleNames();

			/// <summary>
			///     Removes a specific asset bundle by name. If the bundle is currently in use, it will NOT be removed.
			/// </summary>
			/// <see cref="ForceRemove" />
			/// <param name="bundleName"></param>
			[ExcludeFromCodeCoverage]
			public static void Remove(String bundleName) => AssetDatabase.RemoveAssetBundleName(bundleName, false);

			/// <summary>
			///     Removes a specific asset bundle by name. CAUTION: The bundle is removed even if it is currently in use.
			/// </summary>
			/// <see cref="Remove" />
			/// <param name="bundleName"></param>
			[ExcludeFromCodeCoverage]
			public static void ForceRemove(String bundleName) => AssetDatabase.RemoveAssetBundleName(bundleName, true);

			/// <summary>
			///     Returns the bundle names that the given bundle directly depends on.
			/// </summary>
			/// <see cref="GetAllDependencies" />
			/// <param name="bundleName"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static String[] GetDirectDependencies(String bundleName) =>
				AssetDatabase.GetAssetBundleDependencies(bundleName, false);

			/// <summary>
			///     Returns all bundle names that the given bundle directly or indirectly depends on.
			/// </summary>
			/// <see cref="GetDirectDependencies" />
			/// <param name="bundleName"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static String[] GetAllDependencies(String bundleName) =>
				AssetDatabase.GetAssetBundleDependencies(bundleName, true);

			/// <summary>
			///     Returns all asset paths that are marked to be part of a given bundle.
			/// </summary>
			/// <param name="bundleName"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static String[] GetAllPaths(String bundleName) =>
				AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

			/// <summary>
			///     Returns the asset paths in the given bundle whose name (without extension or path) matches
			///     the given asset name.
			/// </summary>
			/// <param name="bundleName"></param>
			/// <param name="assetName"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static String[] GetPaths(String bundleName, String assetName) =>
				AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);

			/// <summary>
			///     Returns the bundle name that contains the asset at the given path.
			/// </summary>
			/// <param name="path"></param>
			/// <returns>The bundle name or empty string.</returns>
			[ExcludeFromCodeCoverage]
			public static String GetOwningBundle(Path path) => AssetDatabase.GetImplicitAssetBundleName(path);

			/// <summary>
			///     Returns the bundle variant name that contains the asset at the given path.
			/// </summary>
			/// <param name="path"></param>
			/// <returns>The bundle name or empty string.</returns>
			[ExcludeFromCodeCoverage]
			public static String GetOwningBundleVariant(Path path) =>
				AssetDatabase.GetImplicitAssetBundleVariantName(path);
		}
	}
}
