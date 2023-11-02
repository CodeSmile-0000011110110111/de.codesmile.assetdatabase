// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		private GUID m_AssetGuid;
		private AssetPath m_AssetPath;
		private Object m_MainObject;
		private Object[] m_AssetObjects;

		public Object MainObject => m_MainObject;
		public AssetPath AssetPath => m_AssetPath;
		public GUID AssetGuid => m_AssetGuid.Empty() ? m_AssetGuid = Guid.Get(m_AssetPath) : m_AssetGuid;

		// public Type MainObjectType => m_MainObject != null ? m_MainObject.GetType() :
		// 	m_Path != null ? AssetDatabase.GetMainAssetTypeAtPath(m_Path) :
		// 	AssetDatabase.GetMainAssetTypeFromGUID(m_Guid);

		public Boolean IsAsset => Database.Contains(m_MainObject);
		public Boolean IsForeignAsset => Status.IsForeignAsset(m_MainObject);
		public Boolean IsNativeAsset => Status.IsNativeAsset(m_MainObject);
		public Boolean IsSubAsset => Status.IsSubAsset(m_MainObject);
		public Boolean IsMainAsset => Status.IsMainAsset(m_MainObject);
		public Boolean IsLoaded => Status.IsLoaded(m_AssetPath);

		// Create => remains a static method

		// TODO: this could operate on the main or any of the sub assets

		// disallow default ctor

		// assumes there is an asset at this path
		// throws if no asset at this path
		// TODO: check file exists, but do not load object

		// object is an existing asset object reference
		// throws if object is not an asset
		// TODO: check asset exists

		// object is created as asset at the given path
		// throws if object is null
		// TODO: check object not null

		// object is an existing asset object reference

		// ----------------
		// Copy => SaveAs
		public Boolean SaveAs(AssetPath destinationPath, Boolean overwriteExisting = false)
		{
			// TODO: check that asset is created/exists

			var newPath = GetTargetPath(destinationPath, overwriteExisting);
			var success = SaveAs(m_AssetPath, newPath);
			if (success)
				SetMainObjectAndPath(newPath);

			return success;
		}

		public Asset Duplicate(AssetPath destinationPath, Boolean overwriteExisting = false)
		{
			// TODO: check that asset is created/exists

			var newPath = GetTargetPath(destinationPath, overwriteExisting);
			return SaveAs(m_AssetPath, newPath) ? new Asset(newPath) : null;
		}

		public void Save() =>
			// TODO: guid overload, check null
			AssetDatabase.SaveAssetIfDirty(m_MainObject);

		public void Delete()
		{
			// TODO: check that asset is created/exists
			// TODO: what to do with the deleted object? object remains, i assume. need to null path
		}

		public void Import()
		{
			// TODO: check that path is valid
		}

		public Boolean Rename(AssetPath destinationPath, out String errorMessage)
		{
			// TODO: if return string empty, move was successful
			// what to do with error message?
			errorMessage = "";
			return errorMessage.Equals(String.Empty);
		}

		public Boolean Move(AssetPath destinationPath, out String errorMessage)
		{
			// TODO: if return string empty, move was successful
			// what to do with error message?
			errorMessage = "";
			return errorMessage.Equals(String.Empty);
		}

		public Boolean CanMove(AssetPath destinationPath, out String errorMessage)
		{
			errorMessage = "";
			return errorMessage.Equals(String.Empty);
		}

		public Boolean OpenExternal(Int32 lineNumber = -1, Int32 columnNumber = -1) =>
			// TODO: overload for object and instanceId
			// TODO: check null
			AssetDatabase.OpenAsset(MainObject, lineNumber, columnNumber);

		public Boolean Trash()
		{
			var didTrash = AssetDatabase.MoveAssetToTrash(m_AssetPath);
			// TODO: update state
			return didTrash;
		}

		public T Load<T>() where T : Object => (T)(m_MainObject = LoadMain<T>(m_AssetPath));
		public Object[] LoadAll() => SelectAndAssignMainObject(LoadAll(m_AssetPath));
		public Object[] LoadOnlyVisible() => SelectAndAssignMainObject(LoadOnlyVisible(m_AssetPath));

		public Object LoadMainAsync(Action<Object> onLoadComplete)
		{
			// TODO: use coroutine to load async
			Object obj = null;
			onLoadComplete?.Invoke(obj);
			return null;
		}

		public Object LoadAsync(Int32 fileId, Action<Object> onLoadComplete)
		{
			// TODO: use coroutine to load async
			Object obj = null;
			onLoadComplete?.Invoke(obj);
			return null;
		}

		// TODO: this could operate on the main or any of the sub assets
		public void AddObject(Object obj)
		{
			// TODO: check obj not null, not same as main
			if (m_MainObject != null)
				AssetDatabase.AddObjectToAsset(obj, m_MainObject);
			else
				AssetDatabase.AddObjectToAsset(obj, m_AssetPath);
		}

		public void RemoveObject(Object obj) {}

		public void SetMainObjectAndImport(Object obj)
		{
			m_MainObject = obj;
			AssetDatabase.SetMainObject(m_MainObject, m_AssetPath);
			Import();
		}

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
