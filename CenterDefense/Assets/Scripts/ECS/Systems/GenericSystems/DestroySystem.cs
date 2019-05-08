using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using WDIB.Components;

[UpdateInGroup(typeof(LifeSystemGroup))]
[UpdateAfter(typeof(LifeTimeSystem))]
public class DestroySystem : JobComponentSystem
{
    [BurstCompile]
    public struct DestroyCollectorJob : IJobForEachWithEntity<LifeTime>
    {
        [WriteOnly] public NativeQueue<Entity>.Concurrent entQueue;

        public void Execute(Entity entity, int index, [ReadOnly] ref LifeTime life)
        {
            if (life <= 0)
            {
                entQueue.Enqueue(entity);
            }
        }
    }

    [BurstCompile]
    public struct FillDestroyArrayJob : IJob
    {
        [WriteOnly] public NativeArray<Entity> entities;
        public NativeQueue<Entity> entityQueue;

        public void Execute()
        {
            for(int i = 0; i < entities.Length; i++)
            {
                entities[i] = entityQueue.Dequeue();
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        // Get the things we need to destroy
        NativeQueue<Entity> destroyQueue = new NativeQueue<Entity>(Allocator.TempJob);
        var destroyJob = new DestroyCollectorJob
        {
            entQueue = destroyQueue.ToConcurrent()
        }.Schedule(this, inputDeps);

        // Make sure we completed our destroy collector
        destroyJob.Complete();

        NativeArray<Entity> entitiesToDestroy = new NativeArray<Entity>(destroyQueue.Count, Allocator.TempJob);
        var fillJob = new FillDestroyArrayJob
        {
            entities = entitiesToDestroy,
            entityQueue = destroyQueue
        }.Schedule(destroyJob);

        // Make sure our array fill job is done
        fillJob.Complete();

        // Destroy all entities in the array
        if(entitiesToDestroy.Length > 0)
        {
            EntityManager.DestroyEntity(entitiesToDestroy);
        } 

        // Dispose of the NativeContainers
        destroyQueue.Dispose();
        entitiesToDestroy.Dispose();

        return fillJob;
    }
}
