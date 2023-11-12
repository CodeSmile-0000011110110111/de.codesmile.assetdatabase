﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		private static String s_LastErrorMessage = String.Empty;

		private Path m_AssetPath;
		private Object m_MainObject;

		/// <summary>
		///     Returns the asset's main object.
		/// </summary>
		public Object MainObject
		{
			get => m_MainObject = LoadMain<Object>();
			set
			{
				SetMainObject(value, m_AssetPath);
				m_MainObject = value;
			}
		}

		/// <summary>
		///     Loads and returns all sub objects the asset is comprised of.
		///     NOTE: Whether the main object is included in this list depends on the type of asset.
		/// </summary>
		public Object[] SubObjects => LoadSubObjects(m_AssetPath);

		/// <summary>
		///     Loads and returns only those asset objects that are shown in the project view.
		///     NOTE: Does NOT include the main asset!
		/// </summary>
		public Object[] VisibleSubObjects => LoadSubObjects(m_AssetPath, true);

		/// <summary>
		///     Returns the type of the main asset.
		/// </summary>
		[ExcludeFromCodeCoverage] public Type MainType => GetMainType(m_AssetPath);

		/// <summary>
		///     Returns the path to the asset (file or folder).
		/// </summary>
		public Path AssetPath => m_AssetPath;

		/// <summary>
		///     Returns the asset's GUID.
		/// </summary>
		public GUID Guid => Path.GetGuid(m_AssetPath);

		/// <summary>
		///     Returns the local FileID of the asset.
		/// </summary>
		[ExcludeFromCodeCoverage] public Int64 LocalFileId => GetLocalFileId(m_MainObject);

		/// <summary>
		///     Sets or gets the labels associated with the asset.
		/// </summary>
		public String[] Labels
		{
			get => GetLabels(m_MainObject);
			set => SetLabels(m_MainObject, value);
		}

		/// <summary>
		///     Returns true after the asset has been deleted.
		///     <p>
		///         <see cref="Delete(CodeSmile.Editor.Asset.Path)" /> - <see cref="Trash(CodeSmile.Editor.Asset.Path)" />
		///     </p>
		/// </summary>
		public Boolean IsDeleted => m_AssetPath == null && m_MainObject == null;

		/// <summary>
		///     Returns whether this is a foreign asset.
		/// </summary>
		/// <see cref="IsForeign" />
		/// <see cref="IsNative" />
		/// <returns></returns>
		[ExcludeFromCodeCoverage] public Boolean IsForeignAsset => IsForeign(m_MainObject);
		/// <summary>
		///     Returns whether this is a native asset.
		/// </summary>
		/// <see cref="IsNative" />
		/// <see cref="IsForeign" />
		/// <returns></returns>
		[ExcludeFromCodeCoverage] public Boolean IsNativeAsset => IsNative(m_MainObject);

		/// <summary>
		///     Returns the icon texture associated with the asset type.
		/// </summary>
		[ExcludeFromCodeCoverage] public Texture Icon => GetIcon(m_AssetPath);

		/// <summary>
		///     Returns the assets dependencies recursively. Returns paths to the dependent assets.
		/// </summary>
		public String[] Dependencies => GetDependencies(m_AssetPath, true);

		/// <summary>
		///     Implicit conversion to UnityEngine.Object by returning the asset's MainObject.
		/// </summary>
		/// <param name="asset">The main object of the asset.</param>
		public static implicit operator Object(Asset asset) => asset != null ? asset.MainObject : null;

		/// <summary>
		///     Implicit conversion of UnityEngine.Object to an asset instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>An asset instance or null if obj is null.</returns>
		public static implicit operator Asset(Object obj) => obj != null ? new Asset(obj) : null;

		/// <summary>
		///     Implicit conversion of Asset.Path to an asset instance.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>An asset instance or null if path is null.</returns>
		public static implicit operator Asset(Path path) => path != null ? new Asset(path) : null;

		/// <summary>
		///     Implicit conversion of GUID to an asset instance.
		/// </summary>
		/// <param name="guid">An asset instance.</param>
		/// <returns>An asset instance or null if guid is empty.</returns>
		public static implicit operator Asset(GUID guid) => guid.Empty() == false ? new Asset(guid) : null;

		/// <summary>
		///     Returns the last error message from some file operations that return a Boolean,
		///     for example: Move, Rename, Copy
		/// </summary>
		public String LastErrorMessage => GetLastErrorMessage();

		/// <summary>
		///     Returns the local FileID of the object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>The local fileID or 0 on failure.</returns>
		[ExcludeFromCodeCoverage] public static Int64 GetLocalFileId(Object obj) =>
			AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var _, out var localId) ? localId : 0;

		/// <summary>
		///     Checks if the object is an asset in the AssetDatabase. If it isn't but you know
		///     the asset file exists then you need to Import() the asset.
		///     Unlike AssetDatabase, will not throw a NullRef if you pass null.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>Returns false if the object isn't in the database or if the object is null.</returns>
		public static Boolean IsImported(Object obj) => obj ? AssetDatabase.Contains(obj) : false;

		/// <summary>
		///     Returns the icon associated with the asset type.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage] public static Texture GetIcon(Path path) => AssetDatabase.GetCachedIcon(path);

		/// <summary>
		///     Returns whether this object is the asset's 'main' object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage] public static Boolean IsMain(Object obj) => AssetDatabase.IsMainAsset(obj);

		/// <summary>
		///     Returns whether this object is a sub-asset of a composite asset. For example an Animation inside an FBX file.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage] public static Boolean IsSub(Object obj) => AssetDatabase.IsSubAsset(obj);

		/// <summary>
		///     <p>
		///         Returns whether this is a foreign asset.
		///     </p>
		///     <p>
		///         A foreign asset is any type of file that Unity doesn't use
		///         directly but rather maintains cached versions of it in the Library folder. For example, a .png image
		///         is a foreign asset, there is no editor inside Unity for it, and the representation of the .png depends
		///         on the asset's settings and build platform (eg compression, max size, etc).
		///         Other foreign assets: scenes (.unity), prefabs, assembly definitions.
		///     </p>
		/// </summary>
		/// <see cref="IsNative" />
		/// <param name="obj"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage] public static Boolean IsForeign(Object obj) => AssetDatabase.IsForeignAsset(obj);

		/// <summary>
		///     Returns whether this is a native asset. Native assets are serialized directly by Unity, such as materials.
		///     Note that scenes, prefabs and assembly definitions are considered foreign assets.
		/// </summary>
		/// <see cref="IsForeign" />
		/// <param name="obj"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage] public static Boolean IsNative(Object obj) => AssetDatabase.IsNativeAsset(obj);

		/// <summary>
		///     Returns whether this object's main asset is loaded.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public static Boolean IsLoaded(Object obj) => AssetDatabase.IsMainAssetAtPathLoaded(Path.Get(obj));

		/// <summary>
		///     Returns whether this path's main asset is loaded.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public static Boolean IsLoaded(Path path) => AssetDatabase.IsMainAssetAtPathLoaded(path);

		/// <summary>
		///     Returns the type of the main asset at the path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>the type of the asset or null if the path does not exist</returns>
		public static Type GetMainType(Path path) => AssetDatabase.GetMainAssetTypeAtPath(path);

		/// <summary>
		///     Returns the dependencies of the asset at the given path. Returns paths to dependent assets.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="recursive"></param>
		/// <returns></returns>
		public static String[] GetDependencies(Path path, Boolean recursive = false) =>
			AssetDatabase.GetDependencies(path, recursive);

		/// <summary>
		///     Returns the dependencies of the assets at the given paths. Returns paths to dependent assets.
		/// </summary>
		/// <param name="paths"></param>
		/// <param name="recursive"></param>
		/// <returns></returns>
		public static String[] GetDependencies(Path[] paths, Boolean recursive = false) =>
			AssetDatabase.GetDependencies(paths.Cast<String>().ToArray(), recursive);

		/// <summary>
		///     Returns the last error message returned by some methods that provide such a message,
		///     for example Move and Rename.
		///     <see cref="Rename" />
		///     <see cref="Move" />
		/// </summary>
		public static String GetLastErrorMessage() => s_LastErrorMessage;

		private static void SetLastErrorMessage(String message) =>
			s_LastErrorMessage = message != null ? message : String.Empty;

		private static Boolean Succeeded(String possibleErrorMessage)
		{
			SetLastErrorMessage(possibleErrorMessage);
			return String.IsNullOrEmpty(GetLastErrorMessage());
		}

		/// <summary>
		///     Returns MainObject cast to T, or null. But recommended usage is:
		///     <p>
		///         MyType t = asset as MyType;
		///     </p>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[ExcludeFromCodeCoverage] public T Get<T>() where T : Object => m_MainObject as T;

		private void InvalidateInstance()
		{
			m_AssetPath = null;
			m_MainObject = null;
			//m_AssetObjects = null;
		}
	}
}
