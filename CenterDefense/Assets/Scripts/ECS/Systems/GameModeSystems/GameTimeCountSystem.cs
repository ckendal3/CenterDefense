using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class GameTimeCountSystem : JobComponentSystem
{


    [BurstCompile]
    public struct CountDownJob : IJobForEach<CountDownTime>
    {
        [ReadOnly] public float deltaTime;

        public void Execute(ref CountDownTime countDown)
        {
            countDown.Value = countDown.Value - deltaTime;
        }
    }

    [BurstCompile]
    public struct CountUpJob : IJobForEach<CountUpTime>
    {
        [ReadOnly] public float deltaTime;

        public void Execute(ref CountUpTime countUp)
        {
            countUp.Value = countUp.Value + deltaTime;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaT = Time.deltaTime;

        JobHandle countDownJob = new CountDownJob
        {
            deltaTime = deltaT
        }.Schedule(this, inputDeps);

        JobHandle countUpJob = new CountUpJob
        {
            deltaTime = deltaT
        }.Schedule(this, inputDeps);


        return JobHandle.CombineDependencies(inputDeps, countDownJob, countUpJob);
    }
}
