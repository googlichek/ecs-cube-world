using Unity.Entities;

namespace Game.Scripts
{
    [GenerateAuthoringComponent]
    public struct ZombieData : IComponentData
    {
        public float Speed;
        public float RotationalSpeed;
        public int CurrentWaypoint;
    }
}
