using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using WDIB.Components;

[RequireComponent(typeof(ConvertToEntity))]
public class Mono_LifeTimeComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    public float life = 5;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LifeTime { Value = life });
    }
}
