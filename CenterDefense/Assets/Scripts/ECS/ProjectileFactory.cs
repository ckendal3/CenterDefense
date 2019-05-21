using WDIB.Parameters;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using WDIB.Components;

// Should there be options to pass in the random seed cone or that should be done else where?
public static class ProjectileFactory
{
    static EntityManager eManager;
    static EntityArchetype projectileArch;

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
                                    ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadWrite<RenderMesh>()
                                    );
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

        SetComponents(projectileID, spawnPos, spawnRot, WeaponParameters.Instance.projectileSet[projectileID]);
    }

    /// <summary>
    /// Set the components for the projectile based on the inputted projectile data
    /// </summary>
    /// <param name="spawnTransform"></param>
    /// <param name="data"></param>
    private static void SetComponents(int projectileID, float3 spawnPos, quaternion spawnRot, ProjectileData data)
    {
        NativeArray<Entity> projectileEnts = new NativeArray<Entity>(data.projectilesPerAShot, Allocator.TempJob);

        // creating and destroying entities is batched
        eManager.CreateEntity(projectileArch, projectileEnts);

        // adding explosion, tracking, or other components go here - just like headshot AddComponent

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
            eManager.SetComponentData(entity, new Translation { Value = spawnPos });
            eManager.SetComponentData(entity, new Rotation { Value = spawnRot });
            eManager.SetSharedComponentData(entity, new RenderMesh { mesh = data.mesh, material = data.material });

            // specific components
            eManager.SetComponentData(entity, new PreviousTranslation { Value = spawnPos });
            eManager.SetComponentData(entity, new Damage { Value = data.damage });
            eManager.SetComponentData(entity, new Speed { Value = data.speed });

            //eManager.SetSharedComponentData(entity, new ProjectileID { Value = projectileID });
            //eManager.SetComponentData(entity, new Owner { ID = ownerID });
            //eManager.SetComponentData(entity, new Distance { currentValue = 0, maxValue = data.distance });
        }

        //we have set all our data, dispose our native container to avoid memory leak
        projectileEnts.Dispose();
    }
}