using Unity.Entities;

namespace _02_Scripts.Enemy
{
    public struct EnemySpawnConfig : IComponentData
    {
        public float SpawnPerSecond;
        public int MaxCount;
        public float MinSpeed;
        public float MaxSpeed;
    }
}