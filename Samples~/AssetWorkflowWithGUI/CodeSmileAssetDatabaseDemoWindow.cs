// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile_AssetDatabase_Demo;
using CodeSmileEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CodeSmileAssetDatabaseDemoWindow : EditorWindow
{
	private static readonly Asset.Path DemoAssetsPath = "Assets/CodeSmile AssetDatabase Demo Assets/Sample Assets";

	[SerializeField] private VisualTreeAsset m_VisualTreeAsset;

	[MenuItem("Window/CodeSmile/AssetDatabase Examples/Workflow Demo", priority = 2999)]
	public static void ShowAssetDatabaseDemoWindow()
	{
		var wnd = GetWindow<CodeSmileAssetDatabaseDemoWindow>();
		wnd.titleContent = new GUIContent("CodeSmile AssetDatabase Demo");
	}

	public void CreateGUI()
	{
		var uiBuilderDocument = m_VisualTreeAsset.Instantiate();
		rootVisualElement.Add(uiBuilderDocument);

		RegisterCallbacks(true);
	}

	private void OnDestroy() => RegisterCallbacks(false);

	private void RegisterCallbacks(Boolean register)
	{
		var createButton = rootVisualElement.Q<Button>("CreateButton");
		var duplicateButton = rootVisualElement.Q<Button>("DuplicateButton");
		var duplicateBatchedButton = rootVisualElement.Q<Button>("DuplicateBatchedButton");
		var deleteAllButton = rootVisualElement.Q<Button>("DeleteAllButton");
		var deletePathButton = rootVisualElement.Q<Button>("DeletePathButton");

		if (register)
		{
			createButton.clicked += OnCreateButtonClicked;
			duplicateButton.clicked += OnDuplicateButtonClicked;
			duplicateBatchedButton.clicked += OnDuplicateBatchedButtonClicked;
			deleteAllButton.clicked += OnDeleteAllButtonClicked;
			deletePathButton.clicked += OnDeletePathButtonClicked;
		}
		else
		{
			createButton.clicked -= OnCreateButtonClicked;
			duplicateButton.clicked -= OnDuplicateButtonClicked;
			duplicateBatchedButton.clicked -= OnDuplicateBatchedButtonClicked;
			deleteAllButton.clicked -= OnDeleteAllButtonClicked;
			deletePathButton.clicked -= OnDeletePathButtonClicked;
		}
	}

	private void OnCreateButtonClicked()
	{
		var numberOfAssets = 32;
		for (var i = 0; i < numberOfAssets; i++)
		{
			// Let's assume we've generated a wonderful mesh together with an impressive material using a beautiful texture and some scripting
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

			// Now we create an asset and stuff it all in there (just for illustration purposes - this asset is totally useless)
			// Note: meshes require the .mesh extension
			var assetPath = $"{DemoAssetsPath}/Mesh #{i}-{Time.realtimeSinceStartupAsDouble:F3}.mesh";
			var createdAsset = new Asset(mesh, assetPath);
			createdAsset.AddSubAsset(texture);
			createdAsset.AddSubAsset(material);
			createdAsset.AddSubAsset(animation);
			createdAsset.AddSubAsset(scriptableObject);
			createdAsset.AddLabels(new[] { "CodeSmile AssetDatabase", "New-Shiny-DemoAsset" });
			createdAsset.Save(); // save is needed to write sub assets and labels to disk - creating the asset by itself does NOT need saving
		}
	}

	private void OnDuplicateButtonClicked()
	{
		// delete the existing duplicates
		var dupePaths = Asset.File.FindPaths("l:No-SubAssets-DemoAsset", new[] { DemoAssetsPath });
		foreach (var dupePath in dupePaths)
			Asset.File.Delete(dupePath);

		// duplicate assets with sub assets
		var assetPaths = Asset.File.FindPaths("l:New-Shiny-DemoAsset", new[] { DemoAssetsPath });
		foreach (var assetPath in assetPaths)
		{
			var sourceAsset = new Asset(assetPath);

			// only duplicate those with (visible in project view) sub-assets
			if (sourceAsset.VisibleSubAssets.Length > 0)
			{
				// create a duplicate
				var dupePath = $"{DemoAssetsPath}/{assetPath.FileNameWithoutExtension} no sub-assets.mesh";
				var dupeAsset = sourceAsset.SaveAsNew(dupePath);

				// remove sub-assets from dupe
				foreach (var subAsset in dupeAsset.VisibleSubAssets)
					dupeAsset.RemoveSubAsset(subAsset);

				// tag and save the dupe
				dupeAsset.AddLabels(sourceAsset.Labels);
				dupeAsset.RemoveLabel("New-Shiny-DemoAsset");
				dupeAsset.AddLabel("No-SubAssets-DemoAsset");
				dupeAsset.Save();
			}
		}
	}

	private void OnDuplicateBatchedButtonClicked()
	{
		// NOTE: it is common to split batched asset operations in multiple discrete steps due to the fact that
		// during batch operations the AssetDatabase does not update its internal state. Any asset that gets
		// created or delete cannot be imported, therefore any operations on such an asset will have to be split.
		// This is even the case for deleting an asset and then trying to create a new one in its place!
		// Sure, the code is longer and more complex, but the execution speed goes up quite noticably. Value the user's time!

		// delete any existing duplicates
		Asset.File.BatchEditing(() =>
		{
			var existingDupePaths = Asset.File.FindPaths("l:No-SubAssets-DemoAsset", new[] { DemoAssetsPath });
			foreach (var dupePath in existingDupePaths)
				Asset.File.Delete(dupePath);
		});

		// duplicate the assets
		var sourcePaths = new List<Asset.Path>();
		var dupePaths = new List<Asset.Path>();
		Asset.File.BatchEditing(() =>
		{
			var assetPaths = Asset.File.FindPaths("l:New-Shiny-DemoAsset", new[] { DemoAssetsPath });
			for (var i = 0; i < assetPaths.Length; i++)
			{
				var sourcePath = assetPaths[i];

				// only duplicate those with (visible in project view) sub-assets
				if (Asset.SubAsset.LoadVisible(sourcePath).Length > 0)
				{
					var suffix = "no sub-assets";
					var dupePath = new Asset.Path($"{DemoAssetsPath}/{sourcePath.FileNameWithoutExtension} {suffix}.mesh");

					Asset.File.CopyAsNew(sourcePath, dupePath);

					// cannot load newly created assets during batch editing => must defer following operations
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

				dupeAsset.AddLabels(Asset.Label.GetAll(sourcePaths[i]));
				dupeAsset.RemoveLabel("New-Shiny-DemoAsset");
				dupeAsset.AddLabel("No-SubAssets-DemoAsset");
				dupeAsset.Save();
			}
		});
	}

	private void OnDeleteAllButtonClicked()
	{
		// Uses only the static API as we don't need to work with an Asset instance here

		var folders = new[] { DemoAssetsPath };
		var paths = Asset.File.FindPaths("l:New-Shiny-DemoAsset l:No-SubAssets-DemoAsset", folders);

		Asset.File.Delete(paths);
	}

	private void OnDeletePathButtonClicked()
	{
		OnDeleteAllButtonClicked();

		// also delete the subfolder and its parent folder
		Asset.File.Delete(DemoAssetsPath);
		Asset.File.Delete(DemoAssetsPath.FolderPath);
	}
}
