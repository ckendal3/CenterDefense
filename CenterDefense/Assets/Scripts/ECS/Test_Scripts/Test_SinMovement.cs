using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

public class Test_SinMovement : MonoBehaviour
{
    private EntityManager eManager;
    private EntityArchetype sinType;

    [SerializeField]
    public float floatiness = 1;

    private void Awake()
    {
        eManager = World.Active.EntityManager;
        sinType = eManager.CreateArchetype(
                                        ComponentType.ReadWrite<LocalToWorld>(), ComponentType.ReadWrite<Translation>(), 
                                        ComponentType.ReadWrite<Rotation>(), ComponentType.ReadWrite<Scale>(),
                                        ComponentType.ReadWrite<RenderMesh>(),
                                        ComponentType.ReadOnly<SineMovementTag>(), ComponentType.ReadWrite<SineMovementFloatiness>() // look at component for notes
                                        );
    }

    private void Start()
    {
        CreateEntity();
        DestroySelf();
    }

    public void CreateEntity()
    {
        Entity newEnt = eManager.CreateEntity(sinType);

        eManager.SetComponentData(newEnt, new Translation { Value = gameObject.transform.position });
        eManager.SetComponentData(newEnt, new Rotation { Value = gameObject.transform.rotation });
        eManager.SetComponentData(newEnt, new Scale { Value = gameObject.transform.localScale.x });
        eManager.SetComponentData(newEnt, new SineMovementFloatiness { Value = floatiness });

        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = gameObject.GetComponent<MeshFilter>().sharedMesh,
            material = gameObject.GetComponent<MeshRenderer>().sharedMaterial
        });
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}