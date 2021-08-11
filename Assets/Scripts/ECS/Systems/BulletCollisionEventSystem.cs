using Unity.Collections;
using Unity.Entities;
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

            public ComponentDataFromEntity<DestroyData> DestroyGroup;

            public void Execute(CollisionEvent collisionEvent)
            {
                var entityA = collisionEvent.EntityA;
                var entityB = collisionEvent.EntityB;

                var isEntityATarget = DestroyGroup.HasComponent(entityA);
                var isEntityBTarget = DestroyGroup.HasComponent(entityB);

                var isEntityABullet = BulletGroup.HasComponent(entityA);
                var isEntityBBullet = BulletGroup.HasComponent(entityB);

                if (isEntityABullet && isEntityBTarget)
                {
                    var component = DestroyGroup[entityB];
                    component.ShouldDestroy = true;

                    DestroyGroup[entityB] = component;
                }

                if (isEntityBBullet && isEntityATarget)
                {
                    var component = DestroyGroup[entityA];
                    component.ShouldDestroy = true;

                    DestroyGroup[entityA] = component;
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
            var job = new CollisionEventJob()
            {
                BulletGroup = GetComponentDataFromEntity<BulletData>(),
                DestroyGroup = GetComponentDataFromEntity<DestroyData>()
            };

            Dependency = job.Schedule(_stepWorld.Simulation, ref _physicsWorld.PhysicsWorld, Dependency);
            Dependency.Complete();
        }
    }
}
