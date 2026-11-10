using _02_Scripts.Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace _02_Scripts.Enemy
{
    [BurstCompile]
    [UpdateAfter(typeof(MoveSystem))]
    public partial struct EnemyBaseArrivalSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LaneConfig>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var lane = SystemAPI.GetSingleton<LaneConfig>();
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new EnemyBaseArrivalJob
            {
                BaseX = lane.BaseX,
                Direction = math.sign(lane.BaseX - lane.SpawnX),
                Ecb = ecb.AsParallelWriter()
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
}