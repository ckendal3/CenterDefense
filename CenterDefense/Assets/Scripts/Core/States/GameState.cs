using System.Collections.Generic;
using UnityEngine;

namespace WDIB.Core.States
{
    public class GameStateBase
    {
        public GameStateBase()
        {
            PlayerStates = new SortedList<uint, PlayerState>();
        }

        public SortedList<uint, PlayerState> PlayerStates { get; private set; }

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
            uint ID = (uint)PlayerStates.Count;
            for (uint i = 0; i < PlayerStates.Count; i++)
            {
                if (PlayerStates[i].ID != i)
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

    public enum GameStateStatusV2
    {
        BetweenRounds,
        StartRound,
        InProgress, 
        GameOver
    }


    public class GameStateV2
    {
        public static KeyValuePair<uint, PlayerState> player;

        public GameStateV2()
        {
            GameEvents.onGameStatusChange += OnGameStatusChange;
            GameEvents.onScoreIncrease += IncreasePlayer1Score;

            player = new KeyValuePair<uint, PlayerState>(0, new PlayerState (0, 0));
        }

        private void OnGameStatusChange(GameStateStatusV2 gameState)
        {
            switch (gameState)
            {
                case GameStateStatusV2.BetweenRounds:
                    PauseGame();
                    break;
                case GameStateStatusV2.StartRound:
                    ResumeGame();
                    break;
                case GameStateStatusV2.InProgress:
                    ResumeGame();
                    break;
                case GameStateStatusV2.GameOver:
                    PauseGame();
                    break;
            }
        }

        public void IncreasePlayer1Score(int amount)
        {
            player.Value.IncreaseScore(amount);
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }
    }

}