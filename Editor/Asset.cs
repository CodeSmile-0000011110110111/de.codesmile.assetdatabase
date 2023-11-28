// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	/// <summary>
	///     Replacement implementation for Unity's *massive* AssetDatabase class with a cleaner interface
	///     and more error checking.
	///     Asset is instantiable so you can work with assets like you do with UnityEngine.Object. In fact, Asset
	///     is essentially a wrapper around the asset's UnityEngine.Object (see: MainObject).
	/// </summary>
	public sealed partial class Asset
	{
		private Path m_AssetPath;
		private Object m_MainObject;

		[ExcludeFromCodeCoverage] private Asset() {} // disallow parameterless ctor

		/// <summary>
		///     Creates an asset file from a byte array.
		/// </summary>
		/// <remarks>Writes the data to a file, then imports the file and loads the asset object.</remarks>
		/// <param name="contents">The data to save as an asset file.</param>
		/// <param name="path">Path where to save the new asset file, with extension.</param>
		/// <param name="overwriteExisting">
		///     If true, will overwrite any existing asset at path. Otherwise does not overwrite but generates a unique
		///     filename (default).
		/// </param>
		public Asset(Byte[] contents, Path path, Boolean overwriteExisting = false)
		{
			ThrowIf.ArgumentIsNull(contents, nameof(contents));
			ThrowIf.ArgumentIsNull(path, nameof(path));

			path = Path.UniquifyAsNeeded(path, overwriteExisting);
			var obj = File.CreateInternal(contents, path);
			InitWithObject(obj);
		}

		/// <summary>
		///     Creates an asset file from a string.
		/// </summary>
		/// <param name="contents">The string to save as an asset file.</param>
		/// <param name="path">Path where to save the new asset file, with extension.</param>
		/// <param name="overwriteExisting">
		///     If true, will overwrite any existing asset at path. Otherwise does not overwrite but generates a unique
		///     filename (default).
		/// </param>
		public Asset(String contents, Path path, Boolean overwriteExisting = false)
		{
			ThrowIf.ArgumentIsNull(contents, nameof(contents));
			ThrowIf.ArgumentIsNull(path, nameof(path));

			path = Path.UniquifyAsNeeded(path, overwriteExisting);
			var obj = File.CreateInternal(contents, path);
			InitWithObject(obj);
		}

		/// <summary>
		///     Creates an asset file from an existing UnityEngine.Object instance.
		/// </summary>
		/// <remarks>
		///     The object must not already be an asset file (throws exception).
		/// </remarks>
		/// <param name="obj">The object to create as an asset file.</param>
		/// <param name="path">Path where to save the new asset file, with extension.</param>
		/// <param name="overwriteExisting">
		///     If true, will overwrite any existing asset at path. Otherwise does not overwrite but generates a unique
		///     filename (default).
		/// </param>
		/// <exception cref="ArgumentNullException">If the object is null.</exception>
		/// <exception cref="ArgumentNullException">If the path is null.</exception>
		/// <exception cref="ArgumentException">If the object is already serialized to an asset file.</exception>
		/// <seealso cref="CodeSmile.Editor.Asset(UnityEngine.Object)" />
		/// <seealso cref="CodeSmile.Editor.Asset(CodeSmile.Editor.Asset.Path)" />
		public Asset(Object obj, Path path, Boolean overwriteExisting = false)
		{
			ThrowIf.ArgumentIsNull(obj, nameof(obj));
			ThrowIf.ArgumentIsNull(path, nameof(path));
			ThrowIf.AlreadyAnAsset(obj);

			path = Path.UniquifyAsNeeded(path, overwriteExisting);
			File.CreateInternal(obj, path);
			InitWithObject(obj);
		}

		/// <summary>
		///     Loads the asset at path.
		/// </summary>
		/// <param name="path">Path to an existing asset file.</param>
		/// <exception cref="ArgumentNullException">If the path is null.</exception>
		/// <exception cref="FileNotFoundException">If the path does not point to an existing asset file.</exception>
		public Asset(Path path) => InitWithPath(path);

		/// <summary>
		///     Loads the asset using its GUID.
		/// </summary>
		/// <param name="assetGuid"></param>
		/// <exception cref="ArgumentException">If the GUID is not in the AssetDatabase (not an asset).</exception>
		public Asset(GUID assetGuid) => InitWithGuid(assetGuid);

		/// <summary>
		///     Uses an existing asset reference.
		/// </summary>
		/// <param name="obj">Instance of an asset.</param>
		/// <exception cref="ArgumentNullException">If the object is null.</exception>
		/// <exception cref="ArgumentException">If the object is not an asset.</exception>
		public Asset(Object obj) => InitWithObject(obj);

		/// <summary>
		///     Implicit conversion to UnityEngine.Object.
		/// </summary>
		/// <param name="asset">The main object of the asset.</param>
		/// <example>
		///     <code>
		///  Object obj = asset; // implicit conversion
		///  MyType my = (MyType)asset; // implicit conversion with cast
		///  MyType my = asset as MyType; // implicit conversion with 'as' operator
		/// 		</code>
		/// </example>
		/// <returns>The asset's MainObject property.</returns>
		public static implicit operator Object(Asset asset) => asset != null ? asset.MainObject : null;

		/// <summary>
		///     Implicit conversion of UnityEngine.Object to an Asset.
		/// </summary>
		/// <remarks>Throws exception if obj is not an asset object.</remarks>
		/// <param name="obj">Existing asset reference.</param>
		/// <returns>An asset instance or null if obj is null.</returns>
		/// <example>
		///     <code>
		///  Asset asset = obj; // implicit conversion: Object to Asset
		/// 		</code>
		/// </example>
		public static implicit operator Asset(Object obj) => obj != null ? new Asset(obj) : null;

		/// <summary>
		///     Implicit conversion of Asset.Path to an Asset instance.
		/// </summary>
		/// <param name="path">Path to an asset file.</param>
		/// <returns>An asset instance or null if path is null.</returns>
		/// <example>
		///     <code>
		///  // this imports & loads the asset, neat ey? :)
		///  Path path = ..;
		///  Asset asset = path;
		/// 		</code>
		/// </example>
		public static implicit operator Asset(Path path) => path != null ? new Asset(path) : null;

		/// <summary>
		///     Implicit conversion of string path to an asset instance.
		/// </summary>
		/// <param name="path">Path to an asset file.</param>
		/// <returns>An asset instance or null if path is null.</returns>
		/// <example>
		///     <code>
		///  // this imports & loads the asset, neat ey? :)
		///  Asset asset = "Assets/Folder/MyAsset.asset";
		/// 		</code>
		/// </example>
		public static implicit operator Asset(String path) => (Path)path; // implicit forward to Asset(Path)

		/// <summary>
		///     Implicit conversion of GUID to an asset instance.
		/// </summary>
		/// <param name="guid">An asset instance.</param>
		/// <returns>An asset instance or null if guid is empty.</returns>
		/// <example>
		///     <code>
		/// // loads the asset
		/// GUID guid = ..
		/// Asset asset = guid;
		/// </code>
		/// </example>
		public static implicit operator Asset(GUID guid) => guid.Empty() == false ? new Asset(guid) : null;

		/// <summary>
		///     Alternative to casting the asset instance.
		/// </summary>
		/// <remarks>
		///     This is an alias for:
		///     <code>var obj = asset as T;</code>
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <returns>Returns MainObject cast to T or null if main object is not of type T.</returns>
		public T Get<T>() where T : Object => m_MainObject as T;

		/// <summary>
		///     Saves any changes to the asset to disk.
		/// </summary>
		/// <remarks>
		///     Not every change marks an object as 'dirty'. In such cases you need to use
		///     CodeSmile.Editor.Asset.ForceSave().
		/// </remarks>
		/// <seealso cref="CodeSmile.Editor.Asset.ForceSave()" />
		public void Save() => File.Save(m_MainObject);

		/// <summary>
		///     Saves the asset to disk, regardless of whether it is marked as 'dirty'.
		/// </summary>
		/// <remarks>
		///     Force saving is achieved by flagging the object as dirty with
		///     <a href="https://docs.unity3d.com/ScriptReference/EditorUtility.SetDirty.html">EditorUtility.SetDirty()</a>.
		/// </remarks>
		/// <seealso cref="CodeSmile.Editor.Asset.Save()" />
		public void ForceSave() => File.ForceSave(m_MainObject);

		/// <summary>
		///     Saves a copy of the asset to a new path. Overwrites any existing asset at path.
		/// </summary>
		/// <remarks>
		///     Will automatically create missing folders.
		/// </remarks>
		/// <param name="path">The path where to save the copy.</param>
		/// <returns>
		///     The copy of the Asset or null if copying failed. Use CodeSmile.Editor.Asset.GetLastErrorMessage to get the
		///     human readable error message.
		/// </returns>
		/// <seealso cref="CodeSmile.Editor.Asset.Save" />
		/// <seealso cref="CodeSmile.Editor.Asset.SaveAsNew" />
		/// <seealso cref="CodeSmile.Editor.Asset.GetLastErrorMessage" />
		public Asset SaveAs(Path path) => File.Copy(m_AssetPath, path) ? new Asset(path) : null;

		/// <summary>
		///     Saves a copy of the asset to a new path. Generates a unique file/folder name if path already exists.
		/// </summary>
		/// <remarks>
		///     Will automatically create missing folders.
		/// </remarks>
		/// <param name="path">The path where to save the copy. Note that actual path of the asset may change.</param>
		/// <returns>
		///     The copy of the Asset or null if copying failed. Use CodeSmile.Editor.Asset.GetLastErrorMessage to get the
		///     human readable error message.
		/// </returns>
		/// <seealso cref="CodeSmile.Editor.Asset.Save()" />
		/// <seealso cref="CodeSmile.Editor.Asset.SaveAs()" />
		/// <seealso cref="CodeSmile.Editor.Asset.GetLastErrorMessage" />
		public Asset SaveAsNew(Path path)
		{
			ThrowIf.ArgumentIsNull(path, nameof(path));

			return SaveAs(path.UniqueFilePath);
		}

		/// <summary>
		///     Creates a duplicate of the asset with a new, unique file name.
		/// </summary>
		/// <remarks>
		///     <code>asset.Duplicate();</code>
		///     is short for <code>asset.SaveAsNew(asset.AssetPath);</code>
		/// </remarks>
		/// <returns>The asset instance of the duplicate.</returns>
		public Asset Duplicate() => SaveAsNew(m_AssetPath);

		/// <summary>
		///     Marks the main object as dirty.
		/// </summary>
		/// <seealso cref="CodeSmile.Editor.Asset.ForceSave()" />
		public void SetDirty() => EditorUtility.SetDirty(m_MainObject);

		// NOTE: there is no public Import() method needed since the main object is guaranteed to be imported
		private void Import() {}

		// Private on purpose: the main object is automatically loaded when instantiating an Asset class.
		private T LoadMain<T>() where T : Object =>
			m_AssetPath != null ? (T)(m_MainObject = File.Load<T>(m_AssetPath)) : null;

		/// <summary>
		///     Loads a (sub) object from the asset identified by type.
		/// </summary>
		/// <remarks>
		///     To load the main object of the Asset instance use the CodeSmile.Editor.Asset.MainObject property.
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <returns>Returns the 'first' asset of the type found.</returns>
		/// <seealso cref="CodeSmile.Editor.Asset.SubAssets" />
		/// <seealso cref="CodeSmile.Editor.Asset.VisibleSubAssets" />
		/// <seealso cref="CodeSmile.Editor.Asset.MainObject" />
		public T Load<T>() where T : Object => File.Load<T>(m_AssetPath);

		/// <summary>
		///     Tests if a Move operation will be successful without actually moving the asset.
		/// </summary>
		/// <remarks>
		///     Returns false if one or more folders in destinationPath do not exist.
		///     On failure, use CodeSmile.Editor.Asset.GetLastErrorMessage to get the failure error message.
		/// </remarks>
		/// <param name="destinationPath">The path where to move the asset to. May have a different extension.</param>
		/// <returns>True if moving the asset will be successful, false otherwise.</returns>
		/// <seealso cref="CodeSmile.Editor.Asset.Move" />
		/// <seealso cref="CodeSmile.Editor.Asset.GetLastErrorMessage" />
		public Boolean CanMove(Path destinationPath) => File.CanMove(m_AssetPath, destinationPath);

		/// <summary>
		///     Moves asset to destination path.
		/// </summary>
		/// <remarks>
		///     Missing folders in destination path will be created automatically.
		///     After the move, the CodeSmile.Editor.Asset.AssetPath property is updated accordingly.
		///     On failure, use CodeSmile.Editor.Asset.GetLastErrorMessage to get the failure error message.
		/// </remarks>
		/// <param name="destinationPath">The path where to move the asset to. May have a different extension.</param>
		/// <returns>True if moving the asset will be successful, false otherwise.</returns>
		/// <seealso cref="CodeSmile.Editor.Asset.CanMove" />
		/// <seealso cref="CodeSmile.Editor.Asset.GetLastErrorMessage" />
		public Boolean Move(Path destinationPath)
		{
			if (File.Move(m_AssetPath, destinationPath))
			{
				SetAssetPathFromObject();
				return true;
			}

			return false;
		}

		/// <summary>
		///     Renames an asset's file name (without extension) or a folder.
		/// </summary>
		/// <remarks>
		///     Use CodeSmile.Editor.Asset.Move if you need to change the file's extension.
		/// </remarks>
		/// <example>
		///     Rename file:
		///     <code>
		/// Asset asset = new Asset(obj, "Assets/initial name.asset");
		/// asset.Rename("new file name");
		/// </code>
		///     Rename folder:
		///     <code>
		/// Asset asset = new Asset(obj, "Assets/subfloder");
		/// asset.Rename("subfolder");
		/// </code>
		/// </example>
		/// <param name="newFileName">
		///     The new name of the file or folder, without extension. Must not be a path.
		/// </param>
		/// <returns>
		///     True if the rename succeeded. The AssetPath property will be updated accordingly.
		///     If false, CodeSmile.Editor.Asset.GetLastErrorMessage provides a human-readable failure reason and
		///     the AssetPath property remains unchanged.
		/// </returns>
		/// <seealso cref="CodeSmile.Editor.Asset.Move" />
		/// <seealso cref="CodeSmile.Editor.Asset.GetLastErrorMessage" />
		public Boolean Rename(String newFileName)
		{
			if (File.Rename(m_AssetPath, newFileName))
			{
				SetAssetPathFromObject();
				return true;
			}

			return false;
		}

		/// <summary>
		///     Returns true if the asset can be opened (edited) by the Unity Editor itself.
		/// </summary>
		/// <example>
		///     Example assets where this is true: materials, .unity (scene), .asset files, ..
		///     Where it is false: audio clips, scripts, reflection probes, ..
		/// </example>
		/// <returns>True if the editor can edit this asset type.</returns>
		/// <seealso cref="CodeSmile.Editor.Asset.OpenExternal" />
		public Boolean CanOpenInEditor() => File.CanOpenInEditor(m_MainObject);

		/// <summary>
		///     Opens the asset in the external (associated) application.
		/// </summary>
		/// <remarks>
		///     Optional line and column numbers can be specified for text files and applications that support this.
		/// </remarks>
		/// <param name="lineNumber">Line number to highlight. Support depends on application. Default: -1</param>
		/// <param name="columnNumber">Column/character number to highlight. Support depends on application. Default: -1</param>
		[ExcludeFromCodeCoverage] // cannot be tested
		public void OpenExternal(Int32 lineNumber = -1, Int32 columnNumber = -1) =>
			File.OpenExternal(m_MainObject, lineNumber, columnNumber);

		/// <summary>
		///     Deletes the asset file.
		/// </summary>
		/// <remarks>
		///     Does NOT destroy the object reference.
		///     CAUTION: The asset instance is no longer valid after this call and should be discarded.
		/// </remarks>
		/// <returns>
		///     If successful, returns the former MainObject. It is no longer an asset but still a valid instance.
		///     Returns null if the object wasn't deleted.
		/// </returns>
		/// <see cref="CodeSmile.Editor.Asset.Trash()" />
		public Object Delete()
		{
			var mainObject = m_MainObject;
			if (File.Delete(m_AssetPath))
				InvalidateInstance();

			return mainObject;
		}

		/// <summary>
		///     Moves the asset to the OS trash. Same as Delete, but recoverable.
		/// </summary>
		/// <remarks>
		///     Does NOT destroy the object reference.
		///     CAUTION: The asset instance is no longer valid after this call and should be discarded.
		/// </remarks>
		/// <returns>
		///     If successful, returns the former MainObject. It is no longer an asset but still a valid instance.
		///     Returns null if the object wasn't deleted.
		/// </returns>
		/// <see cref="CodeSmile.Editor.Asset.Delete()" />
		public Object Trash()
		{
			var mainObject = m_MainObject;
			if (File.Trash(m_AssetPath))
				InvalidateInstance();

			return mainObject;
		}

		/// <summary>
		///     Sets the active AssetImporter type for this asset.
		/// </summary>
		/// <remarks>T is AssetImporter in Unity 2022.1 or newer. In older versions T is ScriptedImporter.</remarks>
		/// <typeparam name="T">The AssetImporter derived type that should handle importing this asset.</typeparam>
		/// <see cref="CodeSmile.Editor.Asset.SetActiveImporterToDefault" />
		/// <see cref="CodeSmile.Editor.Asset.ActiveImporter" />
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
		///     Sets the active AssetImporter type back to the default type.
		/// </summary>
		/// <see cref="CodeSmile.Editor.Asset.SetActiveImporter{T}" />
		/// <see cref="CodeSmile.Editor.Asset.ActiveImporter" />
		public void SetActiveImporterToDefault()
		{
			if (Importer.IsOverridden(m_AssetPath))
				Importer.ClearOverride(m_AssetPath);
		}

		/// <summary>
		///     Sets the asset's labels, replacing all previously existing labels.
		/// </summary>
		/// <param name="labels">An array of labels. If null or empty will remove all labels.</param>
		/// <seealso cref="CodeSmile.Editor.Asset.AddLabel" />
		/// <seealso cref="CodeSmile.Editor.Asset.AddLabels" />
		/// <seealso cref="CodeSmile.Editor.Asset.ClearLabels" />
		public void SetLabels(String[] labels) => Label.SetAll(m_MainObject, labels);

		/// <summary>
		///     Removes all labels from the asset.
		/// </summary>
		/// <remarks>
		///     Same as <code>SetLabels(null)</code>
		/// </remarks>
		/// <seealso cref="CodeSmile.Editor.Asset.SetLabels" />
		public void ClearLabels() => Label.ClearAll(m_MainObject);

		/// <summary>
		///     Adds a label to the asset.
		/// </summary>
		/// <remarks>
		///     When setting multiple labels use CodeSmile.Editor.Asset.AddLabels or CodeSmile.Editor.Asset.SetLabels
		///     as this will be more efficient.
		/// </remarks>
		/// <param name="label">The label to add.</param>
		/// <seealso cref="CodeSmile.Editor.Asset.AddLabels" />
		/// <seealso cref="CodeSmile.Editor.Asset.SetLabels" />
		public void AddLabel(String label) => Label.Add(m_MainObject, label);

		/// <summary>
		///     Adds several labels to the asset.
		/// </summary>
		/// <param name="labels">An array of labels to add.</param>
		/// <seealso cref="CodeSmile.Editor.Asset.AddLabel" />
		/// <seealso cref="CodeSmile.Editor.Asset.SetLabels" />
		public void AddLabels(String[] labels) => Label.Add(m_MainObject, labels);

		/// <summary>
		///     Exports this asset and its dependencies as a .unitypackage.
		/// </summary>
		/// <param name="packagePath">
		///     Full path to a .unitypackage file. May point to any location on the file system
		///     as long as the user has write permissions there.
		/// </param>
		/// <param name="options">
		///     See
		///     <a href="https://docs.unity3d.com/ScriptReference/ExportPackageOptions.html">ExportPackageOptions</a>
		/// </param>
		public void ExportPackage(String packagePath, ExportPackageOptions options = ExportPackageOptions.Default) =>
			Package.Export(m_AssetPath, packagePath, options);

		/// <summary>
		///     Adds an object as a sub-object to the asset. The object must not already be an asset.
		/// </summary>
		/// <param name="subObject">The object instance to add as subobject to this asset.</param>
		/// <seealso cref="RemoveSubAsset" />
		/// <seealso cref="CodeSmile.Editor.Asset.SubAssets" />
		public void AddSubAsset(Object subObject) => SubAsset.Add(subObject, m_MainObject);

		/// <summary>
		///     Removes an object from the asset's sub-objects.
		/// </summary>
		/// <param name="subObject">The sub-asset object to remove.</param>
		/// <seealso cref="AddSubAsset" />
		/// <seealso cref="CodeSmile.Editor.Asset.SubAssets" />
		public void RemoveSubAsset(Object subObject) => SubAsset.Remove(subObject);

		private void InvalidateInstance()
		{
			m_AssetPath = null;
			m_MainObject = null;
		}

		private void SetAssetPathFromObject() => m_AssetPath = Path.Get(m_MainObject);

		private void InitWithPath(Path path)
		{
			ThrowIf.ArgumentIsNull(path, nameof(path));
			ThrowIf.DoesNotExistInFileSystem(path);

			m_AssetPath = path;
			m_MainObject = Status.IsImported(path) ? LoadMain<Object>() : File.ImportAndLoad<Object>(path);

			ThrowIf.AssetLoadReturnedNull(m_MainObject, m_AssetPath);
		}

		private void InitWithObject(Object obj)
		{
			ThrowIf.ArgumentIsNull(obj, nameof(obj));
			ThrowIf.NotInDatabase(obj);

			m_MainObject = obj;
			m_AssetPath = Path.Get(obj);
		}

		private void InitWithGuid(GUID guid)
		{
			ThrowIf.NotAnAssetGuid(guid);

			InitWithPath(Path.Get(guid));
		}
	}
}
