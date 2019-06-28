using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using WDIB.Components;
using WDIB.Core.States;
using WDIB.Parameters;

public class Spawner_Mono : MonoBehaviour
{
    private StageData stageData;

    private int spawnLimit;
    private int spawnCount;

    private int enemyKillCount = 0;
    private int enemyLimit = 0;

    private float timeTillNextSpawn;
    private float timeBetweenSpawns;

    private bool spawnEnemies = true;
    private bool inProgress = false;

    private BoxCollider[] colliders;

    private void Awake()
    {
        GameEvents.onGameStatusChange += OnGameStatusChange;
        GameEvents.onKillEvent += OnEnemyDeath;
    }

    public void Initialize(StageData data, BoxCollider[] bColliders)
    {
        stageData = data;
        colliders = bColliders;
    }

    private void OnGameStatusChange(GameStateStatusV2 gameStatus)
    {
        switch (gameStatus)
        {
            case GameStateStatusV2.BetweenRounds:
                inProgress = false;
                break;
            case GameStateStatusV2.StartRound:
                inProgress = true;
                StartNewStage();
                break;
            case GameStateStatusV2.InProgress:
                inProgress = true;
                break;
            case GameStateStatusV2.GameOver:
                inProgress = false;
                break;
        }
    }

    /// <summary>
    /// This gathers all the appropriate data and sets it
    /// </summary>
    public void StartNewStage()
    {
        timeBetweenSpawns = stageData.enemySpawnData.spawnRate;
        timeTillNextSpawn = timeBetweenSpawns;

        enemyLimit = stageData.enemySpawnData.maxTotalCount;
        spawnLimit = stageData.enemySpawnData.maxTotalCount;

        enemyKillCount = 0;
        spawnCount = 0;

        // initial spawn amount
        SpawnEnemies(stageData.enemySpawnData.initialSpawnCount, stageData.enemySpawnData);
    }

    private void OnEnemyDeath()
    {
        enemyKillCount += 1;

        // if all enemies are dead
        if(enemyKillCount >= enemyLimit)
        {
            GameEvents.onGameStatusChange?.Invoke(GameStateStatusV2.BetweenRounds);
        }
    }

    void Update()
    {
        if(inProgress == false)
        {
            return;
        }

        timeTillNextSpawn -= Time.deltaTime;

        if(timeTillNextSpawn <= 0 && spawnEnemies == true)
        {
            timeTillNextSpawn = timeBetweenSpawns;

            SpawnEnemies(stageData.enemySpawnData.enemiesPerASpawn, stageData.enemySpawnData);
        }
    }

    private void SpawnEnemies(int spawnAmount, EnemySpawnData data)
    {
        Debug.Log("Spawn enmies");

        Entity entity = Entity.Null; 

        GameObject go;
        EnemyData eData;
        MoveRigidbody moveRB;
        DamageObject dmObject;
        CapsuleCollider cCollider;
        for (int i = 0; i < spawnAmount; i++)
        {
            // don't spawn anymore enemies
            if(spawnCount >= spawnLimit)
            {
                Debug.Log($"Reached spawn limit {spawnLimit}");
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

            // entity
            go.AddComponent<EntityComponent>().Initialize(new EnemyTag { });
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