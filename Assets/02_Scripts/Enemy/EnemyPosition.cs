using Unity.Entities;
using Unity.Mathematics;

namespace _02_Scripts.Enemy
{
    public struct EnemyPosition : IComponentData
    {
        public float2 Value;
    }
}