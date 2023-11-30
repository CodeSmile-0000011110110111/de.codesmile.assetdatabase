// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace CodeSmileEditor.Tests.Helper
{
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

		public static DifferentExampleSO DifferentExampleSO()
		{
			var so = ScriptableObject.CreateInstance<DifferentExampleSO>();
			so.Text = so.GetType().AssemblyQualifiedName;
			so.InstanceId = so.GetInstanceID();
			return so;
		}
	}
}
