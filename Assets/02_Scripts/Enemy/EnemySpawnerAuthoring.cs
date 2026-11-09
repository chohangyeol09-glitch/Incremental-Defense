using _02_Scripts.Map;
using Unity.Entities;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private LaneSO lane;
        [SerializeField] private EnemySpawnSO spawn;
        
        private class EnemySpawnerAuthoringBaker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                LaneSO laneData = DependsOn(authoring.lane);
                EnemySpawnSO spawnData = DependsOn(authoring.spawn);
                
                Debug.Assert(laneData != null, $"LaneSO is null : {authoring.gameObject.name}");
                Debug.Assert(spawnData != null, $"EnemySpawnSO is null : {authoring.gameObject.name}");

                var entity = GetEntity(TransformUsageFlags.None);
                
                AddComponent(entity, new LaneConfig
                {
                    SpawnX = laneData.SpawnX,
                    BaseX = laneData.BaseX,
                    MinY = laneData.MinY,
                    MaxY = laneData.MaxY
                });
                
                AddComponent(entity, new EnemySpawnConfig
                {
                    SpawnPerSecond = spawnData.SpawnPerSecond,
                    MaxCount =  spawnData.MaxCount,
                    MinSpeed = spawnData.MinSpeed,
                    MaxSpeed = spawnData.MaxSpeed,
                });
            }
        }
    }
}