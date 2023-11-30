// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;

namespace CodeSmileEditor.Tests.Helper
{
	public static class AssetHelper
	{
		public static Int64 GetFileSize(String path) => new FileInfo(path).Length;
	}
}
