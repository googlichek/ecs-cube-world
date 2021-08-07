using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

namespace Game.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        private World _world = default;

        void Awake()
        {
            _world = World.DefaultGameObjectInjectionWorld;

            HandleSystemDisabling();
        }

        private void HandleSystemDisabling()
        {
            var exposedTypes = new List<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    var item = type.GetCustomAttribute<DisableSystemOnStartAttribute>();
                    if (item != null)
                    {
                        exposedTypes.Add(type);
                    }
                }
            }

            foreach (var type in exposedTypes)
            {
                if (type.BaseType == typeof(SystemBase))
                {
                    var system = _world.GetExistingSystem(type);
                    if (system != null)
                        system.Enabled = false;
                }
            }
        }
    }
}
