// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		private static String s_LastErrorMessage = String.Empty;

		/// <summary>
		///     Returns the last error message from some file operations that return a Boolean,
		///     for example: Move, Rename, Copy
		/// </summary>
		public String LastErrorMessage => GetLastErrorMessage();

		/// <summary>
		///     Returns the last error message returned by some methods that provide such a message,
		///     for example Move and Rename.
		///     <see cref="Rename" />
		///     <see cref="Move" />
		/// </summary>
		public static String GetLastErrorMessage() => s_LastErrorMessage;

		private static void SetLastErrorMessage(String message) =>
			s_LastErrorMessage = message != null ? message : String.Empty;

		private static Boolean Succeeded(String possibleErrorMessage)
		{
			SetLastErrorMessage(possibleErrorMessage);
			return String.IsNullOrEmpty(GetLastErrorMessage());
		}
	}
}
