// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;

namespace Helper
{
	public static class Instantiate
	{
		public static ScriptableObject ExampleSO()
		{
			var so = ScriptableObject.CreateInstance<ExampleSO>();
			so.AssemblyName = so.GetType().AssemblyQualifiedName;
			so.InstanceId = so.GetInstanceID();
			so.ObjectReference = so;
			return so;
		}
	}
}
