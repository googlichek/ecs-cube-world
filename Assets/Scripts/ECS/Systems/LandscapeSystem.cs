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
            var strength1 = GameDataManager.Strength1;
            var strength2 = GameDataManager.Strength2;
            var strength3 = GameDataManager.Strength3;
            var scale1 = GameDataManager.Scale1;
            var scale2 = GameDataManager.Scale2;
            var scale3 = GameDataManager.Scale3;

            Entities
                .ForEach((ref Translation translation, ref BlockData blockData) =>
                {
                    var vertex = translation.Value;

                    var perlin1 = Mathf.PerlinNoise(vertex.x * scale1, vertex.z * scale1) * strength1;
                    var perlin2 = Mathf.PerlinNoise(vertex.x * scale2, vertex.z * scale2) * strength2;
                    var perlin3 = Mathf.PerlinNoise(vertex.x * scale3, vertex.z * scale3) * strength3;

                    var height = perlin1 + perlin2 + perlin3;

                    translation.Value = new float3(vertex.x, height, vertex.z);
                })
                .Schedule();
        }
    }
}
