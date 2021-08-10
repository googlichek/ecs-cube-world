using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class DuckSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _duckPrefab = default;

        [Space] [SerializeField] [Range(0, 10000)]
        private int _maxDucks = 0;

        private World _world = default;

        private BlobAssetStore _store = default;

        void Awake()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _store = new BlobAssetStore();
            var settings = GameObjectConversionSettings.FromWorld(_world, _store);
            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_duckPrefab, settings);

            for (var i = 0; i < _maxDucks; i++)
            {
                var instance = _world.EntityManager.Instantiate(entity);

                var x = Random.Range(-200, 200);
                var y = Random.Range(50, 200);
                var z = Random.Range(-200, 200);

                _world.EntityManager.SetComponentData(instance, new Translation() { Value = new float3(x, y, z) });
            }
        }

        void OnDestroy()
        {
            _store.Dispose();
        }
    }
}
