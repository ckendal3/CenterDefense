using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct WeaponComponent : IComponentData
{
    public int projectileId;
    public int timeBetweenShots;
}

