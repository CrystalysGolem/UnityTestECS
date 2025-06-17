using Configs;
using Leopotam.EcsLite;
using Systems;

namespace Ecs
{
    public sealed class GameEcsSystems
    {
        public static EcsSystems Create(EcsWorld world, BusinessConfigsSO businessConfigs, BusinessNamesSO businessNames, int initialBalance)
        {
            var systems = new EcsSystems(world);

            systems
                .Add(new LoadSystem(businessConfigs, businessNames, initialBalance))
                .Add(new IncomeProgressSystem())
                .Add(new BalanceUpdateSystem())
                .Add(new BusinessUpgradeSystem())
                .Add(new UpgradePurchaseSystem())
                .Add(new UISystem(businessConfigs))
                .Add(new SaveSystem());

            return systems;
        }
    }
}
