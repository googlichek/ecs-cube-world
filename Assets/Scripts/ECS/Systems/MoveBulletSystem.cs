using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Game.Scripts
{
    public class MoveBulletSystem : SystemBase
    {
        private EntityQuery _query = default;

        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities
                .ForEach((
                    ref PhysicsVelocity physicsVelocity,
                    ref Translation translation,
                    ref Rotation rotation,
                    ref BulletData bulletData) =>
                {
                    physicsVelocity.Angular = float3.zero;
                    physicsVelocity.Linear += deltaTime * bulletData.Speed * math.forward(rotation.Value);
                })
                .Schedule(Dependency)
                .Complete();
        }
    }
}
