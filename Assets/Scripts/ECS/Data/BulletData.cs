using Unity.Entities;

namespace Game.Scripts
{
    [GenerateAuthoringComponent]
    public struct BulletData : IComponentData
    {
        public float Speed;
    }
}
