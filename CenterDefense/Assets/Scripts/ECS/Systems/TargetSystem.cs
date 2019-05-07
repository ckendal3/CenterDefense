using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

// There should not be a need for a TargetPositionTag job because 
// the position should always be the same and doesn't need updating
// however, the targeting system may need a bool or something to
// determine if the entity should switch targets or not

// Could also implement TargetID system - component with ushort ID 
// or Entity component and just get its position
// This needs to be more universal so that locked axis work just fine <- this should be done else where not in here

//[UpdateAfter(typeof(InputSystem))]
public class TargetSystem : JobComponentSystem
{
    private EntityQuery m_targetEntityGroup;
    private NativeArray<Translation> targetPositions;
    
    [BurstCompile]
    [RequireComponentTag(typeof(TargetDynamicPositionTag))]
    ///Updates the positions of the entities targeting an entity
    public struct SetupTargetJob : IJobForEach<Translation, TargetPosition>
    {
        // the entities we want to look at
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<Translation> targetsPos;

        public void Execute([ReadOnly] ref Translation position, ref TargetPosition lookPos)
        {
            float loopDist = 0;
            float closestDist = math.distance(lookPos.Value, position.Value);

            float3 newLookPos = targetsPos[0].Value;

            //find the closest position
            for (int i = 0; i < targetsPos.Length; i++)
            {
                loopDist = math.distance(targetsPos[i].Value, position.Value);

                // if new distance is less than the current closest - assign it
                if (loopDist < closestDist)
                {
                    closestDist = loopDist;

                    newLookPos = targetsPos[i].Value;
                }
            }

            lookPos.Value = newLookPos;
        }
    }


    /// <summary>
    /// This handles making objects look at their target's position - moving or not
    /// </summary>
    [BurstCompile]
    public struct TargetPositionJob : IJobForEach<Translation, Rotation, TargetPosition>
    {
        // We constantly snap our objects to look at whatever its target position is
        public void Execute([ReadOnly]ref Translation position, [WriteOnly] ref Rotation rotation, [ReadOnly] ref TargetPosition targetPosition)
        {
            float3 diff = targetPosition.Value - position.Value;
            float3 dir = math.normalize(diff);

            rotation.Value = quaternion.LookRotation(dir, math.up());
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int targetCount = m_targetEntityGroup.CalculateLength();

        targetPositions = m_targetEntityGroup.ToComponentDataArray<Translation>(Allocator.TempJob);

        var setupPosJob = new SetupTargetJob
        {
            targetsPos = targetPositions
        }.Schedule(this, inputDeps);

        setupPosJob.Complete();

        var lookPosJob = new TargetPositionJob
        { }.Schedule(this, setupPosJob);

        return JobHandle.CombineDependencies(lookPosJob, setupPosJob, inputDeps);
    }

    protected override void OnCreate()
    {
        // Get the entities we need to look at
        m_targetEntityGroup = GetEntityQuery(new EntityQueryDesc
        {
            // must have these components
            All = new ComponentType[] { ComponentType.ReadWrite<Translation>(), typeof(DynamicTargetTag) },

            // can't have these components
            None = new ComponentType[] { typeof(TargetStaticPositionTag) }
       });
    }
}
