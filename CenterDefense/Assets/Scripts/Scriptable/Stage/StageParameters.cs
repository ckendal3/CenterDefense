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

        public NativeArray<StageData> GetStagesDataArray()
        {
            NativeArray<StageData> data = new NativeArray<StageData>(stageSet.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < stageSet.Length; i++)
            {
                data[i] = stageSet[i].data;
            }

            return data;
        }
    }
}