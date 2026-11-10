using Unity.Burst;
using Unity.Entities;

namespace _02_Scripts.Movement
{
    public partial struct MoveSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new MoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
}