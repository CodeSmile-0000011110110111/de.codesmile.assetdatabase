// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmileEditor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public partial class AssetDemoWindow : EditorWindow
{
	private static readonly Asset.Path DemoAssetsPath =
		"Assets/CodeSmile AssetDatabase Demo Assets/Sample Assets";
	private static readonly Asset.Path[] SearchPath =
		{ "Assets/CodeSmile AssetDatabase Demo Assets/Sample Assets" };
	private static readonly String[] SearchPathStr =
		{ "Assets/CodeSmile AssetDatabase Demo Assets/Sample Assets" };

	[SerializeField] private VisualTreeAsset m_VisualTreeAsset;

	private API m_SelectedAPI = API.CodeSmile;

	[MenuItem("Window/CodeSmile/AssetDatabase Examples/Workflow Demo", priority = 2999)]
	public static void ShowAssetDatabaseDemoWindow()
	{
		var wnd = GetWindow<AssetDemoWindow>();
		wnd.titleContent = new GUIContent("CodeSmile AssetDatabase Demo");
	}

	public void CreateGUI()
	{
		var uiBuilderDocument = m_VisualTreeAsset.Instantiate();
		rootVisualElement.Add(uiBuilderDocument);

		HandleApiSelection();
		RegisterCallbacks(true);
	}

	private void OnDestroy() => RegisterCallbacks(false);

	private void HandleApiSelection()
	{
		var apiSelection = rootVisualElement.Q<DropdownField>("SelectAPI");
		apiSelection.RegisterValueChangedCallback(evt =>
		{
			RegisterCallbacks(false); // unregister current
			m_SelectedAPI = apiSelection.index == 0 ? API.CodeSmile : API.Unity;
			RegisterCallbacks(true); // re-register new
		});
	}

	private void RegisterCallbacks(Boolean register)
	{
		var createButton = rootVisualElement.Q<Button>("CreateButton");
		var duplicateButton = rootVisualElement.Q<Button>("DuplicateButton");
		var duplicateBatchedButton = rootVisualElement.Q<Button>("DuplicateBatchedButton");
		var deleteAllButton = rootVisualElement.Q<Button>("DeleteAllButton");
		var deletePathButton = rootVisualElement.Q<Button>("DeletePathButton");

		// these handlers use CodeSmile AssetDatabase methods
		if (m_SelectedAPI == API.CodeSmile)
		{
			if (register)
			{
				createButton.clicked += OnCreateButtonClicked_CodeSmile;
				duplicateButton.clicked += OnDuplicateButtonClicked_CodeSmile;
				duplicateBatchedButton.clicked += OnDuplicateBatchedButtonClicked_CodeSmile;
				deleteAllButton.clicked += OnDeleteAllButtonClicked_CodeSmile;
				deletePathButton.clicked += OnDeletePathButtonClicked_CodeSmile;
			}
			else
			{
				createButton.clicked -= OnCreateButtonClicked_CodeSmile;
				duplicateButton.clicked -= OnDuplicateButtonClicked_CodeSmile;
				duplicateBatchedButton.clicked -= OnDuplicateBatchedButtonClicked_CodeSmile;
				deleteAllButton.clicked -= OnDeleteAllButtonClicked_CodeSmile;
				deletePathButton.clicked -= OnDeletePathButtonClicked_CodeSmile;
			}
		}
		else
		{
			// these handlers use Unity's AssetDabase methods
			if (register)
			{
				createButton.clicked += OnCreateButtonClicked_Unity;
				duplicateButton.clicked += OnDuplicateButtonClicked_Unity;
				duplicateBatchedButton.clicked += OnDuplicateBatchedButtonClicked_Unity;
				deleteAllButton.clicked += OnDeleteAllButtonClicked_Unity;
				deletePathButton.clicked += OnDeletePathButtonClicked_Unity;
			}
			else
			{
				createButton.clicked -= OnCreateButtonClicked_Unity;
				duplicateButton.clicked -= OnDuplicateButtonClicked_Unity;
				duplicateBatchedButton.clicked -= OnDuplicateBatchedButtonClicked_Unity;
				deleteAllButton.clicked -= OnDeleteAllButtonClicked_Unity;
				deletePathButton.clicked -= OnDeletePathButtonClicked_Unity;
			}
		}
	}

	private enum API
	{
		CodeSmile,
		Unity,
	}
}
