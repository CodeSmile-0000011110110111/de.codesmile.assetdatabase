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
	///     In addition Asset.Path takes care of all path (folder and file) handling business and ensures
	///     properly formed paths.
	///     Static subclasses group only marginally related AssetDatabase functionality such as Package, Bundle,
	///     VersionControl, CacheServer.
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
		///     Returns the icon texture associated with the asset type.
		/// </summary>
		[ExcludeFromCodeCoverage] public Texture Icon => GetIcon(m_AssetPath);

		/// <summary>
		///     Returns the local FileID of the object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>The local fileID or 0 on failure.</returns>
		[ExcludeFromCodeCoverage] public static Int64 GetLocalFileId(Object obj)
		{
			// variable is to force Rider to not clean this up to 'var' because Unity 2021 has both long and int
			// variants of the TryGetGUID* method and thus cause compile error due to 'call is ambiguous between'
			var localId = Int64.MinValue;

			return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var _, out localId) ? localId : 0L;
		}

		/// <summary>
		///     Returns the icon associated with the asset type.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage] public static Texture GetIcon(Path path) => AssetDatabase.GetCachedIcon(path);

		/// <summary>
		///     Returns the type of the main asset at the path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>the type of the asset or null if the path does not exist</returns>
		public static Type GetMainType(Path path) => AssetDatabase.GetMainAssetTypeAtPath(path);

		/// <summary>
		///     Returns MainObject cast to T, or null. But recommended usage is:
		///     <p>
		///         MyType t = asset as MyType;
		///     </p>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[ExcludeFromCodeCoverage] public T Get<T>() where T : Object => m_MainObject as T;
	}
}
