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
		public void SetActiveImporter<T>()
#if UNITY_2022_1_OR_NEWER
			where T : AssetImporter
#else
			where T : UnityEditor.AssetImporters.ScriptedImporter
#endif
		{
			Importer.SetOverride<T>(m_AssetPath);
		}

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
			public static Type GetDefault(Path path)
			{
#if UNITY_2022_1_OR_NEWER
				return AssetDatabase.GetDefaultImporter(path);
#else
				UnityEngine.Debug.LogWarning("GetDefaultImporter is not available in Unity 2021.3 - returning null");
				return null;
#endif
			}

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
			public static void SetOverride<T>(Path path)
#if UNITY_2022_1_OR_NEWER
				where T : AssetImporter
#else
				where T : UnityEditor.AssetImporters.ScriptedImporter
#endif
			{
				AssetDatabase.SetImporterOverride<T>(path);
			}

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
			public static Type Get(Path path)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.GetImporterType(path);
#else
				UnityEngine.Debug.LogWarning("GetImporterType not available in this Unity version - returning null");
				return null;
#endif
			}

			/// <summary>
			///     Gets the AssetImporter type used for the given asset.
			/// </summary>
			/// <param name="guid"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type Get(GUID guid)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.GetImporterType(guid);
#else
				UnityEngine.Debug.LogWarning("GetImporterType not available in this Unity version - returning null");
				return null;
#endif
			}

			/// <summary>
			///     Gets the AssetImporter types used for the given assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type[] Get(Path[] paths) => Get(Path.ToStrings(paths));

			/// <summary>
			///     Gets the AssetImporter types used for the given assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type[] Get(String[] paths)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.GetImporterTypes(paths);
#else
				UnityEngine.Debug.LogWarning("GetImporterTypes not available in this Unity version - returning empty array");
				return new Type[0];
#endif
			}

			/// <summary>
			///     Gets the AssetImporter types used for the given assets.
			/// </summary>
			/// <param name="guids"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type[] Get(ReadOnlySpan<GUID> guids)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.GetImporterTypes(guids);
#else
				UnityEngine.Debug.LogWarning("GetImporterTypes not available in this Unity version - returning empty array");
				return new Type[0];
#endif
			}

			/// <summary>
			///     Gets the AssetImporter types available for a given asset, eg those that handle assets of the given type.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			[ExcludeFromCodeCoverage]
			public static Type[] GetAvailable(Path path)
			{
#if UNITY_2022_1_OR_NEWER
				return AssetDatabase.GetAvailableImporters(path);
#else
				return AssetDatabase.GetAvailableImporterTypes(path);
#endif
			}

			/// <summary>
			/// Writes any unsaved changes of the importer for the asset at the given path to disk.
			/// Note: this is mainly used to force the CacheServer/Accelerator to import the asset, and to do so
			/// without user interaction.
			/// </summary>
			/// <param name="path"></param>
			[ExcludeFromCodeCoverage]
			public static void SaveSettings(Path path)
			{
				AssetDatabase.WriteImportSettingsIfDirty(path);
			}

			/// <summary>
			/// Writes any unsaved changes of the importer to disk.
			/// Note: this is mainly used to force the CacheServer/Accelerator to import the asset, and to do so
			/// without user interaction.
			/// </summary>
			/// <param name="importer"></param>
			[ExcludeFromCodeCoverage]
			public static void SaveSettings(AssetImporter importer)
			{
				SaveSettings(importer.assetPath);
			}
		}
	}
}
