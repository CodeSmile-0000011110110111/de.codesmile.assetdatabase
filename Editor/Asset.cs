// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		private AssetPath m_Path;
		private Object m_MainObject;
		private Object[] m_AssetObjects;

		public Object MainObject => m_MainObject;
		public AssetPath Path => m_Path;

		[ExcludeFromCodeCoverage] public Boolean IsForeignAsset => Status.IsForeignAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsNativeAsset => Status.IsNativeAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsSubAsset => Status.IsSubAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsMainAsset => Status.IsMainAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsLoaded => Status.IsLoaded(m_MainObject);

		// public Type MainObjectType => m_MainObject != null ? m_MainObject.GetType() :
		// 	m_Path != null ? AssetDatabase.GetMainAssetTypeAtPath(m_Path) :
		// 	AssetDatabase.GetMainAssetTypeFromGUID(m_Guid);

		private static AssetPath GetNewAssetPath(AssetPath destPath, Boolean overwriteExisting) =>
			overwriteExisting ? destPath : destPath.UniqueFilePath;

		private Object[] SelectAndAssignMainObject(Object[] objects)
		{
			m_AssetObjects = objects;

			if (m_MainObject == null)
			{
				foreach (var obj in objects)
				{
					if (AssetDatabase.IsMainAsset(obj))
					{
						m_MainObject = obj;
						break;
					}
				}
			}

			return objects;
		}
	}
}
