using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class Test_ZeroError : MonoBehaviour
{
    private EntityManager eManager;
    private EntityArchetype testArch;

    private void Awake()
    {
        eManager = World.Active.EntityManager;

        testArch = eManager.CreateArchetype(
                                            ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(),
                                            ComponentType.ReadWrite<RenderMesh>(), ComponentType.ReadWrite<LocalToWorld>() // Needs this (is the overall transform representation) but doesn't have to be "directly" wrote to or read from
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

        // *********************************************************************************************************

        Entity newEnt = eManager.CreateEntity(testArch);
        eManager.SetComponentData(newEnt, new Translation { Value = new float3(0, 0, 0) });
        eManager.SetComponentData(newEnt, new Rotation { Value = quaternion.Euler(new float3(0, 0, 0)) });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });

        // *********************************************************************************************************

        newEnt = eManager.CreateEntity(testArch);
        eManager.SetComponentData(newEnt, new Translation { Value = new float3(1,0,0) });
        eManager.SetComponentData(newEnt, new Rotation { Value = quaternion.Euler(new float3(0,0,0)) });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });

        // *********************************************************************************************************

        newEnt = eManager.CreateEntity(testArch);
        eManager.SetComponentData(newEnt, new Translation { Value = new float3(0, 1, 0) });
        eManager.SetComponentData(newEnt, new Rotation { Value = quaternion.Euler(new float3(0, 0, 0)) });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });

        // *********************************************************************************************************

        newEnt = eManager.CreateEntity(testArch);
        eManager.SetComponentData(newEnt, new Translation { Value = new float3(0, 0, 1) });
        eManager.SetComponentData(newEnt, new Rotation { Value = quaternion.Euler(new float3(0, 0, 0)) });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });

        // *********************************************************************************************************

        newEnt = eManager.CreateEntity(testArch);
        eManager.SetComponentData(newEnt, new Translation { Value = new float3(1, 1, 0) });
        eManager.SetComponentData(newEnt, new Rotation { Value = quaternion.Euler(new float3(0, 0, 0)) });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });

        // *********************************************************************************************************

        newEnt = eManager.CreateEntity(testArch);
        eManager.SetComponentData(newEnt, new Translation { Value = new float3(1, 0, 1) });
        eManager.SetComponentData(newEnt, new Rotation { Value = quaternion.Euler(new float3(0, 0, 0)) });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });

        // *********************************************************************************************************

        newEnt = eManager.CreateEntity(testArch);
        eManager.SetComponentData(newEnt, new Translation { Value = new float3(0, 1, 1) });
        eManager.SetComponentData(newEnt, new Rotation { Value = quaternion.Euler(new float3(0, 0, 0)) });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });

        // *********************************************************************************************************

        newEnt = eManager.CreateEntity(testArch);
        eManager.SetComponentData(newEnt, new Translation { Value = new float3(1, 1, 1) });
        eManager.SetComponentData(newEnt, new Rotation { Value = quaternion.Euler(new float3(0, 0, 0)) });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
