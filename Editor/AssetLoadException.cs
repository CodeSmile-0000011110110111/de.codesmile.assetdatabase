// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.Editor
{
	/// <summary>
	///     Thrown in cases where loading an existing asset fails. That means the AssetDatabase returned null on loading.
	///     This happens in a few situations in Unity where the AssetDatabase is unavailable, such as in a static ctor.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class AssetLoadException : UnityException
	{
		/// <summary>
		///     Instantiates exception with default message. Not used.
		/// </summary>
		internal AssetLoadException()
			: base("asset load failed") {}

		/// <summary>
		///     Instantiates exception with message.
		/// </summary>
		/// <param name="message">Exception message</param>
		public AssetLoadException(String message)
			: base(message) {}

		/// <summary>
		///     Instantiates exception with message and inner exception.
		/// </summary>
		/// <param name="message">Exception message</param>
		/// <param name="innerException">Inner exception that is rethrown</param>
		public AssetLoadException(String message, Exception innerException)
			: base(message, innerException) {}
	}
}
