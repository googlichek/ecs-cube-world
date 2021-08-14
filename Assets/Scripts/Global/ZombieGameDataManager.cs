using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Scripts
{
    public class ZombieGameDataManager : MonoBehaviour
    {
        private static ZombieGameDataManager _instance = default;

        [SerializeField]
        private Transform _player = default;

        [SerializeField]
        private List<Transform> _waypoints = new List<Transform>();

        private World _world = default;

        public World World => _world;

        public List<float3> Waypoints { get; private set; }

        public static ZombieGameDataManager Instance => _instance;

        public Transform Player => _player;

        void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            Waypoints = new List<float3>(_waypoints.Count);
            for (var i = 0; i < _waypoints.Count; i++)
            {
                Waypoints.Add(_waypoints[i].position);
            }

            _world = World.DefaultGameObjectInjectionWorld;
        }
    }
}
