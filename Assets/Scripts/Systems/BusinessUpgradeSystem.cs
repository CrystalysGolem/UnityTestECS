using Leopotam.EcsLite;
using Components;

namespace Systems
{
    public sealed class BusinessUpgradeSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld() as EcsWorld;
            if (!TryGetSingleEntity(world, out int balanceEntity))
            {
                return;
            }

            ref var balance = ref world
                .GetPool<BalanceComponent>()
                .Get(balanceEntity);

            var businessPool = world.GetPool<BusinessComponent>();
            var uiPool = world.GetPool<UIComponent>();
            var filter = world
                .Filter<BusinessComponent>()
                .Inc<UIComponent>()
                .End();

            foreach (int entity in filter)
            {
                ref var business = ref businessPool.Get(entity);
                ref var ui = ref uiPool.Get(entity);

                if (!ui.LevelUpRequested)
                {
                    continue;
                }

                HandleLevelUp(ref business, ref ui, ref balance);
            }
        }

        private bool TryGetSingleEntity(EcsWorld world, out int entity)
        {
            var filter = world.Filter<BalanceComponent>().End();
            foreach (int e in filter)
            {
                entity = e;
                return true;
            }

            entity = default;
            return false;
        }

        private void HandleLevelUp(
            ref BusinessComponent business,
            ref UIComponent ui,
            ref BalanceComponent balance)
        {
            ui.LevelUpRequested = false;

            int nextLevel = business.Level + 1;
            float cost = nextLevel * business.Config.BaseCost;

            if (balance.Value < cost)
            {
                return;
            }

            balance.Value -= cost;
            business.Level = nextLevel;
            UpdateUIAfterLevelUp(ui, business, cost);
        }

        private void UpdateUIAfterLevelUp(
            UIComponent ui,
            BusinessComponent business,
            float lastCost)
        {
            ui.LevelUpButtonText.text = lastCost.ToString("F0");
            ui.LevelText.text = $"Level: {business.Level}";
        }
    }
}
