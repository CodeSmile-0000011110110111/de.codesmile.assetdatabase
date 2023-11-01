// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.Editor
{
	public class AssetObject
	{
		public Object Object;
		public AssetPath Path;
		public GUID Guid;
		public long InstanceId;

		private AssetObject() {}

		public AssetObject(AssetPath assetPath)
		{
		}
		public AssetObject(Object obj)
		{
		}
		public AssetObject(GUID guid)
		{
		}

		public Object LoadAsset() { return null;}
		public bool IsLoaded { get; }

		public void CreateAsset() {}
		public bool IsAsset { get; }

		public void Save(){}
		public void SaveAs(AssetPath assetPath){}

	}
}
