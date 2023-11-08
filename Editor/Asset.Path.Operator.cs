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

			public static Boolean operator ==(Path ap1, Path ap2)
			{
				if (ReferenceEquals(ap1, ap2))
					return true;
				if (ReferenceEquals(ap1, null))
					return false;
				if (ReferenceEquals(ap2, null))
					return false;

				return ap1.Equals(ap2);
			}

			public static Boolean operator !=(Path ap1, Path ap2) => !(ap1 == ap2);

			public static Boolean operator ==(Path ap1, Object other) =>
				other is String str ? ap1.Equals(str) : ap1.Equals(other as Path);

			public static Boolean operator !=(Path ap1, Object other) => !(ap1 == other);
			public static Boolean operator ==(Object other, Path ap2) => ap2 == other;
			public static Boolean operator !=(Object other, Path ap2) => !(ap2 == other);
		}
	}
}
