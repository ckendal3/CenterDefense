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

        NativeArray<InputComponent> inputs = m_gunGroup.ToComponentDataArray<InputComponent>(Allocator.TempJob, out JobHandle inputHandle);
        NativeArray<Translation> positions = m_gunGroup.ToComponentDataArray<Translation>(Allocator.TempJob, out JobHandle positionsHandle);
        NativeArray<Rotation> rotations = m_gunGroup.ToComponentDataArray<Rotation>(Allocator.TempJob, out JobHandle rotationsHandle);
        NativeArray<TimeBetweenShots> tBetweenShots = m_gunGroup.ToComponentDataArray<TimeBetweenShots>(Allocator.TempJob, out JobHandle tBetweenShotsHandle);
        NativeArray<ProjectileSpawnPoint> projSpawnPoints = m_gunGroup.ToComponentDataArray<ProjectileSpawnPoint>(Allocator.TempJob, out JobHandle projSpawnPointsHandle);
        NativeArray<Entity> entities = m_gunGroup.ToEntityArray(Allocator.TempJob, out JobHandle entitiesHandle);

        // uh this is really ugly
        inputHandle.Complete();
        positionsHandle.Complete();
        rotationsHandle.Complete();
        tBetweenShotsHandle.Complete();
        projSpawnPointsHandle.Complete();
        entitiesHandle.Complete();

        // create all the projectiles 
        NativeArray <Entity> projectiles = new NativeArray<Entity>(count, Allocator.TempJob);
        eManager.CreateEntity(projectileArch, projectiles);        

        Entity tmp;
        for(int i = 0; i < count; i++)
        {
            tmp = projectiles[i];
            // if we aren't looking to shoot - don't do stuff
            // or
            // if not enough time between shots has passed - dont do stuff
            if (inputs[i].PressedDown == false || tBetweenShots[i].timeLeft > 0)
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

            eManager.SetComponentData(tmp, new WDIB.Components.Damage { Value = 50f }); // grab from projectile sriptable object. - We just need the projectile ID stored


#if UNITY_EDITOR
            eManager.SetName(tmp, "Projectile");
#endif

            //eManager.AddComponentData(tmp, new ControlledByPlayer { lookPosition = positions[i].Value });

            eManager.SetComponentData(entities[i], new TimeBetweenShots { Value = tBetweenShots[i].Value, timeLeft = tBetweenShots[i].Value }); //refactor this could be done in a job
            
        }

        inputs.Dispose();
        positions.Dispose();
        rotations.Dispose();
        projSpawnPoints.Dispose();
        tBetweenShots.Dispose();
        projectiles.Dispose();
        entities.Dispose();

        return inputDeps;
    }

    protected override void OnCreate()
    {
        eManager = World.EntityManager;

        m_gunGroup = GetEntityQuery(new EntityQueryDesc
        {
            // must have these components
            All = new ComponentType[] { ComponentType.ReadOnly<InputComponent>(),ComponentType.ReadOnly<Translation>(),
                                        ComponentType.ReadOnly<Rotation>(), ComponentType.ReadOnly<ShooterTag>(), ComponentType.ReadOnly<ProjectileSpawnPoint>(),
                                        ComponentType.ReadWrite<TimeBetweenShots>() }, 

            // Can have these
            Any = new ComponentType[] { ComponentType.ReadOnly<ControlledByPlayer>() }
        });

        // create our archetype
        projectileArch = eManager.CreateArchetype(
                                    ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(), ComponentType.ReadWrite<Scale>(),
                                    ComponentType.ReadWrite<Speed>(), ComponentType.ReadWrite<Owner>(), ComponentType.ReadWrite<PreviousTranslation>(),
                                    ComponentType.ReadWrite<LifeTime>(),
                                    ComponentType.ReadWrite<RenderMesh>(), ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadOnly<ProjectileTag>(),
                                    ComponentType.ReadWrite<WDIB.Components.Damage>()
                                    );
    }
}