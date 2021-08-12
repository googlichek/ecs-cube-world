using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts
{
    [GenerateAuthoringComponent]
    public struct BoxTriggerData : IComponentData
    {
        public float3 TriggerEffect;
    }
}
