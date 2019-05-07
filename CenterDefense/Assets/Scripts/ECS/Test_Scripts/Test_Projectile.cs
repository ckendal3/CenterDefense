using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

public class Test_Projectile : MonoBehaviour
{
    private EntityManager eManager;
    private EntityArchetype testArch;

    [SerializeField]
    public float speed = 0;

    private void Awake()
    {
        eManager = World.Active.EntityManager;

        testArch = eManager.CreateArchetype(
                                            ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(),
                                            ComponentType.ReadWrite<Speed>(), ComponentType.ReadWrite<PreviousTranslation>(),
                                            ComponentType.ReadWrite<RenderMesh>(), ComponentType.ReadOnly<LocalToWorld>() // Needs this (is the overall transform representation) but doesn't have to be "directly" wrote to or read from
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
        eManager.SetComponentData(newEnt, new PreviousTranslation { Value = go.transform.position });
        eManager.SetComponentData(newEnt, new Rotation { Value = go.transform.rotation });
        eManager.SetComponentData(newEnt, new Speed { Value = speed });
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
