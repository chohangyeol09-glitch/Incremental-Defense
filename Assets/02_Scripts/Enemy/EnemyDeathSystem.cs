using _02_Scripts.Movement;
using Unity.Burst;
using Unity.Entities;

namespace _02_Scripts.Enemy
{
    [UpdateAfter(typeof(MoveSystem))]
    public partial struct EnemyDeathSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.EntityManager.CreateSingleton<KillStats>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            var job = new EnemyDeathJob
            {
                Ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
                StatsEntity = SystemAPI.GetSingletonEntity<KillStats>(),
                StatsLookup = SystemAPI.GetComponentLookup<KillStats>()
            };

            state.Dependency = job.Schedule(state.Dependency);
        }
    }
}