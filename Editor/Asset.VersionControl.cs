// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		private const StatusQueryOptions DefaultStatusQueryOption = StatusQueryOptions.UseCachedIfPossible;

		/// <summary>
		///     Groups version control related functionality.
		/// </summary>
		public static class VersionControl
		{
			/// <summary>
			///     Returns true if the asset can be opened for editing in the version control system.
			///     If false, GetLastErrorMessage() returns the reason.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="options"></param>
			/// <returns></returns>
			/// <see cref="Asset.GetLastErrorMessage" />
			public static Boolean CanMakeEditable(Path path, StatusQueryOptions options = DefaultStatusQueryOption)
			{
				var canOpen = AssetDatabase.CanOpenForEdit(path, out var message, options);
				if (canOpen == false)
					SetLastErrorMessage(message);

				return canOpen;
			}

			/// <summary>
			///     Returns true if the asset can be opened for editing in the version control system.
			///     If false, GetLastErrorMessage() returns the reason.
			/// </summary>
			/// <param name="obj"></param>
			/// <param name="options"></param>
			/// <returns></returns>
			/// <see cref="Asset.GetLastErrorMessage" />
			public static Boolean CanMakeEditable(Object obj, StatusQueryOptions options = DefaultStatusQueryOption)
			{
				ThrowIf.ArgumentIsNull(obj, nameof(obj));

				return CanMakeEditable(Path.Get(obj), options);
			}

			/// <summary>
			///     Returns a list of notEditablePaths that cannot be opened for editing in the version control system.
			///     To get a reason, query the individual paths again.
			/// </summary>
			/// <param name="paths"></param>
			/// <param name="notEditablePaths"></param>
			/// <param name="options"></param>
			/// <see cref="CanMakeEditable(CodeSmile.Editor.Asset.Path,UnityEditor.StatusQueryOptions)" />
			public static void CanMakeEditable(Path[] paths, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption) =>
				CanMakeEditable(Path.ToStrings(paths), out notEditablePaths, options);

			/// <summary>
			///     Returns a list of notEditablePaths that cannot be opened for editing in the version control system.
			///     To get a reason, query the individual paths again.
			/// </summary>
			/// <param name="objects"></param>
			/// <param name="notEditablePaths"></param>
			/// <param name="options"></param>
			/// <see cref="CanMakeEditable(CodeSmile.Editor.Asset.Path,UnityEditor.StatusQueryOptions)" />
			public static void CanMakeEditable(Object[] objects, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption) =>
				CanMakeEditable(Path.ToAssetPaths(objects), out notEditablePaths, options);

			/// <summary>
			///     Returns a list of notEditablePaths that cannot be opened for editing in the version control system.
			///     To get a reason, query the individual paths again.
			/// </summary>
			/// <param name="paths"></param>
			/// <param name="notEditablePaths"></param>
			/// <param name="options"></param>
			/// <see cref="CanMakeEditable(CodeSmile.Editor.Asset.Path,UnityEditor.StatusQueryOptions)" />
			public static void CanMakeEditable(String[] paths, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption)
			{
				notEditablePaths = new List<String>();
				AssetDatabase.CanOpenForEdit(paths, notEditablePaths, options);
			}

			/// <summary>
			///     Returns true if the meta file is open for editing in the version control system.
			///     If false, GetLastErrorMessage() contains the failure reason.
			/// </summary>
			/// <param name="obj"></param>
			/// <param name="options"></param>
			/// <returns></returns>
			public static Boolean IsMetaEditable(Object obj, StatusQueryOptions options = DefaultStatusQueryOption)
			{
				var isOpen = AssetDatabase.IsMetaFileOpenForEdit(obj, out var message, options);
				if (isOpen == false)
					SetLastErrorMessage(message);

				return isOpen;
			}

			/// <summary>
			///     Returns true if the asset file is open for editing in the version control system.
			///     If false, GetLastErrorMessage() contains the failure reason.
			/// </summary>
			/// <param name="obj"></param>
			/// <param name="options"></param>
			/// <returns></returns>
			public static Boolean IsEditable(Object obj, StatusQueryOptions options = DefaultStatusQueryOption)
			{
				var isOpen = AssetDatabase.IsOpenForEdit(obj, out var message, options);
				if (isOpen == false)
					SetLastErrorMessage(message);

				return isOpen;
			}

			/// <summary>
			///     Returns a list of notEditablePaths that cannot be opened for editing in the version control system.
			///     To get a reason, query the individual paths again.
			/// </summary>
			/// <param name="paths"></param>
			/// <param name="notEditablePaths"></param>
			/// <param name="options"></param>
			/// <see cref="CanMakeEditable(CodeSmile.Editor.Asset.Path,UnityEditor.StatusQueryOptions)" />
			public static void IsEditable(Path[] paths, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption) =>
				IsEditable(Path.ToStrings(paths), out notEditablePaths, options);

			/// <summary>
			///     Returns a list of notEditablePaths that cannot be opened for editing in the version control system.
			///     To get a reason, query the individual objects again.
			/// </summary>
			/// <param name="objects"></param>
			/// <param name="notEditablePaths"></param>
			/// <param name="options"></param>
			/// <see cref="CanMakeEditable(CodeSmile.Editor.Asset.Path,UnityEditor.StatusQueryOptions)" />
			public static void IsEditable(Object[] objects, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption) =>
				IsEditable(Path.ToAssetPaths(objects), out notEditablePaths, options);

			/// <summary>
			///     Returns a list of notEditablePaths that cannot be opened for editing in the version control system.
			///     To get a reason, query the individual paths again.
			/// </summary>
			/// <param name="paths"></param>
			/// <param name="notEditablePaths"></param>
			/// <param name="options"></param>
			/// <see cref="CanMakeEditable(CodeSmile.Editor.Asset.Path,UnityEditor.StatusQueryOptions)" />
			public static void IsEditable(String[] paths, out List<String> notEditablePaths,
				StatusQueryOptions options = DefaultStatusQueryOption)
			{
				notEditablePaths = new List<String>();
				AssetDatabase.IsOpenForEdit(paths, notEditablePaths, options);
			}

			/// <summary>
			///     Opens the file at path for editing in the version control system.
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MakeEditable.html">Unity Manual</a>
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public static Boolean MakeEditable(Path path) => AssetDatabase.MakeEditable(path);

			/// <summary>
			///     Opens the file paths for editing in the version control system.
			///     Returns true if all could be opened. Returns false if one or more failed to open and the list of notEditablePaths.
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MakeEditable.html">Unity Manual</a>
			/// </summary>
			/// <param name="paths"></param>
			/// <param name="notEditablePaths"></param>
			/// <returns></returns>
			public static Boolean MakeEditable(Path[] paths, out List<String> notEditablePaths) =>
				MakeEditableInternal(Path.ToStrings(paths), out notEditablePaths);

			/// <summary>
			///     Opens the file paths for editing in the version control system.
			///     Returns true if all could be opened. Returns false if one or more failed to open and the list of notEditablePaths.
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MakeEditable.html">Unity Manual</a>
			/// </summary>
			/// <param name="paths"></param>
			/// <param name="notEditablePaths"></param>
			/// <returns></returns>
			public static Boolean MakeEditable(String[] paths, out List<String> notEditablePaths) =>
				MakeEditableInternal(paths, out notEditablePaths);

			/// <summary>
			///     Opens the file paths for editing in the version control system. Shows a prompt to the user.
			///     Returns true if all could be opened. Returns false if one or more failed to open and the list of notEditablePaths.
			///     <a href="https://docs.unity3d.com/ScriptReference/AssetDatabase.MakeEditable.html">Unity Manual</a>
			/// </summary>
			/// <param name="paths"></param>
			/// <param name="notEditablePaths"></param>
			/// <param name="prompt">a custom prompt or null to use default prompt</param>
			/// <returns></returns>
			public static Boolean MakeEditableInteractive(String[] paths, out List<String> notEditablePaths,
				String prompt = null) => MakeEditableInternal(paths, out notEditablePaths,
				prompt != null ? prompt : "Open for Edit?");

			private static Boolean MakeEditableInternal(String[] paths, out List<String> notEditablePaths,
				String prompt = null)
			{
				notEditablePaths = new List<String>();
				return AssetDatabase.MakeEditable(paths, null, notEditablePaths);
			}
		}
	}
}
