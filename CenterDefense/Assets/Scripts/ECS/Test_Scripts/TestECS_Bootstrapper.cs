using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;
using WDIB.Components;

public class TestECS_Bootstrapper : MonoBehaviour
{
    private EntityManager eManager;
    private EntityArchetype testArch;

    [SerializeField]
    public Transform lookAtTransform;

    [SerializeField]
    public float speed = 0;

    [SerializeField]
    public bool isTargettingEntity = false;

    [SerializeField]
    public bool isDynamicTarget = false;

    private void Awake()
    {
        eManager = World.Active.EntityManager;

        testArch = eManager.CreateArchetype(
                                            ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(), 
                                            ComponentType.ReadWrite<Speed>(), ComponentType.ReadWrite<RenderMesh>(),
                                            ComponentType.ReadOnly<LocalToWorld>() // Needs this (is the overall transform representation) but doesn't have to be "directly" wrote to or read from
                                            );
    }

    private void Start()
    {
        CreateEntity();
        DestroySelf();
    }

    public void CreateEntity()
    {
        GameObject go = gameObject;
        Entity newEnt = eManager.CreateEntity(testArch);

        eManager.SetComponentData(newEnt, new Translation { Value = go.transform.position });
        eManager.SetComponentData(newEnt, new Rotation { Value = go.transform.rotation });
        eManager.SetComponentData(newEnt, new Speed { Value = speed });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
                                                            {
                                                                mesh = go.GetComponent<MeshFilter>().sharedMesh,
                                                                material = go.GetComponent<MeshRenderer>().sharedMaterial
                                                            });

        if (isTargettingEntity)
        {
            eManager.AddComponentData(newEnt, new TargetPosition { Value = lookAtTransform.position });
            // if to follow entity
            eManager.AddComponentData(newEnt, new TargetDynamicPositionTag { });
            
        }
        else if(isDynamicTarget) // entity to be folloed
        {
            eManager.AddComponentData(newEnt, new DynamicTargetTag { });
        }
        else
        {
            // if to just go to position
            eManager.AddComponentData(newEnt, new TargetPosition { Value = lookAtTransform.position });
            eManager.AddComponentData(newEnt, new TargetStaticPositionTag { });
        }
        

    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}
