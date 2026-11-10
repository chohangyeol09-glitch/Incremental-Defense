using Unity.Entities;
using UnityEngine;

namespace _02_Scripts.Rendering
{
    public class SpriteRendererAuthoring : MonoBehaviour
    {
        [SerializeField] private Material material;

        private class SpriteRendererAuthoringBaker : Baker<SpriteRendererAuthoring>
        {
            public override void Bake(SpriteRendererAuthoring authoring)
            {
                Material materialData = DependsOn(authoring.material);

                Debug.Assert(materialData != null, $"Material is null : {authoring.gameObject.name}");

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new SpriteRenderConfig
                {
                    Material = materialData
                });
            }
        }
    }
}