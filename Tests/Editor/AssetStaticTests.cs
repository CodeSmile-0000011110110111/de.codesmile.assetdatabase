// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

// BatchEditing causes the progress bar after test to hang for a long time / indefinitely

#define TESTS_DISABLED_UNTIL_ISSUE_59630_RESOLVED

using CodeSmile.Editor;
using NUnit.Framework;
using System;

public class AssetStaticTests : AssetTestBase
{

	[Test] public void BatchEditing_NullAction_Throws() =>
		Assert.Throws<ArgumentNullException>(() => Asset.BatchEditing(null));

#if !TESTS_DISABLED_UNTIL_ISSUE_59630_RESOLVED
	[Test] public void BatchEditing_ValidAction_GetsInvoked()
	{
		var didInvokeAction = false;

		Asset.BatchEditing(() => didInvokeAction = true);

		Assert.True(didInvokeAction);
	}

	[Test] public void BatchEditing_ActionThrowsException_RethrowsToCaller() =>
		Assert.Throws<Exception>(() => Asset.BatchEditing(() => throw new Exception()));
#endif
}
