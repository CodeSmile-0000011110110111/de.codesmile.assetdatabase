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
