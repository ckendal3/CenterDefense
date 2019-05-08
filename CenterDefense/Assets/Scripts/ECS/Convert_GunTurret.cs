using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

public class Convert_GunTurret : MonoBehaviour
{
    private EntityManager eManager;
    private EntityArchetype gunType;

    [SerializeField]
    public ushort controlledId = 0;

    [SerializeField]
    public float timeBetweenShots = .7f;

    [SerializeField]
    public bool3 AxisToLock;

    [SerializeField]
    public Transform projectileSpawnPoint;

    private void Awake()
    {
        eManager = World.Active.EntityManager;

        //Refactor how turrets are controlled
        // Input component means that the turret can shoot, we don't want that
        // we want for its child, the guns to shoot
        gunType = eManager.CreateArchetype(
                                            ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(),
                                            ComponentType.ReadWrite<Scale>(),
                                            ComponentType.ReadWrite<Owner>(), ComponentType.ReadWrite<ControlledByPlayer>(),
                                            ComponentType.ReadWrite<RenderMesh>(),
                                            ComponentType.ReadWrite<LocalToWorld>(), ComponentType.ReadWrite<InputComponent>(), // Needs this (is the overall transform representation) but doesn't have to be "directly" wrote to or read from
                                            ComponentType.ReadWrite<TimeBetweenShots>()
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
        Entity newEnt = eManager.CreateEntity(gunType);

        eManager.SetComponentData(newEnt, new Translation { Value = go.transform.position });
        eManager.SetComponentData(newEnt, new Rotation { Value = go.transform.rotation });
        eManager.SetComponentData(newEnt, new Scale { Value = go.transform.localScale.x });
        eManager.SetComponentData(newEnt, new Owner { ID = controlledId });
        eManager.SetComponentData(newEnt, new ControlledByPlayer { lookPosition = math.forward(gameObject.transform.rotation) * 100 });
        eManager.SetComponentData(newEnt, new InputComponent { InputPosition = new float2(gameObject.transform.position.x, gameObject.transform.position.y), ID = 0, PressedDown = false });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });

        eManager.SetComponentData(newEnt, new TimeBetweenShots { Value = timeBetweenShots, timeLeft = timeBetweenShots });

        eManager.AddComponentData(newEnt, new LockAxisRotation { Value = new bool3(AxisToLock.x, AxisToLock.y, AxisToLock.z) }); // lock the y-axis, so it only rotates left and right
        eManager.AddComponentData(newEnt, new ProjectileSpawnPoint { Value = projectileSpawnPoint.position });
        eManager.AddComponentData(newEnt, new ShooterTag { } );
        
#if UNITY_EDITOR
        eManager.SetName(newEnt, "Turret_Gun");
#endif
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}