// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static void OpenInDefaultApplication(Object obj, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
			AssetDatabase.OpenAsset(obj, lineNumber, columnNumber);

		public static void OpenInDefaultApplication(Int32 instanceId, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
			AssetDatabase.OpenAsset(instanceId, lineNumber, columnNumber);

		public static void OpenInDefaultApplication(Path path, Int32 lineNumber = -1, Int32 columnNumber = -1) =>
			OpenInDefaultApplication(LoadMain<Object>(path), lineNumber, columnNumber);

		public void OpenInDefaultApplication(Int32 lineNumber = -1, Int32 columnNumber = -1) =>
			OpenInDefaultApplication(m_MainObject, lineNumber, columnNumber);
	}
}
