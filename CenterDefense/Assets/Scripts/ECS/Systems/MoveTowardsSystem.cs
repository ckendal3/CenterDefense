using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;

[UpdateAfter(typeof(TargetSystem))]
public class MoveTowardsSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPWorld;
    private EntityQuery m_physEntitiesGroup;


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        buildPWorld.FinalJobHandle.Complete();

        NativeArray<Speed> speeds = m_physEntitiesGroup.ToComponentDataArray<Speed>(Allocator.TempJob, out JobHandle speedHandle);
        NativeArray<Rotation> rotations = m_physEntitiesGroup.ToComponentDataArray<Rotation>(Allocator.TempJob, out JobHandle rotationHandle);
        NativeArray<Entity> entities = m_physEntitiesGroup.ToEntityArray(Allocator.TempJob, out JobHandle entitiesHandle);

        //JobHandle applyForcesjob = new ApplyForcesForward
        //{
        //    deltaTime = Time.deltaTime//,
        //    //world = buildPWorld.PhysicsWorld
        //}.Schedule(this, inputDeps);

        speedHandle.Complete();
        rotationHandle.Complete();
        entitiesHandle.Complete();

        float3 linearImpulse;
        PhysicsVelocity pVelocity;
        // REFACTOR ------------------------------------------------------
        for(int i = 0; i < entities.Length; i++)
        {
            pVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entities[i]);

            linearImpulse = math.forward(rotations[i].Value) * speeds[i].Value * Time.deltaTime;

            Unity.Physics.Extensions.ComponentExtensions.ApplyLinearImpulse(entities[i], linearImpulse);

            pVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entities[i]);
            EntityManager.SetComponentData(entities[i], new PhysicsVelocity { Linear = pVelocity.Linear,
                                                                              Angular = new float3(0, pVelocity.Angular.y, 0) }) ;           

            Debug.Log("Applying impulse:" + linearImpulse);
        }

        speeds.Dispose();
        rotations.Dispose();
        entities.Dispose();

        //var removeAngularForces = new RemoveAngularXZForces
        //{ }.Schedule(this, inputDeps);


        return inputDeps;
    }

    protected override void OnCreate()
    {
        buildPWorld = World.Active.GetOrCreateSystem<BuildPhysicsWorld>();

        m_physEntitiesGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] {
                                        ComponentType.ReadOnly<PhysicsEntityTag>(), ComponentType.ReadWrite<PhysicsVelocity>(),
                                        ComponentType.ReadOnly<Speed>(), ComponentType.ReadOnly<Rotation>()
                                      }

        });

    }
}
