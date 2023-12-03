// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmileEditor
{
	public sealed partial class Asset
	{
		private static String s_LastErrorMessage = String.Empty;

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

		/// <summary>
		///     Returns the icon associated with the asset type.
		/// </summary>
		/// <param name="path">Path to an asset.</param>
		/// <returns>The icon texture cast as Texture2D, or null.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.GetIcon(Object)" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public static Texture2D GetIcon([NotNull] Path path) => AssetDatabase.GetCachedIcon(path) as Texture2D;

		/// <summary>
		///     Returns the icon associated with the asset type.
		/// </summary>
		/// <param name="asset">The object for which to get the icon.</param>
		/// <returns>The object's icon texture or null. If the obj is a sub-asset then the main asset's icon is returned.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.GetIcon(CodeSmileEditor.Asset.Path)" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public static Texture2D GetIcon([NotNull] Object asset) => GetIcon(Path.Get(asset));

		/// <summary>
		///     Returns the last error message returned by some methods that provide such a failure message.
		/// </summary>
		/// <returns>The last error message or empty string if the last operation succeeded.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmileEditor.Asset.File.Rename" />
		///     - <see cref="CodeSmileEditor.Asset.File.Move" />
		///     - <see cref="CodeSmileEditor.Asset.File.Copy" />
		///     - <see cref="CodeSmileEditor.Asset.File.CopyAsNew" />
		///     - <see cref="CodeSmileEditor.Asset.Path.IsValid" />
		///     - <see cref="CodeSmileEditor.Asset.VersionControl.IsEditable" />
		///     - <see cref="CodeSmileEditor.Asset.VersionControl.IsMetaEditable" />
		///     - <see cref="CodeSmileEditor.Asset.VersionControl.CanMakeEditable" />
		/// </seealso>
		public static String GetLastErrorMessage() => s_LastErrorMessage;

		private static void SetLastErrorMessage(String message) =>
			s_LastErrorMessage = message != null ? message : String.Empty;

		private static Boolean Succeeded(String possibleErrorMessage)
		{
			SetLastErrorMessage(possibleErrorMessage);
			return String.IsNullOrEmpty(GetLastErrorMessage());
		}
	}
}
