using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace _02_Scripts.Enemy
{
    [BurstCompile]
    public partial struct EnemySpawnSystem : ISystem
    {
        private EntityArchetype _enemyArchetype;
        private EntityQuery _enemyQuery;
        private Random _random;
        private float _pendingSpawnCount;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LaneConfig>();
            state.RequireForUpdate<EnemySpawnConfig>();
            
            NativeArray<ComponentType> componentTypes = new NativeArray<ComponentType>(2, Allocator.Temp);
            componentTypes[0] = ComponentType.ReadWrite<EnemyPosition>();
            componentTypes[1] = ComponentType.ReadWrite<EnemySpeed>();
            _enemyArchetype = state.EntityManager.CreateArchetype(componentTypes);
            
            _enemyArchetype = state.EntityManager.CreateArchetype(componentTypes);

            _enemyQuery = SystemAPI.QueryBuilder().WithAll<EnemyPosition>().Build();
            _random = new Random(0x6E624EB7u);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var lane = SystemAPI.GetSingleton<LaneConfig>();
            var spawn = SystemAPI.GetSingleton<EnemySpawnConfig>();

            _pendingSpawnCount += spawn.SpawnPerSecond * SystemAPI.Time.DeltaTime;
            int requestedCount = (int)_pendingSpawnCount;
            _pendingSpawnCount -= requestedCount;

            int remainingCapacity = spawn.MaxCount - _enemyQuery.CalculateEntityCount();
            int spawnCount = math.min(requestedCount, remainingCapacity);
            if (spawnCount <= 0)
                return;

            var entities = new NativeArray<Entity>(spawnCount, Allocator.Temp);
            state.EntityManager.CreateEntity(_enemyArchetype, entities);

            for (int i = 0; i < entities.Length; i++)
            {
                state.EntityManager.SetComponentData(entities[i], new EnemyPosition
                {
                    Value = new float2(lane.SpawnX, _random.NextFloat(lane.MinY, lane.MaxY))
                });

                state.EntityManager.SetComponentData(entities[i], new EnemySpeed
                {
                    Value = _random.NextFloat(spawn.MinSpeed, spawn.MaxSpeed)
                });
            }
        }
    }
}