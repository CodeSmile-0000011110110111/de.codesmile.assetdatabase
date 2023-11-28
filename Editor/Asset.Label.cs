// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
			/// <param name="obj">Instance of an asset.</param>
			/// <returns>The labels of the asset or an empty array.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.Label.Add" />
			/// <seealso cref="CodeSmile.Editor.Asset.Label.SetAll" />
			/// <seealso cref="">
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetLabels.html">AssetDatabase.GetLabels</a>
			/// </seealso>
			public static String[] GetAll(Object obj) => AssetDatabase.GetLabels(obj);

			/// <summary>
			///     Returns an asset's labels.
			/// </summary>
			/// <param name="guid">GUID of an asset.</param>
			/// <returns>The labels of the asset or an empty array.</returns>
			/// <seealso cref="CodeSmile.Editor.Asset.Label.Add" />
			/// <seealso cref="CodeSmile.Editor.Asset.Label.SetAll" />
			/// <seealso cref="">
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetLabels.html">AssetDatabase.GetLabels</a>
			/// </seealso>
			public static String[] GetAll(GUID guid) => AssetDatabase.GetLabels(guid);

			/// <summary>
			///     Sets an asset's labels. Replaces any existing labels.
			/// </summary>
			/// <param name="obj">Instance of an asset.</param>
			/// <param name="labels">An array of labels.</param>
			/// <seealso cref="CodeSmile.Editor.Asset.Label.Add" />
			/// <seealso cref="CodeSmile.Editor.Asset.Label.GetAll" />
			/// <seealso cref="">
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SetLabels.html">AssetDatabase.SetLabels</a>
			/// </seealso>
			public static void SetAll([NotNull] Object obj, [NotNull] String[] labels)
			{
				ThrowIf.ArgumentIsNull(obj, nameof(obj));

				AssetDatabase.SetLabels(obj, labels);
			}

			/// <summary>
			///     Adds a single label to an asset's list of labels.
			/// </summary>
			/// <param name="obj">Instance of an asset.</param>
			/// <param name="label">The label to add.</param>
			/// <seealso cref="CodeSmile.Editor.Asset.Label.Add(UnityEngine.Object,String[])" />
			/// <seealso cref="CodeSmile.Editor.Asset.Label.GetAll" />
			/// <seealso cref="CodeSmile.Editor.Asset.Label.SetAll" />
			public static void Add(Object obj, String label)
			{
				var existingLabels = new List<String>(GetAll(obj));
				existingLabels.Add(label);
				AssetDatabase.SetLabels(obj, existingLabels.ToArray());
			}

			/// <summary>
			///     Adds several labels to an asset's list of labels.
			/// </summary>
			/// <param name="obj">Instance of an asset.</param>
			/// <param name="labels">The labels to add.</param>
			/// <seealso cref="CodeSmile.Editor.Asset.Label.Add(UnityEngine.Object,String)" />
			/// <seealso cref="CodeSmile.Editor.Asset.Label.GetAll" />
			/// <seealso cref="CodeSmile.Editor.Asset.Label.SetAll" />
			public static void Add(Object obj, String[] labels)
			{
				var existingLabels = new List<String>(GetAll(obj));
				existingLabels.AddRange(labels);
				AssetDatabase.SetLabels(obj, existingLabels.ToArray());
			}

			/// <summary>
			///     Clears all labels of an asset.
			/// </summary>
			/// <param name="obj">Instance of an asset.</param>
			/// <seealso cref="CodeSmile.Editor.Asset.Label.GetAll" />
			/// <seealso cref="CodeSmile.Editor.Asset.Label.SetAll" />
			/// <seealso cref="">
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ClearLabels.html">AssetDatabase.ClearLabels</a>
			/// </seealso>
			public static void ClearAll(Object obj) => AssetDatabase.ClearLabels(obj);
		}
	}
}
