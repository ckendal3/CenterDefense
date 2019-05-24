using UnityEngine;
using WDIB.Utilities;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Turret Parameters", menuName = "WDIB/Parameters/TurretParameters")]
    public class TurretParameters : SingletonScriptableObject<TurretParameters>
    {
        [Header("Upgradeable Turret")]
        [Tooltip("Sets of data for the Turret")]
        public TurretData[] turretSets;

    }
}