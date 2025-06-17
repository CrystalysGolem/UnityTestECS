using Leopotam.EcsLite;
using UnityEngine;
using Components;

namespace Systems
{
    public sealed class IncomeProgressSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld() as EcsWorld;
            float deltaTime = Time.deltaTime;

            var progressPool = world.GetPool<IncomeProgressComponent>();
            var businessPool = world.GetPool<BusinessComponent>();
            var uiPool = world.GetPool<UIComponent>();

            var filter = world.Filter<IncomeProgressComponent>()
                              .Inc<BusinessComponent>()
                              .Inc<UIComponent>()
                              .End();

            foreach (int entity in filter)
            {
                ProcessEntity(progressPool, businessPool, uiPool, entity, deltaTime);
            }
        }

        private void ProcessEntity(
            EcsPool<IncomeProgressComponent> progressPool,
            EcsPool<BusinessComponent> businessPool,
            EcsPool<UIComponent> uiPool,
            int entity,
            float deltaTime)
        {
            ref var progress = ref progressPool.Get(entity);
            ref var business = ref businessPool.Get(entity);
            ref var ui = ref uiPool.Get(entity);

            if (business.Level == 0)
            {
                ResetProgress(ref progress, ui);
                return;
            }

            AdvanceProgress(ref progress, business.Config.IncomeDelay, deltaTime);
            UpdateProgressBar(ui, progress.Value);
        }

        private void ResetProgress(
            ref IncomeProgressComponent progress,
            UIComponent ui)
        {
            progress.Value = 0f;
            if (ui.ProgressBar != null)
            {
                ui.ProgressBar.value = 0f;
            }
        }

        private void AdvanceProgress(
            ref IncomeProgressComponent progress,
            float delay,
            float deltaTime)
        {
            progress.Value += deltaTime / delay;
            if (progress.Value > 1f)
            {
                progress.Value = 1f;
            }
        }

        private void UpdateProgressBar(
            UIComponent ui,
            float value)
        {
            if (ui.ProgressBar != null)
            {
                ui.ProgressBar.value = value;
            }
        }
    }
}
