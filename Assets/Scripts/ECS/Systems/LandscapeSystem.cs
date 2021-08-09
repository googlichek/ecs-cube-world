using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Game.Scripts
{
    [DisableSystemOnStart]
    public class LandscapeSystem : SystemBase
    {
        private EntityQuery _query = default;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(typeof(BlockData));
        }

        protected override void OnUpdate()
        {
            var strength1 = GameDataManager.Strength1;
            var strength2 = GameDataManager.Strength2;
            var strength3 = GameDataManager.Strength3;
            var scale1 = GameDataManager.Scale1;
            var scale2 = GameDataManager.Scale2;
            var scale3 = GameDataManager.Scale3;

            float3 offset = GameDataManager.PlayerPosition;

            Entities
                .ForEach((ref Translation translation, ref BlockData blockData) =>
                {
                    var vertex = blockData.InitialPosition + offset;

                    var perlin1 = Mathf.PerlinNoise(vertex.x * scale1, vertex.z * scale1) * strength1;
                    var perlin2 = Mathf.PerlinNoise(vertex.x * scale2, vertex.z * scale2) * strength2;
                    var perlin3 = Mathf.PerlinNoise(vertex.x * scale3, vertex.z * scale3) * strength3;

                    var height = perlin1 + perlin2 + perlin3;

                    translation.Value = new float3(vertex.x, height, vertex.z);
                })
                .Schedule(Dependency)
                .Complete();

            if (!GameDataManager.HasDataChanged)
                return;

            using (var blockEntities = _query.ToEntityArray(Allocator.TempJob))
            {
                foreach (var entity in blockEntities)
                {
                    var height = EntityManager.GetComponentData<Translation>(entity).Value.y;

                    Entity block;
                    if (height <= GameDataManager.DirtLevel)
                        block = GameDataManager.DirtEntity;
                    else if (height <= GameDataManager.GrassLevel)
                        block = GameDataManager.GrassEntity;
                    else if (height <= GameDataManager.RockLevel)
                        block = GameDataManager.RockEntity;
                    else if (height <= GameDataManager.SnowLevel)
                        block = GameDataManager.SnowEntity;
                    else
                        block = GameDataManager.SandEntity;

                    var colorRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(block);
                    var entityRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                    entityRenderMesh.material = colorRenderMesh.material;
                    EntityManager.SetSharedComponentData(entity, entityRenderMesh);
                }
            }

            GameDataManager.HasDataChanged = false;
        }
    }
}
