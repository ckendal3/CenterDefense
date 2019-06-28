using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using WDIB.Core.GameModes;
using WDIB.Core.States;
using WDIB.Parameters;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject TurretObject;

    public GameObject BarricadeObject;

    [SerializeField]
    public GameObject menuObject;

    [SerializeField]
    public GameObject buyMenu;

    [SerializeField]
    private GameObject spawnVolumeObject = null;
    private BoxCollider[] spawnVolumes;
    private GameObject spawnerObject;

    public GameStateV2 gameState;
    public StageModeV2 gameMode;

    private void Awake()
    {
        spawnVolumes = spawnVolumeObject.GetComponentsInChildren<BoxCollider>();

        GameEvents.onGameStatusChange += OnGameStatusChange;
    }

    private void Start()
    {
        gameMode = new StageModeV2("StageMode", StageParameters.Instance.GetStagesDataArray(), StageParameters.Instance.difficultyCurve);
        gameState = new GameStateV2();
    }

    void OnGameStatusChange(GameStateStatusV2 status)
    {
        switch (status)
        {
            case GameStateStatusV2.BetweenRounds:
                EnableBuyMenu(true);
                EnableMainMenu(false);
                break;
            case GameStateStatusV2.StartRound:
                EnableBuyMenu(false);
                EnableMainMenu(false);
                StartNewStage();
                break;
            case GameStateStatusV2.InProgress:
                EnableBuyMenu(false);
                EnableMainMenu(false);
                break;
            case GameStateStatusV2.GameOver:
                EnableBuyMenu(false);
                EnableMainMenu(true);
                break;
            default:
                break;
        }
    }

    private void EnableBuyMenu(bool value)
    {
        buyMenu.SetActive(value);
    }

    private void EnableMainMenu(bool value)
    {
        menuObject.SetActive(value);
    }

    private void StartNewStage()
    {
        gameMode.GoToNextStage();
        CreateSpawnerObject();
    }

    private void CreateSpawnerObject()
    {
        if(spawnerObject != null)
        {
            spawnerObject.GetComponent<Spawner_Mono>().Initialize(gameMode.GetDifficultyAdjustedStage(), spawnVolumes);
            return;
        }

        spawnerObject = new GameObject("Spawner");
        spawnerObject.AddComponent<Spawner_Mono>().Initialize(gameMode.GetDifficultyAdjustedStage(), spawnVolumes);
    }
}