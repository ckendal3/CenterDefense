using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using WDIB.Utilities;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Stage Parameters", menuName = "WDIB/Parameters/StageParameters")]
    public class StageParameters : SingletonScriptableObject<StageParameters>
    {
        [Header("Default Stage")]
        [Tooltip("This is the default stage to start the game on.")]
        public StageObject defaultStage;

        [Header("Stage Data")]
        public StageObject[] stageSet;

        [Header("Overarching Difficulty Curve")]
        public AnimationCurve difficultyCurve;

        public StageData[] GetStagesDataArray()
        {
            StageData[] data = new StageData[stageSet.Length];

            for (int i = 0; i < stageSet.Length; i++)
            {
                data[i] = stageSet[i].data;
            }

            return data;
        }

        /// <summary>
        /// Get the projectile data by the specified ID.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public StageData GetStageDataByID(int ID)
        {
#if UNITY_EDITOR
            if (stageSet == null || stageSet.Length == 0 || ID >= stageSet.Length || stageSet[ID] == null) //projectileSet == null || 
            {
                Debug.LogWarning($"Stage ID {ID} does not exist. Returning default stage.");
                return defaultStage.data;
            }
#endif

            return stageSet[ID].data;
        }
    }
}