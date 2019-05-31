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
    static int debugGroupID = 0;
    static int debugIndividualID = 0;
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
    public static void CreateProjectiles(ushort projectileID, float3 spawnPos, quaternion spawnRot, ushort ownerID)
    {
        #if UNITY_EDITOR
        if (debugGroupID > 65000)
        {
            debugGroupID = 0;
        }
        debugGroupID += 1;
        debugIndividualID = 0;
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
    private static void SetComponents(ushort projectileID, float3 spawnPos, quaternion spawnRot, ProjectileData data, ushort ownerID)
    {
        int projectileCount = data.projectilesPerAShot;

        #region Template Projectile Entity
        Entity templateEnt;

        // create the entity to clone from
        templateEnt = eManager.CreateEntity(projectileArch);

        eManager.AddComponentData(templateEnt, new Scale { Value = .1f});

        #if UNITY_EDITOR
        debugIndividualID += 1;
        eManager.SetName(templateEnt, data.projectileName + " Projectile " + debugIndividualID + " - Group " + debugGroupID);
        #endif

        //Generic Components
        eManager.SetComponentData(templateEnt, new Translation { Value = spawnPos });
        eManager.SetComponentData(templateEnt, new Rotation { Value = spawnRot });
        eManager.SetSharedComponentData(templateEnt, new RenderMesh { mesh = data.mesh, material = data.material });

        // specific components
        eManager.SetComponentData(templateEnt, new PreviousTranslation { Value = spawnPos });
        eManager.SetComponentData(templateEnt, new Owner { ID = ownerID });
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
            NativeArray<Entity> cloneEnts = new NativeArray<Entity>(projectileCount - 1, Allocator.TempJob);

            // creating and destroying entities is batched
            // we are cloning the entity so all components have the same value
            eManager.Instantiate(templateEnt, cloneEnts);

            // get random spreads
            GetConeOfFire(data, spawnRot, projectileCount, out NativeArray<quaternion> rSpreads);

            // We need to add random spread to the first projectile
            eManager.SetComponentData(templateEnt, new Rotation { Value = rSpreads[0] });

            // we start at one because set our templateEnt to the first spread
            for (int i = 1; i < projectileCount; i++)
            {
                #region Entity Debugging
                #if UNITY_EDITOR
                debugIndividualID += 1;
                eManager.SetName(cloneEnts[i -1], data.projectileName + " Projectile " + debugIndividualID + " - Group " + debugGroupID);
                #endif
                #endregion
                
                // we subtract one because our cloned projectiles need to iterate starting at zero
                eManager.SetComponentData(cloneEnts[i - 1], new Rotation { Value = rSpreads[i] });
            }

            //we have set all our data, dispose our native container to avoid memory leak
            cloneEnts.Dispose();
            rSpreads.Dispose();
        }
    }

    private static void GetConeOfFire(ProjectileData data, quaternion rotation, int count, out NativeArray<quaternion> rSpreads)
    {
        rSpreads = new NativeArray<quaternion>(count, Allocator.TempJob);

        PelletSpread pelletSpread = new PelletSpread { minimumSpread = -.2f, maximumSpread = .2f };

        Random r = new Random(21793 * (uint)System.Environment.TickCount);

        float3 deviation;

        for (int i = 0; i < count; i++)
        {
            deviation = new float3(r.NextFloat(pelletSpread.minimumSpread, pelletSpread.maximumSpread),
                           r.NextFloat(pelletSpread.minimumSpread, pelletSpread.maximumSpread),
                           r.NextFloat(pelletSpread.minimumSpread, pelletSpread.maximumSpread));

            rSpreads[i] = math.mul(rotation, quaternion.Euler(deviation));
        }
    }
}

