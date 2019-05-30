using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "New Explosive", menuName = "WDIB/Explosive")]
    public class ExplosiveData : ScriptableObject
    {
        public string explosiveName = "Explosive";

        public float radius;
        public float damage;

        public AnimationCurve damageCurve;

        //add components like ExplodeOnImpact, DelayedExplosion?
    }
}
