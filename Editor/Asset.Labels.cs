// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Returns an asset's labels.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static String[] GetLabels(Object obj) => AssetDatabase.GetLabels(obj);

		/// <summary>
		///     Returns an asset's labels.
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static String[] GetLabels(GUID guid) => AssetDatabase.GetLabels(guid);

		/// <summary>
		///     Sets an asset's labels.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="labels"></param>
		public static void SetLabels(Object obj, String[] labels) => AssetDatabase.SetLabels(obj, labels);

		/// <summary>
		///     Adds a label to an asset's list of labels.
		///     Note: this is less efficient than SetLabels. It is provided as a convenience to add a few labels
		///     without having to do the conversion to List, then add a label, then ToArray() the list.
		///     This is what is done internally.
		/// </summary>
		/// <see cref="AddLabels" />
		/// <param name="obj"></param>
		/// <param name="label"></param>
		public static void AddLabel(Object obj, String label)
		{
			var existingLabels = new List<String>(GetLabels(obj));
			existingLabels.Add(label);
			AssetDatabase.SetLabels(obj, existingLabels.ToArray());
		}

		/// <summary>
		///     Adds several labels to an asset's list of labels.
		/// </summary>
		/// <see cref="AddLabel" />
		/// <param name="obj"></param>
		/// <param name="labels"></param>
		public static void AddLabels(Object obj, String[] labels)
		{
			var existingLabels = new List<String>(GetLabels(obj));
			existingLabels.AddRange(labels);
			AssetDatabase.SetLabels(obj, existingLabels.ToArray());
		}

		/// <summary>
		///     Clears all labels of an asset.
		/// </summary>
		/// <param name="obj"></param>
		public static void ClearLabels(Object obj) => AssetDatabase.ClearLabels(obj);

		/// <summary>
		///     Adds a label to the asset.
		/// </summary>
		/// <param name="label"></param>
		public void AddLabel(String label) => AddLabel(m_MainObject, label);

		/// <summary>
		///     Adds several labels to the asset.
		/// </summary>
		/// <param name="labels"></param>
		public void AddLabels(String[] labels) => AddLabels(m_MainObject, labels);

		/// <summary>
		///     Clears the asset's labels.
		/// </summary>
		public void ClearLabels() => ClearLabels(m_MainObject);
	}
}
