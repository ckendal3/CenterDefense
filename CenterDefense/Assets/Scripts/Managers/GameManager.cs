using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using WDIB.Core.GameModes;
using WDIB.Core.States;
using WDIB.Parameters;

public class GameManager : MonoBehaviour
{
    private bool startStage = true;

    [SerializeField]
    public float delayBetweenStages = 1f;
    private float timeTillNextRound = 5f;

    [SerializeField]
    public GameObject TurretObject;
    public GameObject BarricadeObject;

    [SerializeField]
    public GameObject menuObject;
    private bool menuDisplayed = false;

    [SerializeField]
    private GameObject spawnVolumeObject = null;
    private BoxCollider[] spawnVolumes;
    private GameObject spawnerObject;

    public GameStateBase gameState;
    public StageGameMode gameMode;

    private void Awake()
    {
        spawnVolumes = spawnVolumeObject.GetComponentsInChildren<BoxCollider>();
    }

    private void Start()
    {
        gameMode = new StageGameMode("StageMode", StageParameters.Instance.stageSet, StageParameters.Instance.difficultyCurve);
        gameState = new GameStateBase();

        StartNewStage();
    }

    private void Update()
    {
        timeTillNextRound -= Time.deltaTime;

        if (BarricadeObject == null && TurretObject == null)
        {
            Debug.Log("Game over");

            if (menuDisplayed == false)
            {
                menuObject.SetActive(true);
                menuDisplayed = true;
            }
        }
        else if (timeTillNextRound <= 0 && startStage == true)
        {
            StartNewStage();
            startStage = false;
        }
        else if(spawnerObject == null)
        {
            startStage = true;
        }
    }

    private void StartNewStage()
    {
        if(gameMode.GoToNextStage())
        {
            SpawnObject();
            startStage = false;
        }
    }

    private void SpawnObject()
    {
        if(spawnerObject != null)
        {
            return;
        }

        spawnerObject = new GameObject("Spawner");
        spawnerObject.AddComponent<Spawner_Mono>().Initialize(gameMode.GetDifficultyAdjustedStage(), spawnVolumes);
    }
}