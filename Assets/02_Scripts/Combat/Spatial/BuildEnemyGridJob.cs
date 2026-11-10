using _02_Scripts.Movement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace _02_Scripts.Combat.Spatial
{
    [BurstCompile]
    public struct BuildEnemyGridJob : IJob
    {
        [ReadOnly] public NativeArray<Entity> Entities;
        [ReadOnly] public NativeArray<Position2D> Positions;
        public EnemyGrid Grid;

        public void Execute()
        {
            int cellCount = Grid.Dimensions.x * Grid.Dimensions.y;
            int entryCount = Entities.Length;
            NativeArray<int> cellStarts = Grid.CellStarts;
            NativeList<EnemyGridEntry> entries = Grid.Entries;

            for (int cell = 0; cell < cellCount; cell++)
                cellStarts[cell] = 0;

            for (int i = 0; i < entryCount; i++)
            {
                int cell = Grid.GetCellIndex(Grid.GetCell(Positions[i].Value));
                cellStarts[cell] = cellStarts[cell] + 1;
            }

            for (int cell = 1; cell < cellCount; cell++)
                cellStarts[cell] = cellStarts[cell] + cellStarts[cell - 1];

            cellStarts[cellCount] = entryCount;
            entries.ResizeUninitialized(entryCount);

            for (int i = 0; i < entryCount; i++)
            {
                int cell = Grid.GetCellIndex(Grid.GetCell(Positions[i].Value));
                int index = cellStarts[cell] - 1;
                cellStarts[cell] = index;

                entries[index] = new EnemyGridEntry
                {
                    Entity = Entities[i],
                    Position = Positions[i].Value
                };
            }
        }
    }
}