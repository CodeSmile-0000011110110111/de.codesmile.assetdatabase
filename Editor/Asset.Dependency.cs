﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace CodeSmileEditor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Groups all dependency related functionality.
		/// </summary>
		public static class Dependency
		{
			/// <summary>
			///     Returns the direct dependencies of the asset at the given path.
			/// </summary>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>Paths to dependent assets, or empty array if there are no dependencies.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Dependency.GetAll(CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.Dependency.GetDirect(CodeSmileEditor.Asset.Path[])" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetDependencies.html">AssetDatabase.GetDependencies</a>
			/// </seealso>
			public static String[] GetDirect([NotNull] Path path) => AssetDatabase.GetDependencies(path, false);

			/// <summary>
			///     Returns the direct dependencies of the assets at the given paths.
			/// </summary>
			/// <param name="paths">Paths to asset files.</param>
			/// <returns>Paths to dependent assets, or empty array if there are no dependencies.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Dependency.GetAll(CodeSmileEditor.Asset.Path[])" />
			///     - <see cref="CodeSmileEditor.Asset.Dependency.GetDirect(CodeSmileEditor.Asset.Path)" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetDependencies.html">AssetDatabase.GetDependencies</a>
			/// </seealso>
			public static String[] GetDirect([NotNull] Path[] paths) => AssetDatabase.GetDependencies(Path.ToStrings(paths), false);

			/// <summary>
			///     Returns all (direct and indirect) dependencies of the asset at the given path.
			/// </summary>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>Paths to dependent assets, or empty array if there are no dependencies.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Dependency.GetAll(CodeSmileEditor.Asset.Path[])" />
			///     - <see cref="CodeSmileEditor.Asset.Dependency.GetDirect(CodeSmileEditor.Asset.Path)" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetDependencies.html">AssetDatabase.GetDependencies</a>
			/// </seealso>
			public static String[] GetAll([NotNull] Path path) => AssetDatabase.GetDependencies(path, true);

			/// <summary>
			///     Returns all (direct and indirect) dependencies of the assets at the given paths. Returns paths to dependent assets.
			/// </summary>
			/// <param name="paths">Paths to asset files.</param>
			/// <returns>Paths to dependent assets, or empty array if there are no dependencies.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Dependency.GetAll(CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.Dependency.GetDirect(CodeSmileEditor.Asset.Path[])" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetDependencies.html">AssetDatabase.GetDependencies</a>
			/// </seealso>
			public static String[] GetAll([NotNull] Path[] paths) => AssetDatabase.GetDependencies(Path.ToStrings(paths), true);

			/// <summary>
			///     Registers a custom dependency to be used in conjunction with a custom AssetImporter.
			/// </summary>
			/// <param name="globalDependencyName">A global name for the dependency.</param>
			/// <param name="dependencyHash">
			///     The current hash of the dependency value which, if changed, indicates that the asset has
			///     changed.
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Dependency.Unregister" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.RegisterCustomDependency.html">AssetDatabase.RegisterCustomDependency</a>
			/// </seealso>
			public static void Register([NotNull] String globalDependencyName, Hash128 dependencyHash) =>
				AssetDatabase.RegisterCustomDependency(globalDependencyName, dependencyHash);

			/// <summary>
			///     Unregisters one or more custom dependencies with the given prefix (eg 'StartsWith').
			/// </summary>
			/// <param name="globalDependencyNamePrefix">A 'starts with' filter string of the dependencies to unregister.</param>
			/// <returns>The number of custom dependencies that were removed.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Dependency.Register" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.UnregisterCustomDependencyPrefixFilter.html">AssetDatabase.UnregisterCustomDependencyPrefixFilter</a>
			/// </seealso>
			public static UInt32 Unregister([NotNull] String globalDependencyNamePrefix) =>
				AssetDatabase.UnregisterCustomDependencyPrefixFilter(globalDependencyNamePrefix);

			/// <summary>
			///     Returns the dependency value hash for the asset at path.
			/// </summary>
			/// <remarks>If the hash changed it means the asset contents may have changed.</remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>The dependency hash value for this asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Dependency.Register" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetDependencyHash.html">AssetDatabase.GetAssetDependencyHash</a>
			/// </seealso>
			public static Hash128 GetHash([NotNull] Path path) => AssetDatabase.GetAssetDependencyHash(path);

			/// <summary>
			///     Returns the dependency hash for the asset.
			/// </summary>
			/// <remarks>If the hash changed it means the asset contents may have changed.</remarks>
			/// <param name="guid">GUID of an asset file.</param>
			/// <returns>The dependency hash value for this asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Dependency.Register" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAssetDependencyHash.html">AssetDatabase.GetAssetDependencyHash</a>
			/// </seealso>
			public static Hash128 GetHash(GUID guid) => AssetDatabase.GetAssetDependencyHash(guid);
		}
	}
}
