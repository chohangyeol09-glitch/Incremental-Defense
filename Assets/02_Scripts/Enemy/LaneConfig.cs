using Unity.Entities;

namespace _02_Scripts.Enemy
{
    public struct LaneConfig : IComponentData
    {
        public float SpawnX;
        public float BaseX;
        public float MinY;
        public float MaxY;
    }
}