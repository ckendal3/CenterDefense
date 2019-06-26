using UnityEngine;

namespace WDIB.Core.GameModes
{
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
}