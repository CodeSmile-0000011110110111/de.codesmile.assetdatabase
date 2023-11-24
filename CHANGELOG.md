# Change Log

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
