// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmileEditor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Groups version control related functionality.
		/// </summary>
		public static class VersionControl
		{
			private const StatusQueryOptions DefaultStatusQueryOption = StatusQueryOptions.UseCachedIfPossible;

			/// <summary>
			///     Returns true if the asset can be opened for editing in the version control system.
			/// </summary>
			/// <remarks>
			///     If this method returns false, CodeSmileEditor.Asset.GetLastErrorMessage() returns the error message.
			/// </remarks>
			/// <param name="path">Path to an asset.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/StatusQueryOptions.html">StatusQueryOptions</a>
			/// </param>
			/// <returns>True if the path can be opened for editing, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.MakeEditable" />
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.IsEditable" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CanOpenForEdit.html">AssetDatabase.CanOpenForEdit</a>
			/// </seealso>
			public static Boolean CanMakeEditable(Path path, StatusQueryOptions options = DefaultStatusQueryOption)
			{
				var canOpen = AssetDatabase.CanOpenForEdit(path, out var message, options);
				if (canOpen == false)
					SetLastErrorMessage(message);

				return canOpen;
			}

			/// <summary>
			///     Returns true if the asset can be opened for editing in the version control system.
			/// </summary>
			/// <remarks>
			///     If this method returns false, CodeSmileEditor.Asset.GetLastErrorMessage() returns the error message.
			/// </remarks>
			/// <param name="asset">Instance of an asset.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/StatusQueryOptions.html">StatusQueryOptions</a>
			/// </param>
			/// <returns>True if the path can be opened for editing, false otherwise.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.MakeEditable" />
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.IsEditable" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CanOpenForEdit.html">AssetDatabase.CanOpenForEdit</a>
			/// </seealso>
			public static Boolean CanMakeEditable(Object asset, StatusQueryOptions options = DefaultStatusQueryOption)
			{
				ThrowIf.ArgumentIsNull(asset, nameof(asset));

				return CanMakeEditable(Path.Get(asset), options);
			}

			/// <summary>
			///     Tests which assets can be made editable and provides a list of paths that cannot be opened for editing in the
			///     version control system.
			/// </summary>
			/// <remarks>To get a failure message query each individual path. </remarks>
			/// <param name="paths">Paths to assets.</param>
			/// <param name="notEditablePaths">List of paths that cannot be opened for editing. List is empty if all can be opened.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/StatusQueryOptions.html">StatusQueryOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.MakeEditable" />
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.IsEditable" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CanOpenForEdit.html">AssetDatabase.CanOpenForEdit</a>
			/// </seealso>
			public static void CanMakeEditable(Path[] paths, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption) =>
				CanMakeEditable(Path.ToStrings(paths), out notEditablePaths, options);

			/// <summary>
			///     Tests which assets can be made editable and provides a list of paths that cannot be opened for editing in the
			///     version control system.
			/// </summary>
			/// <remarks>To get a failure message query each individual path. </remarks>
			/// <param name="paths">Instances of assets.</param>
			/// <param name="notEditablePaths">List of paths that cannot be opened for editing. List is empty if all can be opened.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/StatusQueryOptions.html">StatusQueryOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.MakeEditable" />
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.IsEditable" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CanOpenForEdit.html">AssetDatabase.CanOpenForEdit</a>
			/// </seealso>
			public static void CanMakeEditable(String[] paths, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption)
			{
				notEditablePaths = new List<String>();
				AssetDatabase.CanOpenForEdit(paths, notEditablePaths, options);
			}

			/// <summary>
			///     Tests which assets can be made editable and provides a list of paths that cannot be opened for editing in the
			///     version control system.
			/// </summary>
			/// <remarks>To get a failure message query each individual path. </remarks>
			/// <param name="assets">Instances of assets.</param>
			/// <param name="notEditablePaths">List of paths that cannot be opened for editing. List is empty if all can be opened.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/StatusQueryOptions.html">StatusQueryOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.MakeEditable" />
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.IsEditable" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.CanOpenForEdit.html">AssetDatabase.CanOpenForEdit</a>
			/// </seealso>
			public static void CanMakeEditable(Object[] assets, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption) =>
				CanMakeEditable(Path.Get(assets), out notEditablePaths, options);

			/// <summary>
			///     Returns true if the meta file is open for editing in the version control system.
			/// </summary>
			/// <remarks>
			///     If this method returns false, CodeSmileEditor.Asset.GetLastErrorMessage() returns the error message.
			/// </remarks>
			/// <param name="asset">Instance of an asset.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/StatusQueryOptions.html">StatusQueryOptions</a>
			/// </param>
			/// <returns></returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.IsEditable" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsMetaFileOpenForEdit.html">AssetDatabase.IsMetaFileOpenForEdit</a>
			/// </seealso>
			public static Boolean IsMetaEditable(Object asset, StatusQueryOptions options = DefaultStatusQueryOption)
			{
				var isOpen = AssetDatabase.IsMetaFileOpenForEdit(asset, out var message, options);
				if (isOpen == false)
					SetLastErrorMessage(message);

				return isOpen;
			}

			/// <summary>
			///     Returns true if the asset file is open for editing in the version control system.
			/// </summary>
			/// <remarks>
			///     If this method returns false, CodeSmileEditor.Asset.GetLastErrorMessage() returns the error message.
			/// </remarks>
			/// <param name="asset">Instance of an asset.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/StatusQueryOptions.html">StatusQueryOptions</a>
			/// </param>
			/// <returns></returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.IsMetaEditable" />
			///     - <see cref="CodeSmileEditor.Asset.GetLastErrorMessage" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsOpenForEdit.html">AssetDatabase.IsOpenForEdit</a>
			/// </seealso>
			public static Boolean IsEditable(Object asset, StatusQueryOptions options = DefaultStatusQueryOption)
			{
				var isOpen = AssetDatabase.IsOpenForEdit(asset, out var message, options);
				if (isOpen == false)
					SetLastErrorMessage(message);

				return isOpen;
			}

			/// <summary>
			///     Tests if the assets can be opened for editing in the version control system.
			/// </summary>
			/// <remarks>To get an error message query each failed path individually.</remarks>
			/// <param name="paths">Paths to assets.</param>
			/// <param name="notEditablePaths">List of paths that are not editable. Is empty if all paths are editable.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/StatusQueryOptions.html">StatusQueryOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.CanMakeEditable" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsOpenForEdit.html">AssetDatabase.IsOpenForEdit</a>
			/// </seealso>
			public static void IsEditable(Path[] paths, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption) =>
				IsEditable(Path.ToStrings(paths), out notEditablePaths, options);

			/// <summary>
			///     Tests if the assets can be opened for editing in the version control system.
			/// </summary>
			/// <remarks>To get an error message query each failed path individually.</remarks>
			/// <param name="paths">Paths to assets.</param>
			/// <param name="notEditablePaths">List of paths that are not editable. Is empty if all paths are editable.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/StatusQueryOptions.html">StatusQueryOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.CanMakeEditable" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsOpenForEdit.html">AssetDatabase.IsOpenForEdit</a>
			/// </seealso>
			public static void IsEditable(String[] paths, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption)
			{
				notEditablePaths = new List<String>();
				AssetDatabase.IsOpenForEdit(paths, notEditablePaths, options);
			}

			/// <summary>
			///     Tests if the assets can be opened for editing in the version control system.
			/// </summary>
			/// <remarks>To get an error message query each failed path individually.</remarks>
			/// <param name="assets">Instances of assets.</param>
			/// <param name="notEditablePaths">List of paths that are not editable. Is empty if all paths are editable.</param>
			/// <param name="options">
			///     <a href="https://docs.unity3d.com/ScriptReference/StatusQueryOptions.html">StatusQueryOptions</a>
			/// </param>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.CanMakeEditable" />
			///     -
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.IsOpenForEdit.html">AssetDatabase.IsOpenForEdit</a>
			/// </seealso>
			public static void IsEditable(Object[] assets, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption) =>
				IsEditable(Path.Get(assets), out notEditablePaths, options);

			/// <summary>
			///     Tries to open the path for editing in the version control system.
			/// </summary>
			/// <remarks>
			///     Use CodeSmileEditor.Asset.VersionControl.CanMakeEditable to get an error message if this
			///     method returns false.
			/// </remarks>
			/// <param name="path">Path to an asset.</param>
			/// <returns>True if the path is now editable, false if at least one failed to open.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.CanMakeEditable" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MakeEditable.html">AssetDatabase.MakeEditable</a>
			/// </seealso>
			public static Boolean MakeEditable(Path path) => AssetDatabase.MakeEditable(path);

			/// <summary>
			///     Tries to open multiple paths for editing in the version control system.
			/// </summary>
			/// <remarks>
			///     Use CodeSmileEditor.Asset.VersionControl.CanMakeEditable to get an error message for
			///     individual paths.
			/// </remarks>
			/// <param name="paths">Paths to assets.</param>
			/// <param name="notEditablePaths">List of paths that are not editable. Is empty if all paths are editable.</param>
			/// <returns>True if the path is now editable, false if at least one failed to open.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.CanMakeEditable" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MakeEditable.html">AssetDatabase.MakeEditable</a>
			/// </seealso>
			public static Boolean MakeEditable(Path[] paths, out List<String> notEditablePaths) =>
				MakeMultipleEditable(Path.ToStrings(paths), out notEditablePaths);

			/// <summary>
			///     Tries to open multiple paths for editing in the version control system.
			/// </summary>
			/// <remarks>
			///     Use CodeSmileEditor.Asset.VersionControl.CanMakeEditable to get an error message for
			///     individual paths.
			/// </remarks>
			/// <param name="paths">Paths to assets.</param>
			/// <param name="notEditablePaths">List of paths that are not editable. Is empty if all paths are editable.</param>
			/// <returns>True if the paths are now editable, false if at least one failed to open.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.CanMakeEditable" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MakeEditable.html">AssetDatabase.MakeEditable</a>
			/// </seealso>
			public static Boolean MakeEditable(String[] paths, out List<String> notEditablePaths) =>
				MakeMultipleEditable(paths, out notEditablePaths);

			/// <summary>
			///     Tries to open multiple paths for editing in the version control system. Shows a prompt to the user
			///     unless the editor is in batch operation mode.
			/// </summary>
			/// <remarks>
			///     Use CodeSmileEditor.Asset.VersionControl.CanMakeEditable to get an error message for
			///     individual paths.
			/// </remarks>
			/// <param name="paths">Paths to assets.</param>
			/// <param name="notEditablePaths">List of paths that are not editable. Is empty if all paths are editable.</param>
			/// <param name="prompt">A message for the interactive dialog or null to use the default message.</param>
			/// <returns>True if the paths are now editable, false if at least one failed to open.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.CanMakeEditable" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MakeEditable.html">AssetDatabase.MakeEditable</a>
			/// </seealso>
			public static Boolean MakeEditableInteractive(Path[] paths, out List<String> notEditablePaths,
				String prompt = null) => MakeMultipleEditable(Path.ToStrings(paths), out notEditablePaths,
				prompt != null ? prompt : "Open for Edit?");

			/// <summary>
			///     Tries to open multiple paths for editing in the version control system. Shows a prompt to the user
			///     unless the editor is in batch operation mode.
			/// </summary>
			/// <remarks>
			///     Use CodeSmileEditor.Asset.VersionControl.CanMakeEditable to get an error message for
			///     individual paths.
			/// </remarks>
			/// <param name="paths">Paths to assets.</param>
			/// <param name="notEditablePaths">List of paths that are not editable. Is empty if all paths are editable.</param>
			/// <param name="prompt">A message for the interactive dialog or null to use the default message.</param>
			/// <returns>True if the paths are now editable, false if at least one failed to open.</returns>
			/// <seealso cref="">
			///     - <see cref="CodeSmileEditor.Asset.VersionControl.CanMakeEditable" />
			///     - <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MakeEditable.html">AssetDatabase.MakeEditable</a>
			/// </seealso>
			public static Boolean MakeEditableInteractive(String[] paths, out List<String> notEditablePaths,
				String prompt = null) => MakeMultipleEditable(paths, out notEditablePaths,
				prompt != null ? prompt : "Open for Edit?");

			private static Boolean MakeMultipleEditable(String[] paths, out List<String> notEditablePaths,
				String prompt = null)
			{
				notEditablePaths = new List<String>();
				return AssetDatabase.MakeEditable(paths, prompt, notEditablePaths);
			}
		}
	}
}
