using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Utilities;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Stage Parameters", menuName = "WDIB/Parameters/StageParameters")]
    public class StageParameters : SingletonScriptableObject<StageParameters>
    {
        [Header("Default Stage")]
        [Tooltip("This is the default stage to start the game on.")]
        public StageData defaultStage;

        [Header("Stage Data")]
        public StageData[] stageSet;

        [Header("Overarching Difficulty Curve")]
        public AnimationCurve difficultyCurve;
    }
}