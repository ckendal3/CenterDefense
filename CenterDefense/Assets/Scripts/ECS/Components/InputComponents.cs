using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace WDIB.Components
{
    // Input for mouse/input position
    // this could be a shared component and save perf
    // but in reality because it causes a bunch of extra rays 
    // as it currently acts like input 0 is never the same for anything uses input
    public struct InputComponent : IComponentData
    {
        public float2 InputPosition;
        public bool PressedDown;

        public ushort ID;
    }

    // use this to track which input to listen to
    // use a for loop to compare input to array index? 
    public struct ReceiveInput : ISharedComponentData
    {
        public ushort ID;
    }

}
