using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using WDIB.Components;

[UpdateInGroup(typeof(InputSystemGroup))]
public class InputSystem : JobComponentSystem
{
    private Camera mainCam;
    private EntityQuery m_InputGroup;

    // Take all the gathered inputs and apply them to the inputcomponents
    [BurstCompile]
    public struct ApplyNewInputsJob : IJobForEach<InputComponent>
    {
        [ReadOnly] public NativeArray<InputComponent> newInputs;

        public void Execute([WriteOnly]ref InputComponent input)
        {
            for (int i = 0; i < newInputs.Length; i++)
            {
                if(newInputs[i].ID == input.ID)
                {
                    input = newInputs[i];
                }
            }
        }
    }

    // refactor
    // make turret rotate to cast
    [BurstCompile]
    public struct ApplyInputRaycastToControlled : IJobForEach<Owner, ControlledByPlayer>
    {
        [ReadOnly] public NativeArray<float3> lookAtPositions;

        public void Execute([ReadOnly] ref Owner owner, [WriteOnly] ref ControlledByPlayer controlled)
        {
            // Set a default value
            controlled.lookPosition = lookAtPositions[owner.ID];
        }
    }

    //**********************************************************
    //***********************REFACTOR***************************
    //**********************************************************
    // Turret looks down - need axis lock - look at InteractableSystem: UpdateInteractableRotation job
    // Probably could have a network gather inputs and apply to the correct ID job
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        mainCam = Camera.main;

        int inputCount = m_InputGroup.CalculateLength();

        //Setup/Gather local input
        // I can get rid of the oldInputs if I figure out a good way to set the newInput position when no input is received
        NativeArray<InputComponent> oldInputs = m_InputGroup.ToComponentDataArray<InputComponent>(Allocator.TempJob); 
        NativeArray<InputComponent> newInputs = new NativeArray<InputComponent>(inputCount, Allocator.TempJob);


        // Gather inputs
        float2 newInput;
        bool pressedDown;
        for (int j = 0; j < inputCount; j++)
        {
            newInput = oldInputs[j].InputPosition;
            pressedDown = false;
            // if it is the editor and (not in android or ios) mode or it is in windows/mac mode
#if UNITY_EDITOR && (!UNITY_ANDROID || !UNITY_IOS ) || UNITY_STANDALONE
            if (Input.GetMouseButton(0))
            {
                newInput.x = Input.mousePosition.x;
                newInput.y = Input.mousePosition.y;

                // GetMouseButton is infinite shots
                // GetMouseButtonDown is one shoot per a click
                if (Input.GetMouseButton(1))
                {
                    pressedDown = true;
                }
            }

#elif UNITY_ANDROID || UNITY_IOS
            if(Input.touchCount > 0)
            {
                newInput.x = Input.touches[0].position.x;
                newInput.y = Input.touches[0].position.y;

                pressedDown = true;
            }
#endif

            newInputs[j] = new InputComponent { InputPosition = newInput, PressedDown = pressedDown };
        }

        // Apply the new inputs
        JobHandle applyInputsJob = new ApplyNewInputsJob
        {
            newInputs = newInputs
        }.Schedule(this, inputDeps);
        applyInputsJob.Complete();


        // Create Input Rays for controlled turrets look at point
        NativeArray<Ray> inputRays = new NativeArray<Ray>(inputCount, Allocator.TempJob);
        NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(inputRays.Length, Allocator.TempJob);
        for (int k = 0; k < inputCount; k++)
        {
            inputRays[k] = mainCam.ScreenPointToRay(new Vector2(newInputs[k].InputPosition.x, newInputs[k].InputPosition.y));
            commands[k] = new RaycastCommand(inputRays[k].origin, inputRays[k].direction, 2000f);
        }

        NativeArray<RaycastHit> rayHits = new NativeArray<RaycastHit>(commands.Length, Allocator.TempJob);
        JobHandle batchJob = RaycastCommand.ScheduleBatch(commands, rayHits, 1, inputDeps);

        batchJob.Complete();

        // Get the hit points from the hits from the input
        NativeArray<float3> hitPoints = new NativeArray<float3>(rayHits.Length, Allocator.TempJob);
        for (int i = 0; i < rayHits.Length; i++)
        {
            if(rayHits[i].collider)
            {
                hitPoints[i] = rayHits[i].point;
            }
        }

        // Apply the position change to those that need it
        JobHandle applyInputJob = new ApplyInputRaycastToControlled
        {
            lookAtPositions = hitPoints
        }.Schedule(this, batchJob);

        // Make sure we finish
        applyInputJob.Complete();

        oldInputs.Dispose();
        newInputs.Dispose();
        inputRays.Dispose();
        commands.Dispose();
        rayHits.Dispose();
        hitPoints.Dispose();

        return JobHandle.CombineDependencies(applyInputJob, batchJob, inputDeps);
    }

    protected override void OnCreate()
    {
        // Get the entities we need to look at
        m_InputGroup = GetEntityQuery(new EntityQueryDesc
        {
            // must have these components
            All = new ComponentType[] { ComponentType.ReadWrite<InputComponent>()}
        });
    }

}
