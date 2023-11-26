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
			/// <returns></returns>
			/// <see cref="Trash(System.Collections.Generic.IEnumerable{CodeSmile.Editor.Asset.Path})" />
			/// <see cref="Delete(System.Collections.Generic.IEnumerable{CodeSmile.Editor.Asset.Path})" />
			public static IList<String> FailedToDeletePaths => s_FailedToDeletePaths;

			public static Object Create(Byte[] bytes, Path path) => CreateInternal(bytes, path);

			public static Object CreateAsNew(Byte[] bytes, Path path) => CreateInternal(bytes, path.UniqueFilePath);

			public static Object Create(String contents, Path path) => CreateInternal(contents, path);

			public static Object CreateAsNew(String contents, Path path) => CreateInternal(contents, path.UniqueFilePath);

			/// <summary>
			///     Creates (saves) an asset file at the target path. If an asset already exists it will be overwritten.
			/// </summary>
			/// <remarks>
			///     Also creates all non-existing folders in the path.
			/// </remarks>
			/// <param name="obj">The object to save as an asset file.</param>
			/// <param name="path">The relative asset path with filename and extension.</param>
			/// <returns>The created object.</returns>
			public static Object Create(Object obj, Path path) => CreateInternal(obj, path);

			/// <summary>
			///     Creates (saves) a new asset file at the target path. Will not overwrite existing assets.
			/// </summary>
			/// <remarks>
			///     If an asset already exists at the path the new asset will be created with a unique file name.
			///     Also creates all non-existing folders in the path.
			/// </remarks>
			/// <param name="obj">The object to save as an asset file.</param>
			/// <param name="path">
			///     The relative asset path with filename and extension. Note that the actual path of the asset may
			///     differ.
			/// </param>
			/// <returns>The newly created object.</returns>
			public static Object CreateAsNew(Object obj, Path path) => CreateInternal(obj, path.UniqueFilePath);

			internal static Object CreateInternal(Byte[] bytes, Path path)
			{
				ThrowIf.ArgumentIsNull(bytes, nameof(bytes));
				ThrowIf.ArgumentIsNull(path, nameof(path));

				path.CreateFolders();
				System.IO.File.WriteAllBytes(path, bytes);
				return ImportAndLoad<Object>(path);
			}

			internal static Object CreateInternal(String contents, Path path)
			{
				ThrowIf.ArgumentIsNull(contents, nameof(contents));
				ThrowIf.ArgumentIsNull(path, nameof(path));

				path.CreateFolders();
				System.IO.File.WriteAllText(path, contents, Encoding.UTF8); // string assets ought to be UTF8
				return ImportAndLoad<Object>(path);
			}

			internal static Object CreateInternal(Object obj, Path path)
			{
				ThrowIf.ArgumentIsNull(obj, nameof(obj));
				ThrowIf.ArgumentIsNull(path, nameof(path));

				path.CreateFolders();
				AssetDatabase.CreateAsset(obj, path);
				return obj;
			}

			/// <summary>
			///     Tries to load the object at path. If it cannot be loaded, it will be created using the Object instance
			///     returned by the getObjectInstance Func callback.
			///     Alias for LoadOrCreate. This exists mainly for API discovery reasons.
			/// </summary>
			/// <see cref="LoadOrCreate{T}" />
			/// <param name="path"></param>
			/// <param name="getInstance"></param>
			/// <typeparam name="T"></typeparam>
			/// <returns></returns>
			public static T CreateOrLoad<T>(Path path, Func<Object> getInstance) where T : Object =>
				LoadOrCreate<T>(path, getInstance);

			/// <summary>
			///     Saves the object to disk if it is marked dirty.
			///     Note: Depending on how changes were made you may have to use ForceSave(Object) instead.
			/// </summary>
			/// <param name="obj">the asset object to save</param>
			/// <see cref="ForceSave(UnityEngine.Object)" />
			public static void Save(Object obj) => SaveInternal(obj);

			/// <summary>
			///     Forces the object to be saved to disk by first flagging it as dirty.
			///     Note: Be sure that you need to 'force' the save operation, since serializing and object and writing
			///     it to disk is a slow operation.
			/// </summary>
			/// <param name="obj">the asset object to save</param>
			/// <see cref="Save(UnityEngine.Object)" />
			public static void ForceSave(Object obj) => SaveInternal(obj, true);

			/// <summary>
			///     Saves any changes to the object to disk.
			/// </summary>
			/// <param name="guid"></param>
			public static void Save(GUID guid)
			{
				ThrowIf.NotAnAssetGuid(guid);
				AssetDatabase.SaveAssetIfDirty(guid);
			}

			/// <summary>
			///     Imports a file at a given path that was created or modified 'externally', ie not via Asset(Database) methods.
			///     For example, any file/folder modified via System.IO.File.Write*() methods or through batch scripts.
			///     Note: If the path does not exist, this method does nothing.
			///     You will need to call Asset.Database.ImportAll() if you want to get rid of an externally deleted file
			///     that still exists in the AssetDatabase.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="options"></param>
			/// <returns>The input path for method chaining.</returns>
			public static Path Import(Path path, ImportAssetOptions options = ImportAssetOptions.Default)
			{
				if (path != null && path.ExistsInFileSystem)
					AssetDatabase.ImportAsset(path, options);

				return path;
			}

			public static T ImportAndLoad<T>(Path path, ImportAssetOptions options = ImportAssetOptions.ForceUpdate)
				where T : Object => Load<T>(Import(path, options | ImportAssetOptions.ForceUpdate));

			/// <summary>
			///     Loads the main asset object at the path.
			///     Commonly this is the only object of the asset, but there are assets that consist of multiple
			///     objects. For example Mesh assets often contain sub objects like animations and materials.
			/// </summary>
			/// <see cref="Load{T}(CodeSmile.Editor.Asset.Path)" />
			/// <param name="path"></param>
			/// <typeparam name="T"></typeparam>
			/// <returns>The asset object or null if the path does not exist or the asset is not imported.</returns>
			public static T LoadMain<T>(Path path) where T : Object
			{
				ThrowIf.ArgumentIsNull(path, nameof(path));
				ThrowIf.DoesNotExistInFileSystem(path);

				return AssetDatabase.LoadMainAssetAtPath(path) as T;
			}

			/// <summary>
			///     Loads the main asset object for the guid.
			///     Commonly this is the only object of the asset, but there are assets that
			///     consist of multiple objects such as Mesh assets that may contain for example animations and materials.
			/// </summary>
			/// <param name="guid"></param>
			/// <typeparam name="T"></typeparam>
			/// <returns>The asset object or null if the guid is not an asset guid.</returns>
			public static T LoadMain<T>(GUID guid) where T : Object
			{
				ThrowIf.NotAnAssetGuid(guid);

				return LoadMain<T>(Path.Get(guid));
			}

			/// <summary>
			///     Tries to load the object at path. If it cannot be loaded, it will be created using the Object instance
			///     returned by the getObjectInstance Func callback.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="getInstance"></param>
			/// <typeparam name="T"></typeparam>
			/// <returns></returns>
			public static T LoadOrCreate<T>(Path path, Func<Object> getInstance) where T : Object
			{
				if (path.Exists)
					return LoadMain<T>(path);

				var obj = getInstance.Invoke() as T;
				Create(obj, path);

				return obj;
			}

			/// <summary>
			///     Loads the first visible object of the given type from the asset at path. This usually is the main
			///     object but it could be any other visible sub-object, depending on the type.
			/// </summary>
			/// <see cref="LoadMain{T}(CodeSmile.Editor.Asset.Path)" />
			/// <param name="path"></param>
			/// <typeparam name="T"></typeparam>
			/// <returns></returns>
			public static T Load<T>(Path path) where T : Object => AssetDatabase.LoadAssetAtPath<T>(path);

			/// <summary>
			///     Loads an object and its dependencies asynchronously.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="localFileId"></param>
			/// <returns>An AssetDatabaseLoadOperation instance to track progress.</returns>
			public static AssetDatabaseLoadOperation LoadAsync(Path path, Int64 localFileId)
			{
#if UNITY_2022_2_OR_NEWER
				return AssetDatabase.LoadObjectAsync(path, localFileId);
#else
				throw new NotSupportedException($"AssetDatabase.LoadObjectAsync not available in this editor version");
#endif
			}

