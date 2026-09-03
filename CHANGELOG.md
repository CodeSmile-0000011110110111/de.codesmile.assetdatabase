# Change Log

#### Unreleased

- Minimum Unity version raised from 2021.3.3f1 to 2022.3.62f3. Every code path that only existed
  for Unity 2021.3 or 2022.1/2022.2 has been removed, in the package, the tests and the samples.
- Fixed: the package did not compile in Unity 6000.4 or newer, because Unity moved the `GUID` type
  from the `UnityEditor` namespace to `UnityEngine`. The files that use `GUID` now import both
  namespaces; no Unity version declares the type in both, so the type resolves in either.
- Fixed: the package did not compile in Unity 6000.5 or newer, because Unity removed the integer
  instance ID overloads of `AssetDatabase.Contains`, `AssetDatabase.CanOpenAssetInEditor` and
  `AssetDatabase.OpenAsset`, removed `Object.GetInstanceID`, and removed the conversions between
  `Int32` and `EntityId`.
- **API change in Unity 6000.4 and newer only:** `Asset.Database.Contains`,
  `Asset.File.CanOpenInEditor` and `Asset.File.OpenExternal` take a `UnityEngine.EntityId` instead
  of an `Int32` instance ID. In Unity 2022.3 up to and including 6000.3 they still take an `Int32`
  and are unchanged.
  What your code has to do on Unity 6000.4 and newer: replace `someObject.GetInstanceID()` with
  `someObject.GetEntityId()` and pass that. Unity deprecated `GetInstanceID` in 6000.4 and removed
  it in 6000.5, so the integer these methods used to accept can no longer be obtained there.
  The overloads that take an asset `Object` or an `Asset.Path` are unchanged in every version and
  need no edit.
- Fixed: the deprecation warnings Unity 6000.3 and 6000.4 raise for the integer instance ID APIs.
  On 6000.3 the `Int32` signatures are kept and the value is converted to `EntityId` internally;
  from 6000.4 the `Int32` signatures are gone, which is what removes the last warnings.
- Fixed: deprecation warnings in Unity 6000.6 and newer, where `AssetDatabase.ImportPackage` and
  `AssetDatabase.ExportPackage` are deprecated in favour of `UnityEditor.AssetPackage.Package`.
- Fixed: `Asset.SubAsset.SetMain` and the `Asset.MainObject` setter did not take effect in Unity
  6000.7. `AssetDatabase.SetMainObject` only records which object should become the main one on the
  next import; it does not write the asset file. From Unity 6000.7 on, the import that follows
  re-reads the old main object from disk and the change is lost. The asset is now saved between the
  two calls, which is correct in every supported version.
- Removed an unused `using NUnit.Framework;` from `Editor/Asset.Status.cs`, which made the shipped
  editor assembly reference the test framework for no reason.

#### v1.9.1 - Feb 01, 2024

- API Changes: These static methods have been MOVED from **Asset** to **Asset.File**:
  - GetMainType
  - GetSubType
  - GetGuid
  - GetFileId
  - GetGuidAndFileId
  - Rationale: These methods hindered API discovery via auto-completion by cluttering the Asset namespace, and they truly belong to the File API since they query asset file attributes.
- Fixed: 2021.3 shows warning when copying (duplicate, save as new) an asset, complaining that filename must match asset object name. This is likely a bug in Unity.
- Fixed the same 2021.3 issue in workflow sample. Also required importing batched asset after copying them.
- Asset.File.Import takes IEnumerable as input (not just array)

#### v1.8.6 - Jan 30, 2024

- added Sample: Asset Workflow with GUI. Allows selecting between CodeSmile and Unity API. Unity API code is 33% more lines/characters, with folder creation hardcoded.

#### v1.8.5 - Jan 29, 2024

- added Asset.File.FindPaths overload accepting Path[]
- added Asset.Label.GetAll overload accepting Path
- added Asset.Label.Remove and Asset().RemoveLabel
- fixed invalid cast exception (Asset.Path[] to string[])
- removed add sub-asset check for ".asset" extension. Turns out you can also add subassets to a .mesh extension asset and there may be others.

#### v1.8.4 - Jan 24, 2024

- added asset lifetime sample script
- updated documentation

#### v1.8.3 - Jan 22, 2024

