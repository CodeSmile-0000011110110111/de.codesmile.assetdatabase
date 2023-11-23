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
			///     Tests another path for equality. They are equal if their internal relative path strings are equal.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public Boolean Equals(Path other)
			{
				if (ReferenceEquals(other, null))
					return false;
				if (ReferenceEquals(this, other))
					return true;

				return m_RelativePath.Equals(other.m_RelativePath);
			}

			/// <summary>
			///     Tests paths for equality with a string. The string is elevated to a Path instance, and then
			///     their internal path strings are compared.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public Boolean Equals(String other) => m_RelativePath.Equals(new Path(other).m_RelativePath);

			/// <summary>
			///     Tests path for equality with an object. If the object is a Path or String will use the respective
			///     Equals method for these types. In all other cases returns false.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public override Boolean Equals(Object obj)
			{
				if (obj is Path path)
					return Equals(path);
				if (obj is String str)
					return Equals(str);

				return false;
			}

			/// <summary>
			///     Returns the internal path string's hash code.
			/// </summary>
			/// <returns></returns>
			public override Int32 GetHashCode() => m_RelativePath.GetHashCode();
		}
	}
}
