// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public partial class Path
		{
			public Boolean Equals(Path other)
			{
				if (ReferenceEquals(other, null))
					return false;
				if (ReferenceEquals(this, other))
					return true;

				return m_RelativePath.Equals(other.m_RelativePath);
			}

			public Boolean Equals(String other) => m_RelativePath.Equals(new Path(other).m_RelativePath);
			public override Boolean Equals(Object obj) => Equals(obj as Path);
			public override Int32 GetHashCode() => m_RelativePath.GetHashCode();
		}
	}
}