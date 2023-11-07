// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		private Path m_AssetPath;
		private Object m_MainObject;
		private Object[] m_AssetObjects;

		public Object MainObject => m_MainObject;
		public Path AssetPath => m_AssetPath;

		private void InvalidateInstance()
		{
			m_AssetPath = null;
			m_MainObject = null;
			m_AssetObjects = null;
		}
	}
}
