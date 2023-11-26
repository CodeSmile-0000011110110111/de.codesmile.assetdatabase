// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
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
		///     Gets or sets the asset's main object.
		///     CodeSmile.Editor.Asset.GetMainType
		/// </summary>
		/// <example>
		///     To cast the main object to a specific type you may simply cast the asset:
		///     <code>var myObj = (MyType)asset;</code>
		///     Is short for:
		///     <code>var myObj = (MyType)asset.MainObject;</code>
		///     The same works with the 'as' operator:
		///     <code>var myObj = asset as MyType;</code>
		///     Is short for:
		///     <code>var myObj = asset.MainObject as MyType;</code>
		///     Lastly you can also use the generic getter:
		///     <code>var myObj = asset.Get&lt;MyType&gt;();</code>
		/// </example>
		/// <seealso cref="">
		///     <list type="">
		///         <item>
		///             <see cref="CodeSmile.Editor.Asset.SubAsset" />
		///         </item>
		///         <item>
		///             <see cref="CodeSmile.Editor.Asset.SubAsset.SetMain(UnityEngine.Object,CodeSmile.Editor.Asset.Path)" />
		///         </item>
		///         <item>
		///             <see cref="CodeSmile.Editor.Asset.SubAsset.SetMain(UnityEngine.Object,UnityEngine.Object)" />
		///         </item>
		///         <item>
		///             <see cref="CodeSmile.Editor.Asset.File.LoadMain{T}(CodeSmile.Editor.Asset.Path)" />
		///         </item>
		///     </list>
		/// </seealso>
		public Object MainObject
		{
			// This 'loads' the asset but most of the time simply returns the internally cached instance.
			// We need to load the instance because the user may have called static SubAsset.SetMain().
			get => m_MainObject = LoadMain<Object>();
			set
			{
				SubAsset.SetMain(value, m_AssetPath);
				m_MainObject = value;
			}
		}

		/// <summary>
		///     Returns the type of the main asset at the given path.
		/// </summary>
		/// <seealso cref="GetMainType(CodeSmile.Editor.Asset.Path)" />
		public Type MainObjectType => GetMainType(m_AssetPath);

		/// <summary>
		///     Returns the path to the asset (file or folder).
		/// </summary>
		/// <seealso cref="MetaPath" />
		public Path AssetPath => m_AssetPath;

		/// <summary>
		///     Returns the path to the .meta file for the asset.
		/// </summary>
		/// <seealso cref="AssetPath" />
		/// <seealso cref="Path.ToMeta(Path)" />
		public Path MetaPath => Path.ToMeta(m_AssetPath);

		/// <summary>
		///     Returns the asset's GUID.
		/// </summary>
		/// <seealso cref="FileId" />
		/// <seealso cref="Path.GetGuid(Path)" />
		public GUID Guid => Path.GetGuid(m_AssetPath);

		/// <summary>
		///     Returns the local FileID of the asset.
		/// </summary>
		/// <see cref="Guid" />
		public Int64 FileId => GetFileId(m_MainObject);

		/// <summary>
		///     Returns the icon texture associated with the asset type.
		/// </summary>
		public Texture2D Icon => GetIcon(m_AssetPath);

		/// <summary>
		///     Returns the type of the main asset at the path.
		/// </summary>
		/// <param name="path">Path to an asset.</param>
		/// <returns>Type of the asset. Null if the path does not exist.</returns>
		/// <seealso cref="">
		///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetMainAssetTypeAtPath.html">AssetDatabase.GetMainAssetTypeAtPath</a>
		/// </seealso>
		public static Type GetMainType(Path path) => AssetDatabase.GetMainAssetTypeAtPath(path);

		/// <summary>
		///     Returns the type of the main asset for the GUID.
		/// </summary>
		/// <param name="guid">Guid of an asset.</param>
		/// <returns>Type of the asset. Null if the guid is not known or not an asset.</returns>
		/// <seealso cref="">
		///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetMainAssetTypeFromGUID.html">AssetDatabase.GetMainAssetTypeFromGUID</a>
		/// </seealso>
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
		///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetTypeFromPathAndFileID.html">AssetDatabase.GetTypeFromPathAndFileID</a>
		/// </seealso>
		public static Type GetSubType(Path path, Int64 fileId) => AssetDatabase.GetTypeFromPathAndFileID(path, fileId);

		/// <example>
		///     Example usage:
		///     <code>
		/// var (guid, fileId) = Asset.GetGuidAndFileId(obj);
		/// </code>
		/// </example>
		/// <param name="obj">Object from which GUID and FileId should be obtained.</param>
		/// <seealso cref="">
		///     <list type="">
		///         <item>
		///             <see cref="CodeSmile.Editor.Asset.GetGuid" />
		///         </item>
		///         <item>
		///             <see cref="CodeSmile.Editor.Asset.GetFileId" />
		///         </item>
		///         <item>
		///             <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.TryGetGUIDAndLocalFileIdentifier.html">AssetDatabase.TryGetGUIDAndLocalFileIdentifier</a>
		///         </item>
		///     </list>
		/// </seealso>
		/// <returns>
		///     The GUID and local File ID of the object. Returns an empty GUID and 0 if obj is null or not an asset.
		/// </returns>
		// ValueTuple makes doxygen accept it as documented, see: https://github.com/doxygen/doxygen/issues/9618
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
		///     Returns the GUID of an object. Returns an empty GUID if the object is null or not an asset.
		/// </summary>
		/// <param name="obj"></param>
		/// <seealso cref="">
		///     <list type="">
		///         <item>
		///             <see cref="CodeSmile.Editor.Asset.GetFileId" />
		///         </item>
		///         <item>
		///             <see cref="CodeSmile.Editor.Asset.GetGuidAndFileId" />
		///         </item>
		///         <item>
		///             <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.TryGetGUIDAndLocalFileIdentifier.html">AssetDatabase.TryGetGUIDAndLocalFileIdentifier</a>
		///         </item>
		///     </list>
		/// </seealso>
		/// <returns></returns>
		public static GUID GetGuid(Object obj)
		{
			if (obj == null)
				return new GUID();

			// explicit variable + assign because Unity 2021 has both long and int variants of the TryGetGUID* method
			var localId = Int64.MaxValue;
			return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guid, out localId)
				? new GUID(guid)
				: new GUID();
		}

		/// <summary>
		///     Returns the local FileID of the object.
		/// </summary>
		/// <param name="obj"></param>
		/// <seealso cref="">
		///     <list type="">
		///         <item>
		///             <see cref="CodeSmile.Editor.Asset.GetGuid" />
		///         </item>
		///         <item>
		///             <see cref="CodeSmile.Editor.Asset.GetGuidAndFileId" />
		///         </item>
		///         <item>
		///             <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.TryGetGUIDAndLocalFileIdentifier.html">AssetDatabase.TryGetGUIDAndLocalFileIdentifier</a>
		///         </item>
		///     </list>
		/// </seealso>
		/// <returns>The local fileID or 0 if obj is null or not an asset.</returns>
		public static Int64 GetFileId(Object obj)
		{
			if (obj == null)
				return 0L;

			// explicit variable + assign because Unity 2021 has both long and int variants of the TryGetGUID* method
			var localId = Int64.MaxValue;
			return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var _, out localId) ? localId : 0L;
		}

		/// <summary>
		///     Returns the icon associated with the asset type.
		/// </summary>
		/// <param name="path">Path to an asset.</param>
		/// <returns>The icon texture cast as Texture2D, or null.</returns>
		public static Texture2D GetIcon(Path path) => AssetDatabase.GetCachedIcon(path) as Texture2D;

		/// <summary>
		///     Returns the icon associated with the asset type.
		/// </summary>
		/// <param name="obj">The object for which to get the icon.</param>
		/// <returns>The object's icon texture or null. If the obj is a sub-asset then the main asset's icon is returned.</returns>
		public static Texture2D GetIcon(Object obj) => GetIcon(Path.Get(obj));

		/// <remarks>
		///     The alternative is to cast the asset instance:
		///     <code>
		/// var obj = (T)asset;
		/// </code>
		/// </remarks>
		/// <typeparam name="T"></typeparam>
		/// <returns>Returns MainObject cast to T or null if main object is not of type T.</returns>
		public T Get<T>() where T : Object => m_MainObject as T;
	}
}
