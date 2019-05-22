using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

public class Convert_TargetFollow : MonoBehaviour
{
    private EntityManager eManager;
    private EntityArchetype followType;

    [SerializeField]
    public ushort controlledId = 0;

    [SerializeField]
    public bool3 AxisToLock;


    private void Awake()
    {
        eManager = World.Active.EntityManager;

        //Refactor how turrets are controlled
        // Input component means that the turret can shoot, we don't want that
        // we want for its child, the guns to shoot
        followType = eManager.CreateArchetype(
                                            ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(),
                                            ComponentType.ReadWrite<Scale>(),
                                            ComponentType.ReadWrite<Owner>(), ComponentType.ReadWrite<ControlledByPlayer>(),
                                            ComponentType.ReadWrite<RenderMesh>(),
                                            ComponentType.ReadWrite<LocalToWorld>() // Needs this (is the overall transform representation) but doesn't have to be "directly" wrote to or read from
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
        Entity newEnt = eManager.CreateEntity(followType);

        eManager.SetComponentData(newEnt, new Translation { Value = go.transform.position });
        eManager.SetComponentData(newEnt, new Rotation { Value = go.transform.rotation });
        eManager.SetComponentData(newEnt, new Scale { Value = go.transform.localScale.x });
        eManager.SetComponentData(newEnt, new Owner { ID = controlledId });
        eManager.SetComponentData(newEnt, new ControlledByPlayer { lookPosition = math.forward(gameObject.transform.rotation) * 100 });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });


        eManager.AddComponentData(newEnt, new LockAxisRotation { Value = new bool3(AxisToLock.x, AxisToLock.y, AxisToLock.z) }); // lock the y-axis, so it only rotates left and right

#if UNITY_EDITOR
        eManager.SetName(newEnt, "TargetFollower");
#endif
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
