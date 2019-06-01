using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using WDIB.Components;
using WDIB.Parameters;

// TODO: Add damaging
public class ExplosiveSystem : JobComponentSystem
{
    private LayerMask hitLayer;
    private EntityManager eManager;
    private Collider[] colliders;

    private EntityQuery m_explosiveGroup;
    private WeaponParameters wParameters;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int count = m_explosiveGroup.CalculateLength();

        // get the explosive entities
        var entities = m_explosiveGroup.ToEntityArray(Allocator.TempJob, out JobHandle entHandle);
        entHandle.Complete();

        // Get all our ECS components we will need - this are minified jobs
        var explosives = m_explosiveGroup.ToComponentDataArray<Explosive>(Allocator.TempJob, out JobHandle expHandle);
        var positions = m_explosiveGroup.ToComponentDataArray<Translation>(Allocator.TempJob, out JobHandle posHandle);
        var owners = m_explosiveGroup.ToComponentDataArray<Owner>(Allocator.TempJob, out JobHandle ownerHandle);

        // make sure we have all our component data (minified jobs are done)
        expHandle.Complete();
        posHandle.Complete();
        ownerHandle.Complete();


        int explosiveId;
        ExplosiveData data;

        for (int i = 0; i < count; i++)
        {
            explosiveId = (int)explosives[i].ID;
            data = wParameters.explosiveSet[explosiveId];

            // get our hits and process
            int hitCount = Physics.OverlapSphereNonAlloc(positions[i].Value, data.radius, colliders);
            if (hitCount > 0)
            {

                IPushable pushable;
                //go through every hit and do damage
                for (int j = 0; j < hitCount; j++)
                {
                    if (colliders[j] != null)
                    {
                        pushable = colliders[j].GetComponent<IPushable>();

                        // Create helper variables
                        var point = colliders[j].transform.position;
                        var origin = positions[i].Value;

                        var distance = Vector3.Distance(origin, point);
                        var curveVal = data.damageCurve.Evaluate(distance / data.radius);

                        if (pushable != null)
                        {
                            pushable.ApplyImpulseForce(curveVal * data.force, positions[i].Value, data.radius);
                        }

                    }
                }
            }
        }

        // batch destroy explosive entities
        eManager.DestroyEntity(entities);

        positions.Dispose();
        entities.Dispose();
        owners.Dispose();
        explosives.Dispose();

        return inputDeps;
    }

    protected override void OnCreate()
    {
        wParameters = WeaponParameters.Instance;

        eManager = World.Active.EntityManager;

        colliders = new Collider[32];

        m_explosiveGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<Explosive>(), ComponentType.ReadOnly<Translation>() },
            None = new ComponentType[] { ComponentType.ReadOnly<ProjectileID>() }
        });
    }
}
