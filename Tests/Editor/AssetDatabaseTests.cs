// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;

public class AssetDatabaseTests : AssetTestBase
{
	[Test] public void Contains_NullAsset_False() => Assert.IsFalse(Asset.Database.Contains(null));

	[Test] public void Contains_NotAnAsset_False() => Assert.IsFalse(Asset.Database.Contains(Instantiate.ExampleSO()));

	[Test] public void Contains_ExistingAsset_True() =>
		Assert.IsTrue(Asset.Database.Contains(CreateTestAssetObject(TestAssetPath)));

	[Test] public void Exists_NotAnAsset_False() => Assert.IsFalse(Asset.Exists(Instantiate.ExampleSO()));

	[Test] public void Exists_ExistingAsset_True() => Assert.IsTrue(Asset.Exists(CreateTestAssetObject(TestAssetPath)));

	[Test] public void GetMainType_NullPath_False() => Assert.Null(Asset.MainType((String)TestAssetPath));
}
