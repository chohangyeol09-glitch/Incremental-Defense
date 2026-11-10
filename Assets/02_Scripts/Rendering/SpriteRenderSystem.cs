using _02_Scripts.Enemy;
using _02_Scripts.Movement;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace _02_Scripts.Rendering
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class SpriteRenderSystem : SystemBase
    {
        private const int PositionStride = sizeof(float) * 2;
        private const int MinBufferCapacity = 1024;

        private static readonly int PositionsId = Shader.PropertyToID("_Positions");
        private static readonly Bounds RenderBounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

        private EntityQuery _spriteQuery;
        private Mesh _quadMesh;
        private GraphicsBuffer _positionBuffer;
        private MaterialPropertyBlock _propertyBlock;

        protected override void OnCreate()
        {
            RequireForUpdate<SpriteRenderConfig>();

            _spriteQuery = SystemAPI.QueryBuilder().WithAll<Position2D, EnemyTag>().Build();
            _quadMesh = CreateQuadMesh();
            _propertyBlock = new MaterialPropertyBlock();
        }

        protected override void OnDestroy()
        {
            _positionBuffer?.Release();
            CoreUtils.Destroy(_quadMesh);
        }

        protected override void OnUpdate()
        {
            int spriteCount = _spriteQuery.CalculateEntityCount();
            if (spriteCount == 0)
                return;

            var config = SystemAPI.GetSingleton<SpriteRenderConfig>();
            Material material = config.Material.Value;

            EnsureBufferCapacity(spriteCount);

            var positions = _spriteQuery.ToComponentDataArray<Position2D>(Allocator.TempJob);
            _positionBuffer.SetData(positions);
            positions.Dispose();

            _propertyBlock.SetBuffer(PositionsId, _positionBuffer);

            var renderParams = new RenderParams(material)
            {
                worldBounds = RenderBounds,
                matProps = _propertyBlock
            };

            Graphics.RenderMeshPrimitives(renderParams, _quadMesh, 0, spriteCount);
        }

        private void EnsureBufferCapacity(int requiredCount)
        {
            if (_positionBuffer != null && _positionBuffer.count >= requiredCount)
                return;

            _positionBuffer?.Release();

            int capacity = math.max(MinBufferCapacity, math.ceilpow2(requiredCount));
            _positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, capacity, PositionStride);
        }

        private static Mesh CreateQuadMesh()
        {
            var mesh = new Mesh { name = "SpriteQuad" };

            mesh.SetVertices(new[]
            {
                new Vector3(-0.5f, 0f, 0f),
                new Vector3(0.5f, 0f, 0f),
                new Vector3(-0.5f, 1f, 0f),
                new Vector3(0.5f, 1f, 0f)
            });

            mesh.SetUVs(0, new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f)
            });

            mesh.SetTriangles(new[] { 0, 2, 1, 2, 3, 1 }, 0);

            return mesh;
        }
    }
}