// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile_AssetDatabase_Demo;
using CodeSmileEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class AssetDemoWindow
{
	private void OnCreateButtonClicked_Unity()
	{
		var numberOfAssets = 32;
		for (var i = 0; i < numberOfAssets; i++)
		{
			// Let's assume we've generated a wonderful mesh together with
			// an impressive material using a beautiful texture and some scripting
			var mesh = new Mesh();
			mesh.name = "A Mesh, that couldn't be any meshier even if it were a net";
			var texture = new Texture2D(128, 128);
			texture.name = "A texture, so unimaginably beautiful it will melt your eyes";
			var material = new Material(Shader.Find("Standard"));
			material.name = "A material, an impressive sight to behold but untouchable";
			material.mainTexture = texture;
			var animation = new AnimationClip();
			animation.name = "An animation clip, totally neither motion-captured nor AI generated";
			var scriptableObject = CreateInstance<TestAssetSO>();
			scriptableObject.name = "A script, which is quaranteed 99.9999997% bug-free!";

			// Now we create an asset and stuff it all in there for illustration purposes
			// Note: meshes require the .mesh extension
			var parentFolder = Path.GetDirectoryName(DemoAssetsPath);
			if (AssetDatabase.IsValidFolder(parentFolder) == false)
			{
				var folderName = "CodeSmile AssetDatabase Demo Assets";
				AssetDatabase.CreateFolder("Assets", folderName); // hardcoded!
			}
			if (AssetDatabase.IsValidFolder(DemoAssetsPath) == false)
				AssetDatabase.CreateFolder(parentFolder, "Sample Assets"); // hardcoded!

			var createTime = $"{Time.realtimeSinceStartupAsDouble:F3}";
			createTime = createTime.Replace(".", "-").Replace(",", "-");
			var assetPath = $"{DemoAssetsPath}/Mesh #{i}-{createTime}.mesh";

			AssetDatabase.CreateAsset(mesh, assetPath);
			AssetDatabase.AddObjectToAsset(texture, mesh);
			AssetDatabase.AddObjectToAsset(material, mesh);
			AssetDatabase.AddObjectToAsset(animation, mesh);
			AssetDatabase.AddObjectToAsset(scriptableObject, mesh);
			AssetDatabase.SetLabels(mesh,
				new[] { "CodeSmile AssetDatabase", "New-Shiny-DemoAsset" });
			// save is needed only to write the post-creation changes (added sub-assets & labels)
			AssetDatabase.SaveAssetIfDirty(mesh);
		}
	}

	private void OnDuplicateButtonClicked_Unity()
	{
		// delete the existing duplicates
		var dupeGuids = AssetDatabase.FindAssets("l:No-SubAssets-DemoAsset", SearchPathStr);
		foreach (var dupeGuid in dupeGuids)
		{
			var dupePath = AssetDatabase.GUIDToAssetPath(dupeGuid);
			AssetDatabase.DeleteAsset(dupePath);
		}

		// duplicate assets with sub assets
		var assetGuids = AssetDatabase.FindAssets("l:New-Shiny-DemoAsset", SearchPathStr);
		foreach (var assetGuid in assetGuids)
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

			// only duplicate those with (visible in project view) sub-assets
			var visibleSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
			if (visibleSubAssets.Length > 0)
			{
				// create a duplicate
				var filenameNoExt = Path.GetFileNameWithoutExtension(assetPath);
				var dupePath = $"{DemoAssetsPath}/{filenameNoExt} no sub-assets.mesh";
				dupePath = AssetDatabase.GenerateUniqueAssetPath(dupePath);
				AssetDatabase.CopyAsset(assetPath, dupePath);

				// remove sub-assets from dupe
				var dupeAsset = AssetDatabase.LoadMainAssetAtPath(dupePath);
				var dupeSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(dupePath);
				foreach (var subAsset in dupeSubAssets)
					AssetDatabase.RemoveObjectFromAsset(subAsset);

				// tag and save the dupe
				var sourceLabels =
					AssetDatabase.GetLabels(new GUID(assetGuid)); // get labels from path? Nope
				var labels = sourceLabels.ToList();
				labels.Remove("New-Shiny-DemoAsset");
				labels.Add("No-SubAssets-DemoAsset");
				AssetDatabase.SetLabels(dupeAsset, labels.ToArray());
				AssetDatabase.SaveAssetIfDirty(dupeAsset);
			}
		}
	}

	private void OnDuplicateBatchedButtonClicked_Unity()
	{
		// NOTE: it is common to split batched asset operations in multiple discrete steps due to the fact that
		// during batch operations the AssetDatabase does not update its internal state. Any asset that gets
		// created or delete cannot be imported, therefore any operations on such an asset will have to be split.
		// This is even the case for deleting an asset and then trying to create a new one in its place!
		// Sure, the code is longer and more complex, but the execution speed goes up quite noticably. Value the user's time!

		// delete any existing duplicates
		try
		{
			AssetDatabase.StartAssetEditing();

			var existingDupeGuids =
				AssetDatabase.FindAssets("l:No-SubAssets-DemoAsset", SearchPathStr);
			foreach (var dupeGuid in existingDupeGuids)
			{
				var path = AssetDatabase.GUIDToAssetPath(dupeGuid);
				AssetDatabase.DeleteAsset(path);
			}
		}
		finally
		{
			AssetDatabase.StopAssetEditing();
		}

		// duplicate the assets
		var sourcePaths = new List<Asset.Path>();
		var dupePaths = new List<Asset.Path>();
		try
		{
			AssetDatabase.StartAssetEditing();

			var assetGuids = AssetDatabase.FindAssets("l:New-Shiny-DemoAsset", SearchPathStr);
			for (var i = 0; i < assetGuids.Length; i++)
			{
				var sourceGuid = assetGuids[i];
				var sourcePath = AssetDatabase.GUIDToAssetPath(sourceGuid);

				// only duplicate those with (visible in project view) sub-assets
				var visibleSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(sourcePath);
				if (visibleSubAssets.Length > 0)
				{
					var suffix = "no sub-assets";
					var filenameNoExt = Path.GetFileNameWithoutExtension(sourcePath);
					var dupePath = $"{DemoAssetsPath}/{filenameNoExt} {suffix}.mesh";
					dupePath = AssetDatabase.GenerateUniqueAssetPath(dupePath);
#if UNITY_2022_1_OR_NEWER
					AssetDatabase.CopyAsset(sourcePath, dupePath);
#else
					// in Unity 2021 we have to load, clone and create instead
					// because object and file name have to match (likely a bug in that version)
					var original = AssetDatabase.LoadAssetAtPath<Object>(sourcePath);
					var copy = Instantiate(original);
					AssetDatabase.CreateAsset(copy, dupePath);
#endif

					// must defer loading created assets to post-batchediting
					sourcePaths.Add(sourcePath);
					dupePaths.Add(dupePath);
				}
			}
		}
		finally
		{
			AssetDatabase.StopAssetEditing();
		}

		// remove the subassets and modify the labels
		try
		{
			AssetDatabase.StartAssetEditing();

			for (var i = 0; i < dupePaths.Count; i++)
			{
				var dupeAsset = AssetDatabase.LoadMainAssetAtPath(dupePaths[i]);

				// remove sub-assets from dupe
				var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(dupePaths[i]);
				foreach (var subAsset in subAssets)
					AssetDatabase.RemoveObjectFromAsset(subAsset);

				var sourceGuid = AssetDatabase.AssetPathToGUID(sourcePaths[i]);
				var sourceLabels = AssetDatabase.GetLabels(new GUID(sourceGuid));
				var labels = sourceLabels.ToList();
				labels.Remove("New-Shiny-DemoAsset");
				labels.Add("No-SubAssets-DemoAsset");
				AssetDatabase.SetLabels(dupeAsset, labels.ToArray());
				AssetDatabase.SaveAssetIfDirty(dupeAsset);
			}
		}
		finally
		{
			AssetDatabase.StopAssetEditing();
		}
	}

	private void OnDeleteAllButtonClicked_Unity()
	{
		var allDemoAssetsFilter = "l:New-Shiny-DemoAsset l:No-SubAssets-DemoAsset";
		var guids = AssetDatabase.FindAssets(allDemoAssetsFilter, SearchPathStr);
		foreach (var guid in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			AssetDatabase.DeleteAsset(path);
		}
	}

	private void OnDeletePathButtonClicked_Unity()
	{
		OnDeleteAllButtonClicked_Unity();

		// also delete the subfolder and its parent folder
		AssetDatabase.DeleteAsset(DemoAssetsPath);
		var parentFolder = Path.GetDirectoryName(DemoAssetsPath);
		AssetDatabase.DeleteAsset(parentFolder);
	}
}
