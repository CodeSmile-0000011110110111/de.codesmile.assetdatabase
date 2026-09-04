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
  `Tests/Editor/Helper/Instantiate.cs`. Measured by reading the `ObsoleteAttribute` blobs out of the
  installed editors: the `AssetDatabase` integer instance ID overloads are obsolete as a warning on
  6000.3.23f1 and 6000.4.11f1 and as an error on 6000.5.10f1. `Object.GetInstanceID` carries no
  obsolete attribute on 6000.3.23f1, is a warning on 6000.4.11f1 and an error on 6000.5.10f1. The
  `Int32` to `EntityId` conversion carries no obsolete attribute on 6000.3.6f1 or 6000.3.23f1, is a
  warning on 6000.4.11f1, and is an error on 6000.6.0f1. Nothing was removed in any version; every
  one of these members still exists in the assemblies. The public `Int32` parameter of
  `Asset.Database.Contains`, `Asset.File.CanOpenInEditor` and `Asset.File.OpenExternal` therefore
  becomes an `EntityId` from 6000.4 on; 6000.3 keeps the `Int32` and converts internally.
- `UNITY_6000_6_OR_NEWER` in `Asset.Package.cs` and `Asset.cs`: `AssetDatabase.ImportPackage` and
  `AssetDatabase.ExportPackage` carry no obsolete attribute on 6000.5.10f1 and are obsolete as a
  warning on 6000.6.0f1, replaced by `UnityEditor.AssetPackage.Package`, which is absent through
  6000.5.10f1 and present on 6000.6.0f1 and 6000.7.0a6.
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
does afterwards. That save is load-bearing on 6000.7.0a5 and 6000.7.0a6, where importing without it
re-reads the old main object from disk; do not remove it as redundant.

Two Unity type moves have no conditional and must not grow one: the `GUID` struct is declared in
`UnityEditor` up to Unity 6000.3 and in `UnityEngine` from 6000.4 on, and neither namespace
declared it in both on any of the nine editors tested. The files that use `GUID` therefore import
both namespaces and let the compiler resolve it. Removing either `using` breaks one half of the
supported range.

## Claims name their evidence

A statement in this package's documentation about what it does, does not do, requires, or no longer
requires names the evidence - a measurement, a file and line, or a named run - or it is not made.
This includes a consequence adopted from a code review finding, which must be checked against the
repository before it is written down: a review finding is evidence, not a verified claim.

The concrete instance of that rule: a statement about Unity version behaviour names the editor
version it was measured on. Not "recent versions", not a range inferred from two endpoints, not a
version taken from release notes without a local run. The nine editors run in this task are
2022.3.62f3, 6000.0.83f1, 6000.3.6f1, 6000.3.23f1, 6000.4.11f1, 6000.5.10f1, 6000.6.0f1, 6000.7.0a5
and 6000.7.0a6; 6000.1 and 6000.2 are not installed and nothing about them is known.

Four corrections to this package's text produced the rule: a false claim that Unity removed the
integer instance ID APIs in 6000.5, when it marked them obsolete as an error and they still exist in
the assemblies; a lumped warning-phase version that was wrong for `Object.GetInstanceID`, which
carries no obsolete attribute on 6000.3.23f1 and first warns on 6000.4.11f1; an unmeasured
warning-phase claim about `EditorUtility.InstanceIDToObject`, an API this package never calls; and a
claim that removing an unused `using NUnit.Framework;` stopped the package failing to compile in a
project without the test framework, contradicted by the `com.unity.test-framework` dependency
`package.json` declares. When in doubt, weaken the claim to what was measured or drop it, rather
than restating it more vaguely and leaving it just as unsupported.

## Maintaining this file

Keep this file for knowledge useful to almost every future agent session in this project.
Do not repeat what the codebase already shows; point to the authoritative file or command instead.
Prefer rewriting or pruning existing entries over appending new ones.
When updating this file, preserve this bar for all agents and keep entries concise.
