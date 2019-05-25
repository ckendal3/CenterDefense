using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using WDIB.Components;

[UpdateInGroup(typeof(InputSystemGroup))]
[UpdateAfter(typeof(InputSystem))]
public class WeaponInputSystem : JobComponentSystem
{
    private EntityQuery m_WeaponInputGroup, m_InputGroup;

    public struct ApplyWeaponInputJob : IJobParallelFor
    {
        [DeallocateOnJobCompletion]
        public NativeArray<WeaponComponent> weaponComponents;
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<Owner> owners;
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<InputComponent> inputs;

        public void Execute(int index)
        {
            WeaponComponent tmpInput = weaponComponents[index];

            // find the correct input based on ID and apply it to the weapon
            for (int i = 0; i < inputs.Length; i++)
            {
                if (owners[index].ID == inputs[i].ID)
                {
                    weaponComponents[index] = new WeaponComponent
                    {
                        weaponID = tmpInput.weaponID,
                        timeSinceLastBurstRound = tmpInput.timeSinceLastBurstRound,
                        timeSinceLastShot = tmpInput.timeSinceLastShot,
                        input = new WeaponInput { bReload = false, bShoot = inputs[i].PressedDown }
                    };
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int count = m_WeaponInputGroup.CalculateLength();

        JobHandle weaponInputJob = new ApplyWeaponInputJob
        {
            weaponComponents = m_WeaponInputGroup.ToComponentDataArray<WeaponComponent>(Allocator.TempJob),
            owners = m_WeaponInputGroup.ToComponentDataArray<Owner>(Allocator.TempJob), 
            inputs = m_InputGroup.ToComponentDataArray<InputComponent>(Allocator.TempJob),
        }.Schedule(count, 1, inputDeps);

        return weaponInputJob;
    }

    protected override void OnCreate()
    {
        m_WeaponInputGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadWrite<WeaponComponent>(), ComponentType.ReadOnly<Owner>() }
        });

        m_InputGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadWrite<InputComponent>() }
        });
    }
}
