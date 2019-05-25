using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using WDIB.Parameters;

namespace WDIB.Components
{
    public struct WeaponComponent : IComponentData
    {
        public int weaponID;
        public float timeSinceLastShot; // trigger pull
        public float timeSinceLastBurstRound; // time since last bullet released

        public WeaponInput input;

        //public ushort owner; I don't know if we need this
    }

    public struct WeaponInput
    {
        public bool bShoot;
        public bool bReload; 
    }

}