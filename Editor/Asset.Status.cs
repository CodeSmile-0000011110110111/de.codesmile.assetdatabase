// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Groups asset status related functions.
		/// </summary>
		public static class Status
		{
			/// <summary>
			///     Checks if the object is an asset in the AssetDatabase.
			/// </summary>
			/// <remarks>
			///     If you are sure the asset file exists but this method returns false then you need to Import() the asset.
			/// </remarks>
			/// <remarks>Unlike AssetDatabase, will not throw a NullRef if you pass null.</remarks>
			/// <param name="instance">The instance to test.</param>
			/// <returns>Returns false if the object isn't in the database or if the object is null.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Status.IsImported(CodeSmile.Editor.Asset.Path)" />
			///     - <see cref="CodeSmile.Editor.Asset.Status.IsLoaded" />
			///     - <see cref="Contains" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.Contains.html">AssetDatabase.Contains</a>
			/// </seealso>
			public static Boolean IsImported(Object instance) => Database.Contains(instance);

			/// <summary>
			///     Checks if the path is in the AssetDatabase.
			/// </summary>
			/// <remarks>
			///     If you are sure the asset path exists but this method returns false then you need to Import() the asset.
			/// </remarks>
			/// <remarks>Unlike AssetDatabase, will not throw a NullRef if you pass null.</remarks>
			/// <param name="path">Path to an asset.</param>
			/// <returns>Returns false if the path isn't in the database or if the path is null.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Status.IsImported(Object)" />
			///     - <see cref="CodeSmile.Editor.Asset.Status.IsLoaded" />
			///     - <see cref="Contains" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.AssetPathExists.html">AssetDatabase.AssetPathExists</a>
			/// </seealso>
			public static Boolean IsImported(Path path) => path != null && path.Exists;

			/// <summary>
			///     Returns whether the (main) asset at the path is loaded.
			/// </summary>
			/// <param name="path">Path to an asset.</param>
			/// <returns>True if the object at the path is loaded, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Status.IsImported" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsMainAssetAtPathLoaded.html">AssetDatabase.IsMainAssetAtPathLoaded</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Boolean IsLoaded(Path path) => AssetDatabase.IsMainAssetAtPathLoaded(path);

			/// <summary>
			///     Returns whether this object is the asset's 'main' object.
			/// </summary>
			/// <param name="asset">Instance of an asset.</param>
			/// <returns>True if it's the asset's main object, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Status.IsSub" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsMainAsset.html">AssetDatabase.IsMainAsset</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Boolean IsMain(Object asset) => AssetDatabase.IsMainAsset(asset);

			/// <summary>
			///     Returns whether this object is a sub-asset of a composite asset.
			/// </summary>
			/// <param name="asset">Instance of an asset.</param>
			/// <returns>True if it's a sub asset, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Status.IsMain" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsSubAsset.html">AssetDatabase.IsSubAsset</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Boolean IsSub(Object asset) => AssetDatabase.IsSubAsset(asset);

			/// <summary>
			///     Returns whether this is a foreign asset.
			/// </summary>
			/// <remarks>
			///     A foreign asset is any type of file that Unity doesn't edit directly.
			///     Examples of foreign assets: scenes (.unity), prefabs, assembly definitions, ..
			/// </remarks>
			/// <param name="asset">Instance of an asset.</param>
			/// <returns>True if it is a foreign asset, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Status.IsNative" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsForeignAsset.html">AssetDatabase.IsForeignAsset</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Boolean IsForeign(Object asset) => AssetDatabase.IsForeignAsset(asset);

			/// <summary>
			///     Returns whether this is a native asset.
			/// </summary>
			/// <remarks>
			///     Native assets are editable within Unity, for example materials and animation controllers.
			///     There are notable exceptions, such as scenes, prefabs and assembly definitions that are considered foreign assets.
			/// </remarks>
			/// <param name="asset">Instance of an asset.</param>
			/// <returns>True if it is a native asset, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmile.Editor.Asset.Status.IsForeign" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsNativeAsset.html">AssetDatabase.IsNativeAsset</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Boolean IsNative(Object asset) => AssetDatabase.IsNativeAsset(asset);

			/// <summary>
			///     Returns true if the given object is of type SceneAsset.
			/// </summary>
			/// <param name="asset">The instance to test for being a SceneAsset type.</param>
			/// <returns>True if the object is of type SceneAsset. False otherwise.</returns>
			public static Boolean IsScene(Object asset) => asset is SceneAsset;
		}
	}
}
