// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
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
			public static Type GetDefault(Path path)
			{
#if UNITY_2022_1_OR_NEWER
				return AssetDatabase.GetDefaultImporter(path);
#else
				UnityEngine.Debug.LogWarning("GetDefaultImporter is not available in this Unity version - returning null");
				return null;
#endif
			}

			/// <summary>
			///     Returns an asset's default importer type.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Type GetDefault(Object obj) => GetDefault(Path.Get(obj));

			/// <summary>
			///     Returns an asset's overridden importer type.
			/// </summary>
			/// <param name="path"></param>
			/// <returns>the AssetImporter type or null if there is no overridden importer</returns>
			public static Type GetOverride(Path path) => AssetDatabase.GetImporterOverride(path);

			/// <summary>
			///     Returns an asset's overridden importer type.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Type GetOverride(Object obj) => GetOverride(Path.Get(obj));

			/// <summary>
			///     Sets an asset's importer override to the specified AssetImporter type.
			/// </summary>
			/// <param name="path"></param>
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
			public static void ClearOverride(Path path) => AssetDatabase.ClearImporterOverride(path);

			/// <summary>
			///     Returns true if the AssetImporter type for this asset has been overridden.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean IsOverridden(Path path) => GetDefault(path) != GetOverride(path);

			/// <summary>
			///     Gets the active AssetImporter type used for the given asset.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Type GetActive(Path path)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.GetImporterType(path);
#else
				UnityEngine.Debug.LogWarning("GetImporterType not available in this Unity version - returning null");
				return null;
#endif
			}

			/// <summary>
			///     Gets the active AssetImporter type used for the given asset.
			/// </summary>
			/// <param name="guid"></param>
			/// <returns></returns>
			public static Type GetActive(GUID guid)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.GetImporterType(guid);
#else
				UnityEngine.Debug.LogWarning("GetImporterType not available in this Unity version - returning null");
				return null;
#endif
			}

			/// <summary>
			///     Gets the active AssetImporter type used for the given asset.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Type GetActive(Object obj) => GetActive(GetGuid(obj));

			/// <summary>
			///     Gets the active AssetImporter types used for the given assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns></returns>
			public static Type[] GetActive(Path[] paths) => GetActive(Path.ToStrings(paths));

			/// <summary>
			///     Gets the AssetImporter types used for the given assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns></returns>
			public static Type[] GetActive(String[] paths)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.GetImporterTypes(paths);
#else
				UnityEngine.Debug.LogWarning("GetImporterTypes not available in this Unity version - returning empty array");
				return new Type[0];
#endif
			}

			/// <summary>
			///     Gets the active AssetImporter types used for the given assets.
			/// </summary>
			/// <param name="guids"></param>
			/// <returns></returns>
			public static Type[] GetActive(ReadOnlySpan<GUID> guids)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.GetImporterTypes(guids);
#else
				UnityEngine.Debug.LogWarning("GetImporterTypes not available in this Unity version - returning empty array");
				return new Type[0];
#endif
			}

			/// <summary>
			///     Gets the available AssetImporter types for a given asset, eg those that handle assets of the given type.
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Type[] GetAvailable(Path path)
			{
#if UNITY_2022_1_OR_NEWER
				return AssetDatabase.GetAvailableImporters(path);
#else
				return AssetDatabase.GetAvailableImporterTypes(path);
#endif
			}

			/// <summary>
			///     Gets the available AssetImporter types for a given asset, eg those that handle assets of the given type.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static Type[] GetAvailable(Object obj) => GetAvailable(Path.Get(obj));

			/// <summary>
			///     Writes any unsaved changes of the importer for the asset at the given path to disk.
			///     Note: this is mainly used to force the CacheServer/Accelerator to import the asset, and to do so
			///     without user interaction.
			/// </summary>
			/// <param name="path"></param>
			public static void ApplySettings(Path path) => AssetDatabase.WriteImportSettingsIfDirty(path);

			/// <summary>
			///     Writes any unsaved changes of the importer to disk.
			///     Note: this is mainly used to force the CacheServer/Accelerator to import the asset, and to do so
			///     without user interaction.
			/// </summary>
			/// <param name="importer"></param>
			public static void ApplySettings(AssetImporter importer) => ApplySettings(importer.assetPath);
		}
	}
}
