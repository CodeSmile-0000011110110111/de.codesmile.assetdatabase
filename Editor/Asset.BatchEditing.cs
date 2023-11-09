// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

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
		///     <seealso cref="https://docs.unity3d.com/Manual/AssetDatabaseBatching.html" />
		/// </summary>
		/// <param name="assetEditingAction"></param>
		[ExcludeFromCodeCoverage] public static void BatchEditing(Action assetEditingAction)
		{
			ThrowIf.ArgumentIsNull(assetEditingAction, nameof(assetEditingAction));

			try
			{
				Database.StartAssetEditing();
				assetEditingAction.Invoke();
			}
			catch (Exception ex)
			{
				Debug.LogError($"Exception during BatchEditing: {ex.Message}");
				throw ex; // re-throw to caller
			}
			finally
			{
				Database.StopAssetEditing();
			}
		}
	}
}
