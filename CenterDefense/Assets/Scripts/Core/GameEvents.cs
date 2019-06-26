using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Core.States;

public static class GameEvents
{
    public delegate void GameStatusChangeEvent(GameStateStatusV2 status);
    public static GameStatusChangeEvent onGameStatusChange;

    public delegate void ScoreIncreaseHandler(int amount);
    public static ScoreIncreaseHandler onScoreIncrease;
}
