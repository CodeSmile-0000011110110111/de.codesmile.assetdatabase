// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static class SubAsset
		{
			public static Boolean Extract(Object subAsset, AssetPath extractedAssetPath, out String errorMessage)
			{
				errorMessage = AssetDatabase.ExtractAsset(subAsset, extractedAssetPath);
				return errorMessage.Equals(String.Empty);
			}
		}
	}
}
