using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;
using WDIB.Parameters;

public static class ExplosiveFactory
{
    static EntityManager eManager;
    static EntityArchetype explosiveArch;
    static WeaponParameters weaponParameters;

    private static void Initialize()
    {
        eManager = World.Active.EntityManager;

        // create our archetype
        explosiveArch = eManager.CreateArchetype(
                                    ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<ExplosiveID>(),
                                    ComponentType.ReadOnly<LocalToWorld>(), ComponentType.ReadWrite<Owner>()
                                    );

        weaponParameters = WeaponParameters.Instance;
    }

    public static void CreateExplosive(ushort explosiveID, float3 spawnPos, ushort ownerID)
    {
        // -------------------
        // create visuals here
        // Make new method ---
        // -------------------

        Initialize();

        SetComponents(explosiveID, spawnPos, ownerID, weaponParameters.explosiveSet[explosiveID]);
    }

    private static void SetComponents(ushort explosiveID, float3 spawnPos, ushort ownerID, ExplosiveData data)
    {
        Entity templateEnt;

        // create the entity to clone from
        templateEnt = eManager.CreateEntity(explosiveArch);

#if UNITY_EDITOR
        eManager.SetName(templateEnt, data.explosiveName + " Explosive ");
#endif

        //Generic Components
        eManager.SetComponentData(templateEnt, new Translation { Value = spawnPos });
        eManager.SetComponentData(templateEnt, new Owner { ID = ownerID });
        eManager.SetComponentData(templateEnt, new ExplosiveID { ID = explosiveID });
    }
}