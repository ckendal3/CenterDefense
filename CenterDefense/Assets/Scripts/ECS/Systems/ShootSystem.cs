using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

// update after the turrets turn
//[UpdateInGroup(typeof(InputSystemGroup))]
//[UpdateAfter(typeof(InputSystem))]
[UpdateInGroup(typeof(HitSystemGroup))]
public class ShootSystem : JobComponentSystem
{
    private EntityQuery m_gunGroup;
    private EntityArchetype projectileArch;
    private EntityManager eManager;

    // Refactor
    private Mesh useMesh;
    private Material useMat;

    //**********************************************************
    //***********************REFACTOR***************************
    //**********************************************************
    // Should probably check ID so we don't spawn projectiles
    // for every single entity that receives input/ is controlled by any player
    // ------------
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        // refactor ************************************
        useMesh = MeshManager.Instance.GetMesh();
        useMat = MeshManager.Instance.GetMaterial();
        // *********************************************

        int count = m_gunGroup.CalculateLength();

        NativeArray<InputComponent> inputs = m_gunGroup.ToComponentDataArray<InputComponent>(Allocator.TempJob);
        NativeArray<Translation> positions = m_gunGroup.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<Rotation> rotations = m_gunGroup.ToComponentDataArray<Rotation>(Allocator.TempJob);
        NativeArray<ProjectileSpawnPoint> projSpawnPoints = m_gunGroup.ToComponentDataArray<ProjectileSpawnPoint>(Allocator.TempJob);

        // create all the projectiles 
        NativeArray<Entity> projectiles = new NativeArray<Entity>(count, Allocator.TempJob);
        eManager.CreateEntity(projectileArch, projectiles);        

        Entity tmp;
        for(int i = 0; i < count; i++)
        {
            tmp = projectiles[i];
            // if we aren't looking to shoot - don't do stuff
            if (inputs[i].PressedDown == false)
            {
                eManager.DestroyEntity(tmp);
                continue;
            }

            // if we should shoot multiple projectiles at once
            // add logic if we should shoot several projectiles at once
            // add logic to switch to different barrels

            // Fix this to work correctly with the offset - keep(positions[i].Value + math.forward(rotations[i].Value)) then add stuff
            eManager.SetComponentData(tmp, new Translation { Value = positions[i].Value + (math.forward(rotations[i].Value) * math.distance(projSpawnPoints[i].Value, positions[i].Value)) }); //+ (projSpawnPoints[i].Value + math.forward(rotations[i].Value)) + )
            eManager.SetComponentData(tmp, new PreviousTranslation { Value = positions[i].Value });
            eManager.SetComponentData(tmp, new Rotation { Value = rotations[i].Value });
            eManager.SetComponentData(tmp, new Scale { Value = 1f }); // refactor 
            eManager.SetComponentData(tmp, new Speed { Value = 10f }); // refactor use a scriptable object to pull data from
            eManager.SetComponentData(tmp, new Owner { ID = 0 }); //refactor
            eManager.SetComponentData(tmp, new LifeTime { Value = 2.5f }); //refactor use a scriptable object to pull data from
            eManager.SetSharedComponentData(tmp, new RenderMesh { mesh = useMesh, material = useMat});

#if UNITY_EDITOR
            eManager.SetName(tmp, "Projectile");
#endif

            //eManager.AddComponentData(tmp, new ControlledByPlayer { lookPosition = positions[i].Value });
        }

        inputs.Dispose();
        positions.Dispose();
        rotations.Dispose();
        projSpawnPoints.Dispose();
        projectiles.Dispose();

        return inputDeps;
    }

    protected override void OnCreate()
    {
        eManager = World.EntityManager;

        m_gunGroup = GetEntityQuery(new EntityQueryDesc
        {
            // must have these components
            All = new ComponentType[] { ComponentType.ReadOnly<InputComponent>(),ComponentType.ReadOnly<Translation>(),
                                        ComponentType.ReadOnly<Rotation>(), ComponentType.ReadOnly<ShooterTag>(), ComponentType.ReadOnly<ProjectileSpawnPoint>()}, 

            // Can have these
            Any = new ComponentType[] { ComponentType.ReadOnly<ControlledByPlayer>() }
        });

        // create our archetype
        projectileArch = eManager.CreateArchetype(
                                    ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(), ComponentType.ReadWrite<Scale>(),
                                    ComponentType.ReadWrite<Speed>(), ComponentType.ReadWrite<Owner>(), ComponentType.ReadWrite<PreviousTranslation>(),
                                    ComponentType.ReadWrite<LifeTime>(),
                                    ComponentType.ReadWrite<RenderMesh>(), ComponentType.ReadOnly<LocalToWorld>() // Needs this (is the overall transform representation) but doesn't have to be "directly" wrote to or read from
                                    );
    }
}