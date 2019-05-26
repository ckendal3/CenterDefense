using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using WDIB.Components;

public class StageSystem : JobComponentSystem
{
    private EntityQuery m_EnemyGroup;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int enemyCount = m_EnemyGroup.CalculateLength();

        if(enemyCount <= 0)
        {
            GameManager.stageOverEvent?.Invoke(EStageStatus.over);
        }

        return inputDeps;
    }

    protected override void OnCreate()
    {
        m_EnemyGroup = GetEntityQuery(new EntityQueryDesc {
            All = new ComponentType[] { ComponentType.ReadOnly<EnemyTag>() }
        });
    }
}
