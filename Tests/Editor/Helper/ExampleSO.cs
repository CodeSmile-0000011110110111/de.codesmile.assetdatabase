// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class ExampleSO : ScriptableObject
{
	public Object ObjectReference;
	public String AssemblyName;
	public Int32 InstanceId;
	public Single FloatValue;
}