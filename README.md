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
	// Load returns null does *NOT* mean the file doesn't exist!
	// A user may have 'Auto Refresh' disabled => the asset may not have been imported
	AssetDatabase.ImportAsset(assetPath);
	
	// try loading it again now:
	levelData = AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);
  }

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

Now what used to be over 20 lines of code is just this, with all edge-cases taken care of:

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
Yup, that's correct!

You have a GUID? In that case:

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
Asset.Path assetPath = "Assets/Folder/LevelData.asset";
Asset.Path metaPath = assetPath.MetaPath;
```

The secret sauce behind Asset.Path is that you never EVER need to worry about the path being malformed, containing illegal characters, leaving leading/trailing slashes, or the paths not working on Mac OS or Linux due to backslashes. 

Asset.Path also accepts absolute paths and makes them relative. But if the path is pointing outside the project, perhaps another project because you copy/pasted that code, it'll throw an exception rather than letting you modify assets in unrelated (!) projects. Yes, that can happen, with potentially devastating results!  

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

Mind you, those are the static methods. The instance methods are even more concise! For instance:

`var metaPath = asset.MetaPath;`


## Documentation

- [Scripting API Reference](https://codesmile-0000011110110111.github.io/de.codesmile.editor.assetdatabase/html/index.html)
- [Transition Guide](https://docs.google.com/spreadsheets/d/134BEPXTx3z80snNAF3Gafgq3j5kEhmFzFBKT_z1s6Rw/edit?usp=sharing) (AssetDatabase method mapping)

## Requirements

- Unity 2021.3.3f1 or newer
- A smile :)

## Installation

This software is a Unity Package Manager 'npm package'.

- Open Window => Package Manager in Unity Editor
- Choose "Install package from git URL..."
- Enter this URL: `https://github.com/CodeSmile-0000011110110111/de.codesmile.editor.assetdatabase.git`

This package is currently not available on OpenUPM.

## GPL License

This software is licensed under the GNU General Public License v3.0 (GPL 3.0). The main implication is that any work you publish that uses this software requires the entire work to be published as open source software under the same GPL 3.0 license.

This software will also be available on the Unity Asset Store under the Asset Store EULA.

If you wish to license this software under different terms, for example to create Asset Store tools, please contact me!

- Steffen aka CodeSmile
- [Email](mailto:steffen@steffenitterheim.de) / [Discord](https://discord.gg/JN3Jz8qkeV)

## Support & Feeback

Very welcome! Please prefer to create an issue in the GitHub repository, specifically if you encounter issues or to request a feature. Contact me directly (see above) for any other feedback and questions.
