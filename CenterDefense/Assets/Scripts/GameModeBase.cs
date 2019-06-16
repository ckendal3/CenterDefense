using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeBase
{
    public virtual string GameModeName { get; private set; } = "Game Mode";
    public virtual float TimeLimit { get; private set; } = 60f;

    public virtual ushort MaximumAllowedPlayers { get; private set; } = 32;

    /// <summary>
    /// Initialize GameMode specific systems/data here
    /// </summary>
    public virtual void Initialize(string name)
    {
        GameModeName = name;
        Debug.Log($"Initializing {name} Mode");
    }

    public virtual void OnPlayerJoin()
    {
        Debug.Log("Player has joined");
    }

    public virtual void OnPlayerLeave()
    {
        Debug.Log("Player has left");
    }


}

public class GameStateBase
{
    public SortedList<uint, PlayerState> PlayerStates = new SortedList<uint, PlayerState>();

    private double GameDuration { get; set; } = 0f;

    private GameStateStatus GameStateStatus { get; set; } = GameStateStatus.LoadingIn;

    public double GetGameDuration()
    {
        return GameDuration;
    }

    public GameStateStatus GetGameStatus()
    {
        return GameStateStatus;
    }

    public virtual void OnPlayerJoin()
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

    public virtual void OnPlayerLeave(uint ID)
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
    }

    public uint ID;

    private PlayerScore score { get; set; } = new PlayerScore { Value = 0 };

    public int GetScore()
    {
        return score.Value;
    }
}

public struct PlayerScore
{
    // generic score that is wrote to by any game mode
    public int Value;
}