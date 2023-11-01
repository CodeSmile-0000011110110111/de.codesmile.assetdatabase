// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using UnityEngine;
using Object = System.Object;

public class AssetPathTests
{
	[Test] public void AssetPath_DefaultCtor_EqualsAssetsString() =>
		Assert.AreEqual("Assets", new AssetPath().ToString());

	[Test] public void AssetPath_DefaultCtor_ImplicitConversionEqualsAssetsString() =>
		Assert.AreEqual("Assets", new AssetPath());

	[Test] public void AssetPath_DefaultCtor_ExplicitConversionEqualsAssetsString()
	{
		Assert.AreEqual("Assets", (String)new AssetPath());
		new Action<AssetPath>(assetPath => Assert.AreEqual("Assets", assetPath)).Invoke((AssetPath)"Assets");
	}

	[Test] public void AssetPath_NullString_Throws() => Assert.Throws<ArgumentNullException>(() => new AssetPath(null));

	[Test] public void AssetPath_EmptyString_Throws() =>
		Assert.Throws<ArgumentException>(() => new AssetPath(String.Empty));

	[Test] public void AssetPath_WhitespaceString_Throws() =>
		Assert.Throws<ArgumentException>(() => new AssetPath(" "));

	[TestCase("AssetsData")]
	[TestCase("SomeFolder/")]
	[TestCase("Some\\Folder\\")]
	public void AssetPath_RelativePathDoesNotStartWithAssets_Throws(String inputPath) =>
		Assert.Throws<ArgumentException>(() => new AssetPath(inputPath));

	[TestCase("Assets", "Assets")]
	[TestCase("assets", "assets")]
	[TestCase("Assets/", "Assets/")]
	[TestCase("Assets\\", "Assets/")]
	[TestCase("/Assets/folder", "Assets/folder")]
	[TestCase("\\Assets\\folder", "Assets/folder")]
	[TestCase("/Assets/folder\\", "Assets/folder")]
	[TestCase("\\Assets\\folder/", "Assets/folder")]
	public void AssetPath_WithRelativePath_AsExpected(String inputPath, String expected) =>
		Assert.AreEqual(expected, new AssetPath(inputPath));

	[TestCase("Assets", "file name", "Assets/file name.asset")]
	[TestCase("assets", "file name", "assets/file name.asset")]
	[TestCase("Assets/", "file name", "Assets/file name.asset")]
	[TestCase("Assets\\", "file name", "Assets/file name.asset")]
	[TestCase("/Assets/folder", "file name", "Assets/folder/file name.asset")]
	[TestCase("\\Assets\\folder", "file name", "Assets/folder/file name.asset")]
	[TestCase("/Assets/folder\\", "file name", "Assets/folder/file name.asset")]
	[TestCase("\\Assets\\folder/", "file name", "Assets/folder/file name.asset")]
	public void AssetPath_CombinePathAndFileName_AsExpected(String inputPath, String fileName,
		String expected) => Assert.AreEqual(expected, new AssetPath(inputPath, fileName));

	[TestCase("\\Assets\\folder/", "fn", "text", "Assets/folder/fn.text")]
	public void AssetPath_CombinePathFileNameExtension_AsExpected(String inputPath, String fileName, String extension,
		String expected) => Assert.AreEqual(expected, new AssetPath(inputPath, fileName, extension));

	[Test] public void AssetPath_WithFullPath_IsRelativePath()
	{
		var filePath = "/sub/FN.test";
		var fullPath = Application.dataPath + filePath;
		Assert.AreEqual("Assets" + filePath, new AssetPath(fullPath));
	}

	[Test] public void AssetPath_WithFullPath_EqualsFullPath()
	{
		var fullPath = Application.dataPath + "/sub/FN.test";
		Assert.AreEqual(fullPath, new AssetPath(fullPath).FullPath);
	}

	[Test] public void AssetPath_WithFullPathBackslashes_EqualsFullPath()
	{
		var fullPath = Application.dataPath + "/sub/FN.test";
		Assert.AreEqual(fullPath, new AssetPath(fullPath.Replace('/', '\\')).FullPath);
	}

	[Test] public void AssetPath_CombineFullPath_MadeRelative() =>
		Assert.AreEqual("Assets/FN.test", new AssetPath(Application.dataPath, "FN", ".test"));

	[TestCase("/filename")]
	[TestCase("filename/")]
	[TestCase("\\filename\\")]
	[TestCase("file/name")]
	[TestCase("file\\name")]
	public void AssetPath_CombineFileWithSeparators_Throws(String fileName) =>
		Assert.Throws<ArgumentException>(() => new AssetPath("Assets", fileName));

	[TestCase("ext")] [TestCase(".ext")]
	public void AssetPath_CombineExtension_AsExpected(String extension) =>
		Assert.AreEqual("Assets/FN." + extension.TrimStart('.'), new AssetPath("Assets", "FN", extension));

