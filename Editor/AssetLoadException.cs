// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.Editor
{
	[ExcludeFromCodeCoverage]
	public class AssetLoadException : UnityException
	{
		public AssetLoadException()
			: base("asset load failed") {}

		public AssetLoadException(String message)
			: base(message) {}

		public AssetLoadException(String message, Exception innerException)
			: base(message, innerException) {}
	}
}
