// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Helper
{
	public class ExampleSO : ScriptableObject
	{
		public String AssemblyName;
		public Int32 InstanceId;
		public Single FloatValue;
		public Object ObjectReference;
	}
}
