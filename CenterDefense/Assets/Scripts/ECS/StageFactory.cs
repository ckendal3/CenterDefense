using WDIB.Parameters;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using WDIB.Components;
using UnityEngine;

namespace WDIB.Factory
{
    public static class StageFactory
    {
        static EntityManager eManager;
        static EntityArchetype stageArch;

        static StageParameters sParameters;
        static int stageCount = 0;

        private static void Initialize()
        {
            eManager = World.Active.EntityManager;

            // create our archetype
            stageArch = eManager.CreateArchetype(
                                        ComponentType.ReadWrite<EnemySpawnSettings>(), ComponentType.ReadWrite<EnemyCount>()
                                        );
            sParameters = StageParameters.Instance;
        }

        /// <summary>
        /// Create a stage based on the supplied stage ID
        /// </summary>
        /// <param name="stageID"></param>
        public static void CreateStage(int stageID)
        {
            // need to do this just on construction
            Initialize();

            SetComponents(stageID);
        }

        /// <summary>
        /// Set the stage component data
        /// </summary>
        /// <param name="stageID"></param>
        /// <param name="data"></param>
        private static void SetComponents(int stageID)
        {
            StageData data;

            if (sParameters.stageSet[stageID] == null)
            {
                data = sParameters.defaultStage.data;
                Debug.LogWarning("Stage was out of index - applying default stage");
            }
            else
            {
                data = sParameters.stageSet[stageID].data;
            }
             
            Entity entity = eManager.CreateEntity(stageArch);

#if UNITY_EDITOR
            //This is just for debugging
            stageCount += 1;
            eManager.SetName(entity, "Stage count: " + stageCount);
#endif

            //Generic Components
            eManager.SetComponentData(entity, new EnemyCount { Value = 0, maxCount = data.enemySpawnData.maxTotalCount });
            eManager.SetComponentData(entity, new EnemySpawnSettings { Value = stageID });

        }
    }
}