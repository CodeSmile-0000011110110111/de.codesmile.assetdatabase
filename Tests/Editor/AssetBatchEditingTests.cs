// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

#define DISABLED_UNTIL_ISSUE_59630_RESOLVED

using CodeSmile.Editor;
using NUnit.Framework;
using System;

public class AssetBatchEditingTests : AssetTestBase
{
	[Test] public void BatchEditing_NullAction_Throws() =>
		Assert.Throws<ArgumentNullException>(() => Asset.File.BatchEditing(null));

#if !DISABLED_UNTIL_ISSUE_59630_RESOLVED // Start/StopAssetEditing causes TestRunner to hang
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
