using Unity.Entities;

namespace Game.Scripts
{
    public static class GameDataManager
    {
        public static float Strength1;
        public static float Strength2;
        public static float Strength3;
        public static float Scale1;
        public static float Scale2;
        public static float Scale3;

        public static float SandLevel = 0;
        public static float DirtLevel = 0;
        public static float GrassLevel = 0;
        public static float RockLevel = 0;
        public static float SnowLevel = 0;

        public static Entity SandEntity;
        public static Entity DirtEntity;
        public static Entity GrassEntity;
        public static Entity RockEntity;
        public static Entity SnowEntity;

        public static bool HasDataChanged;
    }
}
