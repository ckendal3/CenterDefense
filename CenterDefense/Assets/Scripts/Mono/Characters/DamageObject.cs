using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using WDIB.Interfaces;

[RequireComponent(typeof(MoveRigidbody))]
public class DamageObject : MonoBehaviour
{
    [SerializeField]
    public float damageAmount = 20;
    [SerializeField]
    public float damageDistance = 5f;

    public LayerMask hitMask;

    private bool isHitting = false;

    [SerializeField]
    public float timeBetweenHits = 1;
    private float currentTime = 0;

    private MoveRigidbody moveRB = null;

    private void Start()
    {
        moveRB = gameObject.GetComponent<MoveRigidbody>();

        timeBetweenHits *= gameObject.transform.localScale.x;

        currentTime = timeBetweenHits;
    }

    private void OnEnable()
    {
        isHitting = true;
    }

    private void Update()
    {
        if (isHitting)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < 0)
            {
                currentTime = timeBetweenHits;

                CastDamageRay();
            }
        }
    }


    private void CastDamageRay()
    {
        Ray ray;
        RaycastHit hit;
        ray = new Ray
        {
            direction = math.forward(gameObject.transform.rotation),
            origin = gameObject.transform.position
        };

        IDamageable damageable;
        if(Physics.Raycast(ray, out hit, damageDistance, hitMask)) // distance plus the size of our object
        {
            damageable = hit.collider.gameObject.GetComponent<IDamageable>();

            if (damageable != null)
            {
                moveRB.moveTowards = false;
                damageable.ApplyDamage(gameObject.transform.localScale.x * damageAmount);
            }
        }
        else
        {
            moveRB.moveTowards = true;
        }
    }
}
