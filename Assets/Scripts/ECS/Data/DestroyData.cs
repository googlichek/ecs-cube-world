using Unity.Entities;

namespace Game.Scripts
{
    [GenerateAuthoringComponent]
    public struct DestroyData : IComponentData
    {
        public bool ShouldDestroy;
    }
}
