using Leopotam.EcsLite;
using Components;

namespace Systems
{
    public sealed class UpgradePurchaseSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld() as EcsWorld;
            if (!TryGetSingleEntity(world, out int balanceEntity))
            {
                return;
            }

            ref var balance = ref world.GetPool<BalanceComponent>().Get(balanceEntity);
            var businessPool = world.GetPool<BusinessComponent>();
            var uiPool = world.GetPool<UIComponent>();
            var filter = world
                .Filter<BusinessComponent>()
                .Inc<UIComponent>()
                .End();

            foreach (int entity in filter)
            {
                ProcessBusiness(entity, businessPool, uiPool, ref balance);
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

        private void ProcessBusiness(
            int entity,
            EcsPool<BusinessComponent> businessPool,
            EcsPool<UIComponent> uiPool,
            ref BalanceComponent balance)
        {
            ref var business = ref businessPool.Get(entity);
            ref var ui = ref uiPool.Get(entity);

            int upgradesCount = business.Config.Upgrades.Length;
            if (ui.UpgradeRequested == null || business.UpgradesPurchased == null)
            {
                return;
            }

            for (int i = 0; i < upgradesCount; i++)
            {
                if (!ui.UpgradeRequested[i] || business.UpgradesPurchased[i])
                {
                    continue;
                }

                ui.UpgradeRequested[i] = false;
                TryPurchaseUpgrade(i, ref business, ref ui, ref balance);
            }
        }

        private void TryPurchaseUpgrade(
            int index,
            ref BusinessComponent business,
            ref UIComponent ui,
            ref BalanceComponent balance)
        {
            var upgrade = business.Config.Upgrades[index];
            float price = upgrade.Price;
            if (balance.Value < price)
            {
                return;
            }

            balance.Value -= price;
            business.UpgradesPurchased[index] = true;
            UpdateUIAfterPurchase(index, ui);
        }

        private void UpdateUIAfterPurchase(
            int index,
            UIComponent ui)
        {
            if (ui.UpgradeButtons != null &&
                index < ui.UpgradeButtons.Length &&
                ui.UpgradeButtons[index] != null)
            {
                ui.UpgradeButtons[index].interactable = false;
            }

            if (ui.UpgradeButtonPriceTexts != null &&
                index < ui.UpgradeButtonPriceTexts.Length &&
                ui.UpgradeButtonPriceTexts[index] != null)
            {
                ui.UpgradeButtonPriceTexts[index].text = "Куплено";
            }
        }
    }
}
