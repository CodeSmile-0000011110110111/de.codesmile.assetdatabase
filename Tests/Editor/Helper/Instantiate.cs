// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

public static class Instantiate
{
	public static ScriptableObject ExampleSO()
	{
		var so = ScriptableObject.CreateInstance<ExampleSO>();
		so.Text = so.GetType().AssemblyQualifiedName;
		so.InstanceId = so.GetInstanceID();
		so.Ref = so;
		return so;
	}

	public static ScriptableObject DifferentExampleSO()
	{
		var so = ScriptableObject.CreateInstance<DifferentExampleSO>();
		so.Text = so.GetType().AssemblyQualifiedName;
		so.InstanceId = so.GetInstanceID();
		so.Ref = so;
		return so;
	}
}
