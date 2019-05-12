using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public interface IHealth
{
    bool isAlive { get; }

    void ApplyDamage(float damage);

    void ApplyHealing(float healthRestore);
}

public class Health : MonoBehaviour, IHealth
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
        Debug.Log("Taken " + damage + " damage");
        currentHealth -= damage;

        // if dead
        if (!isAlive)
        {
            Destroy(gameObject);
        }
    }

    public void ApplyHealing(float healthRestore)
    {
        Debug.Log("Given " + healthRestore + " health");
        currentHealth += healthRestore;

        math.clamp(currentHealth, 0, maxHealth);
    }
}