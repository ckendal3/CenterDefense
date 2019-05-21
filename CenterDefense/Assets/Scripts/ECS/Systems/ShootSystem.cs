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
    private EntityManager eManager;

    // Refactor
    private Mesh useMesh;
    private Material useMat;

    //**********************************************************
    //***********************REFACTOR***************************
    //**********************************************************
    // Should probably check ID so we don't spawn projectiles for
    // every entity that receives input/ is controlled by any player
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
        NativeArray<WeaponComponent> weapons = m_gunGroup.ToComponentDataArray<WeaponComponent>(Allocator.TempJob, out JobHandle weaponsHandle);

        // uh this is really ugly
        inputHandle.Complete();
        positionsHandle.Complete();
        rotationsHandle.Complete();
        tBetweenShotsHandle.Complete();
        weaponsHandle.Complete();

        // for every weapon that can shoot - make sure it does
        for(int i = 0; i < weapons.Length; i++)
        {
            // input isn't down or if not enough time between shots has passed - do nothing
            if (!inputs[i].PressedDown || tBetweenShots[i].timeLeft > 0)
            {
                continue;
            }

            // create projectile
            ProjectileFactory.CreateProjectiles(weapons[i].projectileId, positions[i].Value, rotations[i].Value);
        }

        inputs.Dispose();
        positions.Dispose();
        rotations.Dispose();
        tBetweenShots.Dispose();
        weapons.Dispose();

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
                                        ComponentType.ReadWrite<TimeBetweenShots>(), ComponentType.ReadOnly<WeaponComponent>()
                                      }, 

            // Can have these
            Any = new ComponentType[] { ComponentType.ReadOnly<ControlledByPlayer>() }
        });
    }
}