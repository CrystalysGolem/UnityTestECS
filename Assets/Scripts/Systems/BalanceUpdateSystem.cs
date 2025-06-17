using Leopotam.EcsLite;
using UnityEngine;
using Components;
using Utils;
using TMPro;

namespace Systems
{
    public sealed class BalanceUpdateSystem : IEcsRunSystem
    {
        private TMP_Text _balanceText;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld() as EcsWorld;
            InitializeBalanceText();

            if (_balanceText == null)
            {
                Debug.LogWarning("BalanceText UI element not found.");
                return;
            }

            var balancePool = world.GetPool<BalanceComponent>();
            if (!TryGetSingleEntity(world, out int balanceEntity))
            {
                return;
            }

            ref var balance = ref balancePool.Get(balanceEntity);
            ProcessIncomes(world, ref balance);
            UpdateUIText(balance.Value);
        }

        private void InitializeBalanceText()
        {
            if (_balanceText != null)
            {
                return;
            }

            var link = Object.FindObjectOfType<BalanceUILink>();
            _balanceText = link?
                .gameObject.transform.Find("BalanceText")
                ?.GetComponent<TMP_Text>();
        }

        private bool TryGetSingleEntity(EcsWorld world, out int entity)
        {
            var filter = world.Filter<BalanceComponent>().End();
            foreach (var e in filter)
            {
                entity = e;
                return true;
            }

            entity = default;
            return false;
        }

        private void ProcessIncomes(EcsWorld world, ref BalanceComponent balance)
        {
            var progressPool = world.GetPool<IncomeProgressComponent>();
            var businessPool = world.GetPool<BusinessComponent>();
            var filter = world.Filter<IncomeProgressComponent>()
                              .Inc<BusinessComponent>()
                              .End();

            foreach (var e in filter)
            {
                ref var progress = ref progressPool.Get(e);
                if (progress.Value < 1f)
                {
                    continue;
                }

                ref var business = ref businessPool.Get(e);
                balance.Value += CalculateIncome(business);
                progress.Value = Mathf.Max(0f, progress.Value - 1f);
            }
        }

        private float CalculateIncome(BusinessComponent business)
        {
            float multiplier = 1f;
            for (int i = 0; i < business.UpgradesPurchased.Length; i++)
            {
                if (business.UpgradesPurchased[i])
                {
                    multiplier += business.Config.Upgrades[i].Multiplier;
                }
            }

            return business.Level * business.Config.BaseIncome * multiplier;
        }

        private void UpdateUIText(float value)
        {
            _balanceText.text = $"Баланс: {value:F0}$";
        }
    }
}
