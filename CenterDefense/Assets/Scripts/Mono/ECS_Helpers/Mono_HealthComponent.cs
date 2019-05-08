using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using WDIB.Components;

[RequireComponent(typeof(ConvertToEntity))]
public class Mono_HealthComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    public float health = 100;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new WDIB.Components.Health { Value = health });
    }
}
