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
		/// <summary>
		///     Opens the asset in the default (associated) application.
		///     Optional line and column numbers can be specified for text files and applications that support this.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="lineNumber"></param>
		/// <param name="columnNumber"></param>
		[ExcludeFromCodeCoverage]
		public static void OpenInDefaultApplication(Object obj, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
			AssetDatabase.OpenAsset(obj, lineNumber, columnNumber);

		/// <summary>
		///     Opens the asset (by its instanceID) in the default (associated) application.
		///     Optional line and column numbers can be specified for text files and applications that support this.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="lineNumber"></param>
		/// <param name="columnNumber"></param>
		[ExcludeFromCodeCoverage]
		public static void OpenInDefaultApplication(Int32 instanceId, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
			AssetDatabase.OpenAsset(instanceId, lineNumber, columnNumber);

		/// <summary>
		///     Opens the asset at the path in the default (associated) application.
		///     Optional line and column numbers can be specified for text files and applications that support this.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="lineNumber"></param>
		/// <param name="columnNumber"></param>
		[ExcludeFromCodeCoverage]
		public static void OpenInDefaultApplication(Path path, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
			OpenInDefaultApplication(LoadMain<Object>(path), lineNumber, columnNumber);

		/// <summary>
		///     Opens the asset in the default (associated) application.
		///     Optional line and column numbers can be specified for text files and applications that support this.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="lineNumber"></param>
		/// <param name="columnNumber"></param>
		[ExcludeFromCodeCoverage]
		public void OpenInDefaultApplication(Int32 lineNumber = -1, Int32 columnNumber = -1) =>
			OpenInDefaultApplication(m_MainObject, lineNumber, columnNumber);
	}
}
