using Unity.Entities;

namespace _02_Scripts.Enemy
{
    public struct KillStats : IComponentData
    {
        public long TotalKills;
    }
}