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
		///     Returns the default AssetImporter type for this asset.
		/// </summary>
		[ExcludeFromCodeCoverage]
		public Type DefaultImporter => Importer.GetDefault(m_AssetPath);

		/// <summary>
		///     Returns the active AssetImporter type for this asset. Will be the DefaultImporter type unless
		///     the importer was overridden.
		/// </summary>
		/// <see cref="SetActiveImporter{T}" />
		/// <see cref="SetActiveImporterToDefault" />
		/// <see cref="DefaultImporter" />
		[ExcludeFromCodeCoverage]
		public Type ActiveImporter
		{
			get
			{
				var overridden = Importer.GetOverride(m_AssetPath);
				return overridden != null ? overridden : Importer.GetDefault(m_AssetPath);
			}
		}

		/// <summary>
		///     Returns true if the asset's default AssetImporter type has been overridden.
		/// </summary>
		/// <see cref="ActiveImporter" />
		/// <see cref="DefaultImporter" />
		/// <see cref="SetActiveImporter{T}" />
		/// <see cref="SetActiveImporterToDefault" />
		[ExcludeFromCodeCoverage]
		public Boolean IsImporterOverridden => ActiveImporter != DefaultImporter;

		/// <summary>
		///     Sets the active AssetImporter type for this asset.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <see cref="SetActiveImporterToDefault" />
		/// <see cref="ActiveImporter" />
		[ExcludeFromCodeCoverage]
		public void SetActiveImporter<T>() where T : AssetImporter => Importer.SetOverride<T>(m_AssetPath);

		/// <summary>
		///     Resets the active AssetImporter type to the default type, if necessary.
		/// </summary>
		[ExcludeFromCodeCoverage]
		public void SetActiveImporterToDefault()
		{
			if (Importer.IsOverridden(m_AssetPath))
				Importer.ClearOverride(m_AssetPath);
		}

		/// <summary>
		///     Groups all AssetImporter related functionality.
		/// </summary>
		public static class Importer
		{
			/// <summary>
			///     Returns an asset's default importer type.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type GetDefault(Path path) => AssetDatabase.GetDefaultImporter(path);

			/// <summary>
			///     Returns an asset's overridden importer type.
			/// </summary>
			/// <param name="path"></param>
			/// <returns>the AssetImporter type or null if there is no overridden importer</returns>
			[ExcludeFromCodeCoverage]
			public static Type GetOverride(Path path) => AssetDatabase.GetImporterOverride(path);

			/// <summary>
			///     Sets an asset's importer override to the specified AssetImporter type.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static void SetOverride<T>(Path path) where T : AssetImporter =>
				AssetDatabase.SetImporterOverride<T>(path);

			/// <summary>
			///     Clears an asset's importer override for the specified asset.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static void ClearOverride(Path path) => AssetDatabase.ClearImporterOverride(path);

			/// <summary>
			///     Returns true if the AssetImporter type for this asset has been overridden.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Boolean IsOverridden(Path path) => GetDefault(path) != GetOverride(path);

			/// <summary>
			///     Gets the AssetImporter type used for the given asset.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type GetType(Path path) => AssetDatabase.GetImporterType(path);

			/// <summary>
			///     Gets the AssetImporter type used for the given asset.
			/// </summary>
			/// <param name="guid"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type GetType(GUID guid) => AssetDatabase.GetImporterType(guid);

			/// <summary>
			///     Gets the AssetImporter types used for the given assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type[] GetTypes(Path[] paths) => GetTypes(paths.Cast<String>().ToArray());

			/// <summary>
			///     Gets the AssetImporter types used for the given assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type[] GetTypes(String[] paths) => AssetDatabase.GetImporterTypes(paths);

			/// <summary>
			///     Gets the AssetImporter types used for the given assets.
			/// </summary>
			/// <param name="guids"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type[] GetTypes(ReadOnlySpan<GUID> guids) => AssetDatabase.GetImporterTypes(guids);

			/// <summary>
			///     Gets the AssetImporter types available for a given asset, eg those that handle assets of the given type.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type[] GetAvailable(Path path) => AssetDatabase.GetAvailableImporters(path);
		}
	}
}
