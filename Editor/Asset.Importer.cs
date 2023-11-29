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
			///     Gets the active AssetImporter type used for the given asset.
			/// </summary>
			/// <remarks>Available in Unity 2022.2 or newer. In previous versions throws a NotSupportedException.</remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>The AssetImporter type in use for the specified asset.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImporterType.html">AssetDatabase.GetImporterType</a>
			/// </seealso>
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
			/// <remarks>Available in Unity 2022.2 or newer. In previous versions throws a NotSupportedException.</remarks>
			/// <param name="guid">GUID of an asset file.</param>
			/// <returns>The AssetImporter type in use for the specified asset.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImporterType.html">AssetDatabase.GetImporterType</a>
			/// </seealso>
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
			/// <remarks>Available in Unity 2022.2 or newer. In previous versions throws a NotSupportedException.</remarks>
			/// <param name="asset">Instance of an asset file.</param>
			/// <returns>The AssetImporter type in use for the specified asset.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImporterType.html">AssetDatabase.GetImporterType</a>
			/// </seealso>
			public static Type GetActive(Object asset) => GetActive(GetGuid(asset));

			/// <summary>
			///     Gets the active AssetImporter types used for the given assets.
			/// </summary>
			/// <remarks>Available in Unity 2022.2 or newer. Throws NotSupportedException in earlier versions.</remarks>
			/// <param name="paths">Paths to asset files.</param>
			/// <returns>The AssetImporter types in use for the specified assets.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImporterTypes.html">AssetDatabase.GetImporterTypes</a>
			/// </seealso>
			public static Type[] GetActive(Path[] paths) => GetActive(Path.ToStrings(paths));

			/// <summary>
			///     Gets the active AssetImporter types used for the given assets.
			/// </summary>
			/// <remarks>Available in Unity 2022.2 or newer. Throws NotSupportedException in earlier versions.</remarks>
			/// <param name="paths">Paths to asset files.</param>
			/// <returns>The AssetImporter types in use for the specified assets.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImporterTypes.html">AssetDatabase.GetImporterTypes</a>
			/// </seealso>
			public static Type[] GetActive(String[] paths)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.GetImporterTypes(paths);
#else
				throw new NotSupportedException("GetImporterTypes not available in this Unity version");
#endif
			}

			/// <summary>
			///     Gets the active AssetImporter types used for the given assets.
			/// </summary>
			/// <remarks>Available in Unity 2022.2 or newer. Throws NotSupportedException in earlier versions.</remarks>
			/// <param name="guids">GUIDs to asset files.</param>
			/// <returns>The AssetImporter types in use for the specified assets.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImporterTypes.html">AssetDatabase.GetImporterTypes</a>
			/// </seealso>
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
			///     Gets the available AssetImporter types for assets of this kind.
			/// </summary>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>AssetImporter types that handle importing assets of the same kind as the given asset file.</returns>
			/// <seealso cref="">
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAvailableImporters.html">AssetDatabase.GetAvailableImporters</a>
			/// </seealso>
			public static Type[] GetAvailable(Path path)
			{
#if UNITY_2022_1_OR_NEWER
				return AssetDatabase.GetAvailableImporters(path);
#else
				return AssetDatabase.GetAvailableImporterTypes(path);
#endif
			}

			/// <summary>
			///     Gets the available AssetImporter types for assets of this kind.
			/// </summary>
			/// <param name="asset">Instance of an asset file.</param>
			/// <returns>AssetImporter types that handle importing assets of the same kind as the given asset file.</returns>
			/// <seealso cref="">
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetAvailableImporters.html">AssetDatabase.GetAvailableImporters</a>
			/// </seealso>
			public static Type[] GetAvailable(Object asset) => GetAvailable(Path.Get(asset));

			/// <summary>
			///     Returns an asset's default importer type.
			/// </summary>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>The type of the default importer for assets of this kind.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetDefaultImporter.html">AssetDatabase.GetDefaultImporter</a>
			/// </seealso>
			public static Type GetDefault(Path path)
			{
#if UNITY_2022_1_OR_NEWER
				return AssetDatabase.GetDefaultImporter(path);
#else
				throw new NotSupportedException("GetDefaultImporter is not available in this Unity version");
#endif
			}

			/// <summary>
			///     Returns an asset's default importer type.
			/// </summary>
			/// <param name="asset">An asset instance.</param>
			/// <returns>The type of the default importer for assets of this kind.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetDefaultImporter.html">AssetDatabase.GetDefaultImporter</a>
			/// </seealso>
			public static Type GetDefault(Object asset) => GetDefault(Path.Get(asset));

			/// <summary>
			///     Returns an asset's overridden importer type.
			/// </summary>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>The overridden AssetImporter type or null if there is no overridden importer.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.SetOverride{T}" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImporterOverride.html">AssetDatabase.GetImporterOverride</a>
			/// </seealso>
			public static Type GetOverride(Path path) => AssetDatabase.GetImporterOverride(path);

			/// <summary>
			///     Returns an asset's overridden importer type.
			/// </summary>
			/// <param name="asset">An asset instance.</param>
			/// <returns>The overridden AssetImporter type or null if there is no overridden importer.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.SetOverride{T}" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImporterOverride.html">AssetDatabase.GetImporterOverride</a>
			/// </seealso>
			public static Type GetOverride(Object asset) => GetOverride(Path.Get(asset));

			/// <summary>
			///     Sets the custom AssetImporter to use for the specified asset.
			/// </summary>
			/// <param name="path">Path to an asset file.</param>
			/// <typeparam name="T">Type derived from AssetImporter. Note: in Unity 2021.3 T is ScriptedImporter.</typeparam>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.ClearOverride" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SetImporterOverride.html">AssetDatabase.SetImporterOverride</a>
			/// </seealso>
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
			///     Clears an AssetImporter override for the specified asset.
			/// </summary>
			/// <remarks>The asset will then use the default importer.</remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.SetOverride{T}" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ClearImporterOverride.html">AssetDatabase.ClearImporterOverride</a>
			/// </seealso>
			public static void ClearOverride(Path path) => AssetDatabase.ClearImporterOverride(path);

			/// <summary>
			///     Returns true if the AssetImporter type for this asset has been overridden.
			/// </summary>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>True if the importer for the asset is overriden, false if it uses the default importer.</returns>
			/// <seealso cref="">
			/// - <see cref="CodeSmile.Editor.Asset.Importer.SetOverride{T}" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetOverride" />
			/// - <see cref="CodeSmile.Editor.Asset.Importer.GetDefault" />
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetDefaultImporter.html">AssetDatabase.GetDefaultImporter</a>
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ClearImporterOverride.html">AssetDatabase.ClearImporterOverride</a>
			/// </seealso>
			public static Boolean IsOverridden(Path path) => GetDefault(path) != GetOverride(path);

			/// <summary>
			///     Writes any unsaved changes of the given asset's importer to disk.
			/// </summary>
			/// <remarks>
			///     This is mainly used to force the CacheServer/Accelerator to import the asset. It does so without
			///     user interaction.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <seealso cref="">
			/// - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.WriteImportSettingsIfDirty.html">AssetDatabase.WriteImportSettingsIfDirty</a>
			/// </seealso>
			public static void ApplySettings(Path path) => AssetDatabase.WriteImportSettingsIfDirty(path);
		}
	}
}
