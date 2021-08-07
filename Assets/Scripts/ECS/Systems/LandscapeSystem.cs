using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Game.Scripts
{
    public class LandscapeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var strength = GameDataManager.Strength;
            var scale = GameDataManager.Scale;

            Dependency =
                Entities
                    .ForEach((ref Translation translation, ref BlockData blockData) =>
                    {
                        var vertex = translation.Value;
                        var perlin1 = Mathf.PerlinNoise(vertex.x * scale, vertex.z * scale) * strength;

                        translation.Value = new float3(vertex.x, perlin1, vertex.z);
                    })
                    .Schedule(Dependency);

            Dependency.Complete();
        }
    }
}
