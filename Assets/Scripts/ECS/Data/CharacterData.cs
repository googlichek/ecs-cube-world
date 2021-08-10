using Unity.Entities;

namespace Game.Scripts
{
    [GenerateAuthoringComponent]
    public struct CharacterData : IComponentData
    {
        public float Speed;
        public float RotationalSpeed;
    }
}
