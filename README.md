# CodeSmile AssetDatabase

Imagine Unity's AssetDatabase were self-explanatory, enjoyable to use, consistent in design and behaviour, well documented, prevents disasters, and results in concise code.

### Bam, here it is! :)

## Why did I create this?

The AssetDatabase is [heavily fragmented into verbosely named, losely related static methods](https://docs.google.com/spreadsheets/d/134BEPXTx3z80snNAF3Gafgq3j5kEhmFzFBKT_z1s6Rw/edit?usp=sharing) with inconsistent signatures and varying side-effects. It's functional, but it's design is fundamentally broken (absent), leading to terrible code written against it.

Developers commonly employ a trial-and-error approach. Trivial tasks take much longer than estimated. Edge-cases remain to be discovered later. There is a real risk of data loss due to a simple mistake. Cargo-cult and copy-pasta programming needlessly degrade editor performance.

You'll find such bad examples even in popular Asset Store tools used by big game studios! 

A clean start with a consistent API is the best way to solve these issues, speed up development of editor tools, ensure scripts will not fail for users or other editor platforms.

Write fail-safe, readable, concise, efficient asset scripts in less time. That is what **CodeSmile AssetDatabase** provides.

## Main Features

The main class is `CodeSmileEditor.Asset` which provides a static API but it's also instantiable. 

The Asset instance provides access to asset-specific operations and alleviates you from managing separate state (eg asset path, GUID, sub assets, etc). 

The `CodeSmileEditor.Asset.Path` handles asset paths, ensures they are relative and compatible across editor platforms, validates correctness in regards to file system and assets, and provides all path operations and representations (.meta, full path, etc).

Unity's AssetDatabase and existing scripts using it are NOT altered or affected in any way.

BONUS: [Asset Inspector](https://github.com/CodeSmile-0000011110110111/de.codesmile.assetinspector) - view and inspect every (!) detail of selected assets. It also [serves as a showcase](https://github.com/CodeSmile-0000011110110111/de.codesmile.assetinspector/blob/main/Editor/AssetInspector.cs) for CodeSmile AssetDatabase.

There's a whole lot more so be sure to explore and discover!

## Example Code Snippets

Load and Create assets:
- `Asset asset = "Assets/Folder/Data.asset";`
- `var asset = new Asset(bytes, "Assets/Folder/Data.asset");`
- `var obj = Asset.File.Create(str, "Assets/Folder/Data.asset");`
- `var obj = Asset.File.CreateAsNew(bytes, "Assets/Folder/Data.asset");`

Notice the hands-free, does-what-you-need approach.

What's not noticable here is that any non-existing folder in the path is automatically created. Countless asset scripts fail the first time an actual user runs it. I know YOU know it! You read this far! ;)

Other file operations:
- `asset.ForceSave();`
- `var assetDupe = asset.Duplicate();`
- `assetDupe.Delete();`
- `var copy = asset.SaveAsNew("Assets/Elsewhere/Dada.asset");`

Type conversion:
- `var obj = asset.MainObject;`
- `var levelData = asset.GetMain<LevelData>();`
- `var levelData = (LevelData)asset;`

Asset.Path examples:
- `var path = new Asset.Path(@"C:\MyProjects\FudniteClone\Assets\my.asset");`
- `path.CreateFolders();`
- `var absolutePath = path.FullPath;`

Performance:
- `Asset.File.BatchEditing(() => { /* mass file IO */ });`
- `Asset.File.Import(paths);`

Work with Sub-Assets:
- `asset.AddSubAsset(subData);`
- `var subAssets = asset.SubAssets;`

You'll commonly get or set dependencies, importers, labels, paths, asset bundles, etc. via instance properties.

Complete:
- `asset.ExportPackage("I:/leveldata.unitypackage");`
- `asset.ActiveImporter = typeof(MyDataImporter);`
- `Asset.Database.ImportAll();` // Hint: this is "Refresh()"

You'll also find Cache Server, Version Control, etc. in well-defined, logical places.

Error Handling:
- `var msg = Asset.GetLastErrorMessage();` // A file operation failed? Check the error string

Exceptions are also thrown for malformed input to make the API more resilient and reliable, rather than calls silently failing or printing unhelpful console logs.

## Documentation & Support

The [API documentation](https://codesmile-0000011110110111.github.io/de.codesmile.assetdatabase/html/index.html) is more complete with more details and caveats mentioned than Unity's. Of course you'll find these snippets right in your IDE as tooltips.

The [Transition Guide](https://docs.google.com/spreadsheets/d/134BEPXTx3z80snNAF3Gafgq3j5kEhmFzFBKT_z1s6Rw/edit?usp=sharing) helps experienced developers find what each AssetDatabase method maps to in the CodeSmileEditor.Asset class.

If there's anything out of the ordinary, [report an issue](https://github.com/CodeSmile-0000011110110111/de.codesmile.assetdatabase/issues) or [contact me](mailto:steffen@codesmile.de). I also have a [Discord channel](https://discord.gg/JN3Jz8qkeV).

You can get the most up-to-date version on the [CodeSmile AssetDatabase GitHub repository](https://github.com/CodeSmile-0000011110110111/de.codesmile.assetdatabase).

## Installation

This software is a Unity Package Manager 'npm package' available on GitHub (GPL License) or the Unity Asset Store (UAS EULA).

- Open Window => Package Manager in Unity Editor
- Choose "Install package from git URL..."
- Enter this URL: `https://github.com/CodeSmile-0000011110110111/de.codesmile.editor.assetdatabase.git`

## Requirements

- Unity 2021.3.3f1 or newer (*)
- A smile :)

## Licenses

This software is dual-licensed.

### FREE => GPL License
The free GitHub version is distributed under the GNU General Public License v3.0 (GPL 3.0). The main implication is that any work you publish that uses this software requires the entire work to be published as open source software under the same GPL 3.0 license.

### PAID => Unity Asset Store EULA
The paid version is available on the Unity Asset Store (UAS) and licensed under the Asset Store EULA. Users who purchased this software can, at any time, also download the software from GitHub and use it under the terms and conditions of the Asset Store EULA.

## What to expect from me?

I care. A lot.

- The API has been designed to be concise, approachable, elegant, consistent, explorable, complete and whole lot more buzzwords - except they actually mean something to me.

I work obsessively.

- All AssetDatabase methods are included.
- The Asset Inspector window lets you inspect every detail of selected assets in the project.

I follow best practices.

- Included are extensive test cases, the file I/O operations specifically have 100% test coverage. The tests also serve to instruct by example.

I like to share knowledge.

- The documentation is exhaustive, well structured, and contains more information than what you'll find in Unity's manual and script reference.

I'm also deeply honest. And I trust you.

- Admittedly, a very fringe method that returns a NativeArray has been omitted to avoid having to depend on com.unity.collections. Just so you know.
- You can [demo the project on GitHub](https://github.com/CodeSmile-0000011110110111/de.codesmile.assetdatabase) where it's available for free but released under the terms of the GNU GPL 3.0.

I support you.

- [Contact me](mailto:steffen@codesmile.de) if you have any issues or feature requests. I'm also available for tutoring developers about Unity best practices and general contract work.
- You'll also [find me actively supporting](https://forum.unity.com/members/codesmile.602581/) users on the Unity Forums.

## Support, Feeback, Inquiries

Very welcome!

If you wish to license this software under different terms, for example to create Asset Store tools, please contact me!

I'm available for consultation, tutoring best practices, sharing my experience and other contract work.

If you like this asset and want to support my work please favorite, rate and review this asset! It helps a lot to bubble up in the search algorithm.

- Steffen aka CodeSmile
- [Email](mailto:steffen@codesmile.de) / [Website](https://codesmile.de) / [GitHub](https://github.com/CodeSmile-0000011110110111) / [Discord](https://discord.gg/JN3Jz8qkeV) / [Unity Assets](https://assetstore.unity.com/publishers/60108)
