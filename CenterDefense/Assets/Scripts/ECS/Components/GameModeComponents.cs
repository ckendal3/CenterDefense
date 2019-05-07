using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct RoundTimeLimit : ISharedComponentData
{
    public float Value;
}

public struct CountDownTime : IComponentData
{
    public float Value;
}

public struct CountUpTime : IComponentData
{
    public float Value;
}