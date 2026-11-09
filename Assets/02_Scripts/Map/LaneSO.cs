using UnityEngine;

namespace _02_Scripts.Map
{
    [CreateAssetMenu(fileName = "Lane", menuName = "Map/Lane data", order = 0)]
    public class LaneSO : ScriptableObject
    {
        [field: SerializeField] public float SpawnX { get; private set; } = 30f;
        [field: SerializeField] public float BaseX { get; private set; } = -30f;
        [field: SerializeField] public float MinY { get; private set; } = -8f;
        [field: SerializeField] public float MaxY { get; private set; } = 8f;
    }
}