// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmileEditor
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

		/// <summary>
		///     Implicit conversion to UnityEngine.Object.
		/// </summary>
		/// <param name="asset">The main object of the asset.</param>
		/// <example>
		///     <code>
		///  Object obj = asset; // implicit conversion
		/// 		</code>
		/// </example>
		/// <returns>The asset's MainObject property.</returns>
		public static implicit operator Object(Asset asset) => asset != null ? asset.MainObject : null;

		/// <summary>
		///     Implicit conversion of UnityEngine.Object to an Asset.
		/// </summary>
		/// <remarks>Throws exception if obj is not an asset object.</remarks>
		/// <param name="asset">Existing asset reference.</param>
		/// <returns>An asset instance or null if obj is null.</returns>
		/// <example>
		///     <code>
		///  Asset asset = obj; // implicit conversion: Object to Asset
		/// 		</code>
		/// </example>
		public static implicit operator Asset(Object asset) => asset != null ? new Asset(asset) : null;

		/// <summary>
		///     Implicit conversion of Asset.Path to an Asset instance.
		/// </summary>
		/// <param name="path">Path to an asset file.</param>
		/// <returns>An asset instance or null if path is null.</returns>
		/// <example>
		///     <code>
		///  Asset asset = new Path("Assets/folder/file.asset");
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
		/// Asset asset = guid;
		/// </code>
		/// </example>
		public static implicit operator Asset(GUID guid) => guid.Empty() == false ? new Asset(guid) : null;

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
		/// <exception cref="System.ArgumentNullException">If contents is null.</exception>
		/// <exception cref="System.ArgumentNullException">If the path is null.</exception>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset(string,CodeSmileEditor.Asset.Path,bool)" />
		///     - <see cref="CodeSmileEditor.Asset(Object,CodeSmileEditor.Asset.Path,bool)" />
		/// </seealso>
		public Asset(Byte[] contents, Path path, Boolean overwriteExisting = false)
		{
			ThrowIf.ArgumentIsNull(contents, nameof(contents));
			ThrowIf.ArgumentIsNull(path, nameof(path));

			path = Path.UniquifyAsNeeded(path, overwriteExisting);
			var asset = File.CreateInternal(contents, path);
			InitWithMainObject(asset);
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
		/// <exception cref="ArgumentNullException">If contents is null.</exception>
		/// <exception cref="ArgumentNullException">If the path is null.</exception>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset(Byte[],CodeSmileEditor.Asset.Path,Boolean)" />
		///     - <see cref="CodeSmileEditor.Asset(Object,CodeSmileEditor.Asset.Path,Boolean)" />
		/// </seealso>
		public Asset(String contents, Path path, Boolean overwriteExisting = false)
		{
			ThrowIf.ArgumentIsNull(contents, nameof(contents));
			ThrowIf.ArgumentIsNull(path, nameof(path));

			path = Path.UniquifyAsNeeded(path, overwriteExisting);
			var asset = File.CreateInternal(contents, path);
			InitWithMainObject(asset);
		}

		/// <summary>
		///     Creates an asset file from an existing UnityEngine.Object instance.
		/// </summary>
		/// <remarks>
		///     The object must not already be an asset file (throws exception).
		/// </remarks>
		/// <param name="asset">The instance to create as an asset file.</param>
		/// <param name="path">Path where to save the new asset file, with extension.</param>
		/// <param name="overwriteExisting">
		///     If true, will overwrite any existing asset at path. Otherwise does not overwrite but generates a unique
		///     filename (default).
		/// </param>
		/// <exception cref="ArgumentNullException">If the object is null.</exception>
		/// <exception cref="ArgumentNullException">If the path is null.</exception>
		/// <exception cref="ArgumentException">If the object is already an asset on disk.</exception>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset(Byte[],CodeSmileEditor.Asset.Path,Boolean)" />
		///     - <see cref="CodeSmileEditor.Asset(String,CodeSmileEditor.Asset.Path,Boolean)" />
		/// </seealso>
		public Asset(Object asset, Path path, Boolean overwriteExisting = false)
		{
			ThrowIf.ArgumentIsNull(asset, nameof(asset));
			ThrowIf.ArgumentIsNull(path, nameof(path));
			ThrowIf.AlreadyAnAsset(asset);

			path = Path.UniquifyAsNeeded(path, overwriteExisting);
			File.CreateInternal(asset, path);
			InitWithMainObject(asset);
		}

		/// <summary>
		///     Loads the asset at path.
		/// </summary>
		/// <param name="path">Path to an existing asset.</param>
		/// <exception cref="ArgumentNullException">If the path is null.</exception>
		/// <exception cref="FileNotFoundException">If the path is not an asset on disk.</exception>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset(Object)" />
		///     - <see cref="CodeSmileEditor.Asset(GUID)" />
		/// </seealso>
		public Asset(Path path) => InitWithPath(path);

		/// <summary>
		///     Loads the asset using its GUID.
		/// </summary>
		/// <param name="assetGuid">GUID of an asset.</param>
		/// <exception cref="ArgumentException">If the GUID is not in the AssetDatabase (not an asset).</exception>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset(Object)" />
		///     - <see cref="CodeSmileEditor.Asset(CodeSmileEditor.Asset.Path)" />
		/// </seealso>
		public Asset(GUID assetGuid) => InitWithGuid(assetGuid);

		/// <summary>
		///     Uses an existing asset reference.
		/// </summary>
		/// <param name="asset">Instance of an asset.</param>
		/// <exception cref="ArgumentNullException">If the object is null.</exception>
		/// <exception cref="ArgumentException">If the object is not an asset on disk.</exception>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset(CodeSmileEditor.Asset.Path)" />
		///     - <see cref="CodeSmileEditor.Asset(GUID)" />
		/// </seealso>
		public Asset(Object asset) => InitWithMainObject(asset);

		/// <summary>
		///     Gets the main object cast to T.
		/// </summary>
		/// <remarks>
		///     This is an alias for:
		///     <code>var obj = asset.MainObject as T;</code>
		/// </remarks>
		/// <typeparam name="T">Type derived from UnityEngine.Object.</typeparam>
		/// <returns>Returns MainObject cast to T or null if main object is not of type T.</returns>
		public T GetMain<T>() where T : Object => m_MainObject as T;

		/// <summary>
		///     Saves any changes to the asset to disk.
		/// </summary>
		/// <remarks>
		///     Not every change marks an object as 'dirty'. In such cases you need to use
		///     CodeSmileEditor.Asset.ForceSave().
		/// </remarks>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.ForceSave()" />
		/// </seealso>
		public void Save() => File.Save(m_MainObject);

		/// <summary>
		///     Saves the asset to disk, regardless of whether it is marked as 'dirty'.
		/// </summary>
		/// <remarks>
		///     Force saving is achieved by flagging the object as dirty with
		///     <a href="https://docs.unity3d.com/ScriptReference/EditorUtility.SetDirty.html">EditorUtility.SetDirty()</a>.
		/// </remarks>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.Save()" />
		/// </seealso>
		public void ForceSave() => File.ForceSave(m_MainObject);

		/// <summary>
		///     Saves a copy of the asset to a new path. Overwrites any existing asset at path.
		/// </summary>
		/// <remarks>
		///     Will automatically create missing folders.
		/// </remarks>
		/// <param name="path">The path where to save the copy.</param>
		/// <returns>
		///     The copy of the Asset or null if copying failed. Use CodeSmileEditor.Asset.GetLastErrorMessage to get the
		///     human readable error message.
		/// </returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.Save" />
		///     - <see cref="CodeSmileEditor.Asset.SaveAsNew" />
		///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
		/// </seealso>
		public Asset SaveAs(Path path) => File.Copy(m_AssetPath, path) ? new Asset(path) : null;

		/// <summary>
		///     Saves a copy of the asset to a new path. Generates a unique file/folder name if path already exists.
		/// </summary>
		/// <remarks>
		///     Will automatically create missing folders.
		/// </remarks>
		/// <param name="path">The path where to save the copy. Note that actual path of the asset may change.</param>
		/// <returns>
		///     The copy of the Asset or null if copying failed. Use CodeSmileEditor.Asset.GetLastErrorMessage to get the
		///     human readable error message.
		/// </returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.Save" />
		///     - <see cref="CodeSmileEditor.Asset.SaveAs" />
		///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
		/// </seealso>
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
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.ForceSave" />
		/// </seealso>
		public void SetDirty() => EditorUtility.SetDirty(m_MainObject);

		// NOTE: there is no public Import() method needed since the main object is guaranteed to be imported
		[ExcludeFromCodeCoverage] // private, not used
		private void Import() {}

		// Private on purpose: the main object is automatically loaded when instantiating an Asset class.
		private T LoadMain<T>() where T : Object => m_AssetPath != null ? (T)(m_MainObject = File.Load<T>(m_AssetPath)) : null;

		/// <summary>
		///     Loads a (sub) object from the asset identified by type.
		/// </summary>
		/// <remarks>
		///     To load the main object of the Asset instance use the CodeSmileEditor.Asset.MainObject property.
		/// </remarks>
		/// <typeparam name="T">UnityEngine.Object derived type.</typeparam>
		/// <returns>Returns the 'first' asset of the type found.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.SubAssets" />
		///     - <see cref="CodeSmileEditor.Asset.VisibleSubAssets" />
		///     - <see cref="CodeSmileEditor.Asset.MainObject" />
		/// </seealso>
		public T Load<T>() where T : Object => File.Load<T>(m_AssetPath);

		/// <summary>
		///     Tests if a Move operation will be successful without actually moving the asset.
		/// </summary>
		/// <remarks>
		///     Returns false if one or more folders in destinationPath do not exist.
		///     On failure, use CodeSmileEditor.Asset.GetLastErrorMessage to get the failure error message.
		/// </remarks>
		/// <param name="destinationPath">The path where to move the asset to. May have a different extension.</param>
		/// <returns>True if moving the asset will be successful, false otherwise.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.Move" />
		///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
		/// </seealso>
		public Boolean CanMove(Path destinationPath) => File.CanMove(m_AssetPath, destinationPath);

		/// <summary>
		///     Moves asset to destination path.
		/// </summary>
		/// <remarks>
		///     Missing folders in destination path will be created automatically.
		///     After the move, the CodeSmileEditor.Asset.AssetPath property is updated accordingly.
		///     On failure, use CodeSmileEditor.Asset.GetLastErrorMessage to get the failure error message.
		/// </remarks>
		/// <param name="destinationPath">The path where to move the asset to. May have a different extension.</param>
		/// <returns>True if moving the asset will be successful, false otherwise.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.CanMove" />
		///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
		/// </seealso>
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
		///     Use CodeSmileEditor.Asset.Move if you need to change the file's extension.
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
		///     If false, CodeSmileEditor.Asset.GetLastErrorMessage provides a human-readable failure reason and
		///     the AssetPath property remains unchanged.
		/// </returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.Move" />
		///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
		/// </seealso>
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
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.OpenExternal" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
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
		public void OpenExternal(Int32 lineNumber = -1, Int32 columnNumber = -1) => File.OpenExternal(m_MainObject, lineNumber, columnNumber);

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
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.Trash" />
		/// </seealso>
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
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.Delete" />
		/// </seealso>
		public Object Trash()
		{
			var mainObject = m_MainObject;
			if (File.Trash(m_AssetPath))
				InvalidateInstance();

			return mainObject;
		}

		/// <summary>
		///     Sets the asset's labels, replacing all previously existing labels.
		/// </summary>
		/// <param name="labels">An array of labels.</param>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.AddLabel" />
		///     - <see cref="CodeSmileEditor.Asset.AddLabels" />
		///     - <see cref="CodeSmileEditor.Asset.ClearLabels" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public void SetLabels(String[] labels) => Label.SetAll(m_MainObject, labels);

		/// <summary>
		///     Removes all labels from the asset.
		/// </summary>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.RemoveLabel" />
		///     - <see cref="CodeSmileEditor.Asset.AddLabel" />
		///     - <see cref="CodeSmileEditor.Asset.AddLabels" />
		///     - <see cref="CodeSmileEditor.Asset.SetLabels" />
		/// </seealso>
		public void ClearLabels() => Label.ClearAll(m_MainObject);

		/// <summary>
		///     Removes a label from an asset. Does nothing if the label doesn't exist.
		/// </summary>
		/// <param name="label">Label to remove.</param>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.ClearLabels" />
		/// </seealso>
		public void RemoveLabel(String label) => Label.Remove(m_MainObject, label);

		/// <summary>
		///     Adds a label to the asset.
		/// </summary>
		/// <remarks>
		///     When setting multiple labels use CodeSmileEditor.Asset.AddLabels or CodeSmileEditor.Asset.SetLabels
		///     as this will be more efficient.
		/// </remarks>
		/// <param name="label">The label to add.</param>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.AddLabels" />
		///     - <see cref="CodeSmileEditor.Asset.SetLabels" />
		/// </seealso>
		public void AddLabel(String label) => Label.Add(m_MainObject, label);

		/// <summary>
		///     Adds several labels to the asset.
		/// </summary>
		/// <param name="labels">An array of labels to add.</param>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.AddLabel" />
		///     - <see cref="CodeSmileEditor.Asset.SetLabels" />
		/// </seealso>
		public void AddLabels(String[] labels) => Label.Add(m_MainObject, labels);

		/// <summary>
		///     Exports this asset and its dependencies as a .unitypackage.
		/// </summary>
		/// <param name="packagePath">
		///     Full path to a .unitypackage file. May point to any location on the file system
		///     as long as the user has write permissions there.
		/// </param>
		/// <param name="options">
		///     <a href="https://docs.unity3d.com/ScriptReference/ExportPackageOptions.html">ExportPackageOptions</a>
		/// </param>
		[ExcludeFromCodeCoverage] // simple relay
		public void ExportPackage(String packagePath, ExportPackageOptions options = ExportPackageOptions.Default) =>
			Package.Export(m_AssetPath, packagePath, options);

		/// <summary>
		///     Adds an object as a sub-object to the asset. The object must not already be an asset.
		/// </summary>
		/// <remarks>This implicitly saves the change to disk - you do NOT need to call Save() afterwards.</remarks>
		/// <param name="instance">The object instance to add as subobject to this asset.</param>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.RemoveSubAsset" />
		///     - <see cref="CodeSmileEditor.Asset.SubAssets" />
		/// </seealso>
		public void AddSubAsset(Object instance) => SubAsset.Add(instance, m_MainObject);

		/// <summary>
		///     Removes an object from the asset's sub-objects.
		/// </summary>
		/// <param name="subAsset">The sub-asset object to remove.</param>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.AddSubAsset" />
		///     - <see cref="CodeSmileEditor.Asset.SubAssets" />
		/// </seealso>
		public void RemoveSubAsset(Object subAsset) => SubAsset.Remove(subAsset);

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

		private void InitWithMainObject(Object mainObject)
		{
			ThrowIf.ArgumentIsNull(mainObject, nameof(mainObject));
			ThrowIf.NotInDatabase(mainObject);

			m_MainObject = mainObject;
			m_AssetPath = Path.Get(mainObject);
		}

		private void InitWithGuid(GUID guid)
		{
			ThrowIf.NotAnAssetGuid(guid);

			InitWithPath(Path.Get(guid));
		}
	}
}
