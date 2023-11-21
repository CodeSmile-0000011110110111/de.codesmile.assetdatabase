// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Implicit conversion to UnityEngine.Object by returning the asset's MainObject.
		/// </summary>
		/// <param name="asset">The main object of the asset.</param>
		public static implicit operator Object(Asset asset) => asset != null ? asset.MainObject : null;

		/// <summary>
		///     Implicit conversion of UnityEngine.Object to an asset instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>An asset instance or null if obj is null.</returns>
		public static implicit operator Asset(Object obj) => obj != null ? new Asset(obj) : null;

		/// <summary>
		///     Implicit conversion of Asset.Path to an asset instance.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>An asset instance or null if path is null.</returns>
		public static implicit operator Asset(Path path) => path != null ? new Asset(path) : null;

		/// <summary>
		///     Implicit conversion of GUID to an asset instance.
		/// </summary>
		/// <param name="guid">An asset instance.</param>
		/// <returns>An asset instance or null if guid is empty.</returns>
		public static implicit operator Asset(GUID guid) => guid.Empty() == false ? new Asset(guid) : null;
	}
}
