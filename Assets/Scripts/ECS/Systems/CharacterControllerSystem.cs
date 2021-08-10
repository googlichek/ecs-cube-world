using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Game.Scripts
{
    public class CharacterControllerSystem : SystemBase
    {
        private EntityQuery _query = default;

        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            var inputY = Input.GetAxis("Horizontal");
            var inputZ = Input.GetAxis("Vertical");

            Entities
                .ForEach((ref PhysicsVelocity physicsVelocity, ref Rotation rotation, ref CharacterData characterData) =>
                {
                    if (inputZ == 0)
                        physicsVelocity.Linear = float3.zero;
                    else
                        physicsVelocity.Linear += inputZ * deltaTime * characterData.Speed * math.forward(rotation.Value);
                    
                    rotation.Value =
                        math.mul(math.normalize(rotation.Value), quaternion.AxisAngle(math.up(), deltaTime * inputY * characterData.RotationalSpeed));
                })
                .Schedule(Dependency)
                .Complete();
        }
    }
}
