using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Parameters;

public class GameData : MonoBehaviour
{
    [SerializeField]
    public StageData stageData;

    [SerializeField]
    public Object scriptableData;

    private void Awake()
    {
        ApplyNewStageData();
    }

    public void ApplyNewStageData()
    {

    }
}