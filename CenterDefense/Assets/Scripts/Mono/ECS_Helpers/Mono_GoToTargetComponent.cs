using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using WDIB.Components;

[RequireComponent(typeof(ConvertToEntity))]
public class Mono_GoToTargetComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    public Transform targetTransform;

    [SerializeField]
    public ETargetType targetType = ETargetType.staticTarget;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        switch (targetType)
        {
            case ETargetType.staticTarget:
                if (targetTransform == null)
                {
                    Debug.LogError("Position to Target is null - Mono_GoToTarget on object " + gameObject.name);
                    return;
                }

                dstManager.AddComponentData(entity, new TargetStaticPositionTag { });
                break;
            case ETargetType.dynamicTarget:

                //Debug.LogError("Dynamic Target not implemented - Mono_GoToTarget on object " + gameObject.name);
                dstManager.AddComponentData(entity, new TargetDynamicPositionTag { });
                //return;
                break;
        }

        dstManager.AddComponentData(entity, new TargetPosition { Value = targetTransform.position });
    }
}

public enum ETargetType
{
    staticTarget,
    dynamicTarget
}