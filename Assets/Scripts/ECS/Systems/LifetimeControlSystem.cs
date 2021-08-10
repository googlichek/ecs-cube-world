using Unity.Collections;
using Unity.Entities;

namespace Game.Scripts
{
    [UpdateAfter(typeof(MoveBulletSystem))]
    public class LifetimeControlSystem : SystemBase
    {
        [BurstCompatible]
        struct CullingJob : IJobChunk
        {
            public EntityCommandBuffer.ParallelWriter EntityCommandBuffer;

            public ComponentTypeHandle<LifetimeData> LifetimeHandle;

            public float DeltaTime;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var lifetimeData = chunk.GetNativeArray(LifetimeHandle);

                for (var i = 0; i < lifetimeData.Length; i++)
                {
                    var entity = lifetimeData[i].Entity;
                    var remainingLifeTime = lifetimeData[i].Lifetime - DeltaTime;
                    if (remainingLifeTime < 0)
                    {
                        EntityCommandBuffer.DestroyEntity(firstEntityIndex + i, entity);
                    }
                    else
                    {
                        lifetimeData[i] = new LifetimeData { Entity = entity, Lifetime = remainingLifeTime };
                    }
                }
            }
        }

        private EndSimulationEntityCommandBufferSystem _buffer = default;

        private EntityQuery _query = default;

        protected override void OnCreate()
        {
            _buffer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _query = GetEntityQuery(typeof(LifetimeData));
        }

        protected override void OnUpdate()
        {
            var entityCommandBuffer = _buffer.CreateCommandBuffer().AsParallelWriter();

            var job = new CullingJob
            {
                EntityCommandBuffer = entityCommandBuffer,
                LifetimeHandle = GetComponentTypeHandle<LifetimeData>(),
                DeltaTime = Time.DeltaTime
            };

            Dependency = job.Schedule(_query, Dependency);

            _buffer.AddJobHandleForProducer(Dependency);
        }
    }
}
