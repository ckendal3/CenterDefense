using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using WDIB.Components;

public class TimeBetweenSystem : JobComponentSystem
{
    [BurstCompile]
    public struct TBShots : IJobForEach<TimeBetweenShots>
    {
        [ReadOnly] public float deltaTime;

        public void Execute(ref TimeBetweenShots tbShot)
        {
            tbShot.timeLeft -= deltaTime;
        }
    }

    [BurstCompile]
    public struct TBShotBurst : IJobForEach<TimeBetweenBurstShots>
    {
        [ReadOnly] public float deltaTime;

        public void Execute(ref TimeBetweenBurstShots tbShot)
        {
            tbShot.timeLeft -= deltaTime;
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle regShotJob = new TBShots
        {
            deltaTime = Time.deltaTime
        }.Schedule(this, inputDeps);

        JobHandle burstShotJob = new TBShotBurst
        {
            deltaTime = Time.deltaTime
        }.Schedule(this, inputDeps);

        return JobHandle.CombineDependencies(regShotJob, burstShotJob);
    }
}
