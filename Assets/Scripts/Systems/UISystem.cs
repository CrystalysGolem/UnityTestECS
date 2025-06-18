using Leopotam.EcsLite;
using UnityEngine;
using Configs;
using Components;

namespace Systems
{
    public sealed class UISystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly BusinessConfigsSO _configs;
        private UIBusinessLink[] _links;

        public UISystem(BusinessConfigsSO configs)
        {
            _configs = configs;
        }

        public void Init(IEcsSystems systems)
        {
            CacheLinks();

            var world = systems.GetWorld() as EcsWorld;
            var businessPool = world.GetPool<BusinessComponent>();
            var uiPool = world.GetPool<UIComponent>();

            foreach (var link in _links)
            {
                int id = link.BusinessId;

                foreach (var entity in world.Filter<BusinessComponent>().Inc<UIComponent>().End())
                {
                    ref var business = ref businessPool.Get(entity);

                    if (business.Id != id)
                    {
                        continue;
                    }

                    ref var ui = ref uiPool.Get(entity);
                    InitializeUI(ref ui, link, id);
                    BindButtons(world, entity, link);
                    break;
                }
            }
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld() as EcsWorld;
            BalanceComponent balanceComp = GetBalance(world);
            var businessPool = world.GetPool<BusinessComponent>();
            var progressPool = world.GetPool<IncomeProgressComponent>();
            var uiPool = world.GetPool<UIComponent>();
            var filter = world.Filter<BusinessComponent>().Inc<UIComponent>().End();

            UpdateUnlockStates(businessPool, filter);

            foreach (var entity in filter)
            {
                ref var business = ref businessPool.Get(entity);
                ref var ui = ref uiPool.Get(entity);
                ref var progress = ref progressPool.Get(entity);

                UpdateBusinessUI(business, ui, progress, balanceComp.Value);
            }
        }

        private void CacheLinks()
        {
            _links = Object.FindObjectsOfType<UIBusinessLink>();
        }

        private void InitializeUI(
            ref UIComponent ui,
            UIBusinessLink link,
            int id)
        {
            ui.BusinessNameText = link.BusinessNameText;
            ui.LevelText = link.LevelText;
            ui.IncomeText = link.IncomeText;
            ui.ProgressBar = link.ProgressBar;

            ui.LevelUpButton = link.LevelUpButton;
            ui.LevelUpButtonText = link.LevelUpButtonText;
            ui.LevelUpButtonPriceText = link.LevelUpButtonPriceText;

            var upgrades = _configs.Businesses[id].Upgrades;
            ui.UpgradeButtons = link.UpgradeButtons;
            ui.UpgradeButtonNameTexts = link.UpgradeButtonNameTexts;
            ui.UpgradeButtonIncomeTexts = link.UpgradeButtonIncomeTexts;
            ui.UpgradeButtonPriceTexts = link.UpgradeButtonPriceTexts;
            ui.UpgradeRequested = new bool[upgrades.Length];
        }

        private void BindButtons(
            EcsWorld world,
            int entity,
            UIBusinessLink link)
        {
            link.LevelUpButton.onClick.AddListener(() =>
            {
                if (world.GetPool<UIComponent>().Has(entity))
                {
                    world.GetPool<UIComponent>().Get(entity).LevelUpRequested = true;
                }
            });

            for (int i = 0; i < link.UpgradeButtons.Length; i++)
            {
                int idx = i;

                link.UpgradeButtons[i].onClick.AddListener(() =>
                {
                    if (world.GetPool<UIComponent>().Has(entity))
                    {
                        world.GetPool<UIComponent>().Get(entity).UpgradeRequested[idx] = true;
                    }
                });
            }
        }

        private BalanceComponent GetBalance(EcsWorld world)
        {
            foreach (var entity in world.Filter<BalanceComponent>().End())
            {
                return world.GetPool<BalanceComponent>().Get(entity);
            }

            return default;
        }

        private void UpdateUnlockStates(
            EcsPool<BusinessComponent> businessPool,
            EcsFilter filter)
        {
            foreach (var link in _links)
            {
                bool unlocked = false;

                foreach (int entity in filter)
                {
                    if (businessPool.Get(entity).Id != link.BusinessId)
                    {
                        continue;
                    }

                    unlocked =
                        businessPool.Get(entity).Id == 0 ||
                        HasPreviousUnlocked(businessPool, filter, businessPool.Get(entity).Id - 1);
                    break;
                }

                link.Root.SetActive(unlocked);
            }
        }

        private bool HasPreviousUnlocked(
            EcsPool<BusinessComponent> pool,
            EcsFilter filter,
            int prevId)
        {
            foreach (int entity in filter)
            {
                if (pool.Get(entity).Id == prevId && pool.Get(entity).Level > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateBusinessUI(
            BusinessComponent business,
            UIComponent ui,
            IncomeProgressComponent progress,
            float balance)
        {
            ui.BusinessNameText.text = business.Name;
            ui.LevelText.text = $"Level: {business.Level}";

            float nextCost = (business.Level + 1) * business.Config.BaseCost;

            ui.LevelUpButtonText.text =
                business.Level == 0 ? "Купить" : "Улучшить";

            ui.LevelUpButtonPriceText.text = nextCost.ToString("F0");
            ui.LevelUpButton.interactable = balance >= nextCost;

            float multiplier = 1f;

            for (int i = 0; i < business.UpgradesPurchased.Length; i++)
            {
                if (business.UpgradesPurchased[i])
                {
                    multiplier += business.Config.Upgrades[i].Multiplier;
                }
            }

            ui.IncomeText.text = (business.Level * business.Config.BaseIncome * multiplier)
                .ToString("F0");

            if (ui.ProgressBar != null)
            {
                ui.ProgressBar.value = progress.Value;
            }

            for (int i = 0; i < business.UpgradesPurchased.Length; i++)
            {
                var upgrade = business.Config.Upgrades[i];
                ui.UpgradeButtonNameTexts[i].text = upgrade.Name;
                ui.UpgradeButtonIncomeTexts[i].text =
                    $"Доход: +{upgrade.Multiplier * 100:F0}%";

                if (business.UpgradesPurchased[i])
                {
                    ui.UpgradeButtons[i].interactable = false;
                    ui.UpgradeButtonPriceTexts[i].text = "Куплено";
                }
                else
                {
                    ui.UpgradeButtonPriceTexts[i].text = upgrade.Price.ToString("F0");
                    ui.UpgradeButtons[i].interactable = balance >= upgrade.Price;
                }
            }
        }
    }
}