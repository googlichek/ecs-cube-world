using Unity.Entities;

namespace Game.Scripts
{
    [GenerateAuthoringComponent]
    public struct LifetimeData : IComponentData
    {
        public Entity Entity;
        public float Lifetime;
    }
}
