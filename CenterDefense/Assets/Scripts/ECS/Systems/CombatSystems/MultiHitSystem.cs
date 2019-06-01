using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;
using WDIB.Parameters;

public class MultiHitSystem : JobComponentSystem
{
    private EntityManager eManager;
    private EntityQuery m_MultiHitGroup;
    private RaycastHit[] hits;

    [BurstCompile]
    public struct SetupCommandsJob : IJobParallelFor
    {
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<PreviousTranslation> prevPositions;

        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<Translation> positions;

        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<Rotation> rotations;

        [WriteOnly] public NativeArray<RaycastCommand> commands;

        //[ReadOnly] public LayerMask hitLayer;

        public void Execute(int index)
        {
            commands[index] = new RaycastCommand
            {
                from = prevPositions[index].Value,
                direction = math.forward(rotations[index].Value),
                distance = math.distance(positions[index].Value, prevPositions[index].Value)//,
                //layerMask = hitLayer
            };
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int commandCount = m_MultiHitGroup.CalculateLength();

        // Get the entities matching our query
        NativeArray<Entity> entities = m_MultiHitGroup.ToEntityArray(Allocator.TempJob, out JobHandle entArrayHandle);
        var multiHits = m_MultiHitGroup.ToComponentDataArray<MultiHit>(Allocator.TempJob, out JobHandle multiHandle);

        // Setup commands
        NativeArray<RaycastCommand> rayCommands = new NativeArray<RaycastCommand>(commandCount, Allocator.TempJob);
        JobHandle setupJob = new SetupCommandsJob
        {
            // these get the data from the m_HitGroup query
            prevPositions = m_MultiHitGroup.ToComponentDataArray<PreviousTranslation>(Allocator.TempJob),
            positions = m_MultiHitGroup.ToComponentDataArray<Translation>(Allocator.TempJob),
            rotations = m_MultiHitGroup.ToComponentDataArray<Rotation>(Allocator.TempJob),
            commands = rayCommands//,
            //hitLayer = WeaponParameters.Instance.projectileHitLayers

        }.Schedule(commandCount, 32, inputDeps);

        // We need the main thread to wait for all the entities matching our query to be gathered
        entArrayHandle.Complete();
        setupJob.Complete(); // need to make sure all of our commands are setup
        multiHandle.Complete();

        #region Default raycast
        Ray ray;
        uint attackerID;
        int hitCount;
        // for ever raycast
        for (int i = 0; i < commandCount; i++)
        {
            ray = new Ray { origin = rayCommands[i].from, direction = rayCommands[i].direction };

            // if there are hits
            hitCount = Physics.RaycastNonAlloc(ray, hits, rayCommands[i].distance); //, rayCommands[i].layerMask
            if (hitCount > 0)
            {
                // get id and explosion data if we are supposed to
                attackerID = eManager.GetComponentData<Owner>(entities[i]).ID;
                bool hasExplosive = eManager.HasComponent<Explosive>(entities[i]);

                // go through every hit
                for (int j = 0; j < hitCount; j++)
                {
                    // no hit - do nothing
                    if (hits[j].collider == null)
                    {
                        continue;
                    }

                    // check if its the right thing to do damage, spawn impact effects, etc  
                    // if there is explosion data
                    if (hasExplosive == true)
                    {
                        Explosive explosive = eManager.GetComponentData<Explosive>(entities[i]);
                        ExplosiveFactory.CreateExplosive(explosive.ID, hits[j].point, attackerID);
                    }

                    float damage = eManager.GetComponentData<Damage>(entities[i]).Value;

                    MultiHit mHit = eManager.GetComponentData<MultiHit>(entities[i]);
                    eManager.SetComponentData(entities[i], new MultiHit { hits = mHit.hits + 1, maxHits = mHit.maxHits });

                    Debug.Log("Hit");

                    if (mHit.hits >= mHit.maxHits)
                    {
                        eManager.SetComponentData(entities[i], new Distance { distanceTravelled = 100000f, maxAllowedDistance = 0 });
                        break;
                    }

                    #region  Damage
                    //// if we can do damage multihit is good - if not, destroy self because it is environment
                    //if (damageable != null)
                    //{
                    //    // Get the multi hit data and set it
                    //    MultiHit mHit = eManager.GetComponentData<MultiHit>(entities[i]);
                    //    if (mHit.hits >= mHit.maxHits)
                    //    {
                    //        break;
                    //    }

                    //    eManager.SetComponentData(entities[i], new MultiHit { hits = mHit.hits + 1, maxHits = mHit.maxHits });

                    //}
                    //// hit environment - destroy self and don't check more hits
                    //else
                    //{
                    //    // we hit something so set our distance to pretty much dead and don't check any more hits
                    //    eManager.SetComponentData(entities[i], new Distance { currentValue = 100000f, maxValue = 0 });
                    //    break;
                    //}
                    #endregion
                }
            }



        }
        #endregion

        entities.Dispose();
        rayCommands.Dispose();
        multiHits.Dispose();

        return setupJob;
    }

    protected override void OnCreate()
    {
        eManager = World.Active.EntityManager;

        m_MultiHitGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<PreviousTranslation>(), ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Rotation>(), ComponentType.ReadOnly<Damage>(), ComponentType.ReadOnly<Owner>(),
                ComponentType.ReadOnly<MultiHit>()
            }
        });

        hits = new RaycastHit[40];
    }
}
