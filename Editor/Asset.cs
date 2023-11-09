// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		private static String s_ErrorMessage = String.Empty;

		private Path m_AssetPath;
		private Object m_MainObject;
		//private Object[] m_AssetObjects;

		/// <summary>
		///     Returns the asset's main object.
		/// </summary>
		public Object MainObject => m_MainObject;
		/// <summary>
		///     Returns the path to the asset (file or folder).
		/// </summary>
		public Path AssetPath => m_AssetPath;
		/// <summary>
		///     Returns the asset's GUID.
		/// </summary>
		public GUID Guid => Path.GetGuid(m_AssetPath);

		/// <summary>
		///     Returns the last error message returned by some methods that provide such a message,
		///     for example Move and Rename.
		///     <see cref="Rename" />
		///     <see cref="Move" />
		/// </summary>
		public static String LastErrorMessage => s_ErrorMessage;

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

		private static Boolean Succeeded(String possibleErrorMessage)
		{
			s_ErrorMessage = possibleErrorMessage != null ? possibleErrorMessage : String.Empty;
			return String.IsNullOrEmpty(s_ErrorMessage);
		}

		private void InvalidateInstance()
		{
			m_AssetPath = null;
			m_MainObject = null;
			//m_AssetObjects = null;
		}
	}
}
