using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int stageIndex = 1;

    [SerializeField]
    public float delayBetweenStages = 1f;
    private float timeTillNextRound = 5f;

    public delegate void StageOver(EStageStatus status);
    public static StageOver stageOverEvent;

    private bool startStage = true;

    [SerializeField]
    public GameObject TurretObject;
    public GameObject BarricadeObject;

    [SerializeField]
    public GameObject menuObject;
    private bool menuDisplayed = false;

    private GameObject spawnerObject;
    [SerializeField]
    private GameObject spawnVolumeObject;
    private BoxCollider[] spawnVolumes;

    private void Awake()
    {
        spawnVolumes = spawnVolumeObject.GetComponentsInChildren<BoxCollider>();
    }

    private void Start()
    {
        stageOverEvent += StageStatusChange;

        StartNewStage();
    }

    private void Update()
    {
        if (BarricadeObject == null && TurretObject == null)
        {
            Debug.Log("Game over");

            if (menuDisplayed == false)
            {
                menuObject.SetActive(true);
                menuDisplayed = true;

//#if UNITY_EDITOR
//                World.Active.EntityManager.DestroyEntity(World.Active.EntityManager.GetAllEntities());
//#endif
            }
        }
        else if (timeTillNextRound <= 0 && startStage == true)
        {
            StartNewStage();
            startStage = false;
        }
    }

    private void StartNewStage()
    {
        SpawnObject();
        stageIndex += 1;

        timeTillNextRound = delayBetweenStages;

        stageOverEvent?.Invoke(EStageStatus.start);
    }

    private void SpawnObject()
    {
        if(spawnerObject != null)
        {
            return;
        }

        spawnerObject = new GameObject("Spawner");
        spawnerObject.AddComponent<Spawner_Mono>().Initialize(stageIndex, spawnVolumes);
    }

    private void StageStatusChange(EStageStatus status)
    {
        switch (status)
        {
            case EStageStatus.start:
                StartNewStage();
                startStage = true;
                break;
            case EStageStatus.inprogress:
                break;
            case EStageStatus.over:
                break;
        }
    }

}

public enum EStageStatus
{
    start,
    inprogress,
    over
}