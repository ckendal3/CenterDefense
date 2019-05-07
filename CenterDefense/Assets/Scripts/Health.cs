using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    public float health = 20;

    public void TakeDamage(float damage)
    {
        Debug.Log("Taken " + damage + " damage");
        health -= damage;

        if(health < 0)
        {
            Destroy(gameObject);
        }
    }
}