#if !UNITY_2022_2_OR_NEWER
	public class AssetDatabaseLoadOperation {}
#endif

			/// <summary>
			///     Finds the assets by the given filter criteria.
			///     Returns an array of string GUIDs for compatibility reasons.
			/// </summary>
			/// <param name="filter"></param>
			/// <param name="searchInFolders"></param>
			/// <returns></returns>
			/// <see cref="FindGuids" />
			/// <see cref="FindPaths" />
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
			public static GUID[] FindGuids(String filter, String[] searchInFolders = null) =>
				Find(filter, searchInFolders).Select(guid => new GUID(guid)).ToArray();

			/// <summary>
			///     Makes a copy of the source asset and saves it in the destination.
			///     Will create destination path folders if necessary.
			/// </summary>
			/// <param name="sourcePath">the source asset to copy from</param>
			/// <param name="destinationPath">the destination path the copy is saved</param>
			/// <param name="overwriteExisting">
			///     True if an existing destination asset should be replaced. False if a unique filename
			///     should be generated.
			/// </param>
			/// <returns>True if copying succeeded, false if it failed.</returns>
			public static Boolean Copy(Path sourcePath, Path destinationPath) =>
				CopyInternal(sourcePath, destinationPath, true);

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
			///     Tests if a Move operation will be successful without actually moving the asset.
			///     Note: Returns false if any of the folders to destinationPath do not exist.
			/// </summary>
			/// <param name="sourcePath">The path to an existing asset.</param>
			/// <param name="destinationPath">The path where to move the asset to. Can have a different extension.</param>
			/// <returns>True if moving the asset will be successful, false otherwise.</returns>
			public static Boolean CanMove(Path sourcePath, Path destinationPath)
			{
				if (sourcePath == null || destinationPath == null)
					return false;

				return Succeeded(AssetDatabase.ValidateMoveAsset(sourcePath, destinationPath));
			}

			/// <summary>
			///     Moves an asset from source to destination path. Any non-existing folders in destination path
			///     will be created.
			/// </summary>
			/// <param name="sourcePath"></param>
			/// <param name="destinationPath"></param>
			/// <returns>True if the move was successful.</returns>
			public static Boolean Move(Path sourcePath, Path destinationPath)
			{
				if (sourcePath == null || destinationPath == null)
					return false;

				destinationPath.CreateFolders();
				return Succeeded(AssetDatabase.MoveAsset(sourcePath, destinationPath));
			}

			/// <summary>
			///     Renames an asset's file or folder name.
			///     NOTE: Cannot be used to change a file's extension. Use Move instead.
			///     <see cref="Move" />
			/// </summary>
			/// <param name="assetPath">The path to the file or folder to rename.</param>
			/// <param name="newFileName">
			///     The new name of the file or folder, without extension.
			/// </param>
			/// <returns>
			///     True if the rename succeeded, false otherwise.
			///     If false, Asset.LastErrorMessage provides a human-readable failure reason.
			/// </returns>
			public static Boolean Rename(Path assetPath, String newFileName)
			{
				if (assetPath == null || newFileName == null)
					return false;

				return Succeeded(AssetDatabase.RenameAsset(assetPath, newFileName));
			}

			/// <summary>
			///     Returns true if the given object can be opened (edited) by the Unity editor.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
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

			private static void SaveInternal(Object obj, Boolean forceSave = false)
			{
				ThrowIf.ArgumentIsNull(obj, nameof(obj));
				ThrowIf.NotInDatabase(obj);

				if (forceSave)
					EditorUtility.SetDirty(obj);

				AssetDatabase.SaveAssetIfDirty(obj);
			}
		}
	}
}
