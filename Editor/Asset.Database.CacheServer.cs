// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		public static partial class Database
		{
			/// <summary>
			///     Groups CacheServer (Accelerator) related functionality.
			/// </summary>
			public static class CacheServer
			{
				/// <summary>
				///     Returns true if cache server (Accelerator) is enabled in Project Settings / Preferences.
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static Boolean Enabled => AssetDatabase.IsCacheServerEnabled();

				/// <summary>
				///     Returns true if the editor is connected to a cache server (Accelerator).
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static Boolean Connected => AssetDatabase.IsConnectedToCacheServer();

				/// <summary>
				///     Returns the cache server (Accelerator) IP address the editor is currently connected to.
				///     Returns an empty string if there is no connection.
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static String ConnectedAddress => AssetDatabase.GetCurrentCacheServerIp();

				/// <summary>
				///     Returns the cache server (Accelerator) IP address from Project Settings.
				///     Returns an empty string if the IP address is unset.
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static String Address => AssetDatabase.GetCacheServerAddress();

				/// <summary>
				///     Returns the cache server (Accelerator) port number from Project Settings.
				///     Returns 0 if the port is unset.
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static UInt16 Port => AssetDatabase.GetCacheServerPort();

				/// <summary>
				///     Returns the cache server (Accelerator) namespace prefix from Project Settings.
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static String NamespacePrefix => AssetDatabase.GetCacheServerNamespacePrefix();

				/// <summary>
				///     Returns whether the cache server (Accelerator) is allowed to upload files.
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static Boolean UploadsAllowed => AssetDatabase.GetCacheServerEnableUpload();

				/// <summary>
				///     Returns whether the cache server (Accelerator) is allowed to download files.
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static Boolean DownloadsAllowed => AssetDatabase.GetCacheServerEnableUpload();

				/// <summary>
				///     Applies modified cache server (Accelerator) settings so that they take effect immediately.
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.RefreshSettings.html">See manual</a>
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static void ApplySettings() => AssetDatabase.RefreshSettings();

				/// <summary>
				///     Tests whether the cache server (Accelerator) connection can be established with the given IP
				///     address and port number.
				/// </summary>
				/// <param name="ipAddress"></param>
				/// <param name="port"></param>
				/// <returns>True if connection could be established, false otherwise.</returns>
				[ExcludeFromCodeCoverage]
				public static Boolean CanConnect(String ipAddress, UInt16 port) =>
					AssetDatabase.CanConnectToCacheServer(ipAddress, port);

				/// <summary>
				///     Resets the internal reconnect timer which subsequently increases to up to 5 minutes if
				///     connection attempts fail repeatedly.
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static void ResetReconnectTimer() => AssetDatabase.ResetCacheServerReconnectTimer();

				/// <summary>
				///     Disconnects any active cache server (Accelerator) connection. Does nothing if not connected.
				/// </summary>
				[ExcludeFromCodeCoverage]
				public static void Disconnect() => AssetDatabase.CloseCacheServerConnection();
			}
		}
	}
}
