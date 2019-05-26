using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntityComponent : MonoBehaviour
{
    private Entity entity;
    private EntityManager eManager;

    public void Initialize<T>(T componentData) where T : struct, IComponentData
    {
        eManager = World.Active.EntityManager;

        entity = eManager.CreateEntity();

        eManager.AddComponentData<T>(entity, componentData);
    }

    private void OnDestroy()
    {
        if (eManager.Exists(entity))
        {
            eManager.DestroyEntity(entity);
        }
    }
}
