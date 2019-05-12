using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MoveRigidbody : MonoBehaviour
{
    private Transform damageableTransform;
    private Rigidbody rBody;

    public float speed = 2f;

    public bool moveTowards = true;

    void OnEnable()
    {
        damageableTransform = FindClosestDamageable();

        if (damageableTransform == null)
        {
            Debug.LogWarning("No damagables in scene - MoveRigibody");
            return;
        }

        Debug.Log("Closest Damageable: " + damageableTransform.name);

        rBody = gameObject.GetComponent<Rigidbody>();

        LookAtPlayer();
        AddForceTowardsPlayer();
    }

    private void Update()
    {
        // if we are close enough, destroy ourselves
        //if(math.distance(gameObject.transform.position, damageableTransform.position) < 1)
        //{
        //    Destroy(gameObject);
        //}
        if(moveTowards)
        {
            LookAtPlayer();
            AddForceTowardsPlayer();
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

    public Transform FindClosestDamageable()
    {
        GameObject[] damageables = GameObject.FindGameObjectsWithTag("Damageable");

        if(damageables.Length == 0)
        {
            return null;
        }

        GameObject closestGO = damageables[0];
        for (int i = 0; i < damageables.Length; i++)
        {
            // if closest distance is more than new distance - assign new transform
            if (math.distance(closestGO.transform.position, transform.position) > math.distance(damageables[i].transform.position, transform.position))
            {
                closestGO = damageables[i];
            }
        }

        return closestGO.transform;
    }
}
