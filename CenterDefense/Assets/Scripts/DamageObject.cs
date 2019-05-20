using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveRigidbody))]
public class DamageObject : MonoBehaviour
{
    [SerializeField]
    public float damageAmount = 20;
    private Health damageableHealth = null;
    private Collider damageableCollider = null;

    private bool isHitting = false;

    [SerializeField]
    public float timeBetweenHits = 1;
    private float currentTime;

    private MoveRigidbody moveRB = null;

    private void Awake()
    {
        moveRB = gameObject.GetComponent<MoveRigidbody>();
    }

    private void Start()
    {
        timeBetweenHits *= gameObject.transform.localScale.x;

        currentTime = timeBetweenHits;
    }

    // when we are starting to overlap
    private void OnCollisionEnter(Collision collision)
    {
        Health tmpHealth;
        for (int i = 0; i < collision.contactCount; i++)
        {
            tmpHealth = collision.collider.GetComponent<Health>();
            
            // only hit the first damageable object we can
            if (tmpHealth != null)
            {
                Debug.Log("ATTACk");
                isHitting = true;

                moveRB.enabled = false;

                damageableCollider = collision.collider;
                damageableHealth = tmpHealth;
                return;
            }
        }
    }

    // when we are done overlapping
    private void OnCollisionExit(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            // colliders are the same - we are no longer damaging an object
            if (damageableCollider == collision.collider)
            {
                damageableHealth = null;

                // move to next position
                if (moveRB != null)
                {
                    moveRB.enabled = true;
                }

                isHitting = false;

                currentTime = timeBetweenHits;
                return;
            }
        }
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
