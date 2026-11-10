using _02_Scripts.Combat;
using _02_Scripts.Movement;
using Unity.Burst;
using Unity.Entities;

namespace _02_Scripts.Enemy
{
    [BurstCompile]
    [WithAll(typeof(EnemyTag))]
    public partial struct EnemyBaseArrivalJob : IJobEntity
    {
        public float BaseX;
        public float Direction;
        public EntityCommandBuffer.ParallelWriter Ecb;

        private void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Position2D position, in Health health)
        {
            if (health.Value <= 0f) 
                return; 

            bool hasReachedBase = (BaseX - position.Value.x) * Direction <= 0f;
            if (hasReachedBase)
                Ecb.DestroyEntity(chunkIndex, entity);
        }
    }
}