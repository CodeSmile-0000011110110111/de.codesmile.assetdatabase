// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;
using Object = UnityEngine.Object;

namespace CodeSmileEditor.Tests
{
	public class AssetDatabaseTests : AssetTestBase
	{
		[Test] public void IsImported_NullObject_False() => Assert.IsFalse(Asset.Status.IsImported((Object)null));

		[Test] public void IsImported_NullPath_False() => Assert.IsFalse(Asset.Status.IsImported((Asset.Path)null));

		[Test] public void IsImported_NotAnAsset_False() =>
			Assert.IsFalse(Asset.Status.IsImported(Instantiate.ExampleSO()));

		[Test] public void IsImported_ExistingAsset_True() =>
			Assert.IsTrue(Asset.Status.IsImported(CreateTestAssetObject(TestAssetPath)));

		[Test] public void IsImported_ExistingPath_True() =>
			Assert.IsTrue(Asset.Status.IsImported(CreateTestAsset(TestAssetPath).AssetPath));

		[Test] public void GetMainType_NullPath_False() => Assert.Null(Asset.GetMainType((String)TestAssetPath));
	}
}
