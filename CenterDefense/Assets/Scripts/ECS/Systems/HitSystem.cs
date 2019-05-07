using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

[UpdateInGroup(typeof(HitSystemGroup))]
[UpdateAfter(typeof(ShootSystem))]
public class HitSystem : JobComponentSystem
{
    private EntityQuery m_DetectHitGroup;

    //**********************************************************
    //***********************REFACTOR***************************
    //**********************************************************
    // Should be able to process all things like entities with damage, instant death, etc
    // Maybe use a component the tracks the type of hit?
    // EHitType damage, instantkill, trigger, etc
    public struct HitSetupJob : IJobForEachWithEntity<PreviousTranslation, Translation, Rotation>
    {
        [WriteOnly] public NativeArray<RaycastCommand> cmds;

        public void Execute(Entity entity, int index, [ReadOnly] ref PreviousTranslation prevPosition, [ReadOnly] ref Translation position, [ReadOnly] ref Rotation rotation)
        {
            cmds[index] = new RaycastCommand
            {
                from = prevPosition.Value,
                direction = math.forward(rotation.Value),
                distance = math.distance(prevPosition.Value, position.Value)
            };
        }
    }

    [BurstCompile]
    public struct SetupCommandsJob : IJobParallelFor
    {
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<PreviousTranslation> prevPositions;
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<Translation> curPositions;
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<Rotation> rotations;

        [WriteOnly] public NativeArray<RaycastCommand> cmds;

        public void Execute(int index)
        {
            cmds[index] = new RaycastCommand
            {
                from = prevPositions[index].Value,
                direction = math.forward(rotations[index].Value),
                distance = math.distance(prevPositions[index].Value, curPositions[index].Value)
            };
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int count = m_DetectHitGroup.CalculateLength();

        NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(count, Allocator.TempJob);
        var setupCommandsJob = new SetupCommandsJob
        {
            prevPositions = m_DetectHitGroup.ToComponentDataArray<PreviousTranslation>(Allocator.TempJob),
            curPositions = m_DetectHitGroup.ToComponentDataArray<Translation>(Allocator.TempJob),
            rotations = m_DetectHitGroup.ToComponentDataArray<Rotation>(Allocator.TempJob),
            cmds = commands
        }.Schedule(count, 1, inputDeps);

        setupCommandsJob.Complete();

        NativeArray<Entity> entities = m_DetectHitGroup.ToEntityArray(Allocator.TempJob, out JobHandle jobHandle);

        jobHandle.Complete();


        // Try using batch raycasting
        RaycastHit hit;
        for (int i = 0; i < commands.Length; i++)
        {
            if (Physics.Raycast(commands[i].from, commands[i].direction, out hit, commands[i].distance))
            {
                if (hit.collider)
                {
                    // Hit Something so destroy us
                    // Damage whatever it is
                    if(hit.collider.GetComponent<Health>())
                    {
                        hit.collider.GetComponent<Health>().TakeDamage(20);
                    }

                    EntityManager.SetComponentData(entities[i], new LifeTime { Value = 0});
                }
            }
        }

        entities.Dispose();
        commands.Dispose();
        return JobHandle.CombineDependencies(inputDeps, setupCommandsJob, jobHandle);
    }

    protected override void OnCreate()
    {
        m_DetectHitGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PreviousTranslation>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Rotation>() }

        });

    }
}

