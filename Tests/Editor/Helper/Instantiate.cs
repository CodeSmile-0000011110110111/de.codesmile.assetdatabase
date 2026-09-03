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
#if UNITY_6000_3_OR_NEWER
			// Unity deprecated GetInstanceID and the EntityId-to-int conversion in favour of EntityId.
			// EntityId.GetHashCode is the supported way to get an Int32 that identifies the instance.
			so.InstanceId = so.GetEntityId().GetHashCode();
#else
			so.InstanceId = so.GetInstanceID();
#endif
			so.Ref = so;
			return so;
		}

		public static DifferentExampleSO DifferentExampleSO()
		{
			var so = ScriptableObject.CreateInstance<DifferentExampleSO>();
			so.Text = so.GetType().AssemblyQualifiedName;
#if UNITY_6000_3_OR_NEWER
			// Unity deprecated GetInstanceID and the EntityId-to-int conversion in favour of EntityId.
			// EntityId.GetHashCode is the supported way to get an Int32 that identifies the instance.
			so.InstanceId = so.GetEntityId().GetHashCode();
#else
			so.InstanceId = so.GetInstanceID();
#endif
			return so;
		}
	}
}
