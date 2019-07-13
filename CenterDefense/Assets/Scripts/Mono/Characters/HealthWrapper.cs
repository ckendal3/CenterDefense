using Unity.Mathematics;
using UnityEngine;
using WDIB.Interfaces;

public class HealthWrapper : MonoBehaviour
{
    public Health health;
}

public class Health : IDamageable
{
    public Health(float value)
    {
        MaxHealth = value;
        currentHealth = value;
    }

    protected readonly float MaxHealth;
    protected float currentHealth;

    /// <summary>
    /// Return if the player is Alive based on the current health
    /// </summary>
    public bool isAlive => currentHealth > 0;

    /// <summary>
    /// Apply damage by passing in a float value
    /// </summary>
    /// <param name="value"></param>
    /// <returns> Returns true if the entity is still alive </returns>
    public bool ApplyDamage(float value)
    {
        currentHealth -= value;

        // if dead
        // moving out of here
        //if (isAlive == false)
        //{
        //    GameEvents.onKillEvent?.Invoke(team);
        //}

        return isAlive;
    }

    /// <summary>
    /// Apply healing by passing in a float value.
    /// </summary>
    /// <param name="value"></param>
    public void ApplyHealing(float value)
    {
        math.clamp(currentHealth + value, 0, MaxHealth);
    }
}