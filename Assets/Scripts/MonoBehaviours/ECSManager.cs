using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Game.Scripts
{
    public class ECSManager : MonoBehaviour
    {
        [SerializeField] [Range(0, 1024)]
        private int _worldSize = 0;

        [Space] [SerializeField] [Range(0.1f, 10f)]
        private float _strength1 = 0;
        
        [SerializeField] [Range(0.1f, 10f)]
        private float _strength2 = 0;
        
        [SerializeField] [Range(0.1f, 10f)]
        private float _strength3 = 0;

        [Space] [SerializeField] [Range(0.01f, 1f)]
        private float _scale1 = 0;
        
        [SerializeField] [Range(0.01f, 1f)]
        private float _scale2 = 0;
        
        [SerializeField] [Range(0.01f, 1f)]
        private float _scale3 = 0;

        [Space] [SerializeField] [Range(0, 100)]
        private float _sandLevel = 0;

        [SerializeField] [Range(0, 100)]
        private float _dirtLevel = 0;

        [SerializeField] [Range(0, 100)]
        private float _grassLevel = 0;

        [SerializeField] [Range(0, 100)]
        private float _rockLevel = 0;

        [SerializeField] [Range(0, 100)]
        private float _snowLevel = 0;

        [Space] [SerializeField]
        private GameObject _sandPrefab = default;

        [SerializeField]
        private GameObject _dirtPrefab = default;

        [SerializeField]
        private GameObject _grassPrefab = default;

        [SerializeField]
        private GameObject _rockPrefab = default;

        [SerializeField]
        private GameObject _snowPrefab = default;

        private World _world = default;

        void Awake()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            var settings = GameObjectConversionSettings.FromWorld(_world, null);

            GameDataManager.SandEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_sandPrefab, settings);
            GameDataManager.DirtEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_dirtPrefab, settings);
            GameDataManager.GrassEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_grassPrefab, settings);
            GameDataManager.RockEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_rockPrefab, settings);
            GameDataManager.SnowEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_snowPrefab, settings);

            for (var z = -_worldSize; z <= _worldSize; z++)
            {
                for (var x = -_worldSize; x <= _worldSize; x++)
                {
                    var position = new Vector3(x, 0, z);
                    var instance = _world.EntityManager.Instantiate(GameDataManager.SandEntity);
                    _world.EntityManager.SetComponentData(instance, new Translation() { Value = position });
                }
            }

            UpdateStaticFields();
        }

        void OnValidate()
        {
            UpdateStaticFields();
        }

        private void UpdateStaticFields()
        {
            GameDataManager.Strength1 = _strength1;
            GameDataManager.Strength2 = _strength2;
            GameDataManager.Strength3 = _strength3;
            GameDataManager.Scale1 = _scale1;
            GameDataManager.Scale2 = _scale2;
            GameDataManager.Scale3 = _scale3;

            GameDataManager.SandLevel = _sandLevel;
            GameDataManager.DirtLevel = _dirtLevel;
            GameDataManager.GrassLevel = _grassLevel;
            GameDataManager.RockLevel = _rockLevel;
            GameDataManager.SnowLevel = _snowLevel;

            GameDataManager.HasDataChanged = true;
        }
    }
}
