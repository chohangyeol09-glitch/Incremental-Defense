using Unity.Entities;
using Unity.Mathematics;

namespace _02_Scripts.Movement
{
    public struct Position2D : IComponentData
    {
        public float2 Value;
    }
}