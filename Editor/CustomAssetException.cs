// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.Editor
{
	public class CustomAssetException : Exception
	{
		public CustomAssetException() : this("", null) {}
		public CustomAssetException(String message) : this(message, null) {}
		public CustomAssetException(String message, Exception inner) : base(message, inner) {}
	}
}