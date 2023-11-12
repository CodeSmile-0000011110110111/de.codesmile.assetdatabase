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
		///     Finds the assets by the given filter criteria.
		///     Returns an array of string GUIDs for compatibility reasons.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="searchInFolders"></param>
		/// <returns></returns>
		/// <see cref="FindGuids" />
		/// <see cref="FindPaths" />
		[ExcludeFromCodeCoverage]
		public static String[] Find(String filter, String[] searchInFolders = null) => searchInFolders == null
			? AssetDatabase.FindAssets(filter)
			: AssetDatabase.FindAssets(filter, searchInFolders);

		/// <summary>
		///     Finds the assets by the given filter criteria. Returns an array of asset paths.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="searchInFolders"></param>
		/// <returns></returns>
		/// <see cref="Find" />
		/// <see cref="FindGuids" />
		[ExcludeFromCodeCoverage]
		public static Path[] FindPaths(String filter, String[] searchInFolders = null) =>
			Find(filter, searchInFolders).Select(guid => Path.Get(new GUID(guid))).ToArray();

		/// <summary>
		///     Finds the assets by the given filter criteria. Returns an array of GUID instances.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="searchInFolders"></param>
		/// <returns></returns>
		/// <see cref="Find" />
		/// <see cref="FindPaths" />
		[ExcludeFromCodeCoverage]
		public static GUID[] FindGuids(String filter, String[] searchInFolders = null) =>
			Find(filter, searchInFolders).Select(guid => new GUID(guid)).ToArray();
	}
}
