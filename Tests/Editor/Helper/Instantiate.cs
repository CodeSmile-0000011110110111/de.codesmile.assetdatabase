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
			// InstanceId is arbitrary test payload: it only gives the ScriptableObject some serialized
			// content and is never read by any test. GetEntityId is available from 6000.3.6f1 on, and
			// GetInstanceID is obsolete as a warning on 6000.4.11f1 and an error on 6000.5.10f1, so
			// EntityId.GetHashCode is used merely as a convenient Int32. It is not an identity and must
			// not be compared as one.
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
			// InstanceId is arbitrary test payload: it only gives the ScriptableObject some serialized
			// content and is never read by any test. GetEntityId is available from 6000.3.6f1 on, and
			// GetInstanceID is obsolete as a warning on 6000.4.11f1 and an error on 6000.5.10f1, so
			// EntityId.GetHashCode is used merely as a convenient Int32. It is not an identity and must
			// not be compared as one.
			so.InstanceId = so.GetEntityId().GetHashCode();
#else
			so.InstanceId = so.GetInstanceID();
#endif
			return so;
		}
	}
}
