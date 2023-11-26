// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
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
			private static List<String> s_FailedToDeletePaths = new();

			/// <summary>
			///     The paths that failed to be deleted or trashed. Is an empty list if no failure occured on the
			///     last call to DeleteMany or TrashMany.
			/// </summary>
			/// <returns>The list of paths that could not be deleted, or an empty array.</returns>
			/// <see cref="CodeSmile.Editor.Asset.File.Trash(System.Collections.Generic.IEnumerable{CodeSmile.Editor.Asset.Path})" />
			/// <see cref="CodeSmile.Editor.Asset.File.Delete(System.Collections.Generic.IEnumerable{CodeSmile.Editor.Asset.Path})" />
			public static IList<String> FailedToDeletePaths => s_FailedToDeletePaths;

			/// <summary>
			///     Writes the byte array to disk, then imports and loads the asset. Overwrites any existing file.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="contents">The bytes to write.</param>
			/// <param name="path">Path to a file with extension.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateAsNew(Byte[],Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(string,Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(UnityEngine.Object,Path)"/>
			public static Object Create(Byte[] contents, Path path) => CreateInternal(contents, path);

			/// <summary>
			///     Writes the byte array to disk, then imports and loads the asset. Generates a unique file name
			///     if an asset exists at the path.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="contents">The bytes to write.</param>
			/// <param name="path">Path to a file with extension. Note that the asset's actual file name may differ.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(Byte[],Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateAsNew(string,Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateAsNew(UnityEngine.Object,Path)"/>
			public static Object CreateAsNew(Byte[] contents, Path path) =>
				CreateInternal(contents, path.UniqueFilePath);

			internal static Object CreateInternal(Byte[] bytes, Path path)
			{
				ThrowIf.ArgumentIsNull(bytes, nameof(bytes));
				ThrowIf.ArgumentIsNull(path, nameof(path));

				path.CreateFolders();
				System.IO.File.WriteAllBytes(path, bytes);
				return ImportAndLoad<Object>(path);
			}

			/// <summary>
			///     Writes the string to disk, then imports and loads the asset. Overwrites any existing file.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="contents">The string to write.</param>
			/// <param name="path">Path to a file with extension.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateAsNew(string,Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(Byte[],Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(UnityEngine.Object,Path)"/>
			public static Object Create(String contents, Path path) => CreateInternal(contents, path);

			/// <summary>
			///     Writes the string to disk, then imports and loads the asset. Generates a unique file name
			///     if an asset exists at the path.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="contents">The string to write.</param>
			/// <param name="path">Path to a file with extension. Note that the asset's actual file name may differ.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(string,Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateAsNew(Byte[],Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateAsNew(UnityEngine.Object,Path)"/>
			public static Object CreateAsNew(String contents, Path path) =>
				CreateInternal(contents, path.UniqueFilePath);

			internal static Object CreateInternal(String contents, Path path)
			{
				ThrowIf.ArgumentIsNull(contents, nameof(contents));
				ThrowIf.ArgumentIsNull(path, nameof(path));

				path.CreateFolders();
				System.IO.File.WriteAllText(path, contents, Encoding.UTF8); // string assets ought to be UTF8
				return ImportAndLoad<Object>(path);
			}

			/// <summary>
			///     Writes the object to disk. Overwrites any existing file.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="obj">The object to save as an asset file.</param>
			/// <param name="path">Path to a file with extension.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateAsNew(UnityEngine.Object,Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(byte[],Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(string,Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateOrLoad{T}"/>
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CreateAsset.html">AssetDatabase.CreateAsset</a></seealso>
			public static Object Create(Object obj, Path path) => CreateInternal(obj, path);

			/// <summary>
			///     Writes the object to disk. Generates a unique file name if an asset exists at the path.
			/// </summary>
			/// <remarks>Creates missing folders in the destination path. </remarks>
			/// <param name="obj">The object to save as an asset file.</param>
			/// <param name="path">Path to a file with extension. Note that the asset's actual file name may differ.</param>
			/// <returns>The newly created asset.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(UnityEngine.Object,Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateAsNew(byte[],Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateAsNew(string,Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CreateOrLoad{T}"/>
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CreateAsset.html">AssetDatabase.CreateAsset</a></seealso>
			public static Object CreateAsNew(Object obj, Path path) => CreateInternal(obj, path.UniqueFilePath);

			internal static Object CreateInternal(Object obj, Path path)
			{
				ThrowIf.ArgumentIsNull(obj, nameof(obj));
				ThrowIf.ArgumentIsNull(path, nameof(path));

				path.CreateFolders();
				AssetDatabase.CreateAsset(obj, path);
				return obj;
			}

			/// <summary>
			///     Loads or creates an asset at path.
			/// </summary>
			///<remarks>
			/// Will first attempt to load the asset at path. If this fails, will create an asset from the
			/// object returned by getInstance.
			/// This is an alias for CodeSmile.Editor.Asset.LoadOrCreate{T}.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <param name="getInstance">Func that returns a UnityEngine.Object</param>
			/// <typeparam name="T">The type of the asset.</typeparam>
			/// <returns>The loaded or created object of type T, or null if the object is not of type T.
			/// Note that the asset file gets created in this case.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.LoadOrCreate{T}" />
			public static T CreateOrLoad<T>(Path path, Func<T> getInstance) where T : Object =>
				LoadOrCreate<T>(path, getInstance);

			/// <summary>
			///     Saves the object to disk if it is dirty.
			/// </summary>
			/// <remarks>
			///     Depending on how changes were made you may have to use CodeSmile.Editor.Asset.File.ForceSave instead.
			/// </remarks>
			/// <param name="obj">The object to save.</param>
			/// <see cref="CodeSmile.Editor.Asset.File.ForceSave" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SaveAssetIfDirty.html">AssetDatabase.SaveAssetIfDirty</a></seealso>
			public static void Save(Object obj) => SaveInternal(obj);

			/// <summary>
			///     Forces the object to be saved to disk. Marks the object as dirty and then calls
			/// CodeSmile.Editor.Asset.File.Save.
			/// </summary>
			/// <remarks>
			///     This should only be used if the asset doesn't save otherwise, since unnecessarily writing an object
			///		to disk is a slow operation. Example where this may be required would be changes to fields of a
			///		ScriptableObject without using the
			/// <a href="https://docs.unity3d.com/ScriptReference/SerializedObject.html">SerializedObject</a> or
			/// <a href="https://docs.unity3d.com/ScriptReference/SerializedProperty.html">SerializedProperty</a> classes.
			/// </remarks>
			/// <param name="obj">The object to mark as dirty, then save.</param>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Save" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SaveAssetIfDirty.html">AssetDatabase.SaveAssetIfDirty</a></seealso>
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/EditorUtility.SetDirty.html">EditorUtility.SetDirty</a></seealso>
			public static void ForceSave(Object obj) => SaveInternal(obj, true);

			private static void SaveInternal(Object obj, Boolean forceSave = false)
			{
				ThrowIf.ArgumentIsNull(obj, nameof(obj));
				ThrowIf.NotInDatabase(obj);

				if (forceSave)
					EditorUtility.SetDirty(obj);

				AssetDatabase.SaveAssetIfDirty(obj);
			}

			/// <summary>
			///     Saves any changes to the asset to disk, by GUID.
			/// </summary>
			/// <param name="guid">The guid of the asset.</param>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Save" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SaveAssetIfDirty.html">AssetDatabase.SaveAssetIfDirty</a></seealso>
			public static void Save(GUID guid)
			{
				ThrowIf.NotAnAssetGuid(guid);

				AssetDatabase.SaveAssetIfDirty(guid);
			}

			/// <summary>
			///     Imports a file at a given path that was created or modified 'externally'.
			///		Externally refers to any means other than AssetDatabase methods such as System.IO or batch scripts.
			/// </summary>
			/// <remarks>
			///		CodeSmile AssetDatabase provides convenience Create methods that automatically import assets:
			/// <see cref="CodeSmile.Editor.Asset.File.Create(Byte[],Path)"/>
			/// <see cref="CodeSmile.Editor.Asset.File.Create(string,Path)"/>
			///
			///     Call CodeSmile.Editor.Asset.Database.ImportAll to get rid of externally deleted files.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <param name="options"><a href="https://docs.unity3d.com/ScriptReference/ImportAssetOptions.html">ImportAssetOptions</a></param>
			/// <seealso cref="CodeSmile.Editor.Asset.File.ImportAndLoad{T}"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(Byte[],Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(string,Path)"/>
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportAsset.html">AssetDatabase.ImportAsset</a></seealso>
			public static void Import(Path path, ImportAssetOptions options = ImportAssetOptions.Default)
			{
				if (path != null && path.ExistsInFileSystem)
					AssetDatabase.ImportAsset(path, options);
			}

			/// <summary>
			///     Imports a file at a given path that was created or modified 'externally', then loads and returns
			/// the asset object. See CodeSmile.Editor.Asset.File.Import for more info.
			/// </summary>
			/// <remarks>
			///		CodeSmile AssetDatabase provides convenience Create methods that automatically import/load assets:
			/// <see cref="CodeSmile.Editor.Asset.File.Create(Byte[],Path)"/>
			/// <see cref="CodeSmile.Editor.Asset.File.Create(string,Path)"/>
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <param name="options"><a href="https://docs.unity3d.com/ScriptReference/ImportAssetOptions.html">ImportAssetOptions</a></param>
			/// <typeparam name="T">The type of the asset.</typeparam>
			/// <returns>The asset object, or null if the asset could not be loaded or is not of type T.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.ImportAndLoad{T}"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Load{T}(Path)" />
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(Byte[],Path)"/>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Create(string,Path)"/>
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ImportAsset.html">AssetDatabase.ImportAsset</a></seealso>
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAssetAtPath.html">AssetDatabase.LoadAssetAtPath</a></seealso>
			public static T ImportAndLoad<T>(Path path, ImportAssetOptions options = ImportAssetOptions.Default)
				where T : Object
			{
				ThrowIf.ArgumentIsNull(path, nameof(path));

				ImportIfNotImported(path, options);
				return Load<T>(path);
			}


			/// <summary>
			///     Loads an asset at path.
			/// </summary>
			/// <remarks>
			/// Will only load visible sub-objects. Returns the first object of the type found. If there are multiple
			/// objects of the same type, use CodeSmile.Editor.Asset.SubAsset.LoadVisible instead.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <typeparam name="T">Type of the asset.</typeparam>
			/// <returns>The loaded asset object, or null if the asset does not exist or does not contain a
			/// visible object of type T.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.LoadMain{T}" />
			/// <seealso cref="CodeSmile.Editor.Asset.File.LoadOrCreate{T}" />
			/// <seealso cref="CodeSmile.Editor.Asset.SubAsset.LoadVisible" />
			/// <seealso cref="CodeSmile.Editor.Asset.SubAsset.LoadAll" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAssetAtPath.html">AssetDatabase.LoadAssetAtPath</a></seealso>
			public static T Load<T>(Path path) where T : Object
			{
				ThrowIf.ArgumentIsNull(path, nameof(path));
				ThrowIf.DoesNotExistInFileSystem(path);

				ImportIfNotImported(path);
				return AssetDatabase.LoadAssetAtPath<T>(path);
			}

			/// <summary>
			///		Loads an asset at path or creates the asset if needed.
			/// </summary>
			/// <remarks>
			///		If the file does not exist, creates the asset with the object returned from the getInstance Func.
			///		If the asset isn't in the database, imports and loads the asset.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <param name="getInstance">Method that returns an object instance. Invoked only if the asset needs to be created.</param>
			/// <typeparam name="T">The type of the asset.</typeparam>
			/// <returns>The loaded or created asset.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Load{T}(Path)" />
			public static T LoadOrCreate<T>(Path path, Func<T> getInstance) where T : Object
			{
				if (path.ExistsInFileSystem == false)
					return Create(getInstance.Invoke(), path) as T;

				return ImportAndLoad<T>(path);
			}

			/// <summary>
			///     Loads the main asset at the path.
			/// </summary>
			/// <remarks>
			///     Most of the times the main asset is the only object. There are assets that consist of multiple
			///     objects however. For example Mesh assets often contain sub objects like animations and materials.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <typeparam name="T">Type of the asset.</typeparam>
			/// <returns>The asset or null if the path does not exist.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Load{T}(Path)" />
			/// <seealso cref="CodeSmile.Editor.Asset.File.LoadOrCreate{T}" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadMainAssetAtPath.html">AssetDatabase.LoadMainAssetAtPath</a></seealso>
			public static T LoadMain<T>(Path path) where T : Object
			{
				ThrowIf.ArgumentIsNull(path, nameof(path));
				ThrowIf.DoesNotExistInFileSystem(path);

				ImportIfNotImported(path);
				return AssetDatabase.LoadMainAssetAtPath(path) as T;
			}

			/// <summary>
			///     Loads the main asset object for the GUID.
			/// </summary>
			/// <remarks>
			///     Most of the times the main asset is the only object. There are assets that consist of multiple
			///     objects however. For example Mesh assets often contain sub objects like animations and materials.
			/// </remarks>
			/// <param name="guid">GUID of an asset.</param>
			/// <typeparam name="T">Type of the asset.</typeparam>
			/// <seealso cref="CodeSmile.Editor.Asset.File.LoadMain{T}(Path)" />
			/// <seealso cref="CodeSmile.Editor.Asset.File.LoadOrCreate{T}" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadMainAssetAtPath.html">AssetDatabase.LoadMainAssetAtPath</a></seealso>
			/// <returns>The asset object or null if the guid is not an asset guid.</returns>
			public static T LoadMain<T>(GUID guid) where T : Object
			{
				ThrowIf.NotAnAssetGuid(guid);

				var path = Path.Get(guid);
				ImportIfNotImported(path);
				return LoadMain<T>(path);
			}

			private static void ImportIfNotImported(Path path, ImportAssetOptions options = ImportAssetOptions.Default)
			{
				// not in database but on disk? Import. Cannot determine if existing file has been updated though.
				if (path.Exists == false && path.ExistsInFileSystem)
					Import(path, options);
			}

			/// <summary>
			///     Loads an object and its dependencies asynchronously.
			/// </summary>
			/// <remarks>Available in Unity 2022.2 or newer. In previous versions throws a NotSupportedException.</remarks>
			/// <param name="path">The path to an asset file.</param>
			/// <param name="localFileId">The local file ID of the (sub) asset. I'm sorry but this is what Unity requires.</param>
			/// <returns>An AssetDatabaseLoadOperation to track progress.</returns>
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadObjectAsync.html">AssetDatabase.LoadObjectAsync</a></seealso>
			public static AssetDatabaseLoadOperation LoadAsync(Path path, Int64 localFileId)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.LoadObjectAsync(path, localFileId);
