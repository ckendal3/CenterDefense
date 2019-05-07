using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{  
    public float health;

    [SerializeField]
    public float maxHealth;

    private void Start()
    {
        health = maxHealth;
    }

    public void DamageHealth(float damage)
    {
        health -= damage;
    }
}
