using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class ZombieECSManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _zombiePrefab = default;

        [SerializeField] [Range(0, 10000)]
        private int _zombiesCount = 0;

        private World _world = default;

        private BlobAssetStore _store = default;

        void Start()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _store = new BlobAssetStore();
            var settings = GameObjectConversionSettings.FromWorld(_world, _store);

            var template = GameObjectConversionUtility.ConvertGameObjectHierarchy(_zombiePrefab, settings);

            for (var i = 0; i < _zombiesCount; i++)
            {
                var zombie = _world.EntityManager.Instantiate(template);

                var x = Random.Range(-300, 300);
                var y = 0.5f;
                var z = Random.Range(-300, 300);

                var position = transform.TransformPoint(new float3(x, y, z));
                _world.EntityManager.SetComponentData(zombie, new Translation() { Value = position });

                var closestWaypoint = 0;
                var distance = Mathf.Infinity;
                for (var j = 0; j < ZombieGameDataManager.Instance.Waypoints.Count; j++)
                {
                    {
                        var tempDistance = Vector3.Distance(ZombieGameDataManager.Instance.Waypoints[j], position);
                        if (tempDistance < distance)
                        {
                            closestWaypoint = j;
                            distance = tempDistance;
                        }
                    }
                }

                var speed = Random.Range(50, 200);
                var rotationalSpeed = Random.Range(1, 5);
                _world.EntityManager.SetComponentData(zombie, new ZombieData() { Speed = speed, RotationalSpeed = rotationalSpeed, CurrentWaypoint = closestWaypoint });
            }
        }

        void OnDestroy()
        {
            _store.Dispose();
        }
    }
}
