using UnityEngine;

namespace _02_Scripts.Enemy
{
    [CreateAssetMenu(fileName = "Enemy Spawn", menuName = "Enemy/Enemy spawn data", order = 0)]
    public class EnemySpawnSO : ScriptableObject
    {
        [field: SerializeField] public float SpawnPerSecond { get; private set; } = 2500f;
        [field: SerializeField] public int MaxCount { get; private set; } = 50000;
        [field: SerializeField] public float MinSpeed { get; private set; } = 2f;
        [field: SerializeField] public float MaxSpeed { get; private set; } = 3f; 
        [field: SerializeField] public float MaxHealth { get; private set; } = 10f;
    }
}