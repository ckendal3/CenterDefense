using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

[UpdateInGroup(typeof(MovementSystemGroup))]
//[UpdateAfter(typeof(TargetSystem))]
public class MovementSystem : JobComponentSystem
{
    [BurstCompile]
    public struct PreviousPositionUpdateJob : IJobForEach<Translation, PreviousTranslation>
    {
        public void Execute([ReadOnly] ref Translation currentPos, [WriteOnly] ref PreviousTranslation previousPos)
        {
            previousPos.Value = currentPos.Value;
        }
    }

    [BurstCompile]
    [ExcludeComponent(typeof(LockAxisTranslation))]
    public struct MovementJob : IJobForEach<Translation, Rotation, Speed>
    {
        public float deltaTime;

        public void Execute(ref Translation position, [ReadOnly] ref Rotation rotation, [ReadOnly] ref Speed speed)
        {
            float3 newPos = position.Value + (deltaTime * speed) * math.forward(rotation.Value);

            position.Value = newPos;
        }
    }

    // fix this tomorrow
    // TODO: IMPLEMENT THIS
    public struct LockedMovementJob : IJobForEach<Translation, Rotation, Speed, LockAxisTranslation>
    {
        public float deltaTime;


        public void Execute(ref Translation position, [ReadOnly] ref Rotation rotation, [ReadOnly] ref Speed speed, ref LockAxisTranslation lockAxis)
        {
            float3 newPos = position.Value + (deltaTime * speed) * math.forward(rotation.Value);

            position.Value = newPos;
        }
    }

    // REFACTOR
    // Currently will only oscelate the up and down portion of the position
    // Need a better way to determine this
    // SineMovement bool3? much like LockAxis components
    [BurstCompile]
    [RequireComponentTag(typeof(SineMovementTag))]
    public struct SineMovementJob : IJobForEach<Translation, Rotation, SineMovementFloatiness>
    {
        public float deltaTime;
        public float time;

        public void Execute(ref Translation position, [ReadOnly] ref Rotation rotation, [ReadOnly] ref SineMovementFloatiness floatiness)
        {
            float3 newPos = position.Value;

            newPos.y += (math.sin(time) * deltaTime) * floatiness.Value;

            position.Value = newPos;
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle combined;

        var prevPositionJob = new PreviousPositionUpdateJob
        { }.Schedule(this, inputDeps);

        var movementJob = new MovementJob
        {
            deltaTime = UnityEngine.Time.deltaTime
        }.Schedule(this, prevPositionJob);

        var sinMovementJob = new SineMovementJob
        {
            deltaTime = Time.deltaTime,
            time = Time.time
        }.Schedule(this, movementJob);

        combined = JobHandle.CombineDependencies(prevPositionJob, movementJob, sinMovementJob);


        return JobHandle.CombineDependencies(combined, inputDeps);
    }
}
