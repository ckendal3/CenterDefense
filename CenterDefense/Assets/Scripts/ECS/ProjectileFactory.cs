using WDIB.Parameters;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using WDIB.Components;

// Should there be options to pass in the random seed cone or that should be done else where?
//eManager.SetComponentData(tmp, new Translation { Value = positions[i].Value + (math.forward(rotations[i].Value) * math.distance(projSpawnPoints[i].Value, positions[i].Value)) }); //+ (projSpawnPoints[i].Value + math.forward(rotations[i].Value)) + ) 
namespace WDIB.Factory
{
    public static class ProjectileFactory
    {
        static EntityManager eManager;
        static EntityArchetype projectileArch;

        static WeaponParameters wParameters;

        // use for debugging
    #if UNITY_EDITOR
        static int projectileGroupID = 0;
        static int currentProjectileID = 0;
    #endif 

        public static void Initialize()
        {
            eManager = World.Active.EntityManager;

            // create our archetype
            projectileArch = eManager.CreateArchetype(
                                        ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(), ComponentType.ReadWrite<PreviousTranslation>(),
                                        ComponentType.ReadWrite<Speed>(), ComponentType.ReadWrite<Damage>(),
                                        ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadWrite<RenderMesh>(),
                                        ComponentType.ReadWrite<Distance>()
                                        );

            wParameters = WeaponParameters.Instance;
        }

        /// <summary>
        /// Spawn a certain amount of projectiles based on the approprate projectile ID
        /// </summary>
        /// <param name="projectileID"></param>
        /// <param name="spawnAmount"></param>
        public static void CreateProjectiles(int projectileID, float3 spawnPos, quaternion spawnRot)
        {
    #if UNITY_EDITOR

            if (projectileGroupID > 65000) // yes I know this isn't a ushort and a ushort max value is 65535
            {
                projectileGroupID = 0;
            }
            projectileGroupID += 1;
    #endif
            // need to do this just on construction
            Initialize();

            SetComponents(projectileID, spawnPos, spawnRot, wParameters.projectileSet[projectileID]);
        }

        /// <summary>
        /// Set the components for the projectile based on the inputted projectile data
        /// </summary>
        /// <param name="spawnTransform"></param>
        /// <param name="data"></param>
        private static void SetComponents(int projectileID, float3 spawnPos, quaternion spawnRot, ProjectileData data)
        {
            NativeArray<Entity> projectileEnts = new NativeArray<Entity>(data.projectilesPerAShot, Allocator.TempJob);

            // batched entity creation
            eManager.CreateEntity(projectileArch, projectileEnts);

            // for every projectile to spawn per a shot (think shotgun)
            Entity entity;
            for (int i = 0; i < projectileEnts.Length; i++)
            {
                entity = projectileEnts[i];
    #if UNITY_EDITOR
                //This is just for debugging
                currentProjectileID += 1;
                eManager.SetName(entity, data.projectileName + " Projectile " + currentProjectileID + " - Group " + projectileGroupID);
    #endif

                //Generic Components
                eManager.SetComponentData(entity, new Translation { Value = spawnPos + (math.forward(spawnRot) * 2f) });
                eManager.SetComponentData(entity, new Rotation { Value = spawnRot }); // this should take in a seed for spread of cone or something - could be a burstable job
                eManager.SetSharedComponentData(entity, new RenderMesh { mesh = data.mesh, material = data.material }); // needs to pull data from the projectile data

                // specific components
                eManager.SetComponentData(entity, new PreviousTranslation { Value = spawnPos });
                eManager.SetComponentData(entity, new Damage { Value = data.damage });
                eManager.SetComponentData(entity, new Speed { Value = data.speed });
                eManager.SetComponentData(entity, new Distance { distanceTravelled = 0, maxAllowedDistance = data.maxDistance });
            }

            projectileEnts.Dispose();
        }
    }

}

