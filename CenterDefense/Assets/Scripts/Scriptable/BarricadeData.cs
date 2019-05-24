using UnityEngine;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Barricade Data", menuName = "WDIB/Data/Barricade")]
    public class BarricadeData : ScriptableObject
    {
        [Tooltip("Name of the Barricade")]
        public string barricadeName = "Default";

        [Tooltip("Max health of the barricade.")]
        [Range(1, 1000)]
        public float maxHealth = 200;

        [Tooltip("Cost to upgrade to this barricade upgrade.")]
        [Range(1, 1000)]
        public int upgradeCost = 10;

        // Visual settings
        public Mesh mesh;
        public Material material;
    }
}