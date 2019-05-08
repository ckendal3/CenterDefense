using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using WDIB.Components;

public class HealthSystem : JobComponentSystem
{
    [BurstCompile]
    // Damage is done whereever we hit something - this just checks if we should die
    public struct HealthJob : IJobForEach<WDIB.Components.Health, LifeTime>
    {
        public void Execute([ReadOnly] ref WDIB.Components.Health health, [WriteOnly] ref LifeTime lifeTime)
        {
            if(health.Value <= 0)
            {
                lifeTime.Value = 0;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle healthJob = new HealthJob
        { }.Schedule(this, inputDeps);

        return healthJob;
    }
}
