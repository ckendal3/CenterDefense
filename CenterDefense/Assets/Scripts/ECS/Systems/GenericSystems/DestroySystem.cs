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
    public struct CollectExceededLifeTimeJob : IJobForEachWithEntity<LifeTime>
    {
        [WriteOnly] public NativeQueue<Entity>.Concurrent entityQueue;

        public void Execute(Entity entity, int index, [ReadOnly] ref LifeTime life)
        {
            if (life.Value <= 0)
            {
                entityQueue.Enqueue(entity);
            }
        }
    }

    [BurstCompile]
    public struct CollectExceededDistanceJob : IJobForEachWithEntity<Distance>
    {
        [WriteOnly] public NativeQueue<Entity>.Concurrent entityQueue;

        public void Execute(Entity entity, int index, [ReadOnly] ref Distance distance)
        {
            if (distance.distanceTravelled >= distance.maxAllowedDistance)
            {
                entityQueue.Enqueue(entity);
            }
        }
    }

    [BurstCompile]
    public struct CollectDeadHealthJob : IJobForEachWithEntity<WDIB.Components.Health>
    {
        [WriteOnly] public NativeQueue<Entity>.Concurrent entityQueue;

        public void Execute(Entity entity, int index, [ReadOnly] ref WDIB.Components.Health health)
        {
            if (health.Value <= 0)
            {
                entityQueue.Enqueue(entity);
            }
        }
    }

    [BurstCompile]
    public struct FillDestroyArrayJob : IJob
    {
        public NativeArray<Entity> entities;
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
        NativeQueue<Entity> destroyQueue;

        // ********************************************************
        // Destroy Life entities
        destroyQueue = new NativeQueue<Entity>(Allocator.TempJob);
        JobHandle collectLifeJob = new CollectExceededLifeTimeJob
        {
            entityQueue = destroyQueue.ToConcurrent()
        }.Schedule(this, inputDeps);

        collectLifeJob.Complete();
        DestroyCollectedEntities(ref destroyQueue, ref collectLifeJob);


        // ********************************************************
        // Destroy Distance entities
        destroyQueue = new NativeQueue<Entity>(Allocator.TempJob);
        JobHandle collectDistanceJob = new CollectExceededDistanceJob
        {
            entityQueue = destroyQueue.ToConcurrent()
        }.Schedule(this, inputDeps);

        collectDistanceJob.Complete();
        DestroyCollectedEntities(ref destroyQueue, ref collectDistanceJob);


        // ********************************************************
        // Destroy Health entities
        destroyQueue = new NativeQueue<Entity>(Allocator.TempJob);
        JobHandle collectHealthJob = new CollectDeadHealthJob
        {
            entityQueue = destroyQueue.ToConcurrent()
        }.Schedule(this, inputDeps);

        collectHealthJob.Complete();
        DestroyCollectedEntities(ref destroyQueue, ref collectHealthJob);


        return inputDeps;
    }

    public void DestroyCollectedEntities(ref NativeQueue<Entity> destroyQueue, ref JobHandle dependency)
    {
        // if there are entities to destroy
        if(destroyQueue.Count > 0)
        {
            // complete the previous job
            // We get a litter of entities from a queue and write them a native array
            NativeArray<Entity> entitiesToDestroy = new NativeArray<Entity>(destroyQueue.Count, Allocator.TempJob);
            var fillJob = new FillDestroyArrayJob
            {
                entities = entitiesToDestroy,
                entityQueue = destroyQueue
            }.Schedule(dependency);

            // make sure we finish
            fillJob.Complete();

            // Destroy all entities in the array
            EntityManager.DestroyEntity(entitiesToDestroy);

            entitiesToDestroy.Dispose();
        }

        destroyQueue.Dispose();
    }
}
