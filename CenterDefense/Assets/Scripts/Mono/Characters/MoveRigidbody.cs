using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MoveRigidbody : MonoBehaviour
{
    private Transform damageableTransform;
    private Rigidbody rBody;
    private DamageObject damageObj;

    public float speed = 2f;

    [SerializeField]
    public LayerMask hitMask;

    [SerializeField]
    public float hitDistance = 1f;

    // probably do not need this variable anymore
    public bool moveTowards = true;

    private void Start()
    {
        rBody = gameObject.GetComponent<Rigidbody>();
        damageObj = gameObject.GetComponent<DamageObject>();

        FindClosestDamageable();

        damageObj.enabled = false;
    }

    private void OnDisable()
    {
        rBody.velocity = Vector3.zero;
    }

    private void Update()
    {
        CheckHit();

        // lets move towards it
        if(moveTowards)
        {
            if (damageableTransform == null)
            {
                FindClosestDamageable();
                return;
            }

            LookAtPlayer();
            AddForceTowardsPlayer();
        }
        else
        {
            rBody.velocity = Vector3.zero;
        }
    }

    public void AddForceTowardsPlayer()
    {
        rBody.velocity = gameObject.transform.forward * speed * 2;
    }

    public void LookAtPlayer()
    {
        gameObject.transform.LookAt(new Vector3(damageableTransform.position.x, gameObject.transform.position.y, damageableTransform.position.z));

        if(rBody.velocity != gameObject.transform.forward * speed * 2f)
        {
            rBody.velocity = gameObject.transform.forward * speed * 2f;
        }
    }

    public void FindClosestDamageable()
    {
        GameObject[] damageables = GameObject.FindGameObjectsWithTag("Damageable");

        // if there are no damageables - deactivate?
        if(damageables.Length == 0)
        {
            Debug.LogWarning("No damagables in scene - MoveRigibody");
            damageableTransform = null;
            moveTowards = false;
            this.enabled = false;
            return;
        }

        GameObject closestGO = damageables[0];
        for (int i = 0; i < damageables.Length; i++)
        {
            // if current closest distance is more than new distance - assign new transform
            if (math.distance(closestGO.transform.position, transform.position) > math.distance(damageables[i].transform.position, transform.position))
            {
                closestGO = damageables[i];
            }
        }

        damageableTransform = closestGO.transform;
        //Debug.Log("Closest Damageable: " + damageableTransform.name);
    }

    private void CheckHit()
    {
        Ray ray;
        RaycastHit hit;
        ray = new Ray
        {
            direction = math.forward(gameObject.transform.rotation),
            origin = gameObject.transform.position
        };

        Health health;
        if (Physics.Raycast(ray, out hit, hitDistance, hitMask))
        {
            health = hit.collider.gameObject.GetComponent<Health>();

            if (health != null)
            {
                moveTowards = false;
                damageObj.enabled = true;
            }

        }
        else
        {
            damageObj.enabled = false;
            moveTowards = true;
        }
    }
}
