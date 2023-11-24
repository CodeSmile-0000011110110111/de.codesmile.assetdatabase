// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
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

		/// <summary>
		///     Returns the asset's main object.
		/// </summary>
		public Object MainObject
		{
			get => m_MainObject = LoadMain<Object>();
			set
			{
				SubAsset.SetMain(value, m_AssetPath);
				m_MainObject = value;
			}
		}

		/// <summary>
		///     Returns the type of the main asset.
		/// </summary>
		public Type MainObjectType => GetMainType(m_AssetPath);

		/// <summary>
		///     Returns the path to the asset (file or folder).
		/// </summary>
		/// <see cref="MetaPath" />
		public Path AssetPath => m_AssetPath;

		/// <summary>
		///     Returns the path to the .meta file for the asset.
		/// </summary>
		/// <see cref="AssetPath" />
		public Path MetaPath => Path.ToMeta(m_AssetPath);

		/// <summary>
		///     Returns the asset's GUID.
		/// </summary>
		public GUID Guid => Path.GetGuid(m_AssetPath);

		/// <summary>
		///     Returns the local FileID of the asset.
		/// </summary>
		public Int64 LocalFileId => GetFileId(m_MainObject);

		/// <summary>
		///     Returns the icon texture associated with the asset type.
		/// </summary>
		public Texture Icon => GetIcon(m_AssetPath);

		/// <summary>
		///     Returns the local FileID of the object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>The local fileID or 0 if obj is null or not an asset.</returns>
		/// <see cref="GetGuid" />
		/// <see cref="GetGuidAndFileId" />
		public static Int64 GetFileId(Object obj)
		{
			if (obj == null)
				return 0L;

			// explicit variable + assign because Unity 2021 has both long and int variants of the TryGetGUID* method
			var localId = Int64.MaxValue;
			return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var _, out localId) ? localId : 0L;
		}

		/// <summary>
		///     Returns the GUID of an object. Returns an empty GUID if the object is null or not an asset.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <see cref="GetGuidAndFileId" />
		/// <see cref="GetFileId" />
		public static GUID GetGuid(Object obj)
		{
			if (obj == null)
				return new GUID();

			return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guid, out _)
				? new GUID(guid)
				: new GUID();
		}

		/// <summary>
		///     Returns both GUID and local File ID of the object. Returns an empty GUID and 0L if the object is null
		///     or not an asset.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <see cref="GetGuid" />
		/// <see cref="GetFileId" />

		// Use of ValueTuple helps doxygen pick up this method as documented
		// See this issue: https://github.com/doxygen/doxygen/issues/9618
		public static ValueTuple<GUID, Int64> GetGuidAndFileId(Object obj)
		{
			if (obj == null)
				return (new GUID(), 0L);

			// explicit variable + assign because Unity 2021 has both long and int variants of the TryGetGUID* method
			var localId = Int64.MaxValue;
			return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guid, out localId)
				? (new GUID(guid), localId)
				: (new GUID(), 0L);
		}

		/// <summary>
		///     Returns the icon associated with the asset type.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Texture GetIcon(Path path) => AssetDatabase.GetCachedIcon(path);

		/// <summary>
		///     Returns the icon associated with the asset type.
		///     Note: this will not return icons for sub-assets. It will only return the main asset's icon.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static Texture GetIcon(Object obj) => GetIcon(Path.Get(obj));

		/// <summary>
		///     Returns the type of the main asset at the path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>the type of the asset or null if the path does not exist</returns>
		public static Type GetMainType(Path path) => AssetDatabase.GetMainAssetTypeAtPath(path);

		/// <summary>
		///     Returns the type of the main asset for the GUID.
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static Type GetMainType(GUID guid) => AssetDatabase.GetMainAssetTypeFromGUID(guid);

		/// <summary>
		///     Gets the type of a sub asset by the main asset's path and the local file ID of the sub-asset.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="fileId"></param>
		/// <returns></returns>
		public static Type GetSubType(Path path, Int64 fileId) => AssetDatabase.GetTypeFromPathAndFileID(path, fileId);

		/// <summary>
		///     Returns MainObject cast to T, or null. But recommended usage is:
		///     <p>
		///         MyType t = asset as MyType;
		///     </p>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Get<T>() where T : Object => m_MainObject as T;
	}
}
