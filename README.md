# CodeSmile AssetDatabase

Modernized AssetDatabase API that's more consistent, performs more sanity checks and is generally easier to use. 100% covered by tests.

## Quick Introduction

You can work either with Asset instances or the static Asset API. Internally, instance methods call the static API. The API offers both explicit and some implicit operations.

### Working with Assets the way it was meant to be :)

```
// Create new asset of a UnityEngine.Object instance (default extension: ".asset"):
Asset objAsset = Asset.Create(obj, "Assets/Data/MySOAsset.asset");
Asset objAsset = Asset.Create(obj, "Assets/Data/MySOAsset.asset", overwriteExisting: true);
Asset objAsset = new Asset(obj, "Assets/Data/MySOAsset.asset"); // Create asset ctor

// Load asset from an asset path:
Asset objAsset = new Asset("Assets/Example/MySOAsset.asset");
Asset objAsset = (Asset.Path)"Assets/Example/MySOAsset.asset"; // implicit conversion

// Load asset from an asset GUID
var guid = objAsset.Guid;
Asset guidAsset = new Asset(guid);
Asset guidAsset = guid; // implicit conversion

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
objAsset.ForceSave(); // calls SetDirty on the object before saving (not for Bobby, but the script-only modification of the property requires SetDirty) 

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

There's a whole lot more and it's all as simple as that PLUS extra error checking and safety. For instance, batch editing ... yeah, you know you HAVE to wrap it in try/finally but how often did you NOT do it?

Here it's guaranteed:
```
Asset.BatchEditing(() => {
    // your complex, mass asset editing code ...
});
```

## Notable changes / additions

You may have noticed the use of the Asset.Path class. This wraps a path to an asset, ensures the path is valid, relative, uses only forward slashes, and has checks for file existance, methods to create the folders in a path, rename the file, and more.

Path operations are often at the heart of working with the AssetDatabase. It demanded a class of its own to handle all the various quirks and common oversights.

The Asset.Path class implicitly converts to/from string and the conversion to Asset.Path performs validation and sanitation. So you KNOW the moment a path is malformed AND you get a readable error message.

### Where is Refresh() ?

Oh, don't get me started. Something ain't quite right? Add another AssetDatabase.Refresh(). [OMG WTF!](https://forum.unity.com/threads/calling-assetdatabase-refresh-mandatory-reading-or-face-the-consequences.1330947/)

I always wanted this method renamed. Initially I considered the true and honest name: `ImportAllExternallyModifiedAssetsAndUnloadUnusedAssets()`

But I decided to just call it: `ImportAll()`

It is the 'many' companion to Import() the same way SaveAll() is the 'many' companion to Save(). There is no magical 'refresh'. 

It's just a very dumb name that prompted devs to use it too often with no thought given to what it actually does.

# Why the GPL 3.0 license?

This software solely provides code used within the Unity editor application.
It is therefore not being built into executables (known as 'builds') created
by the Unity editor application.

Thus, by merely using this software in your Unity project, the GPL license
does not affect or alter your right to distribute 'builds' of your project.
You do NOT have to publish your entire Unity project (source) itself under the
GPL license.

HOWEVER, it does require that any editor code (scripts) that use this software
be released under the GPL license but ONLY if you distribute said editor code.

Sharing the Unity project within your team / company is not distributing!
BUT if you were to create an editor tool that uses this software, and you were
to distribute that editor tool, you would have to distribute your editor tool
under the same GPL 3.0 license.

In essence, my motivation is this:

- I want to share my work, for free, in source form, to everyone's benefit.
- I want to discourage commercial editor tool developers from basing their
  work on this software for the simple reason that I may do so myself.
  (You cannot publish assets containing GPL licensed software on the Unity
  Asset Store. You can charge clients for any work you do with this software.)
- I want to encourage commercial software developers to obtain a more
  permissive license for this software, for example by purchasing rights to
  this software either via the Unity Asset Store or directly from me.
  This encouragement is mainly because the separation of editor vs runtime
  scripts can be somewhat fluid, and companies ought to to err on the safe
  side. Given how much work (aka 'money') this software can safe even a small
  team it is certainly worth to invest in it.

I sincerely hope you understand my reasons for applying the GPL 3.0 license.

Please contact me if you wish to license the software under different terms,
no matter your intention. Generally feel free to contact me if you have any
questions or feedback.

Thanks for your time and understanding!

- Steffen aka CodeSmile
- [Email](mailto:steffen@steffenitterheim.de) / [Discord](https://discord.gg/JN3Jz8qkeV)
