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
    private EntityQuery m_InputGroup;

    public struct ApplyWeaponInputsJob : IJobForEach<WeaponComponent, Owner>
    {
        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<InputComponent> inputs;

        public void Execute(ref WeaponComponent weaponComponent, ref Owner owner)
        {
            WeaponComponent tmpInput = weaponComponent;

            // find the correct input based on ID and apply it to the weapon
            for (int i = 0; i < inputs.Length; i++)
            {
                if (owner.ID == inputs[i].ID)
                {
                    weaponComponent = new WeaponComponent
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
        JobHandle weaponInputJob = new ApplyWeaponInputsJob
        {
            inputs = m_InputGroup.ToComponentDataArray<InputComponent>(Allocator.TempJob)
        }.Schedule(this, inputDeps);

        return weaponInputJob;
    }

    protected override void OnCreate()
    {
        m_InputGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadWrite<InputComponent>() }
        });
    }
}
