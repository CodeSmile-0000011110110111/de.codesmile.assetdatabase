// Copyright (C) 2021-2024 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;

public class TestAsset : ScriptableObject
{
	public String Message = "To be, or not to be: that is not a question.";

	private void OnEnable() => name = "Test ScriptableObject";
}
