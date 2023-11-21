// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Returns the assets direct dependencies. Returns paths to the dependent assets.
		/// </summary>
		public String[] DirectDependencies => GetDirectDependencies(m_AssetPath);

		/// <summary>
		///     Returns the assets direct and indirect dependencies. Returns paths to the dependent assets.
		/// </summary>
		public String[] Dependencies => GetAllDependencies(m_AssetPath);

		/// <summary>
		///     Returns the direct dependencies of the asset at the given path. Returns paths to dependent assets.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public static String[] GetDirectDependencies(Path path) => AssetDatabase.GetDependencies(path, false);

		/// <summary>
		///     Returns the direct dependencies of the assets at the given paths. Returns paths to dependent assets.
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public static String[] GetDirectDependencies(Path[] paths) =>
			AssetDatabase.GetDependencies(paths.Cast<String>().ToArray(), false);

		/// <summary>
		///     Returns all (direct and indirect) dependencies of the asset at the given path. Returns paths to dependent assets.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public static String[] GetAllDependencies(Path path) => AssetDatabase.GetDependencies(path, true);

		/// <summary>
		///     Returns all (direct and indirect) dependencies of the assets at the given paths. Returns paths to dependent assets.
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public static String[] GetAllDependencies(Path[] paths) =>
			AssetDatabase.GetDependencies(paths.Cast<String>().ToArray(), true);
	}
}
