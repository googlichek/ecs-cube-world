using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Game.Scripts
{
    public class ECSManagerDuck : MonoBehaviour
    {
        [SerializeField]
        private GameObject _playerPrefab = default;

        [SerializeField]
        private GameObject _bulletPrefab = default;

        private World _world = default;

        private BlobAssetStore _store = default;

        void Awake()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _store = new BlobAssetStore();
            var settings = GameObjectConversionSettings.FromWorld(_world, _store);

            var playerEntityTemplate = GameObjectConversionUtility.ConvertGameObjectHierarchy(_playerPrefab, settings);
            var bulletEntityTemplate = GameObjectConversionUtility.ConvertGameObjectHierarchy(_bulletPrefab, settings);

            var player = _world.EntityManager.Instantiate(playerEntityTemplate);
            _world.EntityManager.SetComponentData(player, new Translation() { Value = new float3(0, 2, 0) });
            _world.EntityManager.SetComponentData(player, new CharacterData() { Speed = 5, BulletEntityTemplate = bulletEntityTemplate });
        }

        void OnDestroy()
        {
            _store.Dispose();
        }
    }
}
