# Project agent memory

This file is the project's committed home for project-intrinsic agent knowledge: build, test, release, architecture, and sharp-edge notes that should travel with the code.

## What this is

`de.codesmile.assetdatabase` is a Unity editor package sold on the Unity Asset Store and also
published here under GPL-3.0. Public API breakage costs buyers their compile, so treat every
signature in `Editor/` as shipped API. `README.md` states the supported Unity version range;
`package.json` `unity` and `unityRelease` are the machine-readable form of it.

## Building and testing

There is no CI that compiles this package: `.github/workflows/main.yml` only publishes `Docs~`
to GitHub Pages. Compiling and running the tests requires a real Unity editor.

The test suite is `Tests/Editor`, an EditMode NUnit suite that runs in the Unity Test Runner.
`Samples~` is invisible to Unity because of the trailing tilde, so a compile check that only
opens the package will not compile the two sample assemblies; copy `Samples~` into a project's
`Assets/` to compile them.

## Unity version conditionals

Version conditionals in this package guard real Unity API differences, verified by compiling
against the editor in question, not by reading release notes. The ones present now:

- `UNITY_2023_2_OR_NEWER` in `Asset.Path.cs` and `Asset.File.cs`: `AssetDatabase.AssetPathExists`
  and `AssetDatabase.GetMainAssetTypeFromGUID`.
- `UNITY_6000_3_OR_NEWER`, `UNITY_6000_4_OR_NEWER` in `Asset.Database.cs`, `Asset.File.cs` and
  `Tests/Editor/Helper/Instantiate.cs`: Unity deprecated the integer instance ID APIs in 6000.3,
  deprecated `Object.GetInstanceID` and the `Int32`/`EntityId` conversions in 6000.4, and turned all
  of them into compile errors in 6000.5. The public `Int32` parameter of `Asset.Database.Contains`,
  `Asset.File.CanOpenInEditor` and `Asset.File.OpenExternal` therefore becomes an `EntityId` from
  6000.4 on; 6000.3 keeps the `Int32` and converts internally.
- `UNITY_6000_6_OR_NEWER` in `Asset.Package.cs` and `Asset.cs`: `AssetDatabase.ImportPackage` and
  `AssetDatabase.ExportPackage` were deprecated in favour of `UnityEditor.AssetPackage.Package`.
  It also gates a public parameter, the optional trailing `ownerOrgId` on the three
  `Asset.Package.Export` overloads and on `Asset.ExportPackage`. That parameter must not exist
  below 6000.6, where Unity cannot honour it.

Those four `ownerOrgId` members duplicate their whole XML doc comment, signature and body in both
branches on purpose, and the duplication must not be collapsed. A preprocessor directive inside a
run of `///` lines, or between a doc comment and the declaration it documents, terminates that
comment for the C# compiler: Roslyn then keeps only the last block and silently drops the
`<summary>`. Wrapping a whole, well-formed XML element in the conditional does not avoid this, so
the conditional must wrap entire members. Doxygen is unaffected either way, which is why the
generated pages cannot be used to check this.

`Asset.SubAsset.SetMain` saves the asset between `AssetDatabase.SetMainObject` and the import it
does afterwards. That save is load-bearing from Unity 6000.7 on, where importing without it
re-reads the old main object from disk; do not remove it as redundant.

Two Unity type moves have no conditional and must not grow one: the `GUID` struct is declared in
`UnityEditor` up to Unity 6000.3 and in `UnityEngine` from 6000.4 on, and no Unity version
declares it in both. The files that use `GUID` therefore import both namespaces and let the
compiler resolve it. Removing either `using` breaks one half of the supported range.

## Maintaining this file

Keep this file for knowledge useful to almost every future agent session in this project.
Do not repeat what the codebase already shows; point to the authoritative file or command instead.
Prefer rewriting or pruning existing entries over appending new ones.
When updating this file, preserve this bar for all agents and keep entries concise.
