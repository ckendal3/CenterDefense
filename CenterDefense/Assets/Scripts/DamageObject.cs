using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(MoveRigidbody))]
public class DamageObject : MonoBehaviour
{
    [SerializeField]
    public float damageAmount = 20;
    [SerializeField]
    public float damageDistance = 5f;

    [SerializeField]
    public LayerMask hitMask;

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

        Health health;
        if(Physics.Raycast(ray, out hit, damageDistance + gameObject.transform.localScale.x, hitMask)) // distance plus the size of our object
        {
            health = hit.collider.gameObject.GetComponent<Health>();

            if (health != null)
            {
                moveRB.moveTowards = false;
                DamageHealthObject(health);
            }
        }
        else
        {
            moveRB.moveTowards = true;
        }
    }

    private void DamageHealthObject(Health damageableHealth)
    {
        damageableHealth.ApplyDamage(gameObject.transform.localScale.x * damageAmount);
    }
}
