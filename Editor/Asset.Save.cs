// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Saves the object to disk if it is marked dirty.
		///     Note: Depending on how changes were made you may have to use ForceSave(Object) instead.
		///     <see cref="ForceSave(UnityEngine.Object)" />
		/// </summary>
		/// <param name="obj">returns the object for method chaining</param>
		public static Object Save(Object obj) => SaveInternal(obj);

		/// <summary>
		///     Forces the object to be saved to disk by first flagging it as dirty.
		///     Note: Be sure that you need to 'force' the save operation, since serializing and object and writing
		///     it to disk is a slow operation.
		///     <see cref="Save(UnityEngine.Object)" />
		/// </summary>
		/// <param name="obj">returns the object for method chaining</param>
		public static Object ForceSave(Object obj) => SaveInternal(obj, true);

		/// <summary>
		///     Saves any changes to the object to disk.
		/// </summary>
		/// <param name="guid"></param>
		public static void Save(GUID guid)
		{
			ThrowIf.NotAnAssetGuid(guid);
			AssetDatabase.SaveAssetIfDirty(guid);
		}

		/// <summary>
		///     <p>
		///         Saves all unsaved (dirty) objects.
		///     </p>
		///     <p>
		///         CAUTION: Consider that the user may NOT want to have unsaved assets 'randomly' saved!
		///         If you work with specific object(s) already it is in the user's best interest if you use
		///         Asset.Save(obj) instead. Also consider using Asset.BatchEditing(Action) in that case.
		///     </p>
		///     <see cref="Asset.Save(Object)" />
		///     <see cref="Asset.BatchEditing(System.Action)" />
		/// </summary>
		public static void SaveAll() => AssetDatabase.SaveAssets();

		private static Object SaveInternal(Object obj, Boolean forceSave = false)
		{
			ThrowIf.ArgumentIsNull(obj, nameof(obj));
			ThrowIf.NotInDatabase(obj);

			if (forceSave)
				EditorUtility.SetDirty(obj);

			AssetDatabase.SaveAssetIfDirty(obj);
			return obj;
		}

		/// <summary>
		///     Saves any changes to the object to disk if it is marked as dirty.
		///     <see cref="ForceSave()" />
		/// </summary>
		public void Save()
		{
			ThrowIf.AssetDeleted(this);
			Save(m_MainObject);
		}

		/// <summary>
		///     Forces the object to be written to disk, whether it is dirty or not.
		///     <see cref="Save()" />
		/// </summary>
		public void ForceSave()
		{
			ThrowIf.AssetDeleted(this);
			ForceSave(m_MainObject);
		}

		/// <summary>
		///     Marks the main object as dirty.
		/// </summary>
		public void SetMainObjectDirty() => EditorUtility.SetDirty(m_MainObject);
	}
}
