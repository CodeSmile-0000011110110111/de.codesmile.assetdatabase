﻿// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmileEditor
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
			/// <param name="asset">Instance of an asset.</param>
			/// <returns>The labels of the asset or an empty array.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Label.Add" />
			///     - <see cref="CodeSmileEditor.Asset.Label.SetAll" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetLabels.html">AssetDatabase.GetLabels</a>
			/// </seealso>
			public static String[] GetAll([NotNull] Object asset) => AssetDatabase.GetLabels(asset);

			/// <summary>
			///     Returns an asset's labels.
			/// </summary>
			/// <param name="path">Path to an asset.</param>
			/// <returns>The labels of the asset or an empty array.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Label.Add" />
			///     - <see cref="CodeSmileEditor.Asset.Label.SetAll" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetLabels.html">AssetDatabase.GetLabels</a>
			/// </seealso>
			public static String[] GetAll([NotNull] Path path) => AssetDatabase.GetLabels(path.Guid);

			/// <summary>
			///     Returns an asset's labels.
			/// </summary>
			/// <param name="guid">GUID of an asset.</param>
			/// <returns>The labels of the asset or an empty array.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Label.Add" />
			///     - <see cref="CodeSmileEditor.Asset.Label.SetAll" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetLabels.html">AssetDatabase.GetLabels</a>
			/// </seealso>
			public static String[] GetAll(GUID guid) => AssetDatabase.GetLabels(guid);

			/// <summary>
			///     Sets an asset's labels. Replaces any existing labels.
			/// </summary>
			/// <param name="asset">Instance of an asset.</param>
			/// <param name="labels">An array of labels.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Label.Add" />
			///     - <see cref="CodeSmileEditor.Asset.Label.GetAll" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SetLabels.html">AssetDatabase.SetLabels</a>
			/// </seealso>
			public static void SetAll([NotNull] Object asset, [NotNull] String[] labels)
			{
				ThrowIf.ArgumentIsNull(asset, nameof(asset));
				ThrowIf.ArgumentIsNull(labels, nameof(labels));

				AssetDatabase.SetLabels(asset, labels);
			}

			/// <summary>
			///     Adds a single label to an asset's list of labels.
			/// </summary>
			/// <param name="asset">Instance of an asset.</param>
			/// <param name="label">The label to add.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Label.Add(Object,String[])" />
			///     - <see cref="CodeSmileEditor.Asset.Label.GetAll" />
			///     - <see cref="CodeSmileEditor.Asset.Label.SetAll" />
			/// </seealso>
			public static void Add([NotNull] Object asset, [NotNull] String label)
			{
				var existingLabels = new List<String>(GetAll(asset));
				existingLabels.Add(label);
				AssetDatabase.SetLabels(asset, existingLabels.ToArray());
			}

			/// <summary>
			///     Adds several labels to an asset's list of labels.
			/// </summary>
			/// <param name="asset">Instance of an asset.</param>
			/// <param name="labels">The labels to add.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Label.Add(Object,String)" />
			///     - <see cref="CodeSmileEditor.Asset.Label.GetAll" />
			///     - <see cref="CodeSmileEditor.Asset.Label.SetAll" />
			/// </seealso>
			public static void Add([NotNull] Object asset, [NotNull] String[] labels)
			{
				var existingLabels = new List<String>(GetAll(asset));
				existingLabels.AddRange(labels);
				AssetDatabase.SetLabels(asset, existingLabels.ToArray());
			}

			/// <summary>
			///     Clears all labels of an asset.
			/// </summary>
			/// <param name="asset">Instance of an asset.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Label.Remove" />
			///     - <see cref="CodeSmileEditor.Asset.Label.GetAll" />
			///     - <see cref="CodeSmileEditor.Asset.Label.SetAll" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ClearLabels.html">AssetDatabase.ClearLabels</a>
			/// </seealso>
			public static void ClearAll([NotNull] Object asset) => AssetDatabase.ClearLabels(asset);

			/// <summary>
			///     Removes a label from an asset. Does nothing if the label doesn't exist.
			/// </summary>
			/// <param name="asset">Instance of an asset.</param>
			/// <param name="label">Label to remove.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Label.ClearAll" />
			/// </seealso>
			public static void Remove(Object asset, String label)
			{
				var labels = GetAll(asset).ToList();
				labels.Remove(label);
				SetAll(asset, labels.ToArray());
			}
		}
	}
}
