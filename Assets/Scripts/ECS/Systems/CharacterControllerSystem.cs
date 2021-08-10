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
            var shoot = Input.GetAxis("Fire1");

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

            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((
                    ref PhysicsVelocity physicsVelocity,
                    ref Translation translation,
                    ref Rotation rotation,
                    ref CharacterData characterData) =>
                {
                    if (shoot > 0)
                    {
                        var instance = EntityManager.Instantiate(characterData.BulletEntityTemplate);
                        var offset = new float3(UnityEngine.Random.Range(-1, 2), 0, 1);

                        EntityManager.SetComponentData(instance, new Translation() { Value = translation.Value + math.mul(rotation.Value, offset) });
                        EntityManager.SetComponentData(instance, new Rotation() { Value = rotation.Value });
                        EntityManager.SetComponentData(instance, new LifetimeData() { Entity = instance, Lifetime = 5 });
                    }
                })
                .Run();
        }
    }
}
