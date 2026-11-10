using Unity.Entities;
using UnityEngine;

namespace _02_Scripts.Rendering
{
    public struct SpriteRenderConfig : IComponentData
    {
        public UnityObjectRef<Material> Material;
    }
}