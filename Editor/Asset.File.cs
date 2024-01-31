// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmileEditor
{
	// This file contains asset 'file operations' related method groups, in that order:
	// Create
	// Save
	// Import
	// Load
	// Find
	// Copy
	// Move
	// Rename
	// Open
	// Delete
	// Trash

	public sealed partial class Asset
	{
		/// <summary>
		///     Groups file related operations.
		/// </summary>
		public static class File
		{
			private static List<String> s_PathsNotDeleted = new();

			/// <summary>
			///     The paths that failed to be deleted or trashed. Is an empty list if no failure occured on the
			///     last call to DeleteMany or TrashMany.
			/// </summary>
			/// <returns>The list of paths that could not be deleted, or an empty array.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Delete(IEnumerable{String})" />
			///     - <see cref="CodeSmileEditor.Asset.File.Trash(IEnumerable{String})" />
			/// </seealso>
			[ExcludeFromCodeCoverage] // trivial
			public static IList<String> PathsNotDeleted => s_PathsNotDeleted;

			/// <summary>
			///     Batch multiple asset file operations to improve execution speed.
			/// </summary>
			/// <remarks>
			///     Within the massAssetFileEditAction the AssetDatabase will queue any CodeSmileEditor.Asset.File
			///     operations and runs them afterwards in a single refresh cycle. This can significantly speed up mass
			///     file operations.
			/// </remarks>
			/// <remarks>
			///     The callback Action is safeguarded against exceptions leaving the AssetDatabase in a 'suspended' state.
			///     Also note that Allow.. and DisallowAutoRefresh calls are already implied when using Start/StopAssetEditing.
			///     See the code snippet below for implementation details.
			/// </remarks>
			/// <remarks>
			///     CAUTION:
			///     - Importing an asset and subsequently trying to load the asset within the callback will return null.
			///     - For that reason you cannot create new Asset instances or call methods that create Asset instances, such as Copy,
			///     during batch editing.
			///     - When 'externally' modifying files and importing those, consider the above implication. You need to defer loading
			///     and working with these objects. Calling BatchEditing twice is good practice (first modify & import, then load the
			///     assets).
			/// </remarks>
			/// <param name="massAssetFileEditAction">Write any mass file editing code in this action.</param>
			/// <seealso cref="">
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.StartAssetEditing.html">AssetDatabase.StartAssetEditing</a>
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.StopAssetEditing.html">AssetDatabase.StopAssetEditing</a>
			/// </seealso>
			public static void BatchEditing([NotNull] Action massAssetFileEditAction)
			{
				try
				{
					StartAssetEditing();

					massAssetFileEditAction?.Invoke();
				}
				finally
				{
					StopAssetEditing();
				}
			}

			/// <summary>
			///     Writes the byte array to disk, then imports and loads the asset. Overwrites any existing file.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="contents">The bytes to write.</param>
			/// <param name="path">Path to a file with extension.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.CreateAsNew(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(String,CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Object,CodeSmileEditor.Asset.Path)" />
			/// </seealso>
			public static Object Create([NotNull] Byte[] contents, [NotNull] Path path) => CreateInternal(contents, path);

			/// <summary>
			///     Writes the byte array to disk, then imports and loads the asset. Generates a unique file name
			///     if an asset exists at the path.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="contents">The bytes to write.</param>
			/// <param name="path">Path to a file with extension. Note that the asset's actual file name may differ.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.CreateAsNew(String,CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.CreateAsNew(Object,CodeSmileEditor.Asset.Path)" />
			/// </seealso>
			public static Object CreateAsNew([NotNull] Byte[] contents, [NotNull] Path path) => CreateInternal(contents, path.UniqueFilePath);

			/// <summary>
			///     Writes the string to disk, then imports and loads the asset. Overwrites any existing file.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="contents">The string to write.</param>
			/// <param name="path">Path to a file with extension.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.CreateAsNew(String,CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Object,CodeSmileEditor.Asset.Path)" />
			/// </seealso>
			public static Object Create([NotNull] String contents, [NotNull] Path path) => CreateInternal(contents, path);

			/// <summary>
			///     Writes the string to disk, then imports and loads the asset. Generates a unique file name
			///     if an asset exists at the path.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="contents">The string to write.</param>
			/// <param name="path">Path to a file with extension. Note that the asset's actual file name may differ.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.CreateAsNew(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.CreateAsNew(Object,CodeSmileEditor.Asset.Path)" />
			/// </seealso>
			public static Object CreateAsNew([NotNull] String contents, [NotNull] Path path) => CreateInternal(contents, path.UniqueFilePath);

			/// <summary>
			///     Writes the object to disk. Overwrites any existing file.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="instance">The object to save as an asset file.</param>
			/// <param name="path">Path to a file with extension.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.CreateAsNew(Object,CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(String,CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.CreateOrLoad{T}" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CreateAsset.html">AssetDatabase.CreateAsset</a>
			/// </seealso>
			public static Object Create([NotNull] Object instance, [NotNull] Path path) => CreateInternal(instance, path);

			/// <summary>
			///     Writes the object to disk. Generates a unique file name if an asset exists at the path.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="instance">The object to save as an asset file.</param>
			/// <param name="path">Path to a file with extension. Note that the asset's actual file name may differ.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Object,CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.CreateAsNew(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.CreateAsNew(String,CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.CreateOrLoad{T}" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CreateAsset.html">AssetDatabase.CreateAsset</a>
			/// </seealso>
			public static Object CreateAsNew([NotNull] Object instance, [NotNull] Path path) => CreateInternal(instance, path.UniqueFilePath);

			/// <summary>
			///     Loads or creates an asset at path.
			/// </summary>
			/// <remarks>
			///     Will first attempt to load the asset at path. If this fails, will create an asset from the
			///     object returned by getInstance.
			///     This is an alias for CodeSmileEditor.Asset.LoadOrCreate{T}.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <param name="getInstance">Func that returns a UnityEngine.Object</param>
			/// <typeparam name="T">UnityEngine.Object derived type.</typeparam>
			/// <returns>
			///     The loaded or created object of type T, or null if the object is not of type T.
			///     Note that the asset file gets created in this case.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.LoadOrCreate{T}" />
			/// </seealso>
			public static T CreateOrLoad<T>([NotNull] Path path, [NotNull] Func<T> getInstance) where T : Object =>
				LoadOrCreate(path, getInstance);

			/// <summary>
			///     Saves the object to disk if it is dirty.
			/// </summary>
			/// <remarks>
			///     Depending on how changes were made you may have to use CodeSmileEditor.Asset.File.ForceSave instead.
			/// </remarks>
			/// <param name="asset">The asset to save.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.ForceSave" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SaveAssetIfDirty.html">AssetDatabase.SaveAssetIfDirty</a>
			/// </seealso>
			public static void Save([NotNull] Object asset) => SaveInternal(asset);

			/// <summary>
			///     Forces the object to be saved to disk. Marks the object as dirty and then calls
			///     CodeSmileEditor.Asset.File.Save.
			/// </summary>
			/// <remarks>
			///     This should only be used if the asset doesn't save otherwise, since unnecessarily writing an object
			///     to disk is a slow operation. Example where this may be required would be changes to fields of a
			///     ScriptableObject without using the
			///     <a href="https://docs.unity3d.com/ScriptReference/SerializedObject.html">SerializedObject</a> or
			///     <a href="https://docs.unity3d.com/ScriptReference/SerializedProperty.html">SerializedProperty</a> classes.
			/// </remarks>
			/// <param name="asset">The asset to mark as dirty, then save.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Save" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SaveAssetIfDirty.html">AssetDatabase.SaveAssetIfDirty</a>
			///     - <a href="https://docs.unity3d.com/ScriptReference/EditorUtility.SetDirty.html">EditorUtility.SetDirty</a>
			/// </seealso>
			public static void ForceSave([NotNull] Object asset) => SaveInternal(asset, true);

			/// <summary>
			///     Saves any changes to the asset to disk, by GUID.
			/// </summary>
			/// <param name="guid">The guid of the asset.</param>
			/// <seealso cref="">
			///     - <see cref="File.Save" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SaveAssetIfDirty.html">AssetDatabase.SaveAssetIfDirty</a>
			/// </seealso>
			public static void Save(GUID guid)
			{
				ThrowIf.NotAnAssetGuid(guid);

				AssetDatabase.SaveAssetIfDirty(guid);
			}

			/// <summary>
			///     Imports a file at a given path that was created or modified 'externally'.
			///     Externally refers to any means other than AssetDatabase methods such as System.IO or batch scripts.
			/// </summary>
			/// <remarks>
			///     You may want to use the Create overloads that automatically create the file, then import and load the new file:
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(String,CodeSmileEditor.Asset.Path)" />
			/// </remarks>
			/// <param name="path">Path to an asset.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ImportAssetOptions.html">ImportAssetOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.ImportAndLoad{T}" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(String,CodeSmileEditor.Asset.Path)" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportAsset.html">AssetDatabase.ImportAsset</a>
			/// </seealso>
			public static void Import([NotNull] Path path, ImportAssetOptions options = ImportAssetOptions.Default)
			{
				ThrowIf.ArgumentIsNull(path, nameof(path));
				ThrowIf.DoesNotExistInFileSystem(path);

				AssetDatabase.ImportAsset(path, options);
			}

			/// <summary>
			///     Imports a file at a given path that was created or modified 'externally', then loads and returns
			///     the asset object. See CodeSmileEditor.Asset.File.Import for more info.
			/// </summary>
			/// <remarks>
			///     You may want to use the Create overloads that automatically create the file, then import and load the new file:
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(String,CodeSmileEditor.Asset.Path)" />
			/// </remarks>
			/// <param name="path">Path to an asset.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ImportAssetOptions.html">ImportAssetOptions</a>
			/// </param>
			/// <typeparam name="T">A UnityEngine.Object derived type.</typeparam>
			/// <returns>The asset object, or null if the asset could not be loaded or is not of type T.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.ImportAndLoad{T}" />
			///     - <see cref="CodeSmileEditor.Asset.File.Load{T}(CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(Byte[],CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Create(String,CodeSmileEditor.Asset.Path)" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportAsset.html">AssetDatabase.ImportAsset</a>
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAssetAtPath.html">AssetDatabase.LoadAssetAtPath</a>
			/// </seealso>
			public static T ImportAndLoad<T>([NotNull] Path path,
				ImportAssetOptions options = ImportAssetOptions.Default)
				where T : Object => Load<T>(path);

			/// <summary>
			///     Imports multiple paths that were created or modified 'externally'.
			///     Externally refers to any means other than AssetDatabase methods such as System.IO or batch scripts.
			/// </summary>
			/// <remarks>Internally runs BatchEditing to batch the import operations.</remarks>
			/// <param name="paths">Paths to assets.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ImportAssetOptions.html">ImportAssetOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.BatchEditing" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportAsset.html">AssetDatabase.ImportAsset</a>
			/// </seealso>
			public static void
				Import([NotNull] Path[] paths, ImportAssetOptions options = ImportAssetOptions.Default) =>
				Import(Path.ToStrings(paths), options);

			/// <summary>
			///     Imports multiple paths that were created or modified 'externally'.
			///     Externally refers to any means other than AssetDatabase methods such as System.IO or batch scripts.
			/// </summary>
			/// <remarks>Internally runs BatchEditing to batch the import operations.</remarks>
			/// <param name="paths">Paths to assets.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/ImportAssetOptions.html">ImportAssetOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.BatchEditing" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportAsset.html">AssetDatabase.ImportAsset</a>
			/// </seealso>
			public static void
				Import([NotNull] String[] paths, ImportAssetOptions options = ImportAssetOptions.Default) => BatchEditing(
				() =>
				{
					foreach (var path in paths)
						AssetDatabase.ImportAsset(path, options);
				});

			/// <summary>
			///     Loads an asset at path.
			/// </summary>
			/// <remarks>
			///     - Will import the asset if it is not yet in the AssetDatabase.
			///     - Will only load visible sub-objects. Returns the first object of the type found.
			///     - Use CodeSmileEditor.Asset.SubAsset.LoadVisible if you need a specific sub-asset.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <typeparam name="T">UnityEngine.Object derived type.</typeparam>
			/// <returns>
			///     The loaded asset object, or null if the asset does not exist or does not contain a visible object of type T.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.LoadMain{T}" />
			///     - <see cref="CodeSmileEditor.Asset.File.LoadOrCreate{T}" />
			///     - <see cref="CodeSmileEditor.Asset.SubAsset.LoadVisible" />
			///     - <see cref="CodeSmileEditor.Asset.SubAsset.LoadAll" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAssetAtPath.html">AssetDatabase.LoadAssetAtPath</a>
			/// </seealso>
			public static T Load<T>([NotNull] Path path) where T : Object
			{
				ThrowIf.ArgumentIsNull(path, nameof(path));

				ImportIfNotImported(path);
				return AssetDatabase.LoadAssetAtPath<T>(path);
			}

			/// <summary>
			///     Loads an asset at path or creates the asset if needed.
			/// </summary>
			/// <remarks>
			///     - If the file does not exist, creates the asset using the object returned from **getInstance** parameter.
			///     - If the asset isn't in the database, imports and loads the asset.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <param name="getInstance">Method that returns an object instance. Invoked only if the asset needs to be created.</param>
			/// <typeparam name="T">UnityEngine.Object derived type.</typeparam>
			/// <returns>The loaded or created asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Load{T}(CodeSmileEditor.Asset.Path)" />
			/// </seealso>
			public static T LoadOrCreate<T>([NotNull] Path path, [NotNull] Func<T> getInstance) where T : Object
			{
				if (path.ExistsInFileSystem == false)
					return Create(getInstance.Invoke(), path) as T;

				return ImportAndLoad<T>(path);
			}

			/// <summary>
			///     Loads the main (root) asset at the path.
			/// </summary>
			/// <remarks>
			///     - Will import the asset if it is not yet in the AssetDatabase.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <typeparam name="T">UnityEngine.Object derived type.</typeparam>
			/// <returns>The asset or null if the path does not exist.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Load{T}(CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.LoadOrCreate{T}" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadMainAssetAtPath.html">AssetDatabase.LoadMainAssetAtPath</a>
			/// </seealso>
			public static T LoadMain<T>([NotNull] Path path) where T : Object
			{
				ThrowIf.ArgumentIsNull(path, nameof(path));
				ThrowIf.DoesNotExistInFileSystem(path);

				ImportIfNotImported(path);
				return AssetDatabase.LoadMainAssetAtPath(path) as T;
			}

			/// <summary>
			///     Loads the main (root) asset object for the GUID.
			/// </summary>
			/// <remarks>
			///     - Will import the asset if it is not yet in the AssetDatabase.
			/// </remarks>
			/// <param name="guid">GUID of an asset.</param>
			/// <typeparam name="T">UnityEngine.Object derived type.</typeparam>
			/// <returns>The asset object or null if the guid is not an asset guid.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.LoadMain{T}(CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.LoadOrCreate{T}" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadMainAssetAtPath.html">AssetDatabase.LoadMainAssetAtPath</a>
			/// </seealso>
			public static T LoadMain<T>(GUID guid) where T : Object
			{
				ThrowIf.NotAnAssetGuid(guid);

				var path = Path.Get(guid);
				ImportIfNotImported(path);
				return LoadMain<T>(path);
			}

			/// <summary>
			///     Loads an object and its dependencies asynchronously.
			/// </summary>
			/// <remarks>Available in Unity 2022.2 or newer. In previous versions throws a NotSupportedException.</remarks>
			/// <param name="path">The path to an asset file.</param>
			/// <param name="localFileId">The local file ID of the (sub) asset. I'm sorry but this is what Unity requires.</param>
			/// <returns>
			///     An
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabaseLoadOperation.html">AssetDatabaseLoadOperation</a>
			///     to track progress.
			/// </returns>
			/// <seealso cref="">
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadObjectAsync.html">AssetDatabase.LoadObjectAsync</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static AssetDatabaseLoadOperation LoadAsync([NotNull] Path path, Int64 localFileId)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.LoadObjectAsync(path, localFileId);
#else
				throw new NotSupportedException("AssetDatabase.LoadObjectAsync not available in this editor version");
#endif
			}

			/// <summary>
			///     Finds asset GUIDs by the given filter criteria.
			/// </summary>
			/// <param name="filter">A search filter string.</param>
			/// <param name="searchInFolders">
			///     A list of folders to recursively search for files. Limiting the searched folders speeds
			///     up Find.
			/// </param>
			/// <returns>An array of string GUIDs. Empty array if there were no search results.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.FindGuids" />
			///     - <see cref="CodeSmileEditor.Asset.File.FindPaths" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html">AssetDatabase.FindAssets</a>
			///     -
			///     <a href="https://forum.unity.com/threads/please-document-assetdatabase-findassets-filters.964907/">
			///         Search Filter
			///         String Examples
			///     </a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static String[] Find([NotNull] String filter, String[] searchInFolders = null) => searchInFolders == null
				? AssetDatabase.FindAssets(filter)
				: AssetDatabase.FindAssets(filter, searchInFolders);

			/// <summary>
			///     Finds asset GUIDs by the given filter criteria.
			/// </summary>
			/// <remarks> Casts the result of CodeSmileEditor.Asset.File.Find. </remarks>
			/// <param name="filter">A search filter string.</param>
			/// <param name="searchInFolders">
			///     A list of folders to recursively search for files. Limiting the searched folders speeds
			///     up Find.
			/// </param>
			/// <returns>An array of GUIDs. Empty array if there were no search results.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Find" />
			///     - <see cref="CodeSmileEditor.Asset.File.FindPaths" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html">AssetDatabase.FindAssets</a>
			///     -
			///     <a href="https://forum.unity.com/threads/please-document-assetdatabase-findassets-filters.964907/">
			///         Search Filter
			///         String Examples
			///     </a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static GUID[] FindGuids([NotNull] String filter, String[] searchInFolders = null) =>
				Find(filter, searchInFolders).Select(guid => new GUID(guid)).ToArray();

			/// <summary>
			///     Finds asset paths by the given filter criteria.
			/// </summary>
			/// <remarks>Converts the list of string guids from CodeSmileEditor.Asset.File.Find to actual Paths.</remarks>
			/// <param name="filter">A search filter string.</param>
			/// <param name="searchInFolders">
			///     A list of folders to recursively search for files. Limiting the searched folders speeds up Find.
			/// </param>
			/// <returns>An Path array. Empty array if there were no search results.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Find" />
			///     - <see cref="CodeSmileEditor.Asset.File.FindGuids" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html">AssetDatabase.FindAssets</a>
			///     -
			///     <a href="https://forum.unity.com/threads/please-document-assetdatabase-findassets-filters.964907/">
			///         Search Filter
			///         String Examples
			///     </a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Path[] FindPaths([NotNull] String filter, String[] searchInFolders = null) =>
				Find(filter, searchInFolders).Select(guid => Path.Get(new GUID(guid))).ToArray();

			/// <summary>
			///     Finds asset paths by the given filter criteria.
			/// </summary>
			/// <remarks>Converts the list of string guids from CodeSmileEditor.Asset.File.Find to actual Paths.</remarks>
			/// <param name="filter">A search filter string.</param>
			/// <param name="searchInFolders">
			///     A list of folder paths to recursively search for files. Limiting the searched folders speeds up Find.
			/// </param>
			/// <returns>An Path array. Empty array if there were no search results.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Find" />
			///     - <see cref="CodeSmileEditor.Asset.File.FindGuids" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html">AssetDatabase.FindAssets</a>
			///     -
			///     <a href="https://forum.unity.com/threads/please-document-assetdatabase-findassets-filters.964907/">
			///         Search Filter
			///         String Examples
			///     </a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Path[] FindPaths([NotNull] String filter, Path[] searchInFolders = null) =>
				FindPaths(filter, Path.ToStrings(searchInFolders));

			/// <summary>
			///     Copies an asset from source to destination path. Overwrites any existing assets.
			/// </summary>
			/// <remarks>
			///     Will create any missing destination folders automatically.
			/// </remarks>
			/// <param name="sourcePath">The source asset to copy.</param>
			/// <param name="destinationPath">Path to destination file.</param>
			/// <returns>
			///     True if copying succeeded, false if it failed. Use CodeSmileEditor.Asset.GetLastErrorMessage to get the
			///     failure message.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.CopyAsNew" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CopyAsset.html">AssetDatabase.CopyAsset</a>
			/// </seealso>
			public static Boolean Copy([NotNull] Path sourcePath, [NotNull] Path destinationPath) =>
				CopyInternal(sourcePath, destinationPath, true);

			/// <summary>
			///     Copies an asset from source to destination path. Generates a unique file name if an asset
			///     already exist at destinationPath.
			/// </summary>
			/// <remarks>
			///     Will create any missing destination folders automatically.
			/// </remarks>
			/// <param name="sourcePath">The source asset to copy.</param>
			/// <param name="destinationPath">Path to destination file. Note that the actual file name may differ.</param>
			/// <returns>
			///     True if copying succeeded, false if it failed. Use CodeSmileEditor.Asset.GetLastErrorMessage to get the
			///     failure message.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Copy" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CopyAsset.html">AssetDatabase.CopyAsset</a>
			/// </seealso>
			public static Boolean CopyAsNew([NotNull] Path sourcePath, [NotNull] Path destinationPath) =>
				CopyInternal(sourcePath, destinationPath.UniqueFilePath, false);

			/// <summary>
			///     Tests if an asset can be moved to destination without moving the asset.
			/// </summary>
			/// <remarks>
			///     Note: This returns false if any folders of destinationPath do not exist.
			/// </remarks>
			/// <param name="sourcePath">The path to an asset file.</param>
			/// <param name="destinationPath">The path to move the file to. May have a different extension.</param>
			/// <returns>
			///     True if moving the asset will be successful, false if part of the destinationPath does not exist or other reasons.
			///     Use CodeSmileEditor.Asset.GetLastErrorMessage to get the failure message.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Move" />
			///     - <see cref="CodeSmileEditor.Asset.File.Rename" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ValidateMoveAsset.html">AssetDatabase.ValidateMoveAsset</a>
			/// </seealso>
			public static Boolean CanMove([NotNull] Path sourcePath, [NotNull] Path destinationPath)
			{
				if (sourcePath == null || destinationPath == null)
					return false;

				return Succeeded(AssetDatabase.ValidateMoveAsset(sourcePath, destinationPath));
			}

			/// <summary>
			///     Moves an asset file to destination path.
			/// </summary>
			/// <remarks>
			///     Any missing folders in destinationPath will be created automatically.
			/// </remarks>
			/// <param name="sourcePath">The path to an asset file.</param>
			/// <param name="destinationPath">The path to move the file to. May have a different extension.</param>
			/// <returns>
			///     True if moving the asset will be successful, false if move failed.
			///     Use CodeSmileEditor.Asset.GetLastErrorMessage to get the failure message.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.CanMove" />
			///     - <see cref="CodeSmileEditor.Asset.File.Rename" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MoveAsset.html">AssetDatabase.MoveAsset</a>
			/// </seealso>
			public static Boolean Move([NotNull] Path sourcePath, [NotNull] Path destinationPath)
			{
				if (sourcePath == null || destinationPath == null)
					return false;

				destinationPath.CreateFolders();
				return Succeeded(AssetDatabase.MoveAsset(sourcePath, destinationPath));
			}

			/// <summary>
			///     Renames an asset's file or folder name.
			/// </summary>
			/// <remarks>
			///     Cannot be used to change a file's extension. Use CodeSmileEditor.Asset.File.Move to change the extension.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <param name="newFileName">The new file name without extension.</param>
			/// <returns>
			///     True if the rename succeeded, false otherwise.
			///     On failure use CodeSmileEditor.Asset.GetLastErrorMessage to get the failure reason.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Move" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.RenameAsset.html">AssetDatabase.RenameAsset</a>
			/// </seealso>
			public static Boolean Rename([NotNull] Path path, String newFileName) => String.IsNullOrEmpty(newFileName) == false &&
			                                                                         Succeeded(AssetDatabase.RenameAsset(path, newFileName));

			/// <summary>
			///     Returns true if the given object can be opened (edited) by the Unity editor.
			/// </summary>
			/// <remarks>Returns false if obj is not an asset but an in-memory instance.</remarks>
			/// <param name="instance">The object to test for editability.</param>
			/// <returns>True if Unity can open assets of this type. False if it cannot or if obj is not an asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.OpenExternal" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CanOpenAssetInEditor.html">AssetDatabase.CanOpenAssetInEditor</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Boolean CanOpenInEditor([NotNull] Object instance) => CanOpenInEditor(instance.GetInstanceID());

			/// <summary>
			///     Returns true if the given object can be opened (edited) by the Unity editor.
			/// </summary>
			/// <remarks>Throws an exception if instanceId is not an asset but an in-memory instance.</remarks>
			/// <param name="instanceId">The instance ID of an asset object.</param>
			/// <returns>True if Unity can open assets of this type. False if it cannot or if instanceId is not an asset.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.OpenExternal" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CanOpenAssetInEditor.html">AssetDatabase.CanOpenAssetInEditor</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Boolean CanOpenInEditor(Int32 instanceId) => AssetDatabase.CanOpenAssetInEditor(instanceId);

			/// <summary>
			///     Opens the asset in the application associated with the file's extension.
			/// </summary>
			/// <remarks>
			///     Optional line and column numbers can be specified for text files and applications that support this.
			/// </remarks>
			/// <param name="asset">The asset to open externally.</param>
			/// <param name="lineNumber">Optional line number to highlight. Depends on application support.</param>
			/// <param name="columnNumber">Optional column/character number to highlight. Depends on application support.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.CanOpenInEditor(Object)" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.OpenAsset.html">AssetDatabase.OpenAsset</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // cannot be tested
			public static void OpenExternal([NotNull] Object asset, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
				AssetDatabase.OpenAsset(asset, lineNumber, columnNumber);

			/// <summary>
			///     Opens the asset in the application associated with the file's extension.
			/// </summary>
			/// <remarks>
			///     Optional line and column numbers can be specified for text files and applications that support this.
			/// </remarks>
			/// <param name="instanceId">An instance ID of the asset to open externally.</param>
			/// <param name="lineNumber">Optional line number to highlight. Depends on application support.</param>
			/// <param name="columnNumber">Optional column/character number to highlight. Depends on application support.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.CanOpenInEditor(Int32)" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.OpenAsset.html">AssetDatabase.OpenAsset</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // cannot be tested
			public static void OpenExternal(Int32 instanceId, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
				AssetDatabase.OpenAsset(instanceId, lineNumber, columnNumber);

			/// <summary>
			///     Opens the asset in the application associated with the file's extension.
			/// </summary>
			/// <remarks>
			///     Optional line and column numbers can be specified for text files and applications that support this.
			/// </remarks>
			/// <param name="path">The path to open externally.</param>
			/// <param name="lineNumber">Optional line number to highlight. Depends on application support.</param>
			/// <param name="columnNumber">Optional column/character number to highlight. Depends on application support.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.CanOpenInEditor(Object)" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.OpenAsset.html">AssetDatabase.OpenAsset</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // cannot be tested
			public static void OpenExternal([NotNull] Path path, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
				OpenExternal(Load<Object>(path), lineNumber, columnNumber);

			/// <summary>
			///     Deletes an asset file or folder.
			/// </summary>
			/// <remarks> Does nothing if there is no file or folder at the given path. </remarks>
			/// <param name="path">The path to delete.</param>
			/// <returns>True if the asset was deleted, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Delete(Object)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Trash(CodeSmileEditor.Asset.Path)" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.DeleteAsset.html">AssetDatabase.DeleteAsset</a>
			/// </seealso>
			public static Boolean Delete([NotNull] Path path) =>
				// path.Exists prevents Unity from spitting out an unnecessary warning message
				path != null && path.Exists && AssetDatabase.DeleteAsset(path);

			/// <summary>
			///     Deletes an asset file or folder.
			/// </summary>
			/// <remarks> Does nothing if there is no file or folder at the given path. </remarks>
			/// <param name="asset">The asset to delete.</param>
			/// <returns>True if the asset was deleted, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Delete(CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Trash(Object)" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.DeleteAsset.html">AssetDatabase.DeleteAsset</a>
			/// </seealso>
			public static Boolean Delete([NotNull] Object asset) => Delete(Path.Get(asset));

			/// <summary>
			///     Tries to delete multiple files/folders.
			/// </summary>
			/// <param name="paths">The paths to delete.</param>
			/// <returns>
			///     True if all assets where deleted, false if one or more failed to delete. On failure,
			///     you can access failed paths via CodeSmileEditor.Asset.File.PathsNotDeleted property.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Trash(IEnumerable{String})" />
			///     - <see cref="CodeSmileEditor.Asset.File.PathsNotDeleted" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.DeleteAssets.html">AssetDatabase.DeleteAssets</a>
			/// </seealso>
			public static Boolean Delete([NotNull] IEnumerable<Path> paths) => Delete(Path.ToStrings(paths));

			/// <summary>
			///     Tries to delete multiple files/folders.
			/// </summary>
			/// <param name="paths">The paths to delete.</param>
			/// <returns>
			///     True if all assets where deleted, false if one or more failed to delete. On failure,
			///     you can access failed paths via CodeSmileEditor.Asset.File.PathsNotDeleted property.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Trash(IEnumerable{String})" />
			///     - <see cref="CodeSmileEditor.Asset.File.PathsNotDeleted" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.DeleteAssets.html">AssetDatabase.DeleteAssets</a>
			/// </seealso>
			public static Boolean Delete([NotNull] IEnumerable<String> paths) =>
				AssetDatabase.DeleteAssets(paths.ToArray(), s_PathsNotDeleted = new List<String>());

			/// <summary>
			///     Moves an asset file or folder to the OS trash.
			/// </summary>
			/// <remarks>
			///     Similar to delete, but recoverable by user action.
			///     Does nothing if there is no file or folder at the given path.
			/// </remarks>
			/// <param name="path">The asset path to trash.</param>
			/// <returns>True if the asset was trashed, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Delete(CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Trash(Object)" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MoveAssetToTrash.html">AssetDatabase.MoveAssetToTrash</a>
			/// </seealso>
			public static Boolean Trash([NotNull] Path path) => path != null && AssetDatabase.MoveAssetToTrash(path);

			/// <summary>
			///     Moves an asset file or folder to the OS trash.
			/// </summary>
			/// <remarks>
			///     Similar to delete, but recoverable by user action.
			///     Does nothing if there is no file or folder at the given path.
			/// </remarks>
			/// <param name="asset">The asset to trash.</param>
			/// <returns>True if the asset was trashed, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Delete(CodeSmileEditor.Asset.Path)" />
			///     - <see cref="CodeSmileEditor.Asset.File.Trash(Object)" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MoveAssetToTrash.html">AssetDatabase.MoveAssetToTrash</a>
			/// </seealso>
			public static Boolean Trash([NotNull] Object asset) => Trash(Path.Get(asset));

			/// <summary>
			///     Tries to move multiple files/folders to the OS trash.
			/// </summary>
			/// <param name="paths">The asset paths to trash.</param>
			/// <returns>
			///     True if all assets where trashed, false if one or more failed to trash whose paths
			///     you can access via the CodeSmileEditor.Asset.File.PathsNotDeleted property.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Delete(IEnumerable{String})" />
			///     - <see cref="CodeSmileEditor.Asset.File.PathsNotDeleted" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MoveAssetsToTrash.html">AssetDatabase.MoveAssetsToTrash</a>
			/// </seealso>
			public static Boolean Trash([NotNull] IEnumerable<Path> paths) => Trash(Path.ToStrings(paths));

			/// <summary>
			///     Tries to move multiple files/folders to the OS trash.
			/// </summary>
			/// <param name="paths">The asset paths to trash.</param>
			/// <returns>
			///     True if all assets where trashed, false if one or more failed to trash whose paths
			///     you can access via the CodeSmileEditor.Asset.File.PathsNotDeleted property.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.File.Delete(IEnumerable{String})" />
			///     - <see cref="CodeSmileEditor.Asset.File.PathsNotDeleted" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MoveAssetsToTrash.html">AssetDatabase.MoveAssetsToTrash</a>
			/// </seealso>
			public static Boolean Trash([NotNull] IEnumerable<String> paths) =>
				AssetDatabase.MoveAssetsToTrash(paths.ToArray(), s_PathsNotDeleted = new List<String>());

					/// <summary>
		///     Returns the type of the main asset at the path.
		/// </summary>
		/// <param name="path">Path to an asset.</param>
		/// <returns>Type of the asset. Null if the path does not exist.</returns>
		/// <seealso cref="">
		///     -
		///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetMainAssetTypeAtPath.html">AssetDatabase.GetMainAssetTypeAtPath</a>
		/// </seealso>
		public static Type GetMainType([NotNull] Path path) => AssetDatabase.GetMainAssetTypeAtPath(path);

		/// <summary>
		///     Returns the type of the main asset for the GUID.
		/// </summary>
		/// <remarks>
		///     In Unity 2023.2 it uses AssetDatabase.GetMainAssetTypeFromGUID.
		///     The method exists in 2022.2 but not in the early patch versions 0f1 through 6f1.
		///     In earlier versions the type is obtained from the path's GUID.
		/// </remarks>
		/// <param name="guid">Guid of an asset.</param>
		/// <returns>Type of the asset. Null if the guid is not known or not an asset.</returns>
		/// <seealso cref="">
		///     -
		///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetMainAssetTypeFromGUID.html">AssetDatabase.GetMainAssetTypeFromGUID</a>
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public static Type GetMainType(GUID guid)
		{
#if UNITY_2023_2_OR_NEWER // It's also available in 2022.2 but not in the early patch versions (eg 7f1 onwards)
			return AssetDatabase.GetMainAssetTypeFromGUID(guid);
#else
			return GetMainType(Path.Get(guid));
#endif
		}

		/// <summary>
		///     Gets the type of a sub asset by the main asset's path and the local file ID of the sub-asset.
		/// </summary>
		/// <param name="path">Path to an asset.</param>
		/// <param name="fileId">Local file ID of the sub-asset.</param>
		/// <returns>Type of the SubAsset, or null.</returns>
		/// <seealso cref="">
		///     -
		///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetTypeFromPathAndFileID.html">AssetDatabase.GetTypeFromPathAndFileID</a>
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public static Type GetSubType([NotNull] Path path, Int64 fileId) => AssetDatabase.GetTypeFromPathAndFileID(path, fileId);

		/// <example>
		///     Example usage:
		///     <code>
		/// var (guid, fileId) = Asset.GetGuidAndFileId(obj);
		/// </code>
		/// </example>
		/// <param name="asset">Object from which GUID and FileId should be obtained.</param>
		/// <returns>The GUID and local File ID of the object. Returns an empty GUID and 0 if obj is null or not an asset. </returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.GetGuid" />
		///     - <see cref="CodeSmileEditor.Asset.GetFileId" />
		///     -
		///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.TryGetGUIDAndLocalFileIdentifier.html">AssetDatabase.TryGetGUIDAndLocalFileIdentifier</a>
		/// </seealso>

		// ValueTuple makes doxygen accept it as documented, see: https://github.com/doxygen/doxygen/issues/9618
		public static ValueTuple<GUID, Int64> GetGuidAndFileId([NotNull] Object asset)
		{
			if (asset == null)
				return (new GUID(), 0L);

			// explicit variable + assign because Unity 2021 has both long and int variants of the TryGetGUID* method
			var localId = Int64.MaxValue;
			return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out localId)
				? (new GUID(guid), localId)
				: (new GUID(), 0L);
		}

		/// <summary>
		///     Returns the GUID of an object. Returns an empty GUID if the object is null or not an asset.
		/// </summary>
		/// <param name="asset">An asset instance.</param>
		/// <returns>The GUID of the asset. Returns empty GUID if the asset is null or not an asset.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.GetFileId" />
		///     - <see cref="CodeSmileEditor.Asset.GetGuidAndFileId" />
		///     -
		///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.TryGetGUIDAndLocalFileIdentifier.html">AssetDatabase.TryGetGUIDAndLocalFileIdentifier</a>
		/// </seealso>
		public static GUID GetGuid([NotNull] Object asset)
		{
			if (asset == null)
				return new GUID();

			// explicit variable + assign because Unity 2021 has both long and int variants of the TryGetGUID* method
			var localId = Int64.MaxValue;
			return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out localId)
				? new GUID(guid)
				: new GUID();
		}

		/// <summary>
		///     Returns the local FileID of the object.
		/// </summary>
		/// <param name="asset"></param>
		/// <returns>The local fileID or 0 if obj is null or not an asset.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.GetGuid" />
		///     - <see cref="CodeSmileEditor.Asset.GetGuidAndFileId" />
		///     -
		///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.TryGetGUIDAndLocalFileIdentifier.html">AssetDatabase.TryGetGUIDAndLocalFileIdentifier</a>
		/// </seealso>
		public static Int64 GetFileId([NotNull] Object asset)
		{
			if (asset == null)
				return 0L;

			// explicit variable + assign because Unity 2021 has both long and int variants of the TryGetGUID* method
			var localId = Int64.MaxValue;
			return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var _, out localId) ? localId : 0L;
		}

			// Internal on purpose: use Asset.File.BatchEditing(Action) instead
			[ExcludeFromCodeCoverage] // untestable
			internal static void StartAssetEditing() => AssetDatabase.StartAssetEditing();

			// Internal on purpose: use Asset.File.BatchEditing(Action) instead
			[ExcludeFromCodeCoverage] // untestable
			internal static void StopAssetEditing() => AssetDatabase.StopAssetEditing();

			internal static Object CreateInternal([NotNull] Byte[] bytes, [NotNull] Path path)
			{
				ThrowIf.ArgumentIsNull(bytes, nameof(bytes));
				ThrowIf.ArgumentIsNull(path, nameof(path));

				path.CreateFolders();
				System.IO.File.WriteAllBytes(path, bytes);
				return ImportAndLoad<Object>(path);
			}

			internal static Object CreateInternal([NotNull] String contents, [NotNull] Path path)
			{
				ThrowIf.ArgumentIsNull(contents, nameof(contents));
				ThrowIf.ArgumentIsNull(path, nameof(path));

				path.CreateFolders();
				System.IO.File.WriteAllText(path, contents, Encoding.UTF8); // string assets ought to be UTF8
				return ImportAndLoad<Object>(path);
			}

			internal static Object CreateInternal([NotNull] Object instance, [NotNull] Path path)
			{
				ThrowIf.ArgumentIsNull(instance, nameof(instance));
				ThrowIf.ArgumentIsNull(path, nameof(path));

				path.CreateFolders();
				AssetDatabase.CreateAsset(instance, path);
				return instance;
			}

			internal static Boolean CopyInternal([NotNull] Path sourcePath, [NotNull] Path destinationPath,
				Boolean overwriteExisting)
			{
				ThrowIf.ArgumentIsNull(sourcePath, nameof(sourcePath));
				ThrowIf.ArgumentIsNull(destinationPath, nameof(destinationPath));
				ThrowIf.AssetPathNotInDatabase(sourcePath);
				ThrowIf.SourceAndDestPathAreEqual(sourcePath, destinationPath);

				destinationPath.CreateFolders();

				var success = AssetDatabase.CopyAsset(sourcePath, destinationPath);
				SetLastErrorMessage(success ? String.Empty : $"failed to copy {sourcePath} to {destinationPath}");
				return success;
			}

			private static void SaveInternal([NotNull] Object asset, Boolean forceSave = false)
			{
				ThrowIf.ArgumentIsNull(asset, nameof(asset));
				ThrowIf.NotInDatabase(asset);

				if (forceSave)
					EditorUtility.SetDirty(asset);

				AssetDatabase.SaveAssetIfDirty(asset);
			}

			private static void ImportIfNotImported([NotNull] Path path,
				ImportAssetOptions options = ImportAssetOptions.Default)
			{
				// Not in database but on disk? => Import path
				// Cannot determine if existing file has been updated though.
				if (path.Exists == false && path.ExistsInFileSystem)
					Import(path, options);
			}

#if !UNITY_2022_2_OR_NEWER // dummy for LoadAsync in earlier versions
			public class AssetDatabaseLoadOperation {}
#endif
		}
	}
}
