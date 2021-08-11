using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Game.Scripts
{
    [UpdateAfter(typeof(LifetimeControlSystem))]
    public class BulletCollisionEventSystem : SystemBase
    {
        private BuildPhysicsWorld _physicsWorld = default;
        private StepPhysicsWorld _stepWorld = default;

        private struct CollisionEventJob : ICollisionEventsJob
        {
            [ReadOnly]
            public ComponentDataFromEntity<BulletData> BulletGroup;

            public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;

            public void Execute(CollisionEvent collisionEvent)
            {
                var entityA = collisionEvent.EntityA;
                var entityB = collisionEvent.EntityB;

                var isEtityATarget = PhysicsVelocityGroup.HasComponent(entityA);
                var isEtityBTarget = PhysicsVelocityGroup.HasComponent(entityB);

                var isEntityABullet = BulletGroup.HasComponent(entityA);
                var isEntityBBullet = BulletGroup.HasComponent(entityB);

                if (isEntityABullet && isEtityBTarget)
                {
                    var velocityComponent = PhysicsVelocityGroup[entityA];
                    velocityComponent.Linear = new float3(0, 50, 0);

                    PhysicsVelocityGroup[entityB] = velocityComponent;
                }

                if (isEntityBBullet && isEtityATarget)
                {
                    var velocityComponent = PhysicsVelocityGroup[entityB];
                    velocityComponent.Linear = new float3(0, 50, 0);

                    PhysicsVelocityGroup[entityB] = velocityComponent;
                }
            }
        }

        protected override void OnCreate()
        {
            _physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        protected override void OnUpdate()
        {
            var jobHandle = new CollisionEventJob()
            {
                BulletGroup = GetComponentDataFromEntity<BulletData>(),
                PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>()
            };

            Dependency = jobHandle.Schedule(_stepWorld.Simulation, ref _physicsWorld.PhysicsWorld, Dependency);
            Dependency.Complete();
        }
    }
}
