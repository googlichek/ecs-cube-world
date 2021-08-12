using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Game.Scripts
{
    public class BoxTriggerSystem : SystemBase
    {
        private BuildPhysicsWorld _physicsWorld = default;
        private StepPhysicsWorld _stepWorld = default;

        private struct BoxTriggerJob : ITriggerEventsJob
        {
            [ReadOnly]
            public ComponentDataFromEntity<BoxTriggerData> BoxTriggerDataGroup;

            public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                var entityA = triggerEvent.EntityA;
                var entityB = triggerEvent.EntityB;

                var isBodyATrigger = BoxTriggerDataGroup.HasComponent(entityA);
                var isBodyBTrigger = BoxTriggerDataGroup.HasComponent(entityB);

                if (isBodyATrigger && isBodyBTrigger)
                    return;

                var isBodyADynamic = PhysicsVelocityGroup.HasComponent(entityA);
                var isBodyBDynamic = PhysicsVelocityGroup.HasComponent(entityB);

                if (isBodyATrigger && !isBodyBDynamic ||
                    isBodyBTrigger && !isBodyADynamic)
                    return;

                var triggerEntity = isBodyATrigger ? entityA : entityB;
                var dynamicEntity = isBodyATrigger ? entityB : entityA;

                var component = PhysicsVelocityGroup[dynamicEntity];
                component.Linear += new float3(0, 250, 0);

                PhysicsVelocityGroup[dynamicEntity] = component;
            }
        }

        protected override void OnCreate()
        {
            _physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        protected override void OnUpdate()
        {
            var job = new BoxTriggerJob()
            {
                BoxTriggerDataGroup = GetComponentDataFromEntity<BoxTriggerData>(),
                PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>()
            };

            Dependency = job.Schedule(_stepWorld.Simulation, ref _physicsWorld.PhysicsWorld, Dependency);
            Dependency.Complete();
        }
    }
}
