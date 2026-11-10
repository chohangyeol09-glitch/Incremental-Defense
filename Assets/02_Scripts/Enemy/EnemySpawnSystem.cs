using _02_Scripts.Combat;
using _02_Scripts.Movement;
using _02_Scripts.Rendering;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace _02_Scripts.Enemy
{
    [BurstCompile]
    [UpdateBefore(typeof(MoveSystem))]
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

            var componentTypes = new NativeArray<ComponentType>(5, Allocator.Temp);
            componentTypes[0] = ComponentType.ReadWrite<Position2D>();
            componentTypes[1] = ComponentType.ReadWrite<Velocity2D>();
            componentTypes[2] = ComponentType.ReadWrite<EnemyTag>();
            componentTypes[3] = ComponentType.ReadWrite<SpriteAnimationOffset>(); 
            componentTypes[4] = ComponentType.ReadWrite<Health>();
            _enemyArchetype = state.EntityManager.CreateArchetype(componentTypes);

            _enemyQuery = SystemAPI.QueryBuilder().WithAll<EnemyTag>().Build();
            _random = Random.CreateFromIndex(0);
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

            float direction = math.sign(lane.BaseX - lane.SpawnX);
            
            var entities = new NativeArray<Entity>(spawnCount, Allocator.Temp);
            state.EntityManager.CreateEntity(_enemyArchetype, entities);

            for (int i = 0; i < entities.Length; i++)
            {
                state.EntityManager.SetComponentData(entities[i], new Position2D
                {
                    Value = new float2(lane.SpawnX, _random.NextFloat(lane.MinY, lane.MaxY))
                });

                float speed = _random.NextFloat(spawn.MinSpeed, spawn.MaxSpeed);
                state.EntityManager.SetComponentData(entities[i], new Velocity2D
                {
                    Value = new float2(direction * speed, 0f)
                });
                
                state.EntityManager.SetComponentData(entities[i], new SpriteAnimationOffset 
                {                                                                          
                    Value = _random.NextFloat()                                            
                });                                                                        
                
                state.EntityManager.SetComponentData(entities[i], new Health
                {
                    Value = spawn.MaxHealth
                });
            }
        }
    }
}