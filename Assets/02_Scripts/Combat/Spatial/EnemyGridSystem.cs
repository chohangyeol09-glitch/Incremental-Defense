using _02_Scripts.Enemy;
using _02_Scripts.Movement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace _02_Scripts.Combat.Spatial
{
    [UpdateAfter(typeof(MoveSystem))]
    [UpdateBefore(typeof(EnemyDeathSystem))]
    public partial struct EnemyGridSystem : ISystem
    {
        private const float CellSize = 1f;

        private EntityQuery _enemyQuery;
        private NativeArray<int> _cellStarts;
        private NativeList<EnemyGridEntry> _entries;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LaneConfig>();

            _enemyQuery = SystemAPI.QueryBuilder().WithAll<EnemyTag, Position2D>().Build();
            state.EntityManager.CreateSingleton<EnemyGrid>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (_cellStarts.IsCreated)
                _cellStarts.Dispose();

            if (_entries.IsCreated)
                _entries.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ref EnemyGrid grid = ref SystemAPI.GetSingletonRW<EnemyGrid>().ValueRW;

            if (!_cellStarts.IsCreated)
                InitializeGrid(ref grid, SystemAPI.GetSingleton<LaneConfig>());

            NativeList<Entity> entities = _enemyQuery.ToEntityListAsync(
                state.WorldUpdateAllocator, state.Dependency, out JobHandle entitiesHandle);
            NativeList<Position2D> positions = _enemyQuery.ToComponentDataListAsync<Position2D>(
                state.WorldUpdateAllocator, state.Dependency, out JobHandle positionsHandle);

            var job = new BuildEnemyGridJob
            {
                Entities = entities.AsDeferredJobArray(),
                Positions = positions.AsDeferredJobArray(),
                Grid = grid
            };

            state.Dependency = job.Schedule(JobHandle.CombineDependencies(entitiesHandle, positionsHandle));
        }

        private void InitializeGrid(ref EnemyGrid grid, in LaneConfig lane)
        {
            var min = new float2(math.min(lane.SpawnX, lane.BaseX), lane.MinY);
            var max = new float2(math.max(lane.SpawnX, lane.BaseX), lane.MaxY);
            int2 dimensions = math.max((int2)math.ceil((max - min) / CellSize), new int2(1, 1));

            _cellStarts = new NativeArray<int>(dimensions.x * dimensions.y + 1, Allocator.Persistent);
            _entries = new NativeList<EnemyGridEntry>(1024, Allocator.Persistent);

            grid.Origin = min;
            grid.InverseCellSize = 1f / CellSize;
            grid.Dimensions = dimensions;
            grid.CellStarts = _cellStarts;
            grid.Entries = _entries;
        }
    }
}