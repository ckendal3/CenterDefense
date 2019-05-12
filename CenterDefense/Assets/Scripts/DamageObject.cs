using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    [SerializeField]
    public float damageAmount = 20;
    private Health damageableHealth = null;

    private bool isHitting = false;

    [SerializeField]
    public float timeBetweenHits = 1;
    private float currentTime;

    private MoveRigidbody moveRB = null;

    private void Start()
    {
        moveRB = gameObject.GetComponent<MoveRigidbody>();

        if(moveRB == null)
        {
            Debug.LogWarning("Move Rigidbody is null. - DamageObject");
        }

        timeBetweenHits *= gameObject.transform.localScale.x;

        currentTime = timeBetweenHits;
    }

    // when we are starting to overlap
    private void OnCollisionEnter(Collision collision)
    {
        Health tmpHealth;
        for (int i = 0; i < collision.contactCount; i++)
        {
            tmpHealth = collision.collider.gameObject.GetComponent<Health>();
            
            // only hit the first damageable object we can
            if (tmpHealth != null)
            {
                isHitting = true;

                moveRB.moveTowards = false;
                moveRB.enabled = false;

                damageableHealth = tmpHealth;
                return;
            }
        }
    }

    // when we are done overlapping
    private void OnCollisionExit(Collision collision)
    {
        damageableHealth = null;

        moveRB.enabled = true;
        moveRB.moveTowards = true;
        
        currentTime = timeBetweenHits;
    }

    // called every frame for collision stay
    private void OnCollisionStay(Collision collision)
    {
        if (isHitting)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < 0 && damageableHealth)
            {
                currentTime = timeBetweenHits;

                DamagePlayerNow();
            }
        }
    }

    private void DamagePlayerNow()
    {
        damageableHealth.ApplyDamage(gameObject.transform.localScale.x * damageAmount);
    }
}
