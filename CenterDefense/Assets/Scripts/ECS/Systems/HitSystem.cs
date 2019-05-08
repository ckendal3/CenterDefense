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
using Unity.Physics;

[UpdateInGroup(typeof(HitSystemGroup))]
[UpdateAfter(typeof(ShootSystem))]
public class HitSystem : JobComponentSystem
{
    private CollisionWorld collisionWorld;
    private PhysicsWorld physicsWorld;
    private EntityQuery m_DetectHitGroup;
    private EntityManager eManager;

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

    [BurstCompile]
    public struct CreateRayInputsJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<RaycastCommand> commands;
        [WriteOnly] public NativeArray<RaycastInput> inputs;

        public void Execute(int index)
        {
            inputs[index] = new RaycastInput
            {
                Ray = new Unity.Physics.Ray()
                {
                    Origin = commands[index].from,
                    Direction = commands[index].direction * commands[index].distance // appears direction is direction and distance combined
                },

                Filter = new CollisionFilter()
                {
                    CategoryBits = ~0u, // all 1s, so all layers, collide with everything 
                    MaskBits = ~0u,
                    GroupIndex = 0
                }
            };
        }
    }

    [BurstCompile]
    public struct CastRaysJob : IJobParallelFor
    {
        [ReadOnly] public Unity.Physics.CollisionWorld colWorld;
        [WriteOnly] public NativeArray<Unity.Physics.RaycastHit> hits;
        [ReadOnly] public NativeArray<RaycastInput> inputs;
        [WriteOnly] public NativeArray<bool> bools;

        public void Execute(int index)
        {
            Unity.Physics.RaycastHit hit;
            if(colWorld.CastRay(inputs[index], out hit))
            {
                bools[index] = true;
            }
            else
            {
                bools[index] = false;
            }

            hits[index] = hit;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        eManager = World.Active.EntityManager;

        // get the physics and collision world
        physicsWorld = World.Active.GetOrCreateSystem<Unity.Physics.Systems.BuildPhysicsWorld>().PhysicsWorld;
        collisionWorld = physicsWorld.CollisionWorld;

        int count = m_DetectHitGroup.CalculateLength();

        NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(count, Allocator.TempJob);
        var setupCommandsJob = new SetupCommandsJob
        {
            prevPositions = m_DetectHitGroup.ToComponentDataArray<PreviousTranslation>(Allocator.TempJob),
            curPositions = m_DetectHitGroup.ToComponentDataArray<Translation>(Allocator.TempJob),
            rotations =  m_DetectHitGroup.ToComponentDataArray<Rotation>(Allocator.TempJob),
            cmds = commands
        }.Schedule(count, 1, inputDeps);

        // ---------------------------
        // This can be removed when old physics is gone
        // -----------------------------
        setupCommandsJob.Complete();

        NativeArray<Entity> entities = m_DetectHitGroup.ToEntityArray(Allocator.TempJob, out JobHandle entitiesHandle);

        entitiesHandle.Complete();

        // Try using batch raycasting
        UnityEngine.RaycastHit hit;
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
                        hit.collider.GetComponent<Health>().TakeDamage(40);
                    }

                    EntityManager.SetComponentData(entities[i], new LifeTime { Value = 0});
                }
            }
        }

        NativeArray<RaycastInput> raycastInputs = new NativeArray<RaycastInput>(commands.Length, Allocator.TempJob);
        JobHandle raysInput = new CreateRayInputsJob
        {
            inputs = raycastInputs, 
            commands = commands
        }.Schedule(raycastInputs.Length, 32, setupCommandsJob);

        //***********************REFACTOR?***************************
        NativeArray<Unity.Physics.RaycastHit> raycastHits = new NativeArray<Unity.Physics.RaycastHit>(raycastInputs.Length, Allocator.TempJob);
        NativeArray<bool> hitBools = new NativeArray<bool>(raycastInputs.Length, Allocator.TempJob);
        JobHandle castRays = new CastRaysJob
        {
            colWorld = collisionWorld, 
            hits = raycastHits,
            inputs = raycastInputs,
            bools = hitBools
        }.Schedule(raycastInputs.Length, 32, raysInput);

        castRays.Complete();

        Entity hitEntity;
        for(int i = 0; i < raycastHits.Length; i++)
        {
            hitEntity = physicsWorld.Bodies[raycastHits[i].RigidBodyIndex].Entity;
            if (hitBools[i] == true)
            {
                // Set projectile data to zero
                eManager.DestroyEntity(entities[i]);

                // Set hit object to dead
                eManager.DestroyEntity(hitEntity);
                Debug.Log("ECS HIT");
            }
        }

        entities.Dispose();
        commands.Dispose();

        raycastInputs.Dispose();
        raycastHits.Dispose();
        hitBools.Dispose();

        return JobHandle.CombineDependencies(castRays, inputDeps);
    }

    protected override void OnCreate()
    {

        m_DetectHitGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<PreviousTranslation>(), ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Rotation>(), ComponentType.ReadWrite<ProjectileTag>() }
        });

    }
}

