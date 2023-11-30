// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Gets or sets the asset's main object.
		/// </summary>
		/// <example>
		///     To cast the main object to a specific type you may simply cast the asset:
		///     <code>var myObj = (MyType)asset;</code>
		///     Is short for:
		///     <code>var myObj = (MyType)asset.MainObject;</code>
		///     The same works with the 'as' operator:
		///     <code>var myObj = asset as MyType;</code>
		///     Is short for:
		///     <code>var myObj = asset.MainObject as MyType;</code>
		///     Lastly you can also use the generic getter:
		///     <code>var myObj = asset.Get&lt;MyType&gt;();</code>
		/// </example>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.SubAsset" />
		///     - <see cref="CodeSmile.Editor.Asset.SubAsset.SetMain(UnityEngine.Object,CodeSmile.Editor.Asset.Path)" />
		///     - <see cref="CodeSmile.Editor.Asset.SubAsset.SetMain(UnityEngine.Object,UnityEngine.Object)" />
		///     - <see cref="CodeSmile.Editor.Asset.File.LoadMain{T}(CodeSmile.Editor.Asset.Path)" />
		/// </seealso>
		public Object MainObject
		{
			// This 'loads' the asset but most of the time simply returns the internally cached instance.
			// We need to load the instance because the user may have called static SubAsset.SetMain().
			get => m_MainObject = LoadMain<Object>();
			set
			{
				SubAsset.SetMain(value, m_AssetPath);
				m_MainObject = value;
			}
		}

		/// <summary>
		///     Returns the type of the main asset at the given path.
		/// </summary>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.GetMainType(CodeSmile.Editor.Asset.Path)" />
		/// </seealso>
		public Type MainObjectType => GetMainType(m_AssetPath);

		/// <summary>
		///     Returns the path to the asset (file or folder).
		/// </summary>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.MetaPath" />
		/// </seealso>
		public Path AssetPath => m_AssetPath;

		/// <summary>
		///     Returns the path to the .meta file for the asset.
		/// </summary>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.AssetPath" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public Path MetaPath => Path.ToMeta(m_AssetPath);

		/// <summary>
		///     Returns the asset's GUID.
		/// </summary>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.FileId" />
		/// </seealso>
		public GUID Guid => Path.GetGuid(m_AssetPath);

		/// <summary>
		///     Returns the local FileID of the asset.
		/// </summary>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.Guid" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public Int64 FileId => GetFileId(m_MainObject);

		/// <summary>
		///     Returns the icon texture associated with the asset type.
		/// </summary>
		[ExcludeFromCodeCoverage] // simple relay
		public Texture2D Icon => GetIcon(m_AssetPath);

		/// <summary>
		///     Returns the bundle name the asset belongs to.
		/// </summary>
		/// <param name="path">Path to an asset.</param>
		/// <returns>The bundle name or an empty string.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.OwningBundleVariant" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public String OwningBundle => Bundle.GetOwningBundle(m_AssetPath);

		/// <summary>
		///     Returns the bundle variant name the asset belongs to.
		/// </summary>
		/// <param name="path">Path to an asset.</param>
		/// <returns>The bundle variant name or empty string.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.OwningBundle" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public String OwningBundleVariant => Bundle.GetOwningBundleVariant(m_AssetPath);

		/// <summary>
		///     Returns the assets' direct dependencies.
		/// </summary>
		/// <returns> Returns paths to the directly dependent assets.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.Dependencies" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public String[] DirectDependencies => Dependency.GetDirect(m_AssetPath);

		/// <summary>
		///     Returns the assets' direct and indirect dependencies (recursive).
		/// </summary>
		/// <returns> Returns paths to the dependent assets.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.DirectDependencies" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public String[] Dependencies => Dependency.GetAll(m_AssetPath);

		/// <summary>
		///     Returns the default AssetImporter type for this asset.
		/// </summary>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.ActiveImporter" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public Type DefaultImporter => Importer.GetDefault(m_AssetPath);

		/// <summary>
		///     Returns the active AssetImporter type for this asset.
		/// </summary>
		/// <remarks>Will be the DefaultImporter type unless the importer was overridden.</remarks>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.DefaultImporter" />
		///     - <see cref="CodeSmile.Editor.Asset.SetActiveImporter{T}" />
		///     - <see cref="CodeSmile.Editor.Asset.SetActiveImporterToDefault" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public Type ActiveImporter
		{
			get
			{
				var overridden = Importer.GetOverride(m_AssetPath);
				return overridden != null ? overridden : Importer.GetDefault(m_AssetPath);
			}
		}

		/// <summary>
		///     Returns true if the asset's default AssetImporter type has been overridden.
		/// </summary>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.ActiveImporter" />
		///     - <see cref="CodeSmile.Editor.Asset.DefaultImporter" />
		///     - <see cref="CodeSmile.Editor.Asset.SetActiveImporter{T}" />
		///     - <see cref="CodeSmile.Editor.Asset.SetActiveImporterToDefault" />
		/// </seealso>
		public Boolean IsImporterOverridden => ActiveImporter != DefaultImporter;

		/// <summary>
		///     Sets or gets the labels associated with the asset.
		/// </summary>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.SetLabels" />
		/// </seealso>
		public String[] Labels
		{
			get => Label.GetAll(m_MainObject);
			set => Label.SetAll(m_MainObject, value);
		}

		/// <summary>
		///     Returns true after the asset has been deleted.
		/// </summary>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.File.Delete(CodeSmile.Editor.Asset.Path)" />
		///     - <see cref="CodeSmile.Editor.Asset.File.Trash(CodeSmile.Editor.Asset.Path)" />
		/// </seealso>
		public Boolean IsDeleted => m_AssetPath == null && m_MainObject == null;

		/// <summary>
		///     Returns whether this is a foreign asset.
		/// </summary>
		/// <returns></returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.Status.IsForeign" />
		///     - <see cref="CodeSmile.Editor.Asset.Status.IsNative" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public Boolean IsForeignAsset => Status.IsForeign(m_MainObject);

		/// <summary>
		///     Returns whether this is a native asset.
		/// </summary>
		/// <returns></returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.Status.IsForeign" />
		///     - <see cref="CodeSmile.Editor.Asset.Status.IsNative" />
		/// </seealso>
		[ExcludeFromCodeCoverage] // simple relay
		public Boolean IsNativeAsset => Status.IsNative(m_MainObject);

		/// <summary>
		///     Returns true if this is a scene asset.
		/// </summary>
		public Boolean IsScene => Status.IsScene(m_MainObject);

		/// <summary>
		///     Loads and returns all sub objects the asset is comprised of.
		/// </summary>
		/// <remarks>
		///     Whether the main object is included in this list depends on the type of asset.
		/// </remarks>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.VisibleSubAssets" />
		/// </seealso>
		public Object[] SubAssets => IsScene ? new Object[0] : SubAsset.LoadAll(m_AssetPath);

		/// <summary>
		///     Loads and returns only those asset objects that are shown in the project view.
		/// </summary>
		/// <remarks>
		///     Does not include the main asset.
		/// </remarks>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.Editor.Asset.SubAssets" />
		/// </seealso>
		public Object[] VisibleSubAssets => IsScene ? new Object[0] : SubAsset.LoadVisible(m_AssetPath);
	}
}
