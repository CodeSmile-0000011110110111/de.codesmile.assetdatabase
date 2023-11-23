// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public partial class Path
		{
			/// <summary>
			///     Implicit conversion to string (relative asset path). Same as ToString().
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static implicit operator String(Path path) => path != null ? path.m_RelativePath : null;

			/// <summary>
			///     Implicit conversion of an AssetPath instance from a string path (full or relative).
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static implicit operator Path(String path) => path != null ? new Path(path) : null;

			/// <summary>
			///     Tests two path instances for equality.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="path2"></param>
			/// <returns></returns>
			public static Boolean operator ==(Path path, Path path2)
			{
				if (ReferenceEquals(path, path2))
					return true;
				if (ReferenceEquals(path, null))
					return false;
				if (ReferenceEquals(path2, null))
					return false;

				return path.Equals(path2);
			}

			/// <summary>
			///     Tests two path instances for inequality.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="path2"></param>
			/// <returns></returns>
			public static Boolean operator !=(Path path, Path path2) => !(path == path2);

			/// <summary>
			///     Tests for equality with an object.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="other"></param>
			/// <returns></returns>
			public static Boolean operator ==(Path path, Object other) =>
				other is String str ? path.Equals(str) : path.Equals(other as Path);

			/// <summary>
			///     Tests for inequality with an object.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="other"></param>
			/// <returns></returns>
			public static Boolean operator !=(Path path, Object other) => !(path == other);

			/// <summary>
			///     Tests for equality with an object.
			/// </summary>
			/// <param name="other"></param>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean operator ==(Object other, Path path) => path == other;

			/// <summary>
			///     Tests for inequality with an object.
			/// </summary>
			/// <param name="other"></param>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean operator !=(Object other, Path path) => !(path == other);
		}
	}
}