	[TestCase(null, "file name", "extension")]
	[TestCase("Assets", null, "extension")]
	[TestCase("Assets", "file name", null)]
	public void AssetPath_CombineNullStrings_Throws(String path, String filename, String extension) =>
		Assert.Throws<ArgumentNullException>(() => new AssetPath(path, filename, extension));

	[TestCase(" ", "file name", "extension")]
	[TestCase("Assets", " ", "extension")]
	[TestCase("Assets", "file name", " ")]
	public void AssetPath_CombineWhitespaceStrings_Throws(String path, String filename, String extension) =>
		Assert.Throws<ArgumentException>(() => new AssetPath(path, filename, extension));

	/// <summary>
	///     These paths do not point to the *current* project's root or subfolder.
	/// </summary>
	/// <param name="path"></param>
	[TestCase("C:\\Users\\Urso Clever\\Untiy Projects\\First Porject\\Assets\\folder")]
	[TestCase("\\Users\\Urso Clever\\Untiy Projects\\First Porject\\Assets\\folder")]
	public void AssetPath_NotAProjectPath_Throws(String path) =>
		Assert.Throws<ArgumentException>(() => new AssetPath(path));

	[Test] public void AssetPath_Equality_EqualToSelf()
	{
		var assetPath = new AssetPath(Application.dataPath);
		Assert.True(assetPath.Equals(assetPath));

#pragma warning disable 1718 // did you maybe not want to compare x with itself? Yes, I meant to!
		Assert.True(assetPath == assetPath);
		Assert.False(assetPath != assetPath);
#pragma warning restore 1718
	}

	[Test] public void AssetPath_Equality_EqualToOther()
	{
		Assert.True(new AssetPath("\\Assets\\folder").Equals(new AssetPath("Assets/folder/")));
		Assert.True(new AssetPath(Application.dataPath + "\\") == new AssetPath(Application.dataPath));
	}

	[Test] public void AssetPath_Equality_NotEqualToOther()
	{
		Assert.True(new AssetPath(Application.dataPath + "/file.ext") != new AssetPath(Application.dataPath));
		Assert.False(new AssetPath(Application.dataPath + "/file.ext").Equals(new AssetPath(Application.dataPath)));
	}

	[Test] public void AssetPath_Equality_EqualToString()
	{
		Assert.True("Assets".Equals(new AssetPath(Application.dataPath)));
		Assert.True("Assets" == new AssetPath(Application.dataPath));
		Assert.True("Assets/file.ext".Equals(new AssetPath(Application.dataPath + "/file.ext")));
		Assert.True("Assets/file.ext" == new AssetPath(Application.dataPath + "/file.ext"));
		Assert.True(new AssetPath(Application.dataPath + "/file.ext").Equals("Assets\\file.ext"));
		Assert.True(new AssetPath(Application.dataPath + "/file.ext") == "Assets/file.ext");
	}

	[Test] public void AssetPath_Equality_NotEqualToString()
	{
		Assert.AreNotEqual(Application.dataPath, new AssetPath(Application.dataPath + "/file.ext"));
		Assert.AreNotEqual(null, new AssetPath(Application.dataPath + "/file.ext"));
	}

	[Test] public void AssetPath_Equality_EqualToObject()
	{
		Assert.True(new AssetPath("\\Assets\\folder").Equals((Object)new AssetPath("Assets/folder/")));
		Assert.True(new AssetPath(Application.dataPath + "\\") == (Object)new AssetPath(Application.dataPath));
		Assert.True((Object)new AssetPath(Application.dataPath + "\\") == new AssetPath(Application.dataPath));
	}

	[Test] public void AssetPath_Equality_NotEqualToObject()
	{
		Assert.True(new AssetPath(Application.dataPath + "/file.ext") != (Object)new AssetPath(Application.dataPath));
		Assert.True((Object)new AssetPath(Application.dataPath + "/file.ext") != new AssetPath(Application.dataPath));
		Assert.False(new AssetPath(Application.dataPath + "/f.x").Equals((Object)new AssetPath(Application.dataPath)));
	}

	[Test] public void AssetPath_Equality_NotEqualToNull()
	{
		Assert.True(null != new AssetPath(Application.dataPath));
		Assert.True(new AssetPath(Application.dataPath) != null);
		Assert.False(null == new AssetPath(Application.dataPath));
		Assert.False(new AssetPath(Application.dataPath) == null);
		Assert.False(new AssetPath(Application.dataPath + "/file.ext").Equals(null));
	}

	[Test] public void AssetPath_GetHashCode_SameAsToStringHashCode()
	{
		var assetPath = new AssetPath(Application.dataPath);
		Assert.AreEqual(assetPath.GetHashCode(), assetPath.ToString().GetHashCode());
	}
}
