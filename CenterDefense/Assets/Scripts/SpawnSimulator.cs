using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSimulator : MonoBehaviour
{
    private Health health;
    private MoveRigidbody mRB;

    public float rScale;

    private void Awake()
    {
        health = gameObject.GetComponent<Health>();
        mRB = gameObject.GetComponent<MoveRigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Random r = new Random();

        rScale = Random.Range(.6f, 1.5f);

        gameObject.transform.localScale = new Vector3(rScale, rScale, rScale);

        health.maxHealth *= rScale;

        mRB.speed = 4 - rScale;
    }
}
