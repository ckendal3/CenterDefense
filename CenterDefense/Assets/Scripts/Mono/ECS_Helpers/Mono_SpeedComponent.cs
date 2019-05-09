using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using WDIB.Components;

[RequireComponent(typeof(ConvertToEntity))]
public class Mono_SpeedComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    public float speed = 5;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Speed { Value = speed });
    }
}