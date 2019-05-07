using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SpawnInColliders : MonoBehaviour
{
    [SerializeField]
    public GameObject prefabToSpawnFrom;

    [SerializeField]
    public float timeBetweenSpawns = 5;

    private float currentSpawnTime = 5;

    private BoxCollider[] colliders; 

    private GameObject tmpGO;

    private void Awake()
    {
        currentSpawnTime = timeBetweenSpawns;

        colliders = gameObject.GetComponentsInChildren<BoxCollider>();
    }

    private void Start()
    {
        SpawnEnemies();
    }

    private void Update()
    {
        currentSpawnTime -= Time.deltaTime;

        if(currentSpawnTime < 0)
        {
            currentSpawnTime = timeBetweenSpawns;

            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        if (prefabToSpawnFrom == null) { return; }

        for (int i = 0; i < colliders.Length; i++)
        {
            tmpGO = Instantiate(prefabToSpawnFrom, RandomPointInBounds(colliders[i].bounds), Quaternion.identity);
            tmpGO.SetActive(true);
        }
    }

    public Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
