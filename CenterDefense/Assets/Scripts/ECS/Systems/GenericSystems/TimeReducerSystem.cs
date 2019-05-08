using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using WDIB.Components;

public class TimeReducerSystem : JobComponentSystem
{
    // This could be way more universal
    [BurstCompile]
    public struct ReduceTimersJob : IJobForEach<TimeBetweenShots>
    {
        public float deltaTime;

        public void Execute([ReadOnly] ref TimeBetweenShots tBS)
        {
            tBS.timeLeft -= deltaTime;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle reduceTimersHandle = new ReduceTimersJob
        {
            deltaTime = Time.deltaTime
        }.Schedule(this, inputDeps);

        return JobHandle.CombineDependencies(reduceTimersHandle, inputDeps);

    }
}
