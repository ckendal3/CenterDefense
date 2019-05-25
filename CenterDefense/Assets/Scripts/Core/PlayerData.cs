using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Parameters;

[System.Serializable]
public struct PlayerSpawnData
{
    [Tooltip("The ID of the weapon for the player to start with.")]
    public PlayerData playerData;
}

[System.Serializable]
public struct PlayerData
{
    [Tooltip("The ID of the weapon for the player to start with.")]
    public WeaponData weaponId;

    [Tooltip("The currency the player currently has.")]
    public Currency currency;
}

[System.Serializable]
public struct Currency
{
    [Range(0,100000)]
    public float Value;
}