using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

[UpdateInGroup(typeof(MovementSystemGroup))]
public class InteractablesSystem : JobComponentSystem
{
    private EntityQuery m_InputGroup;

    // Refactor Rotation systems to be universal-ish
    // Need component to lock out (prevent) certain axis from changing in a better way
    // Lock axis but multiply by 0 or 1?
    // LockRotationAxis : IComponentData { public float3 Value };  aka Value = (1,0,1) locks y-axis
    [BurstCompile]
    [ExcludeComponent(typeof(LockAxisRotation), typeof(LocalToParent))]
    public struct UpdateInteractableRotation : IJobForEach<Translation, Rotation, ControlledByPlayer>
    {
        public void Execute([ReadOnly] ref Translation position, [WriteOnly] ref Rotation rotation, [ReadOnly] ref ControlledByPlayer desiredTarget)
        {
            //float3 diff = new float3(desiredTarget.lookPosition.x, position.Value.y, desiredTarget.lookPosition.x) - position.Value;
            float3 diff = desiredTarget.lookPosition - position.Value;

            float3 dir = math.normalize(diff);

            rotation.Value = quaternion.LookRotation(dir, math.up());
        }
    }

    // this might need to be done in the rotation area?
    [BurstCompile]
    [ExcludeComponent(typeof(LocalToParent))]
    public struct UpdateInteractableLockRotation : IJobForEach<Translation, Rotation, ControlledByPlayer, LockAxisRotation>
    {
        public void Execute([ReadOnly] ref Translation position, [WriteOnly] ref Rotation rotation, [ReadOnly] ref ControlledByPlayer desiredTarget, [ReadOnly] ref LockAxisRotation lockedRot)
        {
            float3 diff = desiredTarget.lookPosition - position.Value;

            float3 dir = math.normalize(diff);

            // "Lock" x rotation axis
            if(lockedRot.Value.x == true)
            {
                dir.x = 0;
            }

            // "Lock" y rotation axis
            if (lockedRot.Value.y == true)
            {
                dir.y = 0;
            }

            // "Lock" z rotation axis
            if (lockedRot.Value.z == true)
            {
                dir.z = 0;
            }

            rotation.Value = quaternion.LookRotation(dir, math.up());
        }
    }

    [BurstCompile]
    public struct UpdateLocalInteractableLockRotation : IJobForEach<Translation, Rotation, ControlledByPlayer, LockAxisRotation, LocalToParent, LocalToWorld>
    {
        public void Execute([ReadOnly] ref Translation position, [WriteOnly] ref Rotation rotation, 
                            [ReadOnly] ref ControlledByPlayer desiredTarget, [ReadOnly] ref LockAxisRotation lockedRot, 
                            [ReadOnly] ref LocalToParent localToParent, [ReadOnly] ref LocalToWorld localToWorld)
        {

            float3 worldPos = new float3(localToParent.Value.c1.x, localToParent.Value.c1.y, localToParent.Value.c1.z);

            // look position difference with parent's(?) world position + localPosition?
            float3 diff = desiredTarget.lookPosition - position.Value;

            //diff = diff + position.Value;

            float3 dir = math.normalize(diff);

            ////"Lock" x rotation axis
            //if (lockedRot.Value.x == true)
            //{
            //    dir.x = .5f;
            //}

            ////"Lock" y rotation axis
            //if (lockedRot.Value.y == true)
            //{
            //    dir.y = .5f;
            //}

            ////"Lock" z rotation axis
            //if (lockedRot.Value.z == true)
            //{
            //    dir.z = quaternion.
            //}

            rotation.Value = quaternion.LookRotation(dir, math.up());
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle combined;
        var updateRotJob = new UpdateInteractableRotation
        { }.Schedule(this, inputDeps);

        var updateLockRotJob = new UpdateInteractableLockRotation
        { }.Schedule(this, updateRotJob);

        combined = JobHandle.CombineDependencies(inputDeps, updateRotJob, updateLockRotJob);

        var updateLocalRotJob = new UpdateLocalInteractableLockRotation
        { }.Schedule(this, updateLockRotJob);

        return JobHandle.CombineDependencies(combined, updateLocalRotJob);
    }

    protected override void OnCreate()
    {
        m_InputGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<InputComponent>() }
        });
    }
}
