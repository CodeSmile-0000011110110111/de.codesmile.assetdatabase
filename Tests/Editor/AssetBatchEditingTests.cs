// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

//#define DISABLED_UNTIL_ISSUE_59630_RESOLVED

using CodeSmileEditor.Tests.Helper;
using NUnit.Framework;
using System;

namespace CodeSmileEditor.Tests
{
	public class AssetBatchEditingTests : AssetTestBase
	{
		[Test] public void BatchEditing_ValidAction_GetsInvoked()
		{
			var didInvokeAction = false;

			Asset.File.BatchEditing(() => didInvokeAction = true);

			Assert.True(didInvokeAction);
		}

		[Test] public void BatchEditing_ActionThrowsException_RethrowsToCaller() =>
			Assert.Throws<Exception>(() => Asset.File.BatchEditing(() => throw new Exception()));
	}
}
