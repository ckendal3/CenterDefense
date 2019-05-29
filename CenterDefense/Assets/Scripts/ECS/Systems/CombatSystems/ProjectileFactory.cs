using WDIB.Parameters;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using WDIB.Components;

// Should there be options to pass in the random seed cone or that should be done else where?
//eManager.SetComponentData(tmp, new Translation { Value = positions[i].Value + (math.forward(rotations[i].Value) * math.distance(projSpawnPoints[i].Value, positions[i].Value)) }); //+ (projSpawnPoints[i].Value + math.forward(rotations[i].Value)) + ) 
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
                                    ComponentType.ReadWrite<Speed>(), ComponentType.ReadWrite<Damage>(), ComponentType.ReadWrite<Owner>(),
                                    ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadWrite<RenderMesh>(),
                                    ComponentType.ReadWrite<Distance>(), ComponentType.ReadOnly<ProjectileID>()
                                    );

        wParameters = WeaponParameters.Instance;
    }

    /// <summary>
    /// Spawn a certain amount of projectiles based on the approprate projectile ID
    /// </summary>
    /// <param name="projectileID"></param>
    /// <param name="spawnAmount"></param>
    public static void CreateProjectiles(int projectileID, float3 spawnPos, quaternion spawnRot, int ownerID)
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

        SetComponents(projectileID, spawnPos, spawnRot, wParameters.projectileSet[projectileID], ownerID);
    }

    /// <summary>
    /// Set the components for the projectile based on the inputted projectile data
    /// </summary>
    /// <param name="spawnTransform"></param>
    /// <param name="data"></param>
    private static void SetComponents(int projectileID, float3 spawnPos, quaternion spawnRot, ProjectileData data, int ownerID)
    {
        int projectileCount = data.projectilesPerAShot;

        #region Template Projectile Entity
        Entity templateEnt;

        // create the entity to clone from
        templateEnt = eManager.CreateEntity(projectileArch);

        #if UNITY_EDITOR
        currentProjectileID += 1;
        eManager.SetName(templateEnt, data.projectileName + " Projectile " + currentProjectileID + " - Group " + projectileGroupID);
        #endif

        //Generic Components
        eManager.SetComponentData(templateEnt, new Translation { Value = spawnPos });
        eManager.SetComponentData(templateEnt, new Rotation { Value = spawnRot });
        eManager.SetSharedComponentData(templateEnt, new RenderMesh { mesh = data.mesh, material = data.material });

        // specific components
        eManager.SetComponentData(templateEnt, new PreviousTranslation { Value = spawnPos });
        eManager.SetComponentData(templateEnt, new Owner { ID = (ushort)ownerID });
        eManager.SetComponentData(templateEnt, new Distance { distanceTravelled = 0, maxAllowedDistance = data.maxDistance });
        eManager.SetComponentData(templateEnt, new Damage { Value = data.damage});
        eManager.SetComponentData(templateEnt, new Speed { Value = data.speed });
        eManager.SetSharedComponentData(templateEnt, new ProjectileID { ID = projectileID });

        // Add extra components specified in the projectile data
        //if (data.componentData.Length > 0)
        //{
        //    int componentCount = data.componentData.Length;
        //    for (int i = 0; i < componentCount; i++)
        //    {
        //        AddExtraComponentData(ref templateEnt, data.componentData[i]);
        //    }
        //}
        #endregion

        // if we have more than one projectile
        if (projectileCount > 1)
        {
            // taking into account the template entity - we subtract one
            NativeArray<Entity> projectileEnts = new NativeArray<Entity>(projectileCount - 1, Allocator.TempJob);

            // creating and destroying entities is batched
            // we are cloning the entity so all components have the same value
            eManager.Instantiate(templateEnt, projectileEnts);

            for (int i = 0; i < projectileEnts.Length; i++)
            {
                #region Entity Debugging
                #if UNITY_EDITOR
                currentProjectileID += 1;
                eManager.SetName(projectileEnts[i], data.projectileName + " Projectile " + currentProjectileID + " - Group " + projectileGroupID);
                #endif
                #endregion

                eManager.SetComponentData(projectileEnts[i], new Rotation { Value = spawnRot });
            }

            //we have set all our data, dispose our native container to avoid memory leak
            projectileEnts.Dispose();
        }
    }
}

