using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using WDIB.Parameters;

public static class Player
{
    private static NativeList<PlayerInfo> players = new NativeList<PlayerInfo>(0, Allocator.Persistent);

    /// <summary>
    /// Add a new defaulted player.
    /// </summary>
    public static void AddNewPlayer()
    {
        BarricadeData data = BarricadeParameters.Instance.barricadeSets[0];

        PlayerInfo newPlayer = new PlayerInfo {
                                ID = (ushort) players.Length, TurretID = 0,
                                BarricadeInfo = new BarricadeInfo {
                                                                        BarricadeHealth = data.maxHealth,
                                                                        BarricadeID = 0,
                                                                    }};

        players.Add(newPlayer);
    }

    /// <summary>
    /// Add a player with the predefined data
    /// </summary>
    /// <param name="info"></param>
    public static void AddPlayer(PlayerInfo info)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (info.ID == players[i].ID)
            {
                Debug.LogWarning("Player already exists at this ID - can't add player.");
                return;
            }
        }
        
    }

    public static PlayerInfo GetFirstPlayer()
    {
        if(players.Length > 0)
        {
            return players[0];
        }

        Debug.LogError("Player does not exist. Creating new player.");
        AddNewPlayer();

        // recursion
        return GetFirstPlayer();
    }

    /// <summary>
    /// Try to get a player's info by their index in the players array.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="playerInfo"></param>
    /// <returns></returns>
    public static bool TryGetPlayerAtIndex(int index, out PlayerInfo playerInfo)
    {
        if (index < players.Length)
        {
            playerInfo = players[index];
            return true;
        }
        else
        {
            playerInfo = default;
            return false;
        }
    }

    /// <summary>
    /// Try to get the player's info by their ID.
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="playerInfo"></param>
    /// <returns></returns>
    public static bool TryGetPlayerByID(int ID, out PlayerInfo playerInfo)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if(players[i].ID == ID)
            {
                playerInfo = players[i];
                return true;
            }
        }

        playerInfo = default;
        return false;
    }
}

public struct PlayerInfo
{
    public ushort ID;

    public ushort TurretID;
    public BarricadeInfo BarricadeInfo;
}

public struct BarricadeInfo
{
    public uint BarricadeID;
    public float BarricadeHealth;

    //// <ID, Health>
    //public KeyValuePair<ushort, float>[] BarricadeTurrets;

    //public KeyValuePair<ushort, float> GetTurretAtIndex(int index)
    //{
    //    // if index is less than length
    //    if (index < BarricadeTurrets.Length)
    //    {
    //        return BarricadeTurrets[index];
    //    }

    //    return BarricadeTurrets[0];
    //}
}


public struct TurretStats
{
    public uint kills;
    public uint shotsFired;
}