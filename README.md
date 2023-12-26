# CodeSmile AssetDatabase

It's Unity's age-old AssetDatabase - in clean code form! It will make you smile. :)

## Who needs this?

I do! ![CodeSmile Icon](Media~/steffen%20portrait%20codesmile%20logo%20larger%20top-left-64x62.png) Actually, I WANTED it. :)

I spent a great deal of time to make AssetDatabase tasks dead simple for a layperson.

And anyone who doesn't want to be bothered with how all of this assetcrapbase works and what all the edge-cases and side-effects could be, might be, and really just want to have this working and move on!

## But .. why?

Unload your mind. Put yourself at ease.

For every task there is a single call and you are DONE!

The structure and naming is intended to be EXTREMELY simple to find your way around and then to call the appropriate method with fewer parameters, with names that speak for themselves. 

No longer do you need to wonder what a 'valid folder' might be. Or ponder what it means to 'force reserialize all assets'. 

Let alone the ubiquitous 'SaveAllAssets' followed by 'Refresh' - are you calling that in your scripts? 99% chance you just put it there out of habit. You never gave it any thought. You have no idea what it really does. Not even that it can cripple editor performance. Or when calling it is indeed **required**. (Hint: almost never!)

Or just being confused, once again, about whether you need to use `AssetDatabase.GetTextMetaFilePathFromAssetPath` or `AssetDatabase.GetAssetPathFromTextMetaFilePath`. Or the unholy trinity: `AssetPath.AssetPathToGUID`~`AssetPath.GUIDFromAssetPath`~`AssetPath.GUIDToAssetPath`.

## Example Code Snippets

`Asset data = "Assets/Folder/Data.asset";` // Load an asset from its path

`data.ForceSave();` // mark asset as dirty and save it

`data.AddSubAsset(subData);` // Add a sub-asset (implicitly saved)

`data.ActiveImporter = typeof(MyDataImporter);` // Change asset's importer

`var obj = Asset.File.Create(str, "Assets/Folder/Data.asset");` // Create (overwrite) asset from string

`var obj = Asset.File.CreateAsNew(bytes, "Assets/Folder/Data.asset");` // Create new asset from byte[]

`var asset = new Asset(bytes, "Assets/Folder/Data.asset");` // Same as above using Asset ctor

The 'create' methods above cover EVERY ASPECT and edge-cases:
- Error checking (null arguments, path validation, ..)
- Create non-existing folders of the path
- Generate a unique filename (unless overwriting)
- Write the string/bytes to file
- Import the new asset file
- Load the new asset file

`var actualPath = asset.AssetPath;` // Filename might have changed, eg "Data (3).asset"

`asset.ExportPackage("I:/leveldata.unitypackage");` // Export as .unitypackage

`var obj = asset.MainObject;` // Get asset's UnityEngine.Object instance

`var levelData = asset.GetAs<LevelData>();` // Get it as specific type (may be null)

`var levelData = (LevelData)asset;` // Cast to a type (may throw)

`var subAssets = asset.SubAssets;` // Do I need to keep explaining these calls?

`var assetDupe = asset.Duplicate();` // Because you need a duplicate ..

`assetDupe.Delete();` // .. but then decided you don't.

`var newAsset = asset.SaveAsNew("Assets/Elsewhere/Daydah.asset");` // Now you want a copy?

`newAsset.Trash();` // Okay. Either you're bored or excited to work with the AssetDatabase for the first time EVER. :)

`Asset.File.BatchEditing(() => { /* mass file IO */ });` // Speed up calling many Asset.File.* methods (loop)

`Asset.File.Import(paths);` // Mass import of paths, batched internally

`var msg = Asset.GetLastErrorMessage();` // A file operation failed? Show this!

## I don't trust this ..

The implementation is utmost CORRECT - there are no unnecessary, performance-degrading calls such as 'Refresh' and 'SaveAllAssets' littered throughout like you'll find in most editor scripts - unfortunately even in popular assets/libraries!

It is also extensively unit TESTED to be correct. 

And I happen to love correct, clean code. Most developers move on when their code works. I cannot move on until I understand **why** my code works.

## What about support?

[The documentation](https://codesmile-0000011110110111.github.io/de.codesmile.assetdatabase/html/index.html) is more complete with more details and caveats mentioned than Unity's. 

And if there's anything out of the ordinary, open an issue or [contact me](mailto:steffen@steffenitterheim.de). I also have a [Discord channel](https://discord.gg/JN3Jz8qkeV).

## Where's Refresh?

I did mention you don't need it, right? ;)

But if you do, here's Waldo: `Asset.Database.ImportAll();`

This is an expensive (!) database operation in that it scans the ENTIRE "Assets" tree and tests ALL (!) files for changes made EXTERNALLY (eg System.IO methods, bash scripts). 

Refresh also unloads all unused (cached) resources, forcing them to be reloaded from disk on the next use. You can imagine how this has a negative impact on editor performance.

So if you work with a SINGLE asset (or many in a loop) use the singular Save & Import methods, NOT SaveAllAssets. Likewise, if you modify an asset through AssetDatabase methods, you do **NOT** need to call Refresh(). Ever!

## Documentation

- [Scripting API Reference](https://codesmile-0000011110110111.github.io/de.codesmile.assetdatabase/html/index.html)
- [Transition Guide](https://docs.google.com/spreadsheets/d/134BEPXTx3z80snNAF3Gafgq3j5kEhmFzFBKT_z1s6Rw/edit?usp=sharing) (AssetDatabase method mapping)
- [Changelog](https://github.com/CodeSmile-0000011110110111/de.codesmile.assetdatabase/blob/main/CHANGELOG.md)


## Installation

This software is a Unity Package Manager 'npm package'.

- Open Window => Package Manager in Unity Editor
- Choose "Install package from git URL..."
- Enter this URL: `https://github.com/CodeSmile-0000011110110111/de.codesmile.editor.assetdatabase.git`

This package is currently not available on OpenUPM.

## Requirements

- Unity 2021.3.3f1 or newer (*)
- A smile :)

Sorry, I will not backport to 2020 or older due to extensive use of C# 9 features.

## GPL License

This software is licensed under the GNU General Public License v3.0 (GPL 3.0). The main implication is that any work you publish that uses this software requires the entire work to be published as open source software under the same GPL 3.0 license.

This software will also be available on the Unity Asset Store under the Asset Store EULA.

If you wish to license this software under different terms, for example to create Asset Store tools, please contact me!

- Steffen aka CodeSmile
- [Email](mailto:steffen@steffenitterheim.de) / [Discord](https://discord.gg/JN3Jz8qkeV)

## Support & Feeback

Very welcome! Please prefer to create an issue in the GitHub repository, specifically if you encounter issues or to request a feature. Contact me directly (see above) for any other feedback and questions.
