using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace _02_Scripts.Combat.Spatial
{
    public struct EnemyGrid : IComponentData
    {
        public float2 Origin;
        public float InverseCellSize;
        public int2 Dimensions;
        public NativeArray<int> CellStarts;
        public NativeList<EnemyGridEntry> Entries;

        public int2 GetCell(float2 position)
            => math.clamp((int2)math.floor((position - Origin) * InverseCellSize), int2.zero, Dimensions - 1);

        public int GetCellIndex(int2 cell) => cell.y * Dimensions.x + cell.x;
    }
}