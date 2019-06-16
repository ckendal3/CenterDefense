using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeBase
{
    public string GameModeName { get; private set; } = "Game Mode";

    public ushort MaximumAllowedPlayers { get; private set; } = 32;

    /// <summary>
    /// Initialize GameMode specific systems/data here
    /// </summary>
    public virtual void Initialize()
    {
        Debug.Log("Initializing Game Mode");
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

public class TeamModeBase : GameModeBase
{
    public override void OnPlayerJoin()
    {
        base.OnPlayerJoin();
    }

    public override void OnPlayerLeave()
    {
        base.OnPlayerLeave();

        Debug.Log("Team mode base");
    }

    public void AddToTeam()
    {

    }

}


public class GameStateBase
{
    private double GameDuration { get; set; } = 60f;

    private GameStateStatus GameStateStatus { get; set; } = GameStateStatus.LoadingIn;

    public double GetGameDuration()
    {
        return GameDuration;
    }

    public GameStateStatus GetGameStatus()
    {
        return GameStateStatus;
    }
}

public class GameStateTeamBase : GameStateBase
{
    // <teamID, teamScore>
    private KeyValuePair<ushort, int>[] teamData;

    public int GetTeamScoreByID(ushort ID)
    {
        return teamData[ID].Value;
    }
}

public enum GameStateStatus
{
    LoadingIn, // load phase
    WaitingToStart, // iniatilize waypoints, make sure all players are done loading, etc
    InProgress, // in-game
    WaitingToEnd, // game is ending - save out data from game and fade out, etc
    LoadingOut, // game over
    Aborted // encountered an issue
}

public class PlayerState
{
    private int score { get; set; } = 0;

    private PlayerMatchStatistics stats;

    public int GetScore()
    {
        return score;
    }
}

public class Team
{
    private ushort TeamID { get; set; } = 0;
}

public class ForcedTeam : Team
{
    public Color color { get; set; } = Color.green;
}


public struct PlayerMatchStatistics
{
    public uint kills;
    public uint teamKills;

    public uint assists;

    public uint deaths;

    public uint damageDealt;
    public uint damageTaken;

    public float timeAlive;

    public float GetKillDeathRatio()
    {
        if(deaths == 0)
        {
            return (kills - teamKills);
        }

        return (kills - teamKills) / deaths;
    }

    public float GetKillDeathAssistRatio()
    {
        if(deaths == 0)
        {
            return kills + assists;
        }

        return (kills + assists) / deaths;

    }

    public float GetAverageTimeAlive()
    {
        if(deaths == 0)
        {
            return timeAlive;
        }

        return timeAlive / deaths;
    }

    public float GetNetDamage()
    {
        return damageDealt - damageTaken;
    }
}

public struct RivalPlayer { }

public struct WeaponsUsed
{
    public KeyValuePair<ushort, uint>[] weaponCollection;

    public uint GetWeaponKillsByID(ushort i)
    {
        // try to get the count of this medal earned
        if (i < weaponCollection.Length)
        {
            return weaponCollection[i].Value;
        }

        return 0;
    }

    public KeyValuePair<ushort, uint>[] GetAllWeaponsUsed()
    {
        var weaponList = new List<KeyValuePair<ushort, uint>>();

        KeyValuePair<ushort, uint> tmp;
        for (int i = 0; i < weaponCollection.Length; i++)
        {
            tmp = weaponCollection[i];
            if (tmp.Value > 0)
            {
                weaponList.Add(tmp);
            }
        }

        return weaponList.ToArray();
    }

    public KeyValuePair<ushort, uint> GetMostUsedWeapon()
    {
        KeyValuePair<ushort, uint> mostUsed = weaponCollection[0];

        // go through all weapons
        for (int i = 0; i < weaponCollection.Length; i++)
        {
            // if the current most used is less than the next checked - use the next checked value
            if (mostUsed.Value < weaponCollection[0].Value)
            {
                mostUsed = weaponCollection[i];
            }
        }

        return mostUsed;
    }
}

public struct MedalsEarned
{
    public KeyValuePair<ushort, uint>[] medalCollection;

    public uint GetEarnedMedalCountByID(ushort i)
    {
        // try to get the count of this medal earned
        if(i < medalCollection.Length)
        {
            return medalCollection[i].Value;
        }

        return 0;
    }

    public KeyValuePair<ushort, uint>[] GetAllEarnedMedals()
    {
        var medalList = new List<KeyValuePair<ushort, uint>>();

        KeyValuePair<ushort, uint> tmp;
        for(int i = 0; i < medalCollection.Length; i++)
        {
            tmp = medalCollection[i];
            if(tmp.Value > 0)
            {
                medalList.Add(tmp);
            }
        }

        return medalList.ToArray();
    }

    public KeyValuePair<ushort, uint> GetMostEarnedMedal()
    {
        KeyValuePair<ushort, uint> mostEarned = medalCollection[0];

        // go through all weapons
        for (int i = 0; i < medalCollection.Length; i++)
        {
            // if the current most used is less than the next checked - use the next checked value
            if (mostEarned.Value < medalCollection[0].Value)
            {
                mostEarned = medalCollection[i];
            }
        }

        return mostEarned;
    }
}

public class MedalData : ScriptableObject
{
    public string medalName = "Medal";

    public Sprite image;
    public AudioClip announcerAudio; // should be audiodata in i01
}

public struct FlagStatistics
{
    public ushort FlagsCapture;
    public ushort FlagsDefended;
}

public struct PlayerScore
{
    // generic score that is wrote to by any game mode
    public int score;
}

