using Unity.Mathematics;
using UnityEngine;
using WDIB.Interfaces;

// TODO: Add the onKillEvent in a higher class such as a controller class? (Would have to make it)
// if dead
// moving out of here
//if (isAlive == false)
//{
//    GameEvents.onKillEvent?.Invoke(team);
//}

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
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns>  </returns>

    /// <summary>
    /// Apply damage by passing in a float value
    /// </summary>
    /// <param name="value"> The damage amount to apply </param>
    /// <param name="newHealth"> The new health value of the entity</param>
    /// <returns>Returns true if the entity is still alive </returns>
    public bool ApplyDamage(float value)
    {
        currentHealth -= value;
        return isAlive;
    }

    /// <summary>
    /// Apply damage by passing in a float value
    /// </summary>
    /// <param name="value"> The damage amount to apply </param>
    /// <param name="newHealth"> The new health value of the entity</param>
    /// <returns>Returns true if the entity is still alive </returns>
    public bool ApplyDamageGetNewHealth(float value, out float newHealth)
    {
        currentHealth -= value;

        newHealth = currentHealth;
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