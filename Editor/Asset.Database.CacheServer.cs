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
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsCacheServerEnabled.html">AssetDatabase.IsCacheServerEnabled</a>
				/// </seealso>
				public static Boolean Enabled => AssetDatabase.IsCacheServerEnabled();

				/// <summary>
				///     Returns true if the editor is connected to a cache server (Accelerator).
				/// </summary>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsConnectedToCacheServer.html">AssetDatabase.IsConnectedToCacheServer</a>
				/// </seealso>
				public static Boolean Connected => AssetDatabase.IsConnectedToCacheServer();

				/// <summary>
				///     Returns the cache server (Accelerator) IP address the editor is currently connected to.
				/// </summary>
				/// <remarks>
				///     Returns an empty string if there is no connection.
				/// </remarks>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetCurrentCacheServerIp.html">AssetDatabase.GetCurrentCacheServerIp</a>
				/// </seealso>
				public static String ConnectedAddress => AssetDatabase.GetCurrentCacheServerIp();

				/// <summary>
				///     Returns the cache server (Accelerator) IP address from Project Settings.
				/// </summary>
				/// <remarks>
				///     Returns an empty string if there is no connection.
				/// </remarks>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetCacheServerAddress.html">AssetDatabase.GetCacheServerAddress</a>
				/// </seealso>
				public static String Address => AssetDatabase.GetCacheServerAddress();

				/// <summary>
				///     Returns the cache server (Accelerator) port number from Project Settings.
				/// </summary>
				/// <remarks>
				///     Returns 0 if the port is unset.
				/// </remarks>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetCacheServerPort.html">AssetDatabase.GetCacheServerPort</a>
				/// </seealso>
				public static UInt16 Port => AssetDatabase.GetCacheServerPort();

				/// <summary>
				///     Returns the cache server (Accelerator) namespace prefix from Project Settings.
				/// </summary>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetCacheServerNamespacePrefix.html">AssetDatabase.GetCacheServerNamespacePrefix</a>
				/// </seealso>
				public static String NamespacePrefix => AssetDatabase.GetCacheServerNamespacePrefix();

				/// <summary>
				///     Returns whether the cache server (Accelerator) is allowed to upload files.
				/// </summary>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetCacheServerEnableUpload.html">AssetDatabase.GetCacheServerEnableUpload</a>
				/// </seealso>
				public static Boolean UploadsAllowed => AssetDatabase.GetCacheServerEnableUpload();

				/// <summary>
				///     Returns whether the cache server (Accelerator) is allowed to download files.
				/// </summary>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.GetCacheServerEnableDownload.html">AssetDatabase.GetCacheServerEnableDownload</a>
				/// </seealso>
				public static Boolean DownloadsAllowed => AssetDatabase.GetCacheServerEnableDownload();

				/// <summary>
				///     Applies modified cache server (Accelerator) settings so that they take effect immediately.
				/// </summary>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.RefreshSettings.html">AssetDatabase.RefreshSettings</a>
				/// </seealso>
				[ExcludeFromCodeCoverage] // not testable
				public static void ApplySettings() => AssetDatabase.RefreshSettings();

				/// <summary>
				///     Tests if the cache server (Accelerator) connection can be established with the given IP
				///     address and port number.
				/// </summary>
				/// <param name="ipAddress">The IP address to connect to.</param>
				/// <param name="port">The port number of the Cache Server/Accelerator service.</param>
				/// <returns>True if connection could be established, false otherwise.</returns>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CanConnectToCacheServer.html">AssetDatabase.CanConnectToCacheServer</a>
				/// </seealso>
				[ExcludeFromCodeCoverage] // not testable, requires external service
				public static Boolean CanConnect(String ipAddress, UInt16 port) =>
					AssetDatabase.CanConnectToCacheServer(ipAddress, port);

				/// <summary>
				///     Resets the internal reconnect timer which subsequently increases to up to 5 minutes if
				///     connection attempts fail repeatedly.
				/// </summary>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ResetCacheServerReconnectTimer.html">AssetDatabase.ResetCacheServerReconnectTimer</a>
				/// </seealso>
				[ExcludeFromCodeCoverage] // not testable
				public static void ResetReconnectTimer() => AssetDatabase.ResetCacheServerReconnectTimer();

				/// <summary>
				///     Disconnects any active cache server (Accelerator) connection.
				/// </summary>
				/// <seealso cref="">
				///     -
				///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CloseCacheServerConnection.html">AssetDatabase.CloseCacheServerConnection</a>
				/// </seealso>
				public static void Disconnect() => AssetDatabase.CloseCacheServerConnection();
			}
		}
	}
}
