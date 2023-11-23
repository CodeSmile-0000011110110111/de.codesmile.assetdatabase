# CodeSmile AssetDatabase

It's Unity's AssetDatabase except now it's clean, concise, consistent and powerful.

## Examples

What used to be 20+ lines of code is now two. ;)

### Load or create asset

This is the original code, a common use case from the Unity forum. Notice the extra steps involved to create the asset and the folders.
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

With CodeSmile AssetDatabase the same code is just these two lines:
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

Ah, I see, you have a GUID? In that case it's:

`Asset levelData = thisIsYourGuid;`

Likewise if you already have an asset object instance:

`Asset levelData = yourAssetObject;`

My name is CodeSmile for a reason. :)

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


### Asset is an instance

```
// Create new asset of a UnityEngine.Object instance:
Asset objAsset = new Asset(obj, "Assets/Data/MySOAsset.asset");

// Load asset from an existing asset instance:
Asset existingAsset = new Asset(obj);
Asset existingAsset = obj; // implicit conversion

// Get the asset's path, object and guid:
Asset.Path path = objAsset.AssetPath;
UnityEngine.Object obj = objAsset.MainObject;
var mySO = objAsset as MyScriptableObject; // implicit conversion + cast
GUID guid = objAsset.Guid;

// Save modifications to the object:
(objAsset as MyScriptableObject).BobbyBrown = "My teeth is shiny!";
objAsset.ForceSave(); // calls SetDirty on the object before saving
// (dirty is not for Bobby but the script-only property change requires SetDirty) 

// save object if it was marked dirty, otherwise skips serialization
objAsset.Save();

// Rename the asset file:
objAsset.Rename("different file name");

// Make a copy, will create non-existing folders:
Asset copyAsset = objAsset.Copy("Assets/folder/subfolder/copied.asset");

// You can also move the asset to a non-existing path:
objAsset.Move("Assets/folder/subfolder/moved.asset");

// Delete the asset file from disk: 
objAsset.Delete(); // also: objAsset.Trash()
objAsset = null;
```

There's a whole lot more and it's all as simple as that PLUS extra error checking and safety. For instance, batch
editing ... yeah, you know you HAVE to wrap it in try/finally but how often did you NOT do it?

Here it's guaranteed:

```
Asset.BatchEditing(() => {
    // your complex, mass asset editing code safely wrapped in try/finally ...
});
```

## License

This software is licensed under the GNU General Public License v3.0 (GPL 3.0). The main implication is that any work you build using this software requires the entire work to be published under the GPL.

This software will also be available on the Unity Asset Store under the Asset Store EULA.

If you wish to license this software under different terms please contact me!

- Steffen aka CodeSmile
- [Email](mailto:steffen@steffenitterheim.de) / [Discord](https://discord.gg/JN3Jz8qkeV)

## Requirements

- Unity 2021.3.3f1 or newer
- A smile :)

## Installation

This software is a Unity Package Manager 'npm package'.

- Open Window => Package Manager in Unity Editor
- Choose "Install package from git URL..."
- Enter this URL: `https://github.com/CodeSmile-0000011110110111/de.codesmile.editor.assetinspector.git`

This package is currently not available on OpenUPM.

## Documentation

[Manual + API Reference](https://codesmile-0000011110110111.github.io/de.codesmile.editor.assetdatabase/html/index.html)

tbd: spreadsheet

## Quick Introduction

You can work either with Asset instances or the static Asset API. Internally, instance methods call the static API. The
API offers both explicit and some implicit operations.


## Notable changes / additions

### Asset.Path

You may have noticed the use of the Asset.Path class. This wraps a path to an asset, ensures the path is valid,
relative, uses only forward slashes (compatible with all editor platforms), has checks for file existance, methods to
create the folders in a path, rename the file, and more more more ...

Path operations are at the heart of working with the AssetDatabase, yet it has traditionally been a crud despite a ton
of utility methods because the path ultimately remained a string instance - anyone, anything could tamper it. I KNOW
this happened to you too, right?

Paths to assets demanded a class of its own to handle all the various quirks, compatibility issues, illegal characters
and just common oversights like writing an editor script on Windows that stops working on Mac/Linux due to just one
single backslash.

The Asset.Path class implicitly converts to/from string and the conversion to Asset.Path performs validation and
sanitation. So you KNOW the moment a path is malformed AND you get a readable error message.

```
Asset.Path assetPath = "Assets/subfolder/myfile.asset"; // implicit conversion
Asset.Path assetPath = "myfile.asset"; // ERROR: missing 'Assets'
Asset.Path assetPath = "Assets/subf?lder/my|file.asset"; // ERROR: illegal chars

string strPath = assetPath; // implicit conversion

strPath = "C:\\Users\\Urso Clever\\Desktop\\MyPorject\\Assets\\myfile.asset";
AssetDatabase.CreateAsset(obj, strPath); // ERROR: used an absolute path, stupid!

Asset.Create(obj, strPath); // this just works - it's magic, maaagic!
```

### Where is Refresh() ?

Oh, don't get me started. Something ain't quite right? Add another
AssetDatabase.Refresh(). [OMG WTF!](https://forum.unity.com/threads/calling-assetdatabase-refresh-mandatory-reading-or-face-the-consequences.1330947/)

I always wanted this method renamed. Initially I considered the true and honest
name: `ImportAllExternallyModifiedAssetsAndUnloadUnusedAssets()`

But I decided to just call it: `Asset.Database.ImportAll()`

It is the 'many' companion to Import() the same way SaveAll() is the 'many' companion to Save(). There is no magical '
refresh'.

It's just a very dumb name that prompted devs to use it too often with no thought given to what it actually does.
