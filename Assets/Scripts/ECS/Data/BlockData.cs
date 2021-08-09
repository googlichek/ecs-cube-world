using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts
{
    [GenerateAuthoringComponent]
    public struct BlockData : IComponentData
    {
        public float3 InitialPosition;
    }
}
