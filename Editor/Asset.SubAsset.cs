// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmileEditor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Groups all Sub-Asset related functionality.
		/// </summary>
		public static class SubAsset
		{
			/// <summary>
			///     Extracts a sub-object of an asset as an asset of its own.
			/// </summary>
			/// <remarks>
			///     This is the same as dragging a sub-asset outside the containing asset in the project view.
			///     For example an animation or material dragged from a FBX asset.
			/// </remarks>
			/// <remarks>
			///     Only visible sub assets are extractable.
			/// </remarks>
			/// <param name="subAsset">Instance of a sub-asset.</param>
			/// <param name="destinationPath">Path to the extracted asset file.</param>
			/// <returns>
			///     True if extraction succeeded. False otherwise, in that case CodeSmileEditor.Asset.GetLastErrorMessage
			///     provides the error message.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.SubAsset.Remove" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.ExtractAsset.html">AssetDatabase.ExtractAsset</a>
			/// </seealso>
			[ExcludeFromCodeCoverage] // simple relay
			public static Boolean Extract([NotNull] Object subAsset, [NotNull] Path destinationPath)
			{
				ThrowIf.ArgumentIsNull(subAsset, nameof(subAsset));
				ThrowIf.ArgumentIsNull(destinationPath, nameof(destinationPath));

				return Succeeded(AssetDatabase.ExtractAsset(subAsset, destinationPath));
			}

			/// <summary>
			///     Adds an object as sub-asset to the asset.
			/// </summary>
			/// <param name="subAssetInstance">The object to add as a sub-asset. It must not already be an asset.</param>
			/// <param name="asset">Instance of an asset.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.SubAsset.Remove" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.AddObjectToAsset.html">AssetDatabase.AddObjectToAsset</a>
			/// </seealso>
			public static void Add([NotNull] Object subAssetInstance, [NotNull] Object asset)
			{
				ThrowIf.ArgumentIsNull(subAssetInstance, nameof(subAssetInstance));
				ThrowIf.SubObjectIsGameObject(subAssetInstance);
				ThrowIf.AlreadyAnAsset(subAssetInstance);
				ThrowIf.ArgumentIsNull(asset, nameof(asset));
				ThrowIf.NotAnAssetWithAssetExtension(asset);

				AssetDatabase.AddObjectToAsset(subAssetInstance, asset);
			}

			/// <summary>
			///     Removes a sub-object from the asset it is contained in.
			/// </summary>
			/// <param name="subAsset">Instance of a sub-asset.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.SubAsset.Add" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.RemoveObjectFromAsset.html">AssetDatabase.RemoveObjectFromAsset</a>
			/// </seealso>
			public static void Remove([NotNull] Object subAsset)
			{
				ThrowIf.ArgumentIsNull(subAsset, nameof(subAsset));

				AssetDatabase.RemoveObjectFromAsset(subAsset);
			}

			/// <summary>
			///     Sets (changes) an asset's 'main' object to one of its sub-assets.
			/// </summary>
			/// <remarks> Automatically imports the asset after changing the main type so that the change takes immediate effect. </remarks>
			/// <param name="subAsset">Instance of a sub-asset. Must be a sub-asset of the asset.</param>
			/// <param name="path">Path to the asset file.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Status.IsMain" />
			///     - <see cref="CodeSmileEditor.Asset.Status.IsSub" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SetMainObject.html">AssetDatabase.SetMainObject</a>
			/// </seealso>
			public static void SetMain([NotNull] Object subAsset, [NotNull] Path path)
			{
				AssetDatabase.SetMainObject(subAsset, path);
				File.Import(path);
			}

			/// <summary>
			///     Sets (changes) an asset's 'main' object to one of its sub-assets.
			/// </summary>
			/// <remarks> Automatically imports the asset after changing the main type so that the change takes immediate effect. </remarks>
			/// <param name="subAsset">Instance of a sub-asset. Must be a sub-asset of the asset.</param>
			/// <param name="asset">Instance of the asset.</param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.Status.IsMain" />
			///     - <see cref="CodeSmileEditor.Asset.Status.IsSub" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.SetMainObject.html">AssetDatabase.SetMainObject</a>
			/// </seealso>
			public static void SetMain([NotNull] Object subAsset, [NotNull] Object asset) => SetMain(subAsset, Path.Get(asset));

			/// <summary>
			///     Loads all sub-asset objects of an asset.
			/// </summary>
			/// <remarks>
			///     Whether the main object is included in this list depends on the type of asset.
			/// </remarks>
			/// <remarks>
			///     CAUTION: Calling this on scene assets is not supported (error messages in console).
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>
			///     All sub-assets of the asset, possibly excluding the main asset. Or an empty array if load failed or there are
			///     no sub-assets and the asset type does not include the main type in the sub-assets list.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.SubAsset.LoadVisible" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAllAssetsAtPath.html">AssetDatabase.LoadAllAssetsAtPath</a>
			/// </seealso>
			public static Object[] LoadAll([NotNull] Path path) => AssetDatabase.LoadAllAssetsAtPath(path);

			/// <summary>
			///     Loads only the visible (representation) sub-asset objects of an asset.
			/// </summary>
			/// <remarks>
			///     The visible representations are those sub-assets you see in the project view when an asset is
			///     expandable like a folder.
			/// </remarks>
			/// <remarks>Does not include the main asset.</remarks>
			/// <remarks>
			///     CAUTION: Calling this on scene assets is not supported (error messages in console).
			/// </remarks>
			/// <param name="path">Path to an asset file.</param>
			/// <returns>
			///     All visible sub-assets of the asset, excluding the main asset. Or an empty array if load failed or there are
			///     no sub-assets.
			/// </returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.SubAsset.LoadAll" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAllAssetRepresentationsAtPath.html">AssetDatabase.LoadAllAssetRepresentationsAtPath</a>
			/// </seealso>
			public static Object[] LoadVisible([NotNull] Path path) => AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
		}
	}
}
