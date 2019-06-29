using Unity.Collections;
using UnityEngine;
using WDIB.Parameters;

namespace WDIB.Core.GameModes
{
    public class StageGameMode : GameModeBase
    {

        public StageGameMode(string name, StageObject[] dataObjects, AnimationCurve curve)
        {
            SetGameModeName(name);
            stagesData = dataObjects;
            difficultyCurve = curve;
        }

        // Have a set of stages and an overall stage difficulty curve
        public uint index { get; private set; } = 0;

        public AnimationCurve difficultyCurve;

        public StageObject[] stagesData;

        public bool GoToNextStage()
        {
            // if we still have stages left
            if (index < stagesData.Length)
            {
                index += 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public StageData GetDifficultyAdjustedStage()
        {
            // get the current difficulty on the curve
            float multiplier = difficultyCurve.Evaluate(index / stagesData.Length);

            StageData tempData = stagesData[index].data;

            AdjustStage(tempData);

            return tempData;

            // in-line method
            void AdjustStage(StageData stage)
            {
                // this is resetting the information on the scriptable object
                stage.multiplierData.enemyDifficultyMultiplier *= multiplier;
                stage.multiplierData.enemySpeedMultiplier *= multiplier;
                stage.multiplierData.stageRewardMultiplier *= multiplier;
            }
        }
    }
}

public class StageModeV2
{
    public string GameModeName { get; private set; } = "Game Mode";

    public StageMultiplierData multiplierData = default;

    public StageModeV2(string name, StageData[] inData, AnimationCurve curve)
    {
        GameModeName = name;
        stagesData = inData;
        difficultyCurve = curve;
    }

    // Have a set of stages and an overall stage difficulty curve
    public uint index { get; private set; } = 0;
    public uint count = 0; // used for diffuclty curve

    public AnimationCurve difficultyCurve;

    public StageData[] stagesData;

    public bool GoToNextStage()
    {
        count += 1;

        // if we still have stages left
        if (index < stagesData.Length - 1)
        {
            index += 1;
            return true;
        }
        else // start over
        {
            index = 0;
            return false;
        }
    }

    // Get the multiplier data adjusted based on curve
    public StageData GetDifficultyAdjustedStage()
    {
        uint additional = 0;
        if (count != 0 && index != 0)
        {
            additional = count % index;
        }

        // get the current difficulty on the curve
        float multiplier = difficultyCurve.Evaluate(index / stagesData.Length) + additional;

        StageData tempData = stagesData[index];

        AdjustStage(ref tempData);

        return tempData;

        // in-line method
        void AdjustStage(ref StageData stage)
        {
            // this is resetting the information on the scriptable object
            multiplierData.enemyDifficultyMultiplier = stage.multiplierData.enemyDifficultyMultiplier * multiplier;
            multiplierData.enemySpeedMultiplier = stage.multiplierData.enemySpeedMultiplier * multiplier;
            multiplierData.stageRewardMultiplier = stage.multiplierData.stageRewardMultiplier * multiplier;
        }
    }
}
