using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToDestination : MonoBehaviour
{
    /// <summary>
    /// The destination for the gameobject to go to.
    /// </summary>
    [SerializeField]
    public Vector3 destinationPos = new Vector3(0f, 0f, -10f);

    /// <summary>
    /// How many units per a second should the gameObject move towards the destination.
    /// </summary>
    [SerializeField]
    public float speed = 1;


    private Vector3 heading;
    private float distance;

    private void Awake()
    {
        heading = gameObject.transform.position - destinationPos;
        distance = heading.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        // made it to our position
        if (distance <= 0)
        {
            return;
        }

        // how far to move in a frame
        float step = Time.deltaTime * speed;

        // move towards the destination
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, destinationPos, step);
    }
}