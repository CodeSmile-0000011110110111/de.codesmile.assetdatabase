// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using UnityEngine;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Groups all dependency related functionality.
		/// </summary>
		public static class Dependency
		{
			/// <summary>
			///     Returns the direct dependencies of the asset at the given path. Returns paths to dependent assets.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static String[] GetDirect(Path path) => AssetDatabase.GetDependencies(path, false);

			/// <summary>
			///     Returns the direct dependencies of the assets at the given paths. Returns paths to dependent assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns></returns>
			public static String[] GetDirect(Path[] paths) =>
				AssetDatabase.GetDependencies(Path.ToStrings(paths), false);

			/// <summary>
			///     Returns all (direct and indirect) dependencies of the asset at the given path. Returns paths to dependent assets.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static String[] GetAll(Path path) => AssetDatabase.GetDependencies(path, true);

			/// <summary>
			///     Returns all (direct and indirect) dependencies of the assets at the given paths. Returns paths to dependent assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns></returns>
			public static String[] GetAll(Path[] paths) => AssetDatabase.GetDependencies(Path.ToStrings(paths), true);

			/// <summary>
			///     Sets (registers) a custom dependency to be used in conjunction with a custom AssetImporter.
			///     Call this method again to update existing dependencies.
			///     For details
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.RegisterCustomDependency.html">see the manual</a>.
			/// </summary>
			/// <param name="globalDependencyName">A global name for the dependency.</param>
			/// <param name="dependencyHash">The current hash of the dependency value.</param>
			/// <see cref="Unregister" />
			public static void Register(String globalDependencyName, Hash128 dependencyHash) =>
				AssetDatabase.RegisterCustomDependency(globalDependencyName, dependencyHash);

			/// <summary>
			///     Removes one or more custom dependencies by using a global dependency name's prefix
			///     (eg 'StartsWith') or full name.
			/// </summary>
			/// <param name="globalDependencyNamePrefix"></param>
			/// <returns>The number of custom dependencies that were removed.</returns>
			/// <see cref="Register" />
			public static UInt32 Unregister(String globalDependencyNamePrefix) =>
				AssetDatabase.UnregisterCustomDependencyPrefixFilter(globalDependencyNamePrefix);

			/// <summary>
			///     Returns the dependency hash for the asset at path.
			///     If the hash changes it means the asset contents may have changed.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Hash128 GetHash(Path path) => AssetDatabase.GetAssetDependencyHash(path);

			/// <summary>
			///     Returns the dependency hash for the asset.
			///     If the hash changes it means the asset contents may have changed.
			/// </summary>
			/// <param name="guid"></param>
			/// <returns></returns>
			public static Hash128 GetHash(GUID guid) => AssetDatabase.GetAssetDependencyHash(guid);
		}
	}
}
