using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Parameters;

public class GameModeBase
{
    public string GameModeName { get; private set; } = "Game Mode";
    public float TimeLimit { get; private set; } = 60f;

    public virtual ushort MaximumAllowedPlayers { get; private set; } = 32;

    /// <summary>
    /// Initialize GameMode specific systems/data here
    /// </summary>
    public virtual void Initialize(string name)
    {
        SetGameModeName(name);
        Debug.Log($"Initializing {name} Mode");
    }

    public virtual void OnPlayerJoin()
    {
        // add to a team
        Debug.Log("Player has joined");
    }

    public virtual void OnPlayerLeave()
    {
        // remove to a team
        Debug.Log("Player has left");
    }

    public void SetGameModeName(string newName)
    {
        GameModeName = newName;
    }
}

public class StageMode : GameModeBase
{
    public StageMode(string name, StageData[] data, AnimationCurve curve)
    {
        SetGameModeName(name);
        stagesData = data;
        difficultyCurve = curve;
    }

    // Have a set of stages and an overall stage difficulty curve
    private uint index = 0;

    public AnimationCurve difficultyCurve;

    public StageData[] stagesData;

    public bool GoToNextStage()
    {
        // if we still have stages left
        if(index < stagesData.Length)
        {
            index += 1;
            return true;
        }
        else
        {
            return false;
        }
    }

    public StageData GetDifficultyAdjustedStageMultiplier()
    {
        // get the current difficulty on the curve
        float multiplier = difficultyCurve.Evaluate(index / stagesData.Length);

        StageData tempData = stagesData[index];

        AdjustStage(tempData);

        return tempData;

        void AdjustStage(StageData stage)
        {
            stage.multiplierData.enemyDifficultyMultiplier *= multiplier;
            stage.multiplierData.enemySpeedMultiplier *= multiplier;
            stage.multiplierData.stageRewardMultiplier *= multiplier;
        }
    }
}


public class GameStateBase
{
    public SortedList<uint, PlayerState> PlayerStates = new SortedList<uint, PlayerState>();

    private double GameDuration { get; set; } = 0f;
    private GameStateStatus GameStateStatus { get; set; } = GameStateStatus.LoadingIn;

    /// <summary>
    /// Get the current duration of the game.
    /// </summary>
    /// <returns>Double</returns>
    public double GetGameDuration()
    {
        return GameDuration;
    }
    /// <summary>
    /// Get the status of the game.
    /// </summary>
    /// <returns>Double</returns>
    public GameStateStatus GetGameStatus()
    {
        return GameStateStatus;
    }

    /// <summary>
    /// Add a new player and a new player state.
    /// </summary>
    public void OnPlayerJoin()
    {
        uint ID = (uint) PlayerStates.Count;
        for(uint i = 0; i < PlayerStates.Count; i++)
        {
            if(PlayerStates[i].ID != i)
            {
                ID = i;
            }
        }

        PlayerStates.Add(ID, new PlayerState(ID));
        Debug.Log($"Player {ID} has joined - adding new player state.");
    }
    /// <summary>
    /// Remove a player and its state from the list.
    /// </summary>
    /// <param name="ID"></param>
    public void OnPlayerLeave(uint ID)
    {
        for (uint i = 0; i < PlayerStates.Count; i++)
        {
            if (PlayerStates[i].ID == ID)
            {
                PlayerStates.Remove(i);
                Debug.Log($"Player {ID} has left - removing player state");
                return;
            }
        }

        Debug.LogWarning($"Player {ID} does not exist.");
    }
    /// <summary>
    /// Retrieve a player's state information using an ID
    /// </summary>
    /// <param name="ID"></param>
    /// <returns>PlayerState or Null.</returns>
    public PlayerState GetPlayerStateByID(uint ID)
    {
        for (uint i = 0; i < PlayerStates.Count; i++)
        {
            if (PlayerStates[i].ID == ID)
            {
                return PlayerStates[i];
            }
        }

        return null;
    }
}

public enum GameStateStatus
{
    LoadingIn,          // load phase
    WaitingToStart,     // iniatilize waypoints, make sure all players are done loading, etc
    InProgress,         // in-game
    WaitingToEnd,       // game is ending - save out data from game and fade out, etc
    LoadingOut,         // game over
    Aborted             // encountered an issue
}

public class PlayerState
{
    public PlayerState(uint newID)
    {
        ID = newID;
        score = new PlayerScore { Value = 0 };
    }

    public PlayerState(uint newID, int newScore)
    {
        ID = newID;
        score = new PlayerScore { Value = newScore };
    }

    public uint ID { get; private set; }
    private PlayerScore score { get; set; }

    public int GetScore()
    {
        return score.Value;
    }

    public uint GetPlayerID()
    {
        return ID;
    }
}

public struct PlayerScore
{
    // generic score that is wrote to by any game mode
    public int Value;
}