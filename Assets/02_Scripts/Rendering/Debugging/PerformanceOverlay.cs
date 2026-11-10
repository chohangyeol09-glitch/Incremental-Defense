using _02_Scripts.Enemy;
using Unity.Entities;
using UnityEngine;

namespace _02_Scripts.Debugging
{
    public class PerformanceOverlay : MonoBehaviour
    {
        [SerializeField] private float refreshInterval = 0.5f;

        private World _world;
        private EntityQuery _enemyQuery;
        private EntityQuery _killStatsQuery;
        private GUIStyle _labelStyle;
        private long _killCount;
        private float _elapsedTime;
        private int _frameCount;
        private float _framesPerSecond;
        private int _enemyCount;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
        }

        private void Start()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            Debug.Assert(_world != null, $"ECS World is null : {gameObject.name}");

            _enemyQuery = _world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<EnemyTag>());
            _killStatsQuery = _world.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<KillStats>());
        }

        private void Update()
        {
            _elapsedTime += Time.unscaledDeltaTime;
            _frameCount++;

            if (_elapsedTime < refreshInterval)
                return;

            _framesPerSecond = _frameCount / _elapsedTime;
            _enemyCount = _enemyQuery.CalculateEntityCount();
            
            _killStatsQuery.CompleteDependency(); 
            if (_killStatsQuery.TryGetSingleton(out KillStats killStats)) 
                _killCount = killStats.TotalKills; 
            
            _elapsedTime = 0f;
            _frameCount = 0;
        }

        private void OnGUI()
        {
            _labelStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 28 };

            float frameMilliseconds = _framesPerSecond > 0f ? 1000f / _framesPerSecond : 0f;

            GUI.Label(new Rect(20f, 20f, 800f, 40f), $"FPS {_framesPerSecond:F1} ({frameMilliseconds:F2} ms)", _labelStyle);
            GUI.Label(new Rect(20f, 60f, 800f, 40f), $"Enemies {_enemyCount:N0}", _labelStyle);
            GUI.Label(new Rect(20f, 100f, 800f, 40f), $"Kills {_killCount:N0}", _labelStyle);
        }

        private void OnDestroy()
        {
            if (_world != null && _world.IsCreated)
            {
                _enemyQuery.Dispose();
                _killStatsQuery.Dispose();
            }
            
        }
    }
}