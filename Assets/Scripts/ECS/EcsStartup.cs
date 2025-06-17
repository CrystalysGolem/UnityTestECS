using Configs;
using Leopotam.EcsLite;
using UnityEngine;

namespace Ecs
{
    public sealed class EcsStartup : MonoBehaviour
    {
        EcsWorld _world;
        EcsSystems _systems;

        [SerializeField] private BusinessConfigsSO _businessConfigs;
        [SerializeField] private BusinessNamesSO _businessNames;

        [SerializeField] private int _initialBalance = 0;

        private void Start()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            _systems = GameEcsSystems.Create(_world, _businessConfigs, _businessNames, _initialBalance);
            _systems.Init();
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
            }

            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }
    }
}