#else
				throw new NotSupportedException($"AssetDatabase.LoadObjectAsync not available in this editor version");
#endif
			}


#if !UNITY_2022_2_OR_NEWER // dummy for LoadAsync in earlier versions
			public class AssetDatabaseLoadOperation {}
#endif

			/// <summary>
			///     Finds asset GUIDs by the given filter criteria.
			/// </summary>
			/// <param name="filter">A search filter string.</param>
			/// <param name="searchInFolders">A list of folders to recursively search for files. Limiting the searched folders speeds up Find.</param>
			/// <returns>An array of string GUIDs. Empty array if there were no search results.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.FindGuids" />
			/// <seealso cref="CodeSmile.Editor.Asset.File.FindPaths" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html">AssetDatabase.FindAssets</a></seealso>
			/// <seealso cref=""><a href="https://forum.unity.com/threads/please-document-assetdatabase-findassets-filters.964907/">Search Filter String Examples</a></seealso>
			public static String[] Find(String filter, String[] searchInFolders = null) => searchInFolders == null
				? AssetDatabase.FindAssets(filter)
				: AssetDatabase.FindAssets(filter, searchInFolders);

			/// <summary>
			///     Finds asset GUIDs by the given filter criteria.
			/// </summary>
			/// <remarks> Casts the result of CodeSmile.Editor.Asset.File.Find. </remarks>
			/// <param name="filter">A search filter string.</param>
			/// <param name="searchInFolders">A list of folders to recursively search for files. Limiting the searched folders speeds up Find.</param>
			/// <returns>An array of GUIDs. Empty array if there were no search results.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Find" />
			/// <seealso cref="CodeSmile.Editor.Asset.File.FindPaths" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html">AssetDatabase.FindAssets</a></seealso>
			/// <seealso cref=""><a href="https://forum.unity.com/threads/please-document-assetdatabase-findassets-filters.964907/">Search Filter String Examples</a></seealso>
			public static GUID[] FindGuids(String filter, String[] searchInFolders = null) =>
				Find(filter, searchInFolders).Select(guid => new GUID(guid)).ToArray();

			/// <summary>
			///     Finds asset paths by the given filter criteria.
			/// </summary>
			/// <remarks>Converts the list of string guids from CodeSmile.Editor.Asset.File.Find to actual Paths.</remarks>
			/// <param name="filter">A search filter string.</param>
			/// <param name="searchInFolders">A list of folders to recursively search for files. Limiting the searched folders speeds up Find.</param>
			/// <returns>An Path array. Empty array if there were no search results.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Find" />
			/// <seealso cref="CodeSmile.Editor.Asset.File.FindGuids" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html">AssetDatabase.FindAssets</a></seealso>
			/// <seealso cref=""><a href="https://forum.unity.com/threads/please-document-assetdatabase-findassets-filters.964907/">Search Filter String Examples</a></seealso>
			public static Path[] FindPaths(String filter, String[] searchInFolders = null) =>
				Find(filter, searchInFolders).Select(guid => Path.Get(new GUID(guid))).ToArray();

			/// <summary>
			///     Copies an asset from source to destination path. Overwrites any existing assets.
			/// </summary>
			/// <remarks>
			///     Will create any missing destination folders automatically.
			/// </remarks>
			/// <param name="sourcePath">The source asset to copy.</param>
			/// <param name="destinationPath">Path to destination file.</param>
			/// <returns>True if copying succeeded, false if it failed. Use CodeSmile.Editor.Asset.GetLastErrorMessage to get the failure message.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CopyAsNew" />
			/// <seealso cref="CodeSmile.Editor.Asset.GetLastErrorMessage" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CopyAsset.html">AssetDatabase.CopyAsset</a></seealso>
			public static Boolean Copy(Path sourcePath, Path destinationPath) =>
				CopyInternal(sourcePath, destinationPath, true);

			/// <summary>
			///     Copies an asset from source to destination path. Generates a unique file name if an asset
			/// already exist at destinationPath.
			/// </summary>
			/// <remarks>
			///     Will create any missing destination folders automatically.
			/// </remarks>
			/// <param name="sourcePath">The source asset to copy.</param>
			/// <param name="destinationPath">Path to destination file. Note that the actual file name may differ.</param>
			/// <returns>True if copying succeeded, false if it failed. Use CodeSmile.Editor.Asset.GetLastErrorMessage to get the failure message.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Copy" />
			/// <seealso cref="CodeSmile.Editor.Asset.GetLastErrorMessage" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CopyAsset.html">AssetDatabase.CopyAsset</a></seealso>
			public static Boolean CopyAsNew(Path sourcePath, Path destinationPath) =>
				CopyInternal(sourcePath, destinationPath.UniqueFilePath, false);

			internal static Boolean CopyInternal(Path sourcePath, Path destinationPath, Boolean overwriteExisting)
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

			/// <summary>
			///     Tests if an asset can be moved to destination without moving the asset.
			/// </summary>
			/// <remarks>
			///     Note: This returns false if any folders of destinationPath do not exist.
			/// </remarks>
			/// <param name="sourcePath">The path to an asset file.</param>
			/// <param name="destinationPath">The path to move the file to. May have a different extension.</param>
			/// <returns>True if moving the asset will be successful, false if part of the destinationPath does not exist or other reasons.
			/// Use CodeSmile.Editor.Asset.GetLastErrorMessage to get the failure message.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Move" />
			/// <seealso cref="CodeSmile.Editor.Asset.File.Rename" />
			/// <seealso cref="CodeSmile.Editor.Asset.GetLastErrorMessage" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ValidateMoveAsset.html">AssetDatabase.ValidateMoveAsset</a></seealso>
			public static Boolean CanMove(Path sourcePath, Path destinationPath)
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
			/// <returns>True if moving the asset will be successful, false if move failed.
			/// Use CodeSmile.Editor.Asset.GetLastErrorMessage to get the failure message.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.CanMove" />
			/// <seealso cref="CodeSmile.Editor.Asset.File.Rename" />
			/// <seealso cref="CodeSmile.Editor.Asset.GetLastErrorMessage" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MoveAsset.html">AssetDatabase.MoveAsset</a></seealso>
			public static Boolean Move(Path sourcePath, Path destinationPath)
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
			///     Cannot be used to change a file's extension. Use CodeSmile.Editor.Asset.File.Move to change the extension.
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <param name="newFileName">The new file name without extension.</param>
			/// <returns>
			///     True if the rename succeeded, false otherwise.
			///     On failure use CodeSmile.Editor.Asset.GetLastErrorMessage to get the failure reason.
			/// </returns>
			/// <seealso cref="CodeSmile.Editor.Asset.File.Move" />
			/// <seealso cref="CodeSmile.Editor.Asset.GetLastErrorMessage" />
			/// <seealso cref=""><a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.RenameAsset.html">AssetDatabase.RenameAsset</a></seealso>
			public static Boolean Rename(Path path, String newFileName)
			{
				if (path == null || newFileName == null)
					return false;

				return Succeeded(AssetDatabase.RenameAsset(path, newFileName));
			}

			/// <summary>
			///     Returns true if the given object can be opened (edited) by the Unity editor.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			/// <param name="FAIL ERROR TBD CONTINUE HERE TOMORROW">added so i know where i was</param>
			public static Boolean CanOpenInEditor(Object obj)
			{
				ThrowIf.ArgumentIsNull(obj, nameof(obj));

				return CanOpenInEditor(obj.GetInstanceID());
			}

			/// <summary>
			///     Returns true if the given object can be opened (edited) by the Unity editor.
			/// </summary>
			/// <param name="instanceId"></param>
			/// <returns></returns>
			public static Boolean CanOpenInEditor(Int32 instanceId) => AssetDatabase.CanOpenAssetInEditor(instanceId);

			/// <summary>
			///     Opens the asset in the default (associated) application.
			///     Optional line and column numbers can be specified for text files and applications that support this.
			/// </summary>
			/// <param name="obj"></param>
			/// <param name="lineNumber"></param>
			/// <param name="columnNumber"></param>
			[ExcludeFromCodeCoverage] // cannot be tested
			public static void OpenExternal(Object obj, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
				AssetDatabase.OpenAsset(obj, lineNumber, columnNumber);

			/// <summary>
			///     Opens the asset (by its instanceID) in the default (associated) application.
			///     Optional line and column numbers can be specified for text files and applications that support this.
			/// </summary>
			/// <param name="instanceId"></param>
			/// <param name="lineNumber"></param>
			/// <param name="columnNumber"></param>
			[ExcludeFromCodeCoverage] // cannot be tested
			public static void OpenExternal(Int32 instanceId, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
				AssetDatabase.OpenAsset(instanceId, lineNumber, columnNumber);

			/// <summary>
			///     Opens the asset at the path in the default (associated) application.
			///     Optional line and column numbers can be specified for text files and applications that support this.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="lineNumber"></param>
			/// <param name="columnNumber"></param>
			[ExcludeFromCodeCoverage] // cannot be tested
			public static void OpenExternal(Path path, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
				OpenExternal(Load<Object>(path), lineNumber, columnNumber);

			/// <summary>
			///     Deletes the asset file. Does nothing if there is no file at the given path.
			/// </summary>
			/// <param name="path"></param>
			/// <returns>True if the asset was deleted, false otherwise.</returns>
			public static Boolean Delete(Path path)
			{
				if (path == null || path.Exists == false)
					return false;

				return AssetDatabase.DeleteAsset(path);
			}

			/// <summary>
			///     Deletes the asset. Does nothing if the object is not an asset.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns>True if the asset was deleted, false otherwise.</returns>
			public static Boolean Delete(Object obj) => Delete(Path.Get(obj));

			/// <summary>
			///     Tries to delete all the given assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns>
			///     True if all assets where deleted, false if one or more failed to delete whose paths
			///     you can access via the Asset.Database.DeleteFailedPaths property.
			/// </returns>
			/// <see cref="FailedToDeletePaths" />
			public static Boolean Delete(IEnumerable<Path> paths) => Delete(paths.Cast<String>());

			/// <summary>
			///     Tries to delete all the given assets.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns>
			///     True if all assets where deleted, false if one or more failed to delete whose paths
			///     you can access via the Asset.Database.DeleteFailedPaths property.
			/// </returns>
			/// <see cref="FailedToDeletePaths" />
			public static Boolean Delete(IEnumerable<String> paths) =>
				AssetDatabase.DeleteAssets(paths.ToArray(), s_FailedToDeletePaths = new List<String>());

			/// <summary>
			///     Moves the asset file to the OS trash (same as Delete, but recoverable).
			///     Does nothing if there is no file at the given path.
			/// </summary>
			/// <param name="path"></param>
			/// <returns>True if successfully trashed</returns>
			public static Boolean Trash(Path path) =>
				path != null && path.Exists && AssetDatabase.MoveAssetToTrash(path);

			/// <summary>
			///     Moves the asset to the OS trash (same as Delete, but recoverable).
			///     Does nothing if the object is not an asset.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns>True if successfully trashed</returns>
			public static Boolean Trash(Object obj) => Trash(Path.Get(obj));

			/// <summary>
			///     Tries to move all the given assets to the OS trash.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns>
			///     True if all assets where trashed, false if one or more failed to trash whose paths
			///     you can access via the Asset.Database.DeleteFailedPaths property.
			/// </returns>
			/// <see cref="File.FailedToDeletePaths" />
			public static Boolean Trash(IEnumerable<Path> paths) => Trash(paths.Cast<String>());

			/// <summary>
			///     Tries to move all the given assets to the OS trash.
			/// </summary>
			/// <param name="paths"></param>
			/// <returns>
			///     True if all assets where trashed, false if one or more failed to trash whose paths
			///     you can access via the Asset.Database.DeleteFailedPaths property.
			/// </returns>
			/// <see cref="File.FailedToDeletePaths" />
			public static Boolean Trash(IEnumerable<String> paths) =>
				AssetDatabase.MoveAssetsToTrash(paths.ToArray(), s_FailedToDeletePaths = new List<String>());

			/// <summary>
			///     Speed up mass asset editing (create, modify, delete, import, etc).
			///     Within the callback action, the AssetDatabase does neither import nor auto-refresh assets.
			///     This can significantly speed up mass asset editing tasks where you work with individual assets
			///     in a loop.
			///     Internally calls <a href="https://docs.unity3d.com/Manual/AssetDatabaseBatching.html">Start/StopAssetEditing</a>
			///     in a try/finally block so that exceptions will not cause the AssetDatabase to remain stopped indefinitely.
			/// </summary>
			/// <param name="massAssetFileEditAction"></param>
			/// <param name="rethrowExceptions"></param>
			public static void BatchEditing(Action massAssetFileEditAction, Boolean rethrowExceptions = false)
			{
				ThrowIf.ArgumentIsNull(massAssetFileEditAction, nameof(massAssetFileEditAction));

				try
				{
					Database.StartAssetEditing();
					massAssetFileEditAction.Invoke();
				}
				catch (Exception ex)
				{
					Debug.LogError($"Exception during BatchEditing: {ex.Message}");

					if (rethrowExceptions)
						throw ex; // re-throw to caller
				}
				finally
				{
					Database.StopAssetEditing();
				}
			}
		}
	}
}
