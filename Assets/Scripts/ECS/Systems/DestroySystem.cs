using Unity.Entities;

namespace Game.Scripts
{
    [UpdateAfter(typeof(MoveBulletSystem))]
    public class DestroySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entity, ref DestroyData destroyData) =>
                {
                    if (destroyData.ShouldDestroy)
                    {
                        EntityManager.DestroyEntity(entity);
                    }
                })
                .Run();
        }
    }
}
