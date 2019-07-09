//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Physics;
//using Unity.Physics.Systems;
//using UnityEngine;

//[UpdateAfter(typeof(StepPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
//public class TriggerSystem : JobComponentSystem
//{

//    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
//    StepPhysicsWorld m_StepPhysicsWorldSystem;
//    private EndFramePhysicsSystem m_EndFramePhysicsSystem;

//    private struct TriggerJob : IJobParallelFor
//    {
//        [ReadOnly] public PhysicsWorld World;
//        [ReadOnly] public TriggerEvents TriggerEvents;

//        [ReadOnly] public ComponentDataFromEntity<Attackable> attackables;

//        public unsafe void Execute(int index)
//        {
//            foreach (var triggerEvent in TriggerEvents)
//            {
//                RigidBody bodyA = World.Bodies[triggerEvent.BodyIndices.BodyAIndex];
//                RigidBody bodyB = World.Bodies[triggerEvent.BodyIndices.BodyBIndex];

//                if (attackables.Exists(bodyA.Entity))
//                {
//                    Debug.Log("Close to " + bodyB.Entity);
//                }
//                else if (attackables.Exists(bodyB.Entity))
//                {
//                    Debug.Log("Close to " + bodyA.Entity);
//                }
//            }
//        }
//    }


//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        inputDeps = JobHandle.CombineDependencies(inputDeps,
//            m_BuildPhysicsWorldSystem.FinalJobHandle,
//            m_StepPhysicsWorldSystem.FinalSimulationJobHandle);

//        var handle = new TriggerJob
//        {
//            World = m_BuildPhysicsWorldSystem.PhysicsWorld,
//            TriggerEvents = m_StepPhysicsWorldSystem.Simulation.TriggerEvents,
//            Players = GetComponentDataFromEntity<Player>()
//        }.Schedule(1, 1, inputDeps);
//        m_EndFramePhysicsSystem.HandlesToWaitFor.Add(handle);
//        return handle;
//    }

//    protected override void OnCreate()
//    {
//        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
//        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
//        m_EndFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
//    }
//}
