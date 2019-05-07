using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField]
    public PlayerHealth pHealth;

    [SerializeField]
    public float damageAmount = 20;

    private bool isHitting = false;

    [SerializeField]
    public float timeBetweenHits = 1;
    private float currentTime;

    private void Update()
    {
        if(isHitting)
        {
            currentTime -= Time.deltaTime;

            if(currentTime < 0)
            {
                currentTime = timeBetweenHits;

                DamagePlayerNow();
            }
        }
    }

    private void Start()
    {
        timeBetweenHits *= gameObject.transform.localScale.x;

        currentTime = timeBetweenHits;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject;

        for(int i = 0; i < collision.contactCount; i++)
        {
            hitObject = collision.collider.gameObject;
            if (hitObject.GetComponent<PlayerHealth>())
            {
                DamagePlayerNow();

                isHitting = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject hitObject;

        for (int i = 0; i < collision.contactCount; i++)
        {
            hitObject = collision.collider.gameObject;
            if (hitObject.GetComponent<PlayerHealth>())
            {
                isHitting = false;
                return;
            }
        }
    }

    private void DamagePlayerNow()
    {
        pHealth.DamageHealth(gameObject.transform.localScale.x * damageAmount);
    }
}
