// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{

		/// <summary>
		///     Speed up mass asset editing (create, modify, delete, import, etc).
		///     Within the callback action, the AssetDatabase does neither import nor auto-refresh assets.
		///     This can significantly speed up mass asset editing tasks where you work with individual assets
		///     in a loop.
		///     Internally calls Start/StopAssetEditing in a try/finally block so that exceptions will
		///     not cause the AssetDatabase to remain stopped indefinitely.
		/// </summary>
		/// <param name="assetEditingAction"></param>
		public static void BatchEditing(Action assetEditingAction)
		{
			ThrowIf.ArgumentIsNull(assetEditingAction, nameof(assetEditingAction));

			try
			{
				Database.StartAssetEditing();
				assetEditingAction.Invoke();
			}
			finally
			{
				Database.StopAssetEditing();
			}
		}
	}
}
