using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using WDIB.Components;

[DisableAutoCreation]
public class StageSystem : JobComponentSystem
{
    private EntityQuery m_EnemyGroup;
    private EntityQuery m_StageGroup;

    private EntityManager eManager;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int enemyCount = m_EnemyGroup.CalculateLength();
        int stageCount = m_StageGroup.CalculateLength();

        // Get our stage entities before continuing
        NativeArray<Entity> stageEntities = m_StageGroup.ToEntityArray(Allocator.TempJob, out JobHandle entitiesHandle);
        entitiesHandle.Complete();

        #if UNITY_EDITOR
        if(stageCount > 1)
        {
            Debug.LogWarning("There is more than one stage currently in the scene.");
        }
        #endif

        // we don't have a stage
        if(stageCount == 0)
        {
            // this will spawn the default stage if there are none
            WDIB.Factory.StageFactory.CreateStage(1000);
            stageEntities.Dispose();

            #if UNITY_EDITOR
            Debug.LogError("There was no stage in the scene - creating default stage");
            #endif

            return inputDeps;
        }

        Entity stageEntity = stageEntities[0];
        EnemyCount enemyCountComp = eManager.GetComponentData<EnemyCount>(stageEntity);

        // if we have killed and spawned all enemies
        if (enemyCount <= 0 && enemyCountComp.Value >= enemyCountComp.maxCount)
        {
            eManager.DestroyEntity(stageEntities);
            Debug.Log("Stage Over - ECS Stage System");
        }

        stageEntities.Dispose();
        return inputDeps;
    }

    protected override void OnCreate()
    {
        m_EnemyGroup = GetEntityQuery(new EntityQueryDesc {
            All = new ComponentType[] { ComponentType.ReadOnly<EnemyTag>() }
        });

        m_StageGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<EnemyCount>() }
        });

        eManager = World.Active.EntityManager;
    }
}
