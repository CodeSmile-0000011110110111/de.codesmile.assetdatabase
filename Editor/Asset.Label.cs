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
		///     Groups all asset label related static methods.
		/// </summary>
		public static class Label
		{
			/// <summary>
			///     Returns an asset's labels.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public static String[] GetAll(Object obj) => AssetDatabase.GetLabels(obj);

			/// <summary>
			///     Returns an asset's labels.
			/// </summary>
			/// <param name="guid"></param>
			/// <returns></returns>
			public static String[] GetAll(GUID guid) => AssetDatabase.GetLabels(guid);

			/// <summary>
			///     Sets an asset's labels, replacing any existing labels.
			/// </summary>
			/// <param name="obj"></param>
			/// <param name="labels"></param>
			public static void SetAll(Object obj, String[] labels)
			{
				ThrowIf.ArgumentIsNull(obj, nameof(obj));

				if (labels == null || labels.Length == 0)
					AssetDatabase.ClearLabels(obj);
				else
					AssetDatabase.SetLabels(obj, labels);
			}

			/// <summary>
			///     Adds a label to an asset's list of labels.
			///     Note: this is less efficient than SetLabels. It is provided as a convenience to add a few labels
			///     without having to do the conversion to List, then add a label, then ToArray() the list.
			///     This is what is done internally.
			/// </summary>
			/// <see cref="Add(UnityEngine.Object,string[])" />
			/// <param name="obj"></param>
			/// <param name="label"></param>
			public static void Add(Object obj, String label)
			{
				var existingLabels = new List<String>(GetAll(obj));
				existingLabels.Add(label);
				AssetDatabase.SetLabels(obj, existingLabels.ToArray());
			}

			/// <summary>
			///     Adds several labels to an asset's list of labels.
			/// </summary>
			/// <see cref="Add(UnityEngine.Object,string)" />
			/// <param name="obj"></param>
			/// <param name="labels"></param>
			public static void Add(Object obj, String[] labels)
			{
				var existingLabels = new List<String>(GetAll(obj));
				existingLabels.AddRange(labels);
				AssetDatabase.SetLabels(obj, existingLabels.ToArray());
			}

			/// <summary>
			///     Clears all labels of an asset.
			/// </summary>
			/// <param name="obj"></param>
			public static void ClearAll(Object obj) => AssetDatabase.ClearLabels(obj);
		}
	}
}
