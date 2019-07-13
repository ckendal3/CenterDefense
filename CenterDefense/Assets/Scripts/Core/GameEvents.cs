using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Core.States;
using WDIB.Parameters;

public static class GameEvents
{
    public delegate void GameStatusChangeHandler(GameStateStatusV2 status);
    public static GameStatusChangeHandler onGameStatusChange;

    public delegate void ScoreIncreaseHandler(int amount);
    public static ScoreIncreaseHandler onScoreIncrease;

    public delegate void OnKillHandler(TeamOrientation team); //EnemyData data
    public static OnKillHandler onKillEvent;

    public delegate void OnHealthChangeHandler(float value, float maxValue);
    public static OnHealthChangeHandler onHealthEvent;
}