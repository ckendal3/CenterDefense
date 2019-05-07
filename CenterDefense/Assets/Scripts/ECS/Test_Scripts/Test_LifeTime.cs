using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

// Maybe Create a test class for all the systems?
public class Test_LifeTime : MonoBehaviour
{
    private EntityManager eManager;
    private EntityArchetype testArch;

    [SerializeField]
    public float lifeTime = 5f;

    private void Awake()
    {
        eManager = World.Active.EntityManager;

        testArch = eManager.CreateArchetype(
                                            ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(),
                                            ComponentType.ReadWrite<Scale>(),
                                            ComponentType.ReadWrite<LifeTime>(), ComponentType.ReadWrite<RenderMesh>(),
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
        eManager.SetComponentData(newEnt, new Scale { Value = go.transform.localScale.x });
        eManager.SetComponentData(newEnt, new LifeTime { Value = lifeTime });
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
