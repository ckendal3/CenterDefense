using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Parameters;

public class GameData : MonoBehaviour
{
    private int stageIndex = 0;

    [SerializeField]
    public StageData stageData;

    [SerializeField]
    public Object scriptableData;

    private void Awake()
    {
#if UNITY_EDITOR
        ApplyDefaultStage();
#else
        ApplyNewStageData();
#endif
    }

#if UNITY_EDITOR
    public void ApplyDefaultStage()
    {
        stageIndex += 1;
        stageData = StageParameters.Instance.defaultStage.data;
    }
#endif

    public void ApplyNewStageData(int index)
    {
        stageIndex += 1;
        stageData = StageParameters.Instance.stageSet[index].data;
    }
}