- Importer tests: disable some tests in 2021.3 because the method is not available
- Rename test: log warning in 2021.3 to mention that the following warning is expected

#### v1.8.2 - Dec 26, 2023

- updated README and API docs
- removed extra call to "ImportIfNotImported" in load asset method chain

#### v1.8.1 - Dec 21, 2023

- added AvailableImporters property
- added missing Asset.Importer, Asset.Path and Asset.File.Create tests

#### v1.8.0 - Dec 09, 2023

- renamed ForceReserializeAssets to UpgradeAssetSerializationVersion and
- renamed ForceReserializeAllAssets to UpgradeAllAssetSerializationVersions (*)

(*) To provide more rationale: I saw a user's script where the user helplessly threw in the usual candidates due to some issue, but this also included "ForceReserializeAssets". That's when I realized the name of the method only describes what it does internally but not its intent respectively what it should be used for: upgrading the version (format) of serialized assets when you change editor versions and you want version control not polluting these upgrades in individual future commits.

#### v1.7.1 - Dec 05, 2023

- rename package to de.codesmile.assetdabase (removed the 'editor' part)
- Asset.File.Delete avoids unnecessary warning message from Unity if path.Exists is false

#### v1.7.0 - Dec 03, 2023

- Import() throws if file does not exist
- added setter to ActiveImporter property
- removed SetActiveImporterToDefault and SetActiveImporter methods
- added [NotNull] attributes
- code cleanup

#### v1.6.0 - Nov 30, 2023

- changed namespace from CodeSmile.Editor.* to CodeSmileEditor.*
- new Asset(Path) now auto-imports assets that exist on disk but not in DB
- changed load methods to import assets that exist on disk but not in DB
- added create asset from string (ctor and Asset.Create)
- added create asset from byte[] (ctor and Asset.Create)
- added ImportAndLoad<T>
- added Import(Path[]) to batch-import multiple assets
- added IsImported(Path)
- added setter to DirectoryMonitoring
- added Database.Contains back and redirected Status.IsImported (same)
- added paths to all important subfolders (eg Packages, Library, ..)
- split Create into Create and CreateAsNew
- split Copy into Copy and CopyAsNew
- renamed Copy instance method to SaveAs and SaveAsNew
- renamed FailedToDeletePaths to PathsNotDeleted
- renamed ToAssetPaths(Object[]) to Get(Object[])
- fixed compile errors in some Unity versions 
- GetIcon returns Texture2D

#### v1.4.1 - Nov 24, 2023

- added static GetMainType(guid)

#### v1.4.0 - Nov 24, 2023

- added static GetSubType
- renamed Status IsForeignAsset to IsForeign
- renamed Status IsNativeAsset to IsNative
- renamed Database DirectoryMonitoringEnabled to DirectoryMonitoring
- renamed Dependency Set to Register
- renamed Dependency Remove to Unregister
- renamed Importer SaveSettings to ApplySettings
- renamed Path OpenFolder to OpenExternal
- renamed Path UniquifyFilename to UniquifyFileName (camel case)
- removed various ExcludeFromCodeCoverage attributes, added comment with reason to the remaining ones

#### v1.3.1 - Nov 24, 2023

- added several 'Object' getters to methods with only 'Path' parameters
- added static GetGuid method
- added static GetGuidAndFileId method

#### v1.3.0 - Nov 23, 2023

- Fixed all technical documentation issues.
- CreateFolders now internalized the assumption that the last part of a path, if it contains no extension, is also a folder. 
- Removed FolderPathAssumptive
- FolderPath does not throw but returns null if called on a root folder path ie "Assets".

#### v1.2.1 - Nov 22, 2023

- Fixed compile error in Unity 2021.3.
- Tested in all minor Unity versions from 2021.3 through 2023.3.

#### v1.2.0 - Nov 22, 2023

- First release that's nearly complete. Redesign for 99% of all AssetDatabase methods. 
- Major refactoring of API Design. 

#### v1.1.4 - Nov 13, 2023

- Fixed a compile error in Unity 2021.3.

#### v1.1.3 - Nov 12, 2023

- Documentation added and updated.

#### v1.1.0 - Nov 10, 2023

- Mainly refactoring.

#### v1.0.0 - Nov 9, 2023

- First release
