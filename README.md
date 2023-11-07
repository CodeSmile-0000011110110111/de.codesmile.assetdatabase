# EasyAssetDatabase

Modernized AssetDatabase API that's more consistent, performs more sanity checks and is generally easier to use. It's also fully tested.

## Quick Introduction

You can work either with Asset instances or the static Asset.Database API: 

### Asset Instance Methods
```
// Create new asset of a UnityEngine.Object instance (default extension: ".asset"):
Asset objAsset = new Asset(obj, "Assets/Data/MySOAsset.asset");
Asset objAsset = new Asset(obj, "Assets/Data/MySOAsset.asset", overwriteExisting: true);

// Load from an asset path:
Asset objAsset = new Asset("Assets/Example/MySOAsset.asset");

// Save modifications to the object:
obj.CoolStuff = "This object is very dirty!";
objAsset.Save();

// mark object as dirty before save; needed in some cases, slow in all others
objAsset.ForceSave();

// Delete the asset file from disk: 
objAsset.Delete();
objAsset = null;

// Delete by moving the asset to the OS trash bin:
objAsset.Trash();
objAsset = null;
```

### Static Asset Methods
```
// Create new asset of a UnityEngine.Object instance:
MySO assetSO = Asset.Create<MySO>(obj, "Assets/Data/MySOAsset.asset");
MySO assetSO = Asset.Create<MySO>(obj, "Assets/Data/MySOAsset.asset", overwriteExisting: true);

// Load from an asset path:
MySO assetSO = Asset.LoadMain<MySO>("Assets/Data/MySOAsset.asset");

// Save modifications to the object:
assetSO.CoolStuff = "This object is very dirty!";
Asset.Save(assetSO);

// mark object as dirty before save; needed in some cases, slow in all others
Asset.ForceSave(assetSO);

// Delete the asset file from disk:
Asset.Delete(assetSO); 

// Delete by moving the asset to the OS trash bin:
Asset.Trash(assetSO); 
```
