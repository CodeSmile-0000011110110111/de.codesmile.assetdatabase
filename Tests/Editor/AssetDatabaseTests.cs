// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;

public class AssetDatabaseTests : AssetTestBase
{
	[Test] public void DatabaseContains_NullAsset_False() => Assert.IsFalse(Asset.Database.Contains(null));

	[Test] public void DatabaseContains_NotAnAsset_False()
	{
		var obj = Instantiate.ExampleSO();
		Assert.IsFalse(Asset.Database.Contains(obj));
		Assert.IsFalse(Asset.Exists(obj));
	}

	[Test] public void DatabaseContains_ExistingAsset_True()
	{
		var obj = CreateTestAsset(TestAssetPath);
		Assert.IsTrue(Asset.Database.Contains(obj));
		Assert.IsTrue(Asset.Exists(obj));
	}
}
