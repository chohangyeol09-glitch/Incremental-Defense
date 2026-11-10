using Unity.Burst;
using Unity.Entities;

namespace _02_Scripts.Movement
{
    [BurstCompile]
    public partial struct MoveJob : IJobEntity
    {
        public float DeltaTime;
        
        private void Execute(ref Position2D position, in Velocity2D velocity)
            => position.Value += velocity.Value * DeltaTime;
    }
}