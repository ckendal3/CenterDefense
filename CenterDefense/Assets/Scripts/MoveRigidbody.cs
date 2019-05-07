using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MoveRigidbody : MonoBehaviour
{
    private Transform playerTransform;
    private Rigidbody rBody;

    public float speed = 2f;

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").transform;
        rBody = gameObject.GetComponent<Rigidbody>();

        LookAtPlayer();
        AddForceTowardsPlayer();
    }

    private void Update()
    {
        // if we are close enough, destroy ourselves
        if(math.distance(gameObject.transform.position, playerTransform.position) < 1)
        {
            Destroy(gameObject);
        }

        LookAtPlayer();
        AddForceTowardsPlayer();
    }

    public void AddForceTowardsPlayer()
    {
        rBody.velocity = gameObject.transform.forward * speed * 2;
    }

    public void LookAtPlayer()
    {
        gameObject.transform.LookAt(new Vector3(playerTransform.position.x, gameObject.transform.position.y, playerTransform.position.z));

        if(rBody.velocity != gameObject.transform.forward * speed * 2f)
        {
            rBody.velocity = gameObject.transform.forward * speed * 2f;
        }
    }
}
