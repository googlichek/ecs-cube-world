using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Game.Scripts
{
    public class MoveSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var waypoints = new NativeArray<float3>(ZombieGameDataManager.Instance.Waypoints.ToArray(), Allocator.TempJob);
            var deltaTime = Time.DeltaTime;

            var nextWaypoint = UnityEngine.Random.Range(0, ZombieGameDataManager.Instance.Waypoints.Count);

            Entities
                .ForEach((
                    ref PhysicsVelocity velocity,
                    ref PhysicsMass mass,
                    ref Translation translation,
                    ref Rotation rotation,
                    ref ZombieData zombieData) =>
                {
                    var distance = math.distance(translation.Value, waypoints[zombieData.CurrentWaypoint]);
                    if (distance < 5)
                    {
                        zombieData.CurrentWaypoint = nextWaypoint;
                        if (zombieData.CurrentWaypoint >= waypoints.Length)
                            zombieData.CurrentWaypoint = 0;
                    }

                    var direction = waypoints[zombieData.CurrentWaypoint] - translation.Value;

                    var lookRotation = quaternion.LookRotation(direction, math.up());
                    rotation.Value = math.slerp(rotation.Value, lookRotation.value, deltaTime * zombieData.RotationalSpeed);

                    velocity.Linear = zombieData.Speed * math.forward(rotation.Value) * deltaTime;

                    mass.InverseInertia[0] = 0;
                    mass.InverseInertia[1] = 0;
                    mass.InverseInertia[2] = 0;
                })
                .Schedule(Dependency)
                .Complete();

            waypoints.Dispose(Dependency);
        }
    }
}
