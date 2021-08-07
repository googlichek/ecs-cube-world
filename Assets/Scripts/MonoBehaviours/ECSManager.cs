using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Game.Scripts
{
    public class ECSManager : MonoBehaviour
    {
        [SerializeField] [Range(0, 1024)]
        private int _worldSize = 0;

        [SerializeField] [Range(0.1f, 10f)]
        private float _strength = 0;

        [SerializeField] [Range(0.01f, 1f)]
        private float _scale = 0;

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

            var sandEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_sandPrefab, settings);

            for (var z = -_worldSize; z <= _worldSize; z++)
            {
                for (var x = -_worldSize; x <= _worldSize; x++)
                {
                    var position = new Vector3(x, 0, z);
                    var instance = _world.EntityManager.Instantiate(sandEntity);
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
            GameDataManager.Strength = _strength;
            GameDataManager.Scale = _scale;
        }
    }
}
