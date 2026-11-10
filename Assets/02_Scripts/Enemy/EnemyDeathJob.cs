using _02_Scripts.Combat;
using Unity.Burst;
using Unity.Entities;

namespace _02_Scripts.Enemy
{
    [BurstCompile]
    [WithAll(typeof(EnemyTag))]
    public partial struct EnemyDeathJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;
        public Entity StatsEntity;
        public ComponentLookup<KillStats> StatsLookup;

        private void Execute(Entity entity, in Health health)
        {
            if (health.Value > 0f)
                return;

            Ecb.DestroyEntity(entity);
            StatsLookup.GetRefRW(StatsEntity).ValueRW.TotalKills++;
        }
    }
}