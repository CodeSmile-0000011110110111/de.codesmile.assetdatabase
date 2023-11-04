// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.Editor
{
	public partial class AssetPath
	{
		/// <summary>
		///     Implicit conversion to string (relative asset path). Same as ToString().
		/// </summary>
		public static implicit operator String(AssetPath assetPath) =>
			assetPath != null ? assetPath.m_RelativePath : null;

		/// <summary>
		///     Explicit creation of an AssetPath instance initialized with a string path (full or relative).
		/// </summary>
		/// <param name="fullOrRelativePath"></param>
		/// <returns></returns>
		public static explicit operator AssetPath(String fullOrRelativePath) =>
			fullOrRelativePath != null ? new AssetPath(fullOrRelativePath) : null;

		public static Boolean operator ==(AssetPath ap1, AssetPath ap2)
		{
			if (ReferenceEquals(ap1, ap2))
				return true;
			if (ReferenceEquals(ap1, null))
				return false;
			if (ReferenceEquals(ap2, null))
				return false;

			return ap1.Equals(ap2);
		}

		public static Boolean operator !=(AssetPath ap1, AssetPath ap2) => !(ap1 == ap2);

		public static Boolean operator ==(AssetPath ap1, Object other) =>
			other is String str ? ap1.Equals(str) : ap1.Equals(other as AssetPath);

		public static Boolean operator !=(AssetPath ap1, Object other) => !(ap1 == other);
		public static Boolean operator ==(Object other, AssetPath ap2) => ap2 == other;
		public static Boolean operator !=(Object other, AssetPath ap2) => !(ap2 == other);
	}
}
