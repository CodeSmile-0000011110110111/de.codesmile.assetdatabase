// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;

public class AssetStatusTests : AssetTestBase
{
	[Test] public void GetMainType_NullPath_False() => Assert.Null(Asset.GetMainType((string)TestAssetPath));
}
