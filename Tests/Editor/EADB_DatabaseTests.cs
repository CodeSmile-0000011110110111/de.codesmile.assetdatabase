// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile;
using CodeSmile.Editor;
using NUnit.Framework;
using System;

public class EADB_DatabaseTests
{
	[Test] public void BatchEditing_ThrowsException_Rethrows() =>
		Assert.Throws<RankException>(() => Asset.DB.BatchEditing(() => throw new RankException()));

	[Test] public void BatchEditing_NullAction_DoesNothing() => Assert.DoesNotThrow(() => Asset.DB.BatchEditing(null));
}
