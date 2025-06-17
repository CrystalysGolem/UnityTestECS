using Leopotam.EcsLite;
using Components;
using Data;
using Utils;
using System.Collections.Generic;

namespace Systems
{
    public sealed class SaveSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld() as EcsWorld;
            var saveData = new SaveData
            {
                Balance = GetCurrentBalance(world),
                Businesses = GetBusinessesData(world)
            };

            SaveLoadUtility.Save(saveData);
        }

        private float GetCurrentBalance(EcsWorld world)
        {
            var balancePool = world.GetPool<BalanceComponent>();
            var filter = world.Filter<BalanceComponent>().End();

            foreach (int e in filter)
            {
                return balancePool.Get(e).Value;
            }

            return 0f;
        }

        private BusinessSaveData[] GetBusinessesData(EcsWorld world)
        {
            var businessPool = world.GetPool<BusinessComponent>();
            var progressPool = world.GetPool<IncomeProgressComponent>();
            var filter = world
                .Filter<BusinessComponent>()
                .Inc<IncomeProgressComponent>()
                .End();

            var list = new List<BusinessSaveData>(filter.GetEntitiesCount());
            foreach (int e in filter)
            {
                ref var business = ref businessPool.Get(e);
                ref var progress = ref progressPool.Get(e);

                list.Add(CreateBusinessSaveData(business, progress.Value));
            }

            return list.ToArray();
        }

        private BusinessSaveData CreateBusinessSaveData(
            BusinessComponent business,
            float progressValue)
        {
            var upgradesArray = new UpgradeSaveData[business.UpgradesPurchased.Length];
            for (int i = 0; i < upgradesArray.Length; i++)
            {
                upgradesArray[i] = new UpgradeSaveData
                {
                    Purchased = business.UpgradesPurchased[i]
                };
            }

            return new BusinessSaveData
            {
                Level = business.Level,
                Progress = progressValue,
                UpgradesPurchased = upgradesArray
            };
        }
    }
}
