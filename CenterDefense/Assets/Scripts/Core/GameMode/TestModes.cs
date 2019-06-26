using System.Collections.Generic;
using UnityEngine;

public class GameModeBase
{

    public virtual void StartGame()
    {
        Debug.Log("Starting Game - GameModeBase");
    }

    public virtual void EndGame()
    {
        Debug.Log("Ending Game - GameModeBase");
    }

    public virtual void IncreaseScore()
    {
        Debug.Log("Increasing Score - GameModeBase");
    }
}

public class GameState
{
    public List<PlayerData> players;


    public GameModeBase gameMode;

    public void Initialize(GameModeBase mode)
    {
        gameMode = mode;
    }

}

public class StageMode : GameModeBase
{

    public override void StartGame()
    {
        base.StartGame();
    }
}
