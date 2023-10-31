// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using UnityEditor;
using UnityEngine;

public class AssetPathTests
{
	[TestCase("Assets", "file name")]
	[TestCase("Assets/", "file name")]
	[TestCase("Assets\\", "file name")]
	[TestCase("Assets/Subfolder", "file name")]
	[TestCase("Assets/Subfolder/", "file name")]
	[TestCase("Assets\\Subfolder", "file name")]
	[TestCase("Assets\\Subfolder\\", "file name")]
	[TestCase("Assets/Subfolder\\", "file name")]
	[TestCase("Assets\\Subfolder/", "file name")]
	public void Combine_RelativePaths_AsExpected(String path, String filename)
	{
		var normalizedPath = Asset.Path.ToLogical(path).Trim('/');
		Assert.AreEqual($"{normalizedPath}/{filename}.asset", Asset.Path.Combine(path, filename));
	}

	[Test] public void Combine_FullPath_AsExpected()
	{
		var fullPath = Application.dataPath + "/sub";
		Assert.AreEqual("Assets/sub/FN.asset", Asset.Path.Combine(fullPath, "FN"));
	}

	[TestCase("custom")]
	[TestCase(".custom")]
	public void Combine_WithExtension_AsExpected(String extension)
	{
		var path = Application.dataPath;
		Assert.AreEqual($"Assets/FN.{extension.TrimStart('.')}", Asset.Path.Combine(path, "FN", extension));
	}

	[TestCase(null, "file name", "extension")]
	[TestCase("Assets", null, "extension")]
	[TestCase("Assets", "file name", null)]
	public void Combine_NullStrings_Throws(String path, String filename, String extension) =>
		Assert.Throws<ArgumentNullException>(() => Asset.Path.Combine(path, filename, extension));

	[TestCase(" ", "file name", "extension")]
	[TestCase("Assets", " ", "extension")]
	[TestCase("Assets", "file name", " ")]
	public void Combine_WhitespaceStrings_Throws(String path, String filename, String extension) =>
		Assert.Throws<ArgumentException>(() => Asset.Path.Combine(path, filename, extension));

	[TestCase("Assets", "Assets")]
	[TestCase("Assets/", "Assets/")]
	[TestCase("Assets\\", "Assets/")]
	[TestCase("/Assets/folder", "Assets/folder")]
	[TestCase("\\Assets\\folder", "Assets/folder")]
	public void ToLogical_Convert_AsExpected(String input, String expected) =>
		Assert.AreEqual(expected, Asset.Path.ToLogical(input));

	[TestCase("/Assets/AssetDB.unity")]
	public void ToLogical_LeadingSlash_IsRemoved(String path) =>
		Assert.NotNull(AssetDatabase.LoadAssetAtPath<SceneAsset>(Asset.Path.ToLogical(path)));

	[TestCase("Assets")]
	[TestCase("Assets/")]
	[TestCase("\\Assets\\")]
	[TestCase("Assets/AssetDB.unity")]
	[TestCase("/Assets/AssetDB.unity")]
	public void IsRelative_RelativePaths_IsTrue(String path) => Assert.True(Asset.Path.IsRelative(path));

	[TestCase(null)]
	[TestCase(" ")]
	[TestCase("AssetsSomething/folder")]
	[TestCase("C:\\Assets")]
	public void IsRelative_NotRelativePaths_IsFalse(String path) => Assert.False(Asset.Path.IsRelative(path));

	[TestCase("AssetDB.unity")]
	[TestCase("/AssetDB.unity")]
	[TestCase("some path/that is not starting with/Assets")]
	public void ToRelative_DoesNotStartWithAssets_Throws(String path) =>
		Assert.Throws<ArgumentException>(() => Asset.Path.ToRelative(path));

	[TestCase("Assets/AssetDB.unity")]
	[TestCase("/Assets/AssetDB.unity")]
	[TestCase("\\Assets/AssetDB.unity")]
	public void ToRelative_AlreadyRelativePath_ReturnsInput(String path) =>
		Assert.AreEqual(path.NormalizePathSeparators().TrimStart('/'), Asset.Path.ToRelative(path));

	/// <summary>
	///     These paths do not point to the *current* project's root or subfolder.
	/// </summary>
	/// <param name="path"></param>
	[TestCase("C:\\Users\\Urso Clever\\Untiy Projects\\First Porject\\Assets\\folder")]
	[TestCase("\\Users\\Urso Clever\\Untiy Projects\\First Porject\\Assets\\folder")]
	public void ToRelative_ConvertWrongProjectFullPath_Throws(String path) =>
		Assert.Throws<ArgumentException>(() => Asset.Path.ToRelative(path));

	[Test] public void ToRelative_ConvertProjectFullPath_AsExpected() =>
		Assert.AreEqual("Assets/sub", Asset.Path.ToRelative(Application.dataPath + "/sub"));
}
