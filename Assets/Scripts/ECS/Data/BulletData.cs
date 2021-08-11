using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts
{
    [GenerateAuthoringComponent]
    public struct BulletData : IComponentData
    {
        public float Speed;
        public float3 CollisionVelocity;
    }
}
