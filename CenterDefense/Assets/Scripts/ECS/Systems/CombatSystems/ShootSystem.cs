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
using WDIB.Parameters;

// update after the turrets turn
//[UpdateInGroup(typeof(InputSystemGroup))]
//[UpdateAfter(typeof(InputSystem))]
[UpdateInGroup(typeof(HitSystemGroup))]
public class ShootSystem : JobComponentSystem
{
    private EntityQuery m_gunGroup;
    private EntityManager eManager;

    //**********************************************************
    //***********************REFACTOR***************************
    //**********************************************************
    // Should probably check ID so we don't spawn projectiles for
    // every entity that receives input/ is controlled by any player
    // ------------
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int count = m_gunGroup.CalculateLength();

        NativeArray<Translation> positions = m_gunGroup.ToComponentDataArray<Translation>(Allocator.TempJob, out JobHandle positionsHandle);
        NativeArray<Rotation> rotations = m_gunGroup.ToComponentDataArray<Rotation>(Allocator.TempJob, out JobHandle rotationsHandle);
        NativeArray<TimeBetweenShots> tBetweenShots = m_gunGroup.ToComponentDataArray<TimeBetweenShots>(Allocator.TempJob, out JobHandle tBetweenShotsHandle);
        NativeArray<WeaponComponent> weaponComponents = m_gunGroup.ToComponentDataArray<WeaponComponent>(Allocator.TempJob, out JobHandle weaponsHandle);
        NativeArray<Entity> entities = m_gunGroup.ToEntityArray(Allocator.TempJob, out JobHandle entitiesHandle);

        // uh this is really ugly
        positionsHandle.Complete();
        rotationsHandle.Complete();
        tBetweenShotsHandle.Complete();
        weaponsHandle.Complete();
        entitiesHandle.Complete();

        int projectileId;
        float timeResetValue;
        // for every weapon that can shoot - make sure it does
        for(int i = 0; i < weaponComponents.Length; i++)
        {
#if UNITY_EDITOR
            if(WeaponParameters.Instance == null)
            {
                Debug.Log("Weapon Parameters is null");
                continue;
            }
#endif
            projectileId = WeaponParameters.Instance.weaponSet[weaponComponents[i].weaponID].projectileId;
            timeResetValue = tBetweenShots[i].resetValue;

            // input isn't down or if not enough time between shots has passed - do nothing
            if (!weaponComponents[i].input.bShoot || tBetweenShots[i].timeLeft > 0)
            {
                continue;
            }

            // create projectile
            WDIB.Factory.ProjectileFactory.CreateProjectiles(projectileId, positions[i].Value, rotations[i].Value);
            // Reset time between shots on entity
            eManager.SetComponentData(entities[i], new TimeBetweenShots { resetValue = timeResetValue, timeLeft = timeResetValue });
        }

        positions.Dispose();
        rotations.Dispose();
        tBetweenShots.Dispose();
        weaponComponents.Dispose();
        entities.Dispose();

        return inputDeps;
    }

    protected override void OnCreate()
    {
        eManager = World.EntityManager;

        m_gunGroup = GetEntityQuery(new EntityQueryDesc
        {
            // must have these components
            All = new ComponentType[] { ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Rotation>(),
                                         ComponentType.ReadOnly<ProjectileSpawnPoint>(), ComponentType.ReadWrite<TimeBetweenShots>(),
                                         ComponentType.ReadOnly<WeaponComponent>()
                                      }, 

            // Can have these
            Any = new ComponentType[] { ComponentType.ReadOnly<ControlledByPlayer>() }
        });
    }
}