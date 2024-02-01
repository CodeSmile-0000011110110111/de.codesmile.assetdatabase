// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile_AssetDatabase_Demo;
using CodeSmileEditor;
using System.Collections.Generic;
using UnityEngine;

public partial class AssetDemoWindow
{
	private void OnCreateButtonClicked_CodeSmile()
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
			var createTime = $"{Time.realtimeSinceStartupAsDouble:F3}";
			createTime = createTime.Replace(".", "-").Replace(",", "-");
			var assetPath = $"{DemoAssetsPath}/Mesh #{i}-{createTime}.mesh";

			var createdAsset = new Asset(mesh, assetPath);
			createdAsset.AddSubAsset(texture);
			createdAsset.AddSubAsset(material);
			createdAsset.AddSubAsset(animation);
			createdAsset.AddSubAsset(scriptableObject);
			createdAsset.AddLabels(new[] { "CodeSmile AssetDatabase", "New-Shiny-DemoAsset" });
			// save is needed only to write the post-creation changes (added sub-assets & labels)
			createdAsset.Save();
		}
	}

	private void OnDuplicateButtonClicked_CodeSmile()
	{
		// delete the existing duplicates
		var dupePaths = Asset.File.FindPaths("l:No-SubAssets-DemoAsset", SearchPath);
		foreach (var dupePath in dupePaths)
			Asset.File.Delete(dupePath);

		// duplicate assets with sub assets
		var assetPaths = Asset.File.FindPaths("l:New-Shiny-DemoAsset", SearchPath);
		foreach (var assetPath in assetPaths)
		{
			var sourceAsset = new Asset(assetPath);

			// only duplicate those with (visible in project view) sub-assets
			if (sourceAsset.VisibleSubAssets.Length > 0)
			{
				// create a duplicate
				var filenameNoExt = $"{assetPath.FileNameWithoutExtension} no sub-assets";
				var dupePath = $"{DemoAssetsPath}/{filenameNoExt}.mesh";
				var dupeAsset = sourceAsset.SaveAsNew(dupePath);

				// remove sub-assets from dupe
				foreach (var subAsset in dupeAsset.VisibleSubAssets)
					dupeAsset.RemoveSubAsset(subAsset);

				// tag and save the dupe
				dupeAsset.Labels = sourceAsset.Labels; // restore source labels
				dupeAsset.RemoveLabel("New-Shiny-DemoAsset");
				dupeAsset.AddLabel("No-SubAssets-DemoAsset");
				dupeAsset.Save();
			}
		}
	}

	private void OnDuplicateBatchedButtonClicked_CodeSmile()
	{
		// NOTE: it is common to split batched asset operations in multiple discrete steps due to the fact that
		// during batch operations the AssetDatabase does not update its internal state. Any asset that gets
		// created or delete cannot be imported, therefore any operations on such an asset will have to be split.
		// This is even the case for deleting an asset and then trying to create a new one in its place!
		// Sure, the code is longer and more complex, but the execution speed goes up quite noticably. Value the user's time!

		// delete any existing duplicates
		Asset.File.BatchEditing(() =>
		{
			var existingDupePaths =
				Asset.File.FindPaths("l:No-SubAssets-DemoAsset", SearchPath);
			foreach (var dupePath in existingDupePaths)
				Asset.File.Delete(dupePath);
		});

		// duplicate the assets
		var sourcePaths = new List<Asset.Path>();
		var dupePaths = new List<Asset.Path>();
		Asset.File.BatchEditing(() =>
		{
			var assetPaths = Asset.File.FindPaths("l:New-Shiny-DemoAsset", SearchPath);
			for (var i = 0; i < assetPaths.Length; i++)
			{
				var sourcePath = assetPaths[i];

				// only duplicate those with (visible in project view) sub-assets
				if (Asset.SubAsset.LoadVisible(sourcePath).Length > 0)
				{
					var suffix = "no sub-assets";
					var filenameNoExt = sourcePath.FileNameWithoutExtension;
					Asset.Path dupePath = $"{DemoAssetsPath}/{filenameNoExt} {suffix}.mesh";
					Asset.File.CopyAsNew(sourcePath, dupePath);

					// must defer loading created assets to post-batchediting
					sourcePaths.Add(sourcePath);
					dupePaths.Add(dupePath);
				}
			}
		});

		// remove the subassets and modify the labels
		Asset.File.BatchEditing(() =>
		{
			for (var i = 0; i < dupePaths.Count; i++)
			{
				var dupeAsset = new Asset(dupePaths[i]);

				// remove sub-assets from dupe
				foreach (var subAsset in dupeAsset.VisibleSubAssets)
					dupeAsset.RemoveSubAsset(subAsset);

				dupeAsset.Labels = Asset.Label.GetAll(sourcePaths[i]); // restore source labels
				dupeAsset.RemoveLabel("New-Shiny-DemoAsset");
				dupeAsset.AddLabel("No-SubAssets-DemoAsset");
				dupeAsset.Save();
			}
		});
	}

	private void OnDeleteAllButtonClicked_CodeSmile()
	{
		// Uses only the static API as we don't need to work with an Asset instance here
		var allDemoAssetsFilter = "l:New-Shiny-DemoAsset l:No-SubAssets-DemoAsset";
		var paths = Asset.File.FindPaths(allDemoAssetsFilter, SearchPath);
		Asset.File.Delete(paths);
	}

	private void OnDeletePathButtonClicked_CodeSmile()
	{
		OnDeleteAllButtonClicked_CodeSmile();

		// also delete the subfolder and its parent folder
		Asset.File.Delete(DemoAssetsPath);
		Asset.File.Delete(DemoAssetsPath.FolderPath);
	}
}
