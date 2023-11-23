# CodeSmile AssetDatabase

It's Unity's AssetDatabase except now it's clean, concise, consistent and powerful.

## Examples

My name is CodeSmile for a reason. Read on. :)

### Load or create asset

A common use case: if an asset cannot be loaded, create it. That only slightly complicates things with the AssetDatabase:

```
public static LevelData GetLevelDataAsset(int level)
{
  string assetPath = "Assets/DataStore/Level{level}/LevelData.asset";

  var levelData = AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);
  if (levelData == null)
  {
	// create datastore folder if not exists
	if (!AssetDatabase.IsValidFolder(@"Assets/DataStore"))
	  AssetDatabase.CreateFolder("Assets", "DataStore");

	// create level folder if not exists
	if (!AssetDatabase.IsValidFolder(@"Assets/DataStore/Level{level}"))
	  AssetDatabase.CreateFolder("Assets/DataStore", "Level{level}");

	levelData = CreateInstance<LevelData>();
	AssetDatabase.CreateAsset(levelData, assetPath);
  }
  
  return levelData;
}
```

Now what used to be 20+ lines of code is just this:

```
public static LevelData GetLevelDataAsset(int level)
{
	string assetPath = "Assets/DataStore/Level{level}/LevelData.asset";
	return Asset.LoadOrCreate<LevelData>(assetPath, () => CreateInstance<LevelData>());
}
```

### Load an asset 

Easy:

`Asset levelData = "Assets/Example/LevelData.asset";`

Wait .. wut?? :)

I see, you have a GUID? In that case:

`Asset levelData = thisIsYourGuid;`

Or if you already have an instance loaded:

`Asset levelData = yourAssetObject;`

The opposite also works, of course:

`UnityEngine.Object obj = asset;`

Not the right type? Just cast it:

`var levelData = (LevelData)asset;`

The Asset instance provides you access to everything you might want to do with an asset. Save, copy, rename, delete, you name it. 

You also get an asset's Labels, GUID, FileId, path, .meta path and so on. It's right where you expect it.

### Get an asset's path

Assuming you have an `asset` instance:

`var assetPath = asset.AssetPath;`

Oh right, you need the meta file path?
Well, you could inquire the AssetDatabase:

`var metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);`

But I'd much rather have you do this:

`var metaPath = asset.MetaPath;`

Or if you just need to work with just paths:

```
Path assetPath = "Assets/Folder/LevelData.asset";
Path metaPath = assetPath.MetaPath;
```

The secret sauce behind Asset.Path is that you never EVER need to worry about the path being malformed, containing illegal characters, mixing forward with backward slashes, or leaving leading/trailing slashes. 

Asset.Path also gives access to commonly used System.IO.Path functionality:

```
Path assetPath = "Assets/Folder/LevelData.asset";
Path assetDir = assetPath.DirectoryPath;
String assetFileName = assetPath.FileName;
String assetFileNameNoExt = assetPath.FileNameWithoutExtension;
String assetExtension = assetPath.Extension;
```

### Be nice, be concise!

Thus far you were forced to write rather verbose, convoluted code. This package will improve your work-smile balance. :)

---

`string[] paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);`

`string[] paths = Asset.Bundle.GetPaths(bundleName, assetName);`

---

`uint count = AssetDatabase.UnregisterCustomDependencyPrefixFilter(prefix);`

`uint count = Asset.Dependency.Remove(prefix);`

---

`var metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);`

`var metaPath = Asset.Path.ToMeta(assetPath);`

## Documentation

[Manual + API Reference](https://codesmile-0000011110110111.github.io/de.codesmile.editor.assetdatabase/html/index.html)

[AssetDatabase to CodeSmile AssetDatabase](https://docs.google.com/spreadsheets/d/134BEPXTx3z80snNAF3Gafgq3j5kEhmFzFBKT_z1s6Rw/edit?usp=sharing)
This is a spreadsheet that maps all AssetDatabase methods to their counterpart. Notice the logical grouping and simple, consistent naming scheme.

## Requirements

- Unity 2021.3.3f1 or newer
- A smile :)

## Installation

This software is a Unity Package Manager 'npm package'.

- Open Window => Package Manager in Unity Editor
- Choose "Install package from git URL..."
- Enter this URL: `https://github.com/CodeSmile-0000011110110111/de.codesmile.editor.assetinspector.git`

This package is currently not available on OpenUPM.

## License

This software is licensed under the GNU General Public License v3.0 (GPL 3.0). The main implication is that any work you build using this software requires the entire work to be published under the GPL.

This software will also be available on the Unity Asset Store under the Asset Store EULA.

If you wish to license this software under different terms please contact me!

- Steffen aka CodeSmile
- [Email](mailto:steffen@steffenitterheim.de) / [Discord](https://discord.gg/JN3Jz8qkeV)
