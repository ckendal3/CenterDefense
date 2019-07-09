using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface IDamageable
{
    bool isAlive { get; }

    void ApplyDamage(float damage);

    void ApplyHealing(float healthRestore);
}

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField]
    public float MaximumHealth = 100;

    public float MaxHealth
    {
        set
        {
            maxHealth = value;
            currentHealth = value;
        }

        get
        {
            return maxHealth;
        }
    }
    private float maxHealth;

    //[HideInInspector]
    public float currentHealth = 100;

    /// <summary>
    /// Return if the player is Alive based on the current health
    /// </summary>
    public bool isAlive { get { return (currentHealth > 0); } }

    private void Awake()
    {
        MaxHealth = MaximumHealth;
    }

    public void ApplyDamage(float damage)
    {
        if(isAlive == true)
        {
            currentHealth -= damage;

            // if dead
            if (isAlive == false)
            {
                GameEvents.onKillEvent?.Invoke();
                Destroy(gameObject);
            }
        }
    }

    public void ApplyHealing(float healthRestore)
    {
        currentHealth += healthRestore;

        math.clamp(currentHealth, 0, maxHealth);
    }
}