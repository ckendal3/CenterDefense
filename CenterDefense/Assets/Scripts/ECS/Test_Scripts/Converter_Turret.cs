using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

public class Converter_Turret : MonoBehaviour
{
    private EntityManager eManager;
    private EntityArchetype testArch;

    [SerializeField]
    public ushort controlledId = 0;


    // probably just make GameObject array for children
    [SerializeField]
    public GameObject Child1;

    [SerializeField]
    public Transform projectileSpawnTransform1;

    [SerializeField]
    public Transform projectileSpawnTransform2;

    private void Awake()
    {
        eManager = World.Active.EntityManager;

        //Refactor how turrets are controlled
        // Input component means that the turret can shoot, we don't want that
        // we want for its child, the guns to shoot
        testArch = eManager.CreateArchetype(
                                            ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(),
                                            ComponentType.ReadWrite<Scale>(),
                                            ComponentType.ReadWrite<Owner>(), ComponentType.ReadWrite<ControlledByPlayer>(),
                                            ComponentType.ReadWrite<RenderMesh>(),
                                            ComponentType.ReadWrite<LocalToWorld>(), ComponentType.ReadWrite<InputComponent>() // Needs this (is the overall transform representation) but doesn't have to be "directly" wrote to or read from
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
        //eManager.SetComponentData(newEnt, new Translation { Value = new float3(1,1,1) });
        eManager.SetComponentData(newEnt, new Rotation { Value = go.transform.rotation });
        eManager.SetComponentData(newEnt, new Scale { Value = go.transform.localScale.x });
        eManager.SetComponentData(newEnt, new Owner { ID = controlledId });
        eManager.SetComponentData(newEnt, new ControlledByPlayer { lookPosition = math.forward(gameObject.transform.rotation) * 100});
        eManager.SetComponentData(newEnt, new InputComponent { InputPosition = new float2(gameObject.transform.position.x, gameObject.transform.position.y), ID = 0, PressedDown = false });
        eManager.SetSharedComponentData(newEnt, new RenderMesh
        {
            mesh = go.GetComponent<MeshFilter>().sharedMesh,
            material = go.GetComponent<MeshRenderer>().sharedMaterial
        });

        // this component is causing issues
        //eManager.AddComponentData(newEnt, new LockAxisRotation { Value = new bool3(true, false, true) }); // lock everything but the y-axis, so it rotates left and right

       // CreateChildEntity(newEnt);
    }

    public void CreateChildEntity(Entity parent)
    {
        Entity newEnt = eManager.CreateEntity();


        // Set child's parent to the main entity
        // Set the local position relative to the parent
        // Set our scale
        // set our projectile spawn points
        // add turret stuff
        // add the follow input thing - ControlledByPlayer and InputComponent
        // lock its axis
        // add rendermesh
        // set owner
        eManager.AddComponentData(newEnt, new LocalToWorld {});
        eManager.AddComponentData(newEnt, new LocalToParent { });
        eManager.AddComponentData(newEnt, new Parent { Value = parent });
        eManager.AddComponentData(newEnt, new Translation { Value = Child1.transform.position - gameObject.transform.position });
        eManager.AddComponentData(newEnt, new Scale { Value = Child1.transform.localScale.x });
        eManager.AddComponentData(newEnt, new DoubleProjectileSpawnPoint { Position1 = projectileSpawnTransform1.position - gameObject.transform.position,
                                                                            Position2 = projectileSpawnTransform2.position - gameObject.transform.position
                                                                            });
        eManager.AddComponentData(newEnt, new ControlledByPlayer { lookPosition = math.forward(gameObject.transform.rotation) * 100});
        eManager.AddComponentData(newEnt, new InputComponent { InputPosition = new float2(gameObject.transform.position.x, gameObject.transform.position.y), ID = 0, PressedDown = false });
        eManager.AddComponentData(newEnt, new LockAxisRotation { Value = new bool3(false, true, true)}); // lock everything but the x-axis for rotating so it just looks up and down
        eManager.AddSharedComponentData(newEnt, new ShooterTag { });
        eManager.AddComponentData(newEnt, new Owner { ID = controlledId });

        eManager.AddSharedComponentData(newEnt, new RenderMesh
        {
            mesh = Child1.GetComponent<MeshFilter>().sharedMesh,
            material = Child1.GetComponent<MeshRenderer>().sharedMaterial
        });
    }

    public void DestroySelf()
    {
        Destroy(Child1);
        Destroy(gameObject);
    }
}
