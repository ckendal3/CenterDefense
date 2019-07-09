using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using WDIB.Components;
using WDIB.Parameters;
using Random = UnityEngine.Random;

namespace WDIB.Factory
{
    public static class EnemyFactory
    {
        static StageParameters sParameters;
        static BoxCollider[] colliders = null;

        public static void Initialize()
        {
            sParameters = StageParameters.Instance;

            GameObject.Find("Spawner").GetComponentsInChildren<BoxCollider>();
        }

        /// <summary>
        /// Spawn a certain amount of projectiles based on the approprate projectile ID
        /// </summary>
        /// <param name="projectileID"></param>
        /// <param name="spawnAmount"></param>
        public static void CreateEnemy(int stageID, float3 spawnPos, quaternion spawnRot, int spawnCount)
        {
            // need to do this just on construction
            Initialize();

            SetComponents(stageID, spawnPos, spawnRot, sParameters.stageSet[stageID].data, spawnCount);
        }

        /// <summary>
        /// Set the components for the projectile based on the inputted projectile data
        /// </summary>
        /// <param name="spawnTransform"></param>
        /// <param name="data"></param>
        private static void SetComponents(int projectileID, float3 spawnPos, quaternion spawnRot, StageData data, int spawnCount)
        {
            CapsuleCollider cCollider;
            DamageObject dmObject;
            MoveRigidbody moveRB;
            EnemyData eData;

            // for every projectile to spawn per a shot (think shotgun)
            GameObject go;
            for (int i = 0; i < spawnCount; i++)
            {
                eData = GetRandomEnemyData(data);

                // Create gameObject with random position
                go = new GameObject("Enemy " + i);
                go.transform.position = GetRandomSpawnPointInBounds();
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
                go.AddComponent<Health>().MaxHealth = eData.stats.health * data.multiplierData.enemyDifficultyMultiplier;

                // Freeze rigidbody
                go.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;

                // create damageable object
                dmObject = go.AddComponent<DamageObject>();
                dmObject.damageAmount = eData.stats.damage * data.multiplierData.enemyDifficultyMultiplier;
                dmObject.damageDistance = 1.5f;
                dmObject.hitMask = 1 << 10;//LayerMask.NameToLayer("Damageable");
                dmObject.timeBetweenHits = eData.stats.attackRate / data.multiplierData.enemyDifficultyMultiplier;

                // create movement for rigidbody
                moveRB = go.GetComponent<MoveRigidbody>();
                moveRB.hitMask = 1 << 10; //LayerMask.NameToLayer("Damageable");
                moveRB.speed = eData.stats.speed * data.multiplierData.enemySpeedMultiplier;
                moveRB.hitDistance = 1.2f;

                // entity
                go.AddComponent<EntityComponent>().Initialize(new EnemyTag { });
            }

        }

        private static EnemyData GetRandomEnemyData(StageData data)
        {
            float rInt = Random.Range(0, 1);
            EnemySpawnChance[] spawnables = data.enemySpawnData.spawnChance;

            EnemyData eData = data.enemySpawnData.spawnChance[0].enemyData;
            for (int i = 0; i < spawnables.Length; i++)
            {
                // if spawn chance
                if (spawnables[i].spawnChance > 1 - rInt)
                {
                    return data.enemySpawnData.spawnChance[i].enemyData;
                }
            }

            return null;
        }

        private static Vector3 GetRandomSpawnPointInBounds()
        {
            Bounds bounds = colliders[Random.Range(0, colliders.Length - 1)].bounds;

            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }
    }
}