# Change Log

#### v1.8.0 - Dec 09, 2023

- renamed ForceReserializeAssets to UpgradeAssetSerializationFormats 
- renamed ForceReserializeAllAssets to UpgradeAllAssetSerializationFormats (*)

(*) To provide more rationale: I saw a user's script that wasn't working as intended and thus the user helplessly threw in every non-obviously named AssetDatabase method - the usual candidates that I've already covered but the use of "ForceReserializeAssets" was new to me. This struck me as yet another non-obvious method name and of course it bothered me so I had to come up with a more descriptive name.

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
