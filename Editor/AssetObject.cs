// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public class AssetObject
	{
		private readonly AssetPath m_Path;
		private readonly GUID m_Guid;
		private readonly Int32 m_InstanceId = 0;
		private Object m_MainObject;
		private Object[] m_Objects;


		public Object MainObject => m_MainObject;
		public AssetPath Path => m_Path;
		public GUID Guid => m_Guid;
		public Int32 InstanceId => m_InstanceId;

		public Type MainObjectType => m_MainObject != null ? m_MainObject.GetType() :
			m_Path != null ? AssetDatabase.GetMainAssetTypeAtPath(m_Path) :
			AssetDatabase.GetMainAssetTypeFromGUID(m_Guid);
		public Boolean IsAsset =>
			m_InstanceId != 0 ? AssetDatabase.Contains(m_InstanceId) : AssetDatabase.Contains(m_MainObject);
		public Boolean IsForeignAsset => m_InstanceId != 0
			? AssetDatabase.IsForeignAsset(m_InstanceId)
			: AssetDatabase.IsForeignAsset(m_MainObject);
		public Boolean IsNativeAsset => m_InstanceId != 0
			? AssetDatabase.IsNativeAsset(m_InstanceId)
			: AssetDatabase.IsNativeAsset(m_MainObject);
		public Boolean IsSubAsset => m_InstanceId != 0
			? AssetDatabase.IsSubAsset(m_InstanceId)
			: AssetDatabase.IsSubAsset(m_MainObject);
		public Boolean IsMainAsset => m_InstanceId != 0
			? AssetDatabase.IsMainAsset(m_InstanceId)
			: AssetDatabase.IsMainAsset(m_MainObject);
		public Boolean IsLoaded => AssetDatabase.IsMainAssetAtPathLoaded(m_Path);

		// Create => remains a static method
		public static void Create(Object obj, AssetPath assetPath, Boolean overwriteExisting = false) =>
			AssetDatabase.CreateAsset(obj, overwriteExisting ? assetPath : assetPath.UniquePath);

		public static void Create(Object obj, String path, Boolean overwriteExisting = false) =>
			Create(obj, (AssetPath)path, overwriteExisting);

		// TODO: this could operate on the main or any of the sub assets
		public static Boolean ExtractObject(Object subAsset, AssetPath extractedAssetPath, out String errorMessage)
		{
			errorMessage = AssetDatabase.ExtractAsset(subAsset, extractedAssetPath);
			return errorMessage.Equals(String.Empty);
		}

		// disallow default ctor
		private AssetObject() {}

		// assumes there is an asset at this path
		// throws if no asset at this path
		// TODO: check file exists, but do not load object
		public AssetObject(AssetPath assetPath)
		{
			m_MainObject = null;
			m_Path = assetPath;
		}

		public AssetObject(String path) : this(new AssetPath(path)) {}

		// object is an existing asset object reference
		// throws if object is not an asset
		// TODO: check asset exists
		public AssetObject(Object obj)
		{
			m_MainObject = obj;
			m_Path = new AssetPath(AssetDatabase.GetAssetPath(obj));
		}

		// object is created as asset at the given path
		// throws if object is null
		// TODO: check object not null
		public AssetObject(Object obj, AssetPath assetPath, Boolean overwriteExisting = false)
		{
			m_MainObject = obj;
			m_Path = assetPath;
			Create(m_MainObject, m_Path, overwriteExisting);
			AssetDatabase.CreateAsset(m_MainObject, overwriteExisting ? m_Path : m_Path.UniquePath);
		}

		public AssetObject(Object obj, String path, Boolean overwriteExisting = false) :
			this(obj, (AssetPath)path, overwriteExisting) {}

		// object is an existing asset object reference
		public AssetObject(GUID guid)
		{
			m_MainObject = null;
			m_Path = null;
			m_Guid = guid;
		}

		// ----------------
		// Copy => SaveAs
		public AssetObject SaveAs(AssetPath savePath)
		{
			// TODO: check that asset is created/exists
			// TODO:
			// decide: update existing AssetObject vs returning a new AssetObject for the copy
			// leaning towards => return copy
			//		seems more consistent and in line with expectations
			//		can return null if copy not successful

			var success = AssetDatabase.CopyAsset(Path, savePath);
			if (success == false)
				return null;

			return new AssetObject(savePath);
		}

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

		public void Save() =>
			// TODO: guid overload, check null
			AssetDatabase.SaveAssetIfDirty(m_MainObject);

		public Boolean Trash()
		{
			var didTrash = AssetDatabase.MoveAssetToTrash(m_Path);
			// TODO: update state
			return didTrash;
		}

		// differences: https://discussions.unity.com/t/loadassetatpath-vs-loadallassetatpath-vs-loadmainassetatpath/197816/2
		public T LoadFirst<T>() where T : Object
		{
			var obj = AssetDatabase.LoadAssetAtPath<T>(m_Path);
			if (AssetDatabase.IsMainAsset(obj))
				m_MainObject = obj;

			return obj;
		}

		public T LoadMain<T>() where T : Object
		{
			if (m_MainObject == null)
				m_MainObject = AssetDatabase.LoadMainAssetAtPath(m_Path);

			return (T)m_MainObject;
		}

		public Object[] LoadAll() => AssignMainObject(AssetDatabase.LoadAllAssetsAtPath(m_Path));
		public Object[] LoadAllVisible() => AssignMainObject(AssetDatabase.LoadAllAssetRepresentationsAtPath(m_Path));

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
				AssetDatabase.AddObjectToAsset(obj, m_Path);
		}

		public void RemoveObject(Object obj)
		{

		}

		public void SetMainObjectAndImport(Object obj)
		{
			m_MainObject = obj;
			AssetDatabase.SetMainObject(m_MainObject, m_Path);
			Import();
		}

		private Object[] AssignMainObject(Object[] objects)
		{
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
