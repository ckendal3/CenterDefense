using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Parameters;

public class Spawner_Mono : MonoBehaviour
{
    private StageData stageData;
    private int index = 0;

    private int spawnCount = 0;
    private int maxSpawnCount = 0;

    private float timeTillNextSpawn;
    private float timeBetweenSpawns;

    private bool spawnEnemies = true;

    public BoxCollider[] colliders;

    private void Awake()
    {
        stageData = StageParameters.Instance.stageSet[index];
        timeBetweenSpawns = stageData.enemySpawnData.enemySpawnRate;

        maxSpawnCount = stageData.enemySpawnData.maxEnemySpawnCount;
    }

    private void Start()
    {
        // initial spawn amount
        SpawnEnemies(stageData.enemySpawnData.initialEnemySpawnAmount, stageData.enemySpawnData);
    }

    void Update()
    {
        timeTillNextSpawn -= Time.deltaTime;

        if(timeTillNextSpawn <= 0 && spawnEnemies == true)
        {
            timeTillNextSpawn = timeBetweenSpawns;

            SpawnEnemies(stageData.enemySpawnData.enemiesPerASpawn, stageData.enemySpawnData);
        }
    }


    private void SpawnEnemies(int spawnAmount, EnemySpawnData data)
    {
        GameObject go;
        EnemyData eData;
        MoveRigidbody moveRB;
        DamageObject dmObject;
        CapsuleCollider cCollider;
        for (int i = 0; i < spawnAmount; i++)
        {
            // don't spawn anymore enemies
            if(spawnCount >= maxSpawnCount)
            {
                spawnEnemies = false;
                return;
            }

            eData = GetRandomEnemyData();

            spawnCount += 1;

            // Create gameObject with random position
            go = new GameObject("Enemy " + i);
            go.transform.position = GetRandomSpawnPointInBounds(colliders[Random.Range(0, colliders.Length -1)].bounds);
            go.tag = "Enemy";
            go.layer = LayerMask.NameToLayer("Enemy");

            // add visuals
            go.AddComponent<MeshFilter>().sharedMesh = eData.mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = eData.material;

            // Add Collider
            cCollider = go.AddComponent<CapsuleCollider>();
            cCollider.radius = .6f;
            cCollider.height = 2.3f;


            // add health
            go.AddComponent<Health>().MaxHealth = eData.stats.health * stageData.multiplierData.enemyDifficultyMultiplier;

            // Freeze rigidbody
            go.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;

            // create damageable object
            dmObject = go.AddComponent<DamageObject>();
            dmObject.damageAmount = eData.stats.damage * stageData.multiplierData.enemyDifficultyMultiplier;
            dmObject.damageDistance = 1.5f;
            dmObject.hitMask = 1 << 10;//LayerMask.NameToLayer("Damageable");
            dmObject.timeBetweenHits = eData.stats.attackRate / stageData.multiplierData.enemyDifficultyMultiplier;

            // create movement for rigidbody
            moveRB = go.GetComponent<MoveRigidbody>();
            moveRB.hitMask = 1 << 10; //LayerMask.NameToLayer("Damageable");
            moveRB.speed = eData.stats.speed * stageData.multiplierData.enemySpeedMultiplier;
            moveRB.hitDistance = 1.2f;
        }
    }

    private EnemyData GetRandomEnemyData()
    {
        float rInt = Random.Range(0, 1);
        EnemySpawnChance[] spawnables = stageData.enemySpawnData.spawnChance;

        EnemyData data = stageData.enemySpawnData.spawnChance[0].enemyData;
        for (int i = 0; i < spawnables.Length; i++)
        {
            // if spawn chance
            if (spawnables[i].spawnChance > 1 - rInt)
            {
                data = stageData.enemySpawnData.spawnChance[i].enemyData;
            }
        }

        return data;
    }

    private Vector3 GetRandomSpawnPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
