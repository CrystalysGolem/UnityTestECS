using Leopotam.EcsLite;
using Configs;
using Components;
using Utils;
using Data;

namespace Systems
{
    public sealed class LoadSystem : IEcsInitSystem
    {
        private readonly BusinessConfigsSO _businessConfigs;
        private readonly BusinessNamesSO _businessNames;
        private readonly int _initialBalance;

        public LoadSystem(
            BusinessConfigsSO businessConfigs,
            BusinessNamesSO businessNames,
            int initialBalance
        )
        {
            _businessConfigs = businessConfigs;
            _businessNames = businessNames;
            _initialBalance = initialBalance;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld() as EcsWorld;
            var saveData = SaveLoadUtility.Load();

            InitializeBalance(world, saveData);
            InitializeBusinesses(world, saveData);
        }

        private void InitializeBalance(EcsWorld world, SaveData saveData)
        {
            var balanceEntity = world.NewEntity();
            ref var balanceC = ref world
                .GetPool<BalanceComponent>()
                .Add(balanceEntity);

            balanceC.Value = saveData?.Balance ?? _initialBalance;
        }

        private void InitializeBusinesses(EcsWorld world, SaveData saveData)
        {
            var businessPool = world.GetPool<BusinessComponent>();
            var progressPool = world.GetPool<IncomeProgressComponent>();
            var uiPool = world.GetPool<UIComponent>();

            for (int i = 0; i < _businessConfigs.Businesses.Length; i++)
            {
                var config = _businessConfigs.Businesses[i];
                var name = _businessNames.BusinessNames[i];
                var data = GetBusinessSaveData(saveData, i);
                var entity = world.NewEntity();

                AddBusinessComponent(businessPool, entity, i, config, name, data);
                AddProgressComponent(progressPool, entity, data);
                uiPool.Add(entity);
            }
        }

        private BusinessSaveData GetBusinessSaveData(SaveData saveData, int index)
        {
            if (saveData?.Businesses != null &&
                index < saveData.Businesses.Length)
            {
                return saveData.Businesses[index];
            }

            return null;
        }

        private void AddBusinessComponent(
            EcsPool<BusinessComponent> pool,
            int entity,
            int id,
            BusinessConfig config,
            string name,
            BusinessSaveData data
        )
        {
            ref var comp = ref pool.Add(entity);

            comp.Id = id;
            comp.Config = config;
            comp.Name = name;
            comp.Level = data?.Level ?? (id == 0 ? 1 : 0);

            int upgradesCount = config.Upgrades.Length;
            comp.UpgradesPurchased = new bool[upgradesCount];
            var savedUpgrades = data?.UpgradesPurchased;
            for (int u = 0; u < upgradesCount; u++)
            {
                comp.UpgradesPurchased[u] =
                    savedUpgrades != null &&
                    u < savedUpgrades.Length &&
                    savedUpgrades[u].Purchased;
            }
        }

        private void AddProgressComponent(
            EcsPool<IncomeProgressComponent> pool,
            int entity,
            BusinessSaveData data
        )
        {
            ref var comp = ref pool.Add(entity);
            comp.Value = data?.Progress ?? 0f;
        }
    }
}
