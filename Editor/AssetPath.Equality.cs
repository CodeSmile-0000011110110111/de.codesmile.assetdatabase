// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;

namespace CodeSmile.Editor
{
	public partial class AssetPath
	{
		public Boolean Equals(AssetPath other)
		{
			if (ReferenceEquals(other, null))
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return m_RelativePath.Equals(other.m_RelativePath);
		}

		public Boolean Equals(String other) => m_RelativePath.Equals(new AssetPath(other).m_RelativePath);
		public override Boolean Equals(Object obj) => Equals(obj as AssetPath);
		public override Int32 GetHashCode() => m_RelativePath.GetHashCode();

	}
}
