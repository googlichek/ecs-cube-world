using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    namespace Game.Scripts
    {
        public class EntityTracker : MonoBehaviour
        {
            private Entity _target = Entity.Null;

            public void SetTargetEntity(Entity entity)
            {
                _target = entity;
            }

            void LateUpdate()
            {
                if (_target != Entity.Null)
                {
                    try
                    {
                        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                        transform.position = entityManager.GetComponentData<Translation>(_target).Value;
                        transform.rotation = entityManager.GetComponentData<Rotation>(_target).Value;
                    }
                    catch
                    {
                        _target = Entity.Null;
                    }
                }
            }
        }
    }
}
