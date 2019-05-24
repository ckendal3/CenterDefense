using UnityEngine;
using WDIB.Utilities;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Barricade Parameters", menuName = "WDIB/Parameters/BarricadeParameters")]
    public class BarricadeParameters : SingletonScriptableObject<BarricadeParameters>
    {
        [Header("Upgradeable Walls")]
        [Tooltip("Sets of data for the barricade sets")]
        public BarricadeData[] barricadeSets;

    }
}