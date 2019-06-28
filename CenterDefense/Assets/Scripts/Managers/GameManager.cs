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
    private bool menuDisplayed = false;

    [SerializeField]
    public GameObject buyMenu;
    private bool buyMenuDisplayed = false;

    [SerializeField]
    private GameObject spawnVolumeObject = null;
    private BoxCollider[] spawnVolumes;
    private GameObject spawnerObject;

    public GameStateBase gameState;
    public StageModeV2 gameMode;

    private void Awake()
    {
        spawnVolumes = spawnVolumeObject.GetComponentsInChildren<BoxCollider>();

        GameEvents.onGameStatusChange += OnGameStatusChange;
    }

    private void Start()
    {
        gameMode = new StageModeV2("StageMode", StageParameters.Instance.GetStagesDataArray(), StageParameters.Instance.difficultyCurve);
        gameState = new GameStateBase();
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
                EnableMainMenu(true);
                break;
            default:
                break;
        }
    }

    private void EnableBuyMenu(bool value)
    {
        buyMenu.SetActive(value);
        buyMenuDisplayed = value;
    }

    private void EnableMainMenu(bool value)
    {
        menuObject.SetActive(value);
        menuDisplayed = value;
    }

    private void StartNewStage()
    {
        gameMode.GoToNextStage();
        SpawnObject();
    }

    private void SpawnObject()
